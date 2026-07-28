using ModbusPilot.Core;
using ModbusPilot.Core.Driver;
using ModbusPilot.Core.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ModbusPilot.UI.Common
{
    public partial class UC_WidgetSwitch : UC_WidgetBase
    {
       

        public UC_WidgetSwitch(TrendDragData data, ModbusMaster master)
        {
            InitializeComponent();

            base.Init(data, master);

            chkSwitch = new CheckBox();
            chkSwitch.Appearance = Appearance.Button;
            chkSwitch.FlatStyle = FlatStyle.Flat; // 扁平化
            chkSwitch.FlatAppearance.BorderSize = 0; // 无边框

            chkSwitch.TextAlign = ContentAlignment.MiddleCenter;
            chkSwitch.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            chkSwitch.Size = new Size(120, 50);

            // 居中布局
            chkSwitch.Location = new Point((this.pnlContent.Width - chkSwitch.Width) / 2, 10);
            chkSwitch.Anchor = AnchorStyles.None; // 随容器缩放居中

            chkSwitch.Text = "OFF";
            chkSwitch.BackColor = Color.WhiteSmoke;
            chkSwitch.ForeColor = Color.Gray;

            chkSwitch.Click += ChkSwitch_Click;

            this.pnlContent.Controls.Add(chkSwitch);
        }
        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            // 立即根据当前状态刷新颜色
            UpdateStyle(chkSwitch.Checked);
        }
        public override void UpdateValue(string val)
        {
            bool isOn = (val == "True" || val == "ON" || val == "1");

            if (chkSwitch.Checked != isOn)
            {
                chkSwitch.Checked = isOn;
            }
            UpdateStyle(isOn);           
        }
        // 抽取样式方法，方便 ApplyTheme 调用
        private void UpdateStyle(bool isOn)
        {
            if (isOn)
            {
                chkSwitch.Text = "ON";
                chkSwitch.BackColor = StatusOnBack; // 使用主题色
                chkSwitch.ForeColor = StatusOnText; // 使用主题色
            }
            else
            {
                chkSwitch.Text = "OFF";
                chkSwitch.BackColor = StatusOffBack; // 使用主题色
                chkSwitch.ForeColor = StatusOffText; // 使用主题色
            }
        }
        private void ChkSwitch_Click(object sender, EventArgs e)
        {
            if (Master == null)
            {
                MessageBox.Show(
                  LangProvider.Get("Msg_ChNotStarted"),
                  LangProvider.Get("Title_NotConn"),
                  MessageBoxButtons.OK,
                  MessageBoxIcon.Warning
              );
                // 回滚 Checked 状态 (因为点击时状态已经变了，要变回去)
                chkSwitch.Checked = !chkSwitch.Checked;
                return;
            }

            bool targetVal = chkSwitch.Checked;

            try
            {
                Master.WritePoint(BoundPoint, targetVal, SlaveId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed: " + ex.Message);
                chkSwitch.Checked = !targetVal;
            }
        }
    }
}