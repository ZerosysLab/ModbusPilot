using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModbusPilot.UI
{
    public partial class F_BaseForm : Form
    {
        public F_BaseForm()
        {
            // 核心魔法：所有继承此类的窗口，自动提取 EXE 的图标
            // 这样你以后换了 EXE 图标，所有窗口都会自动更新
            try
            {
                // 注意：设计器模式下这段代码可能会报错，所以加个判断
                if (!DesignMode)
                {
                    var exePath = Application.ExecutablePath;
                    this.Icon = Icon.ExtractAssociatedIcon(exePath);
                }
            }
            catch { /* 忽略异常，防止设计器崩溃 */ }
        }
    }
}
