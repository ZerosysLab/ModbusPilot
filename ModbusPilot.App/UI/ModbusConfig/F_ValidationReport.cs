using System;
using System.Drawing;
using System.Windows.Forms;

namespace ModbusPilot.UI.Common
{
    // 如果你有 F_BaseForm，建议这里改为 : F_BaseForm
    public partial class F_ValidationReport : Form
    {
        public F_ValidationReport(int valid, int warning, int critical, string detailLog)
        {
            InitializeComponent();

            // 如果继承了 F_BaseForm，图标会自动处理；如果没有，这里手动设置
            // this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            // 1. 设置统计信息
            lblValid.Text = $"✅ 有效数据: {valid} 行";
            lblValid.ForeColor = Color.Green;

            lblWarning.Text = $"⚠️ 警告 (类型未知/默认): {warning} 行";
            lblWarning.ForeColor = (warning > 0) ? Color.DarkOrange : Color.Gray;

            lblCritical.Text = $"⛔ 严重错误 (将跳过): {critical} 行";
            lblCritical.ForeColor = (critical > 0) ? Color.Red : Color.Gray;

            // 2. 填充日志
            rtbLog.Text = detailLog;

            // 3. 逻辑控制：如果没有有效数据，禁止“继续”
            btnContinue.Enabled = (valid > 0);
        }
    }
}