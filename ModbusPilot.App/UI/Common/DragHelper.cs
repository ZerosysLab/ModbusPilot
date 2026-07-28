using ScottPlot.PlotStyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusPilot.UI.Common
{
    public class DragHelper : Form
    {
        private Bitmap _bmp;

        public DragHelper(Control ctrl)
        {
            // 1. 截图
            _bmp = new Bitmap(ctrl.Width, ctrl.Height);
            ctrl.DrawToBitmap(_bmp, new Rectangle(Point.Empty, ctrl.Size));

            // 2. 设置窗口属性 (无边框、置顶、不抢焦点)
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Size = ctrl.Size;
            this.TopMost = true;
            this.BackColor = Color.Magenta;
            this.TransparencyKey = Color.Magenta; // 简单的透明处理
            this.Opacity = 0.7; // 半透明
            this.Enabled = false; // 鼠标穿透，不影响底层拖拽

            // 绘制图片
            this.BackgroundImage = _bmp;
            this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        public void MoveTo(Point p)
        {
            // 让窗口中心对准鼠标
            this.Location = new Point(p.X - Width / 2, p.Y - Height / 2);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            _bmp?.Dispose();
        }

        // 让窗体完全不接受鼠标事件 (鼠标穿透)
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }
    }
}
