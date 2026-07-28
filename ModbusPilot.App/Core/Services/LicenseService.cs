using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using ModbusPilot.Core.Utils;
using ModbusPilot.Core.Models;
using Microsoft.Win32;

namespace ModbusPilot.Core.Services
{
    internal class LicenseService
    {
        // === 版本属性 ===
        // 只有编译时确定是 BetaBuild，才会去检查公测日期
        internal static bool IsBetaBuild => true;

        // 核心逻辑：当前是否处于公测有效期内
        internal static bool IsBetaActive => IsBetaBuild && DateTime.Now < LinkManager.BetaExpiryDate;

        // --------------------------------------------------------------------------------
        // 【最高机密】这是你的 RSA 公钥 (用于验签)
        // 实际开发中，建议把这串字符做简单的 Base64 混淆或拆分存储，防止被一搜就着
        // --------------------------------------------------------------------------------
        private const string _publicKeyXml = @"<RSAKeyValue><Modulus>tT+Zu0C8xJiK5fbnP4yFE/Dx7w0pagJqb+ys9rOjGb7mhDYyPH5MsoiH+6EG8nrOyL3VF9ZEoPhGAWiYUpClPNUobZFhzKYkqESCX2aUj68dE+Tt35dnImmGBzIkEyTeyCBPn4phEF8HP6iB+Mh1CbysaeozD9VO2ejAsxo5+FVGgrC4sEv8rwgBp7jVuGmpZHIn2l8jlIOa3UfB9JxxFua1t3afiri6SHwtOK6aFibx9Ix63PSrmp4t9PRlHRijP8NlwssUG1AEP8jhxtz/2HTWg9+a71NcZeapPaK+6VVVNcS1rZjxqyee3tmg3in2QxCJspllpSMNb+8jXb1IpQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        private static LicenseStatus _currentStatus = new LicenseStatus();
        // 【修改点】重新定义 Current，让其 Message 属性能够动态感知 Beta 状态
        internal static LicenseStatus Current
        {
            get
            {
                // 如果已激活专业版，直接返回
                if (_currentStatus.Type == LicenseType.Professional) return _currentStatus;

                // 如果未激活，但处于公测期
                if (IsBetaActive)
                {
                    return new LicenseStatus
                    {
                        Type = LicenseType.Free, // 虽然权限是Pro，但身份仍是Free
                        Message = $"公测版 (至{LinkManager.BetaExpiryDate:yyyy-MM-dd})"
                    };
                }

                // 否则返回原始状态（即 基础免费版）
                return _currentStatus;
            }
        }

        private static readonly string _licensePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "license.lic");
        private const string RegPath = @"Software\ModbusPilot";
        private const string RegKey = "LicenseData";
        public static string CachedMachineCode { get; private set; }


        /// <summary>
        /// 启动时调用：检查本地授权文件
        /// </summary>
        internal static void Initialize()
        {
          

            string licenseKey = "";

            // 1. 优先尝试从文件读取
            if (File.Exists(_licensePath))
            {
                licenseKey = File.ReadAllText(_licensePath).Trim();
            }
            else
            {
                // 2. 文件不存在，尝试从注册表恢复
                try
                {
                    using (var key = Registry.CurrentUser.OpenSubKey(RegPath))
                    {
                        if (key != null)
                        {
                            licenseKey = key.GetValue(RegKey)?.ToString() ?? "";
                            // 自动修复：如果注册表有但文件没了，把文件补回来
                            if (!string.IsNullOrEmpty(licenseKey))
                            {
                                File.WriteAllText(_licensePath, licenseKey);
                                SystemLogger.WriteLog("授权文件已从注册表备份中恢复。", "LICENSE");
                            }
                        }
                    }
                }
                catch { /* 忽略注册表读取错误 */ }
            }

            // 3. 执行验证
            if (!string.IsNullOrEmpty(licenseKey))
            {
                _currentStatus = ValidateLicense(licenseKey);
            }
            else
            {
                _currentStatus = new LicenseStatus { Type = LicenseType.Free, Message = "未激活（基础版）" };
            }

            // 在程序启动的后台线程或初始化时就先算好
            Task.Run(() => {
                CachedMachineCode = HardwareHelper.GetMachineCode();
            });
        }

