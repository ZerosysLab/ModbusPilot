using ModbusPilot.Core;
using ModbusPilot.Core.Services;
using ModbusPilot.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModbusPilot.App
{
    public partial class F_About : F_BaseForm
    {
        public F_About()
        {
            InitializeComponent();

            SetupUI();      // 设置窗口属性 & 动态版本信息

            ApplyUIText();  // 【新增】应用语言字典
        }

        private void SetupUI()
        {
            this.Text = "About ModbusPilot";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            // 获取当前运行的 EXE 路径
            string exePath = Application.ExecutablePath;

            // 读取文件信息
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(exePath);

            //// 1. 获取版本信息 (自动读取 Directory.Build.props 里的配置)
            //var assembly = Assembly.GetExecutingAssembly();
            //var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            // 2. 动态填充标签 (假设你在设计器里拖了 labelAppName, labelVer, labelCopy)
            // 如果不想拖控件，这里也可以纯代码生成，为了省事建议设计器拖一下
            lblVersion.Text = $"Version {fileVersionInfo.FileVersion}";
            lblCopyright.Text = fileVersionInfo.LegalCopyright; // "Copyright © 2025 Zerosys Lab..."
            lblCompany.Text = fileVersionInfo.CompanyName;      // "Zerosys Lab"
        }

        // ==========================================
        // 【核心】实现界面文字替换
        // ==========================================
        private void ApplyUIText()
        {
            // 1. 窗口标题
            this.Text = LangProvider.Get("About_Title");

            // 2. 描述文字
            txtDescription.Text = LangProvider.Get("About_Desc");

            // 3. 链接文字 (可选，如果你想让链接也变的话)
            // linkWeb.Text = LangProvider.Get("About_Link");

            // 4. 按钮
            btnOk.Text = LangProvider.Get("Btn_OK");
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkWeb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // 访问官网
            OpenUrl(LinkManager.HomeUrl);
        }
        // GitHub 链接点击事件
        private void btnGithub_Click(object sender, EventArgs e)
        {
            // 访问官网
            OpenUrl(LinkManager.HomeUrl);
        }

        // 通用打开网页方法 (.NET 8 需要 UseShellExecute)
        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                string msg = string.Format(LangProvider.Get("Msg_OpenUrlErr"), ex.Message);
                MessageBox.Show(msg, LangProvider.Get("Title_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
