using ModbusPilot.Core.Models;
using ModbusPilot.Core.Services;
using ModbusPilot.Core.Utils;
using System.Diagnostics;

namespace ModbusPilot.UI.Common
{
    public partial class F_Registration : Form
    {
        // 你的售卖渠道链接
        private const string StoreUrl = "https://aifadian.net/@yourname";

        public F_Registration(string customPrompt = "")
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(customPrompt))
                lblPrompt.Text = customPrompt;

            LoadHtmlContent();

            // 优先用缓存，秒开窗口
            if (!string.IsNullOrEmpty(LicenseGuard.MachineCode))
            {
                txtMachineCode.Text = LicenseGuard.MachineCode;
            }
            else
            {
                // 兜底方案：如果还没算完，就现场算（虽然会卡一下，但保证有数据）
                txtMachineCode.Text = HardwareHelper.GetMachineCode();
            }

            // 3. 处理顶部提示词（Prompt）
            if (!string.IsNullOrEmpty(customPrompt))
            {
                // 如果是从收费拦截点跳过来的，显示拦截理由（例如：导出功能需要专业版）
                lblPrompt.Text = $"⚠️ {customPrompt}";
                lblPrompt.ForeColor = Color.Red;
            }
            else if (LicenseGuard.IsBetaMode)
            {
                // 正常打开窗口，且在公测期
                lblPrompt.Text = $"🚀 当前为公测版，全功能开放至 {LinkManager.BetaExpiryDate:yyyy-MM-dd}";
                lblPrompt.ForeColor = Color.LimeGreen;
            }
            else
            {
                // 既没有拦截理由，也不在公测期
                lblPrompt.Text = "获取专业版授权，解锁全部工业级功能";
                lblPrompt.ForeColor = Color.White;
            }

