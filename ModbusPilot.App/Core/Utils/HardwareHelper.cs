using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ModbusPilot.Core.Utils
{
    public static class HardwareHelper
    {
        /// <summary>
        /// 获取当前电脑的唯一机器码 (MBP-XXXX-XXXX-XXXX)
        /// </summary>
        public static string GetMachineCode()
        {
            try
            {
                // 1. 提取 CPU ID 和 主板序列号
                string rawId = GetCpuId() + GetMotherboardSerialNumber();

                // 2. 使用加盐哈希，防止逆向原始硬件信息
                return GenerateProfessionalId(rawId);
            }
            catch
            {
                return "MBP-ERROR-DEVICE-ID-FAIL"; // 容错处理
            }
        }

        private static string GetCpuId()
        {
            string cpuInfo = "";
            using (ManagementClass mc = new ManagementClass("Win32_Processor"))
            {
                foreach (ManagementObject mo in mc.GetInstances())
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value?.ToString();
                    if (!string.IsNullOrEmpty(cpuInfo)) break;
                }
            }
            return cpuInfo ?? "CPU-UNKNOWN";
        }

        private static string GetMotherboardSerialNumber()
        {
            string mbInfo = "";
            using (ManagementClass mc = new ManagementClass("Win32_BaseBoard"))
            {
                foreach (ManagementObject mo in mc.GetInstances())
                {
                    mbInfo = mo.Properties["SerialNumber"].Value?.ToString();
                    if (!string.IsNullOrEmpty(mbInfo)) break;
                }
            }
            return mbInfo ?? "MB-UNKNOWN";
        }

        private static string GenerateProfessionalId(string input)
        {
            // 加上只有你才知道的“盐”值
            string saltedInput = input + "ModbusPilot_Secret_Salt_2026";

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedInput));
                // 转为 Base64 并只取前 15 位字符
                string base64 = Convert.ToBase64String(hashBytes)
                    .Replace("+", "").Replace("/", "").ToUpper();

                string shortCode = base64.Substring(0, 12);

                // 格式化为：MBP-XXXX-XXXX-XXXX
                return "MBP-" + Regex.Replace(shortCode, ".{4}", "$0-").TrimEnd('-');
            }
        }
    }
}