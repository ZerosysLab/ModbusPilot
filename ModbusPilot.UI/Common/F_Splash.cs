using ModbusPilot.Core.Utils;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ModbusPilot.UI.Common
{
    public partial class F_Splash : Form
    {
        public F_Splash()
        {
            InitializeComponent();

            lblSub.Text = "Ver:" + AppInfoHelper.Version;
        }

        // 重写 OnPaint 画一个细细的灰色边框，增加精致感
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Pen p = new Pen(Color.FromArgb(60, 60, 60), 1))
            {
                e.Graphics.DrawRectangle(p, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

        /// <summary>
        /// 提供给 Program.cs 更新状态文字
        /// </summary>
        public void UpdateStatus(string msg)
        {
            // 确保线程安全
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new Action(() => UpdateStatus(msg)));
            }
            else
            {
                lblStatus.Text = msg;
                lblStatus.Refresh(); // 强制重绘，防止阻塞时文字不更新

                // 顺便让界面响应一下，防止“假死”感
                Application.DoEvents();
            }
        }
    }
}