            UpdateStatus();
        }

        private void LoadHtmlContent()
        {
            string html = @"
<div style='font-family: ""Microsoft YaHei UI""; padding: 15px; background-color: #1E1E1E; color: #DCDCDC;'>
    <div style='font-size: 14pt; color: #FFD700; font-weight: bold; margin-bottom: 12px; border-bottom: 1px solid #444; padding-bottom: 5px;'>
          <span style='margin-right:8px;'>♛</span> ModbusPilot 版本功能对比
    </div>
    <table style='width: 100%; border-collapse: collapse; font-size: 9pt;'>
        <tr style='background-color: #333; color: #EEE;'>
            <th style='padding: 8px; text-align: left;'>核心维度</th>
            <th style='padding: 8px; text-align: center;'>基础免费版</th>
            <th style='padding: 8px; text-align: center; color: #FFD700;'>专业授权版</th>
        </tr>
        <tr>
            <td style='padding: 7px; border-bottom: 1px solid #333;'>通讯通道 / 从站设备</td>
            <td style='padding: 7px; border-bottom: 1px solid #333; text-align: center;'>2个 / 4台</td>
            <td style='padding: 7px; border-bottom: 1px solid #333; text-align: center; color: #00FF00;'>✔ 无限制</td>
        </tr>                   
        <tr>                    
            <td style='padding: 7px; border-bottom: 1px solid #333;'>单项目变量点位 (Tags)</td>
            <td style='padding: 7px; border-bottom: 1px solid #333; text-align: center;'>1,000 个</td>
            <td style='padding: 7px; border-bottom: 1px solid #333; text-align: center; color: #00FF00;'>✔ 海量支持</td>
        </tr>                   
        <tr>                    
            <td style='padding: 7px; border-bottom: 1px solid #333;'>仪表盘看板卡片</td>
            <td style='padding: 7px; border-bottom: 1px solid #333; text-align: center;'>20 个 (一屏)</td>
            <td style='padding: 7px; border-bottom: 1px solid #333; text-align: center; color: #00FF00;'>✔ 无限制布局</td>
        </tr>                   
        <tr>                    
            <td style='padding: 7px; border-bottom: 1px solid #333;'>变量趋势图监控</td>
            <td style='padding: 7px; border-bottom: 1px solid #333; text-align: center;'>4条 / 30分钟</td>
            <td style='padding: 7px; border-bottom: 1px solid #333; text-align: center; color: #00FF00;'>✔ 无限制 / 内存自适应</td>
        </tr>                   
        <tr>                    
            <td style='padding: 7px; border-bottom: 1px solid #333;'>变量趋势数据导出</td>
            <td style='padding: 7px; border-bottom: 1px solid #333; text-align: center;'>✘</td>
            <td style='padding: 7px; border-bottom: 1px solid #333; text-align: center; color: #00FF00;'>✔ Excel / CSV 导出</td>
        </tr>                   
        <tr>                    
            <td style='padding: 7px; border-bottom: 1px solid #333;'>报文诊断自动化留档</td>
            <td style='padding: 7px; border-bottom: 1px solid #333; text-align: center;'>窗口实时查看</td>
            <td style='padding: 7px; border-bottom: 1px solid #333; text-align: center; color: #00FF00;'>✔ 自动存盘</td>
        </tr>
        <tr>
            <td style='padding: 7px; border-bottom: 1px solid #333;'>F11 全屏沉浸监控</td>
            <td style='padding: 7px; border-bottom: 1px solid #333; text-align: center;'>✘</td>
            <td style='padding: 7px; border-bottom: 1px solid #333; text-align: center; color: #00FF00;'>✔ 支持</td>
        </tr>
    </table>
    
         <div style='margin-top: 15px; padding: 12px; background-color: #2D2D30; border-radius: 4px; border: 1px solid #444;'>
            <div style='color: #FFD700; font-size: 10pt; font-weight: bold; margin-bottom: 8px;'> 
                <span style='margin-right: 5px;'>◆</span> 为什么选择专业版？
            </div>
            <div style='color: #BBB; font-size: 8.5pt; line-height: 1.7;'>
                • <b>长时间故障追溯</b>：解锁全天候报文自动留档。人不在场，也能精准捕捉凌晨三点的通讯波动。<br/>
                • <b>大规模工程支持</b>：解锁 1000+ 变量点位与多通道并发，应对复杂大型项目不再捉襟见肘。<br/>
                • <b>数据分析与交付</b>：解锁 Excel/CSV 原始数据导出。让调试记录变成专业的验收报告。<br/>
                • <b>沉浸式调试体验</b>：解锁 F11 全屏模式，去除所有限制弹窗。让软件回归工具本质，专注解决问题。
            </div>
         </div>

        <!-- 承诺区 -->
        <div style='margin-top: 15px; background-color: #252526; padding: 10px; border-left: 3px solid #32CD32;'>
            <div style='font-size: 8.5pt; color: #DCDCDC; font-weight: bold;'>一份属于工控人的“终身契约”</div>
            <div style='font-size: 8pt; color: #888; margin-top: 5px; line-height: 1.5;'>
                本项目源于一线调试现场的需求，坚持<b>“一次买断、永久使用、免费升级”</b>。不设订阅，不强制联网，只为您在现场能有一把趁手的“瑞士军刀”。
            </div>
        </div>

</div>";
            htmlCompare.Text = html;
        }

        private void UpdateStatus()
        {
            // 统一获取当前状态
            var status = LicenseGuard.CurrentStatus;
            lblStatus.Text = $"当前状态：{status.Message}";

            if (status.Type == LicenseType.Professional)
            {
                // 真正的专业版：绿色
                lblStatus.ForeColor = Color.Lime;
                btnActivate.Text = "更新授权";
            }
            else if (LicenseGuard.IsBetaMode)
            {
                // 正在公测中：橙色/亮绿色（醒目但不代表永久激活）
                lblStatus.ForeColor = Color.Orange;
                btnActivate.Text = "激活专业版";
            }
            else
            {
                // 真正的免费版：灰色或红色
                lblStatus.ForeColor = Color.Tomato;
                btnActivate.Text = "激活专业版";
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtMachineCode.Text);
            MessageBox.Show("机器码已复制，请在下单时粘贴至备注栏。", "提示");
        }

        private void lnkBuy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(StoreUrl) { UseShellExecute = true });
            }
            catch { MessageBox.Show("无法打开链接，请手动访问：" + StoreUrl); }
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            string key = txtLicenseKey.Text.Trim();
            if (string.IsNullOrEmpty(key)) return;

            string message;
            bool isSuccess = LicenseGuard.TryActivate(key, out message);

            if (isSuccess)
            {
                MessageBox.Show("恭喜！专业版授权已激活成功。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("激活失败：" + message, "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}