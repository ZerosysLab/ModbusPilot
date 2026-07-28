using ModbusPilot.Core;
using ModbusPilot.Core.Services;
using ModbusPilot.Core.Utils;
using ModbusPilot.UI;
using System.Diagnostics;
using System.Reflection;
using TheArtOfDev.HtmlRenderer.WinForms;

namespace ModbusPilot.App
{
    public partial class F_About1 : F_BaseForm
    {
        private HtmlPanel _htmlAbout;

        public F_About1()
        {
            InitializeComponent();
            SetupUI();
            LoadHtmlContent();
        }

        private void SetupUI()
        {
            this.Text = LangProvider.Get("About_Title", "关于 ModbusPilot");
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            //this.Size = new Size(500, 500); // 稍微调大一点

            // 初始化 HtmlPanel
            _htmlAbout = new HtmlPanel
            {
                Dock = DockStyle.Fill,
                IsContextMenuEnabled = false,
                IsSelectionEnabled = true // 允许用户选中复制机器码
            };

            // 处理 HTML 中的链接点击
            _htmlAbout.LinkClicked += (s, e) =>
            {
                e.Handled = true;

                if (e.Link.StartsWith("copy_email:"))
                {
                    // 提取冒号后面的邮箱地址
                    string email = e.Link.Substring("copy_email:".Length);

                    // 执行复制到剪贴板
                    Clipboard.SetText(email);

                    // 弹窗提示（或者用更轻量级的气泡，这里用 MessageBox 保证用户能看到）
                    MessageBox.Show($"作者邮箱：{email}\n已成功复制到剪贴板。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // 如果是正常的 http 链接，调用原有的跳转逻辑
                    OpenUrl(e.Link);
                }
            };

            this.Controls.Add(_htmlAbout);
        }

        private void LoadHtmlContent()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.ProductVersion ?? fvi.FileVersion;
            string copyright = fvi.LegalCopyright;
            // 确保机器码已经获取，如果没有则临时计算一次
            string machineCode = LicenseGuard.MachineCode ?? HardwareHelper.GetMachineCode();

            // 你的邮箱
            string myEmail = LinkManager.ContactEmail;

            // --- 【核心重构：授权状态逻辑】 ---
            string statusHtml = "";

            // 1. 优先级最高：检查是否已经输入激活码并验证通过
            if (LicenseGuard.CurrentStatus.Type == ModbusPilot.Core.Models.LicenseType.Professional)
            {
                statusHtml = "<span style='color:#00FF00; font-weight:bold;'>✔ 已激活专业版</span>";
            }
            // 2. 优先级次之：检查是否处于公测期 (使用我们重构的 IsBetaActive)
            else if (LicenseGuard.IsBetaMode)
            {
                statusHtml = $"<span style='color:#FFA500; font-weight:bold;'>🚀 公测预览版 (限时全功能开放)</span><br/>" +
                             $"<span style='font-size:8pt; color:#888;'>有效期至: {LinkManager.BetaExpiryDate:yyyy-MM-dd}</span>";
            }
            // 3. 最后：真正的免费版
            else
            {
                statusHtml = "<span style='color:#FF4500; font-weight:bold;'>✘ 基础免费版 (功能受限)</span>";
            }

            //// 授权状态逻辑
            //string statusHtml = "";
            //var status = LicenseService.Current;
            //if (status.Type == ModbusPilot.Core.Models.LicenseType.Professional)
            //    statusHtml = "<span style='color:#00FF00;'>✔ 已激活专业版</span>";
            //else if (LicenseService.IsBeta && LicenseService.IsInBetaPeriod())
            //    statusHtml = $"<span style='color:#FFA500;'>🚀 公测预览版 (有效期至: {LinkManager.BetaExpiryDate:yyyy-MM-dd})</span>";
            //else
            //    statusHtml = "<span style='color:#FF4500;'>✘ 基础免费版</span>";

            string html = $@"
<div style='font-family: ""Microsoft YaHei UI"", sans-serif; padding: 20px; background-color: #1E1E1E; color: #DCDCDC; height:100%;'>
    
    <!-- 头部信息 -->
    <div style='text-align: center; margin-bottom: 20px;'>
        <div style='font-size: 24pt; font-weight: bold; color: #00BFFF;'>Modbus Pilot</div>
        <div style='font-size: 9pt; color: #666; margin-top:3px;'>版本：v{version}</div>
    </div>

    <!-- 授权与机器码 -->
    <div style='border-top: 1px solid #333; padding-top: 15px; font-size: 9.5pt;'>
        <div style='margin-bottom:8px;'><b>授权状态：</b>{statusHtml}</div>
        <div><b>机器识别码 (MID)：</b></div>
        <div style='background-color:#2D2D30; padding:6px; border:1px solid #444; color:#00BFFF; font-family:Consolas; margin-top:5px; border-radius:3px;'>
            {machineCode}
        </div>
    </div>

    <!-- 互动链接 -->
    <div style='margin-top: 25px; text-align: center; font-size: 10pt;'>
        <a href='{LinkManager.HomeUrl}' style='color: #32CD32; text-decoration: none;'>项目主页</a> &nbsp; | &nbsp; 
        <a href='copy_email:{myEmail}' style='color: #32CD32; text-decoration: none;'>复制作者邮箱</a> &nbsp; | &nbsp; 
        <a href='{LinkManager.IssueUrl}' style='color: #32CD32; text-decoration: none;'>反馈建议</a>
    </div>
    <div style='text-align: center; font-size: 8.5pt; color: #666; margin-top: 5px;'>
        作者邮箱：{myEmail}
    </div>
    <!-- 免责声明 (核心部分) -->
<div style='margin-top: 25px; padding: 12px; background-color: #252526; border: 1px solid #444; border-radius: 4px;'>
    <div style='font-size: 9pt; color: #CC9900; font-weight: bold; margin-bottom: 8px;'>⚠️ 重要安全警告与免责声明</div>
    
    <div style='font-size: 8pt; color: #A07000; line-height: 1.6; text-align: justify;'>
        1. 本软件按“原样”提供，不提供任何形式的明示或暗示保证。<br/>
        2. <b>禁止高风险用途</b>：本软件严禁用于生命维持、医疗设备、核动力、航空航天、武器系统等任何可能导致人身伤亡或重大环境损害的环境。<br/>
        3. 作者不对因使用本软件导致的任何硬件损坏、生产停机、数据丢失或业务中断承担法律责任。<br/>
        4. 工业现场环境复杂，用户有义务在正式操作前进行充分的离线测试。
    </div>

    <!-- 协议确认行：独立出来并加粗 -->
    <div style='margin-top: 10px; padding-top: 8px; border-top: 1px dashed #444; font-size: 8.5pt; color: #CC9900; font-weight: bold; text-align: center;'>
        使用本软件即表示您已阅读、理解并完全同意以上所有条款。
    </div>
</div>

    <!-- 版权底部 -->
    <div style='margin-top: 20px; text-align: center; font-size: 8pt; color: #555;'>
        {copyright}
    </div>
</div>";

            _htmlAbout.Text = html;
        }

        private void OpenUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;

            try
            {
                // 1. 去除可能存在的首尾空格
                string target = url.Trim();

                // 2. 核心逻辑：.NET 8 中必须设置 UseShellExecute = true 
                // 这样系统才会根据协议（http/mailto）自动去调用默认浏览器或邮件程序
                Process.Start(new ProcessStartInfo
                {
                    FileName = target,
                    UseShellExecute = true
                });
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // 常见于：点击 mailto 时，用户电脑上没装 Outlook/Foxmail 等邮件客户端
                string msg = url.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase)
                    ? "未找到关联的邮件客户端。请手动发送邮件至：support@modbuspilot.com"
                    : "无法找到默认浏览器。请手动访问项目主页。";

                MessageBox.Show(msg, "操作提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"链接跳转失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //private void OpenUrl(string url)
        //{
        //    if (string.IsNullOrEmpty(url)) return;
        //    try
        //    {
        //        Process.Start(new ProcessStartInfo
        //        {
        //            FileName = url,
        //            UseShellExecute = true
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"无法打开链接: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
    }
}