using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheArtOfDev.HtmlRenderer.WinForms;

namespace ModbusPilot.App
{
    public partial class F_Notice : Form
    {
        public F_Notice(string content)
        {
            InitializeComponent();

            // --- 1. 窗体基础风格 ---
            this.Text = "📢 系统公告";
            this.Size = new Size(480, 320);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 【关键】将窗体底色设为与 HTML 一致的深色，彻底干掉白边
            Color themeBack = Color.FromArgb(30, 30, 30);
            this.BackColor = themeBack;

            // --- 2. HTML 内容区 ---
            HtmlPanel html = new HtmlPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent, // 设为透明，直接透出窗体的深色
                IsContextMenuEnabled = false,
                IsSelectionEnabled = false,
                // 解决 HTML 内部可能出现的白色边距
                BaseStylesheet = "body { margin: 0; padding: 0; border: none; }"
            };

            // 构造 HTML 字符串
            html.Text = $@"
            <div style='font-family: ""Microsoft YaHei UI""; padding: 25px; color: #DCDCDC; line-height: 1.6;'>
                <div style='font-size: 15pt; color: gold; font-weight: bold; margin-bottom: 15px; border-bottom: 1px solid #444; padding-bottom: 10px;'>
                    最新公告
                </div>
                <div style='font-size: 10pt;'>
                    {content.Replace("\n", "<br/>")}
                </div>
            </div>";

            // --- 3. 底部按钮容器 (增加间距，避免按钮太突兀) ---
            Panel pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(10, 10, 10, 10), // 让按钮四周有呼吸感
                BackColor = themeBack
            };

            Button btnOk = new Button
            {
                Text = "我知道了",
                Dock = DockStyle.Right, // 靠右对齐更符合 Windows 规范
                Width = 100,
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(63, 63, 70), // 科技灰
                ForeColor = Color.White,
                Font = new Font("Microsoft YaHei UI", 9F)
            };
            btnOk.FlatAppearance.BorderSize = 0; // 去掉按钮默认边框
            btnOk.Click += (s, e) => this.Close();

            // 组合控件
            pnlBottom.Controls.Add(btnOk);
            this.Controls.Add(html);
            this.Controls.Add(pnlBottom);
        }
    }
}