        /// <summary>
        /// 检查版本是否仍在公测期内
        /// </summary>
        internal static bool IsInBetaPeriod()
        {
            return DateTime.Now < LinkManager.BetaExpiryDate;
        }
        /// <summary>
        /// 核心校验逻辑：验证激活码的真实性与合法性
        /// </summary>
        internal static LicenseStatus ValidateLicense(string licenseBase64)
        {
            try
            {
                // 1. 解码最外层的 Base64
                byte[] combinedBytes = Convert.FromBase64String(licenseBase64);
                string combinedString = Encoding.UTF8.GetString(combinedBytes);

                // 2. 分离 JSON 数据和 签名
                string[] parts = combinedString.Split('|');
                if (parts.Length != 2) return Invalid("激活码格式非法");

                string jsonData = parts[0];
                string signatureBase64 = parts[1];

                // 3. RSA 验签
                if (!Verify(jsonData, signatureBase64))
                {
                    return Invalid("数字签名验证失败，激活码可能已被篡改");
                }

                // 4. 解析业务数据
                var info = JsonConvert.DeserializeObject<LicenseInfo>(jsonData);
                if (info == null) return Invalid("无法解析授权信息");

                // 5. 比对机器码
                string currentMachine = HardwareHelper.GetMachineCode();
                if (info.MachineCode != currentMachine)
                {
                    return Invalid($"该激活码已绑定到其他设备:\n{info.MachineCode}");
                }

                // 6. 检查有效期
                if (info.ExpiryDate < DateTime.Now.Date)
                {
                    return Invalid($"该激活码已于 {info.ExpiryDate:yyyy-MM-dd} 过期");
                }

                // 全部通过
                return new LicenseStatus
                {
                    Type = LicenseType.Professional,
                    ExpiryDate = info.ExpiryDate,
                    Message = "已激活专业版"
                };
            }
            catch (Exception ex)
            {
                return Invalid("验证过程出错: " + ex.Message);
            }
        }

        private static bool Verify(string data, string signatureBase64)
        {
            // 同样使用 ProviderType 24 以确保支持 SHA256
            var cspParams = new CspParameters { ProviderType = 24 };
            using (var rsa = new RSACryptoServiceProvider(cspParams))
            {
                try
                {
                    rsa.FromXmlString(_publicKeyXml);
                    byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                    byte[] sigBytes = Convert.FromBase64String(signatureBase64);

                    // 必须与注册机使用的 HashAlgorithmName 和 Padding 完全一致
                    return rsa.VerifyData(dataBytes, sigBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                }
                catch
                {
                    return false;
                }
            }
        }

        internal static LicenseStatus Invalid(string msg) => new LicenseStatus { Type = LicenseType.Free, Message = msg };

        /// <summary>
        /// 将激活码持久化保存（文件+注册表双备份）
        /// </summary>
        internal static void SaveLicense(string licenseKey)
        {
            try
            {
                // 1. 保存到本地文件 (用于绿色版运行)
                File.WriteAllText(_licensePath, licenseKey.Trim());

                // 2. 备份到注册表 (防止文件被误删)
                try
                {
                    using (var key = Registry.CurrentUser.CreateSubKey(RegPath))
                    {
                        if (key != null) key.SetValue(RegKey, licenseKey.Trim());
                    }
                }
                catch (Exception ex)
                {
                    // 注册表权限受限时不强求，记录日志即可
                    SystemLogger.WriteLog($"注册表备份失败: {ex.Message}", "LICENSE");
                }

                SystemLogger.WriteLog("授权激活码已成功保存至本地。", "LICENSE");
            }
            catch (Exception ex)
            {
                SystemLogger.WriteError(ex, "SAVE_LICENSE_FAIL");
                throw new Exception("授权文件保存失败，请检查程序是否有写入权限。");
            }
        }
    }
}