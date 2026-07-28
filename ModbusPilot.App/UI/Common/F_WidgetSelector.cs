using ModbusPilot.Core;
using ModbusPilot.Core.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ModbusPilot.UI.Common
{
    public partial class F_WidgetSelector : F_BaseForm
    {
        public WidgetMode SelectedMode { get; private set; }

        public F_WidgetSelector(ModbusPoint point)
        {
            InitializeComponent();

            //this.Text = "选择卡片类型";
            this.Text = LangProvider.Get("Sel_Title");

            // 2. 设置顶部信息 (使用 Format 填充参数)
            string fmt = LangProvider.Get("Sel_PointInfo");
            lblPointName.Text = string.Format(fmt, point.Name, point.Address);

            GenerateButtons(point);
        }

        private void GenerateButtons(ModbusPoint point)
        {
            // 1. 基础卡片：所有点位都支持“数值/状态监视”
            // 使用字典替换硬编码
            AddButton(LangProvider.Get("Sel_Btn_Monitor"), WidgetMode.Monitor, Color.WhiteSmoke);

            // 2. 开关控制：仅支持 0x 线圈
            if (point.Zone == StorageZone.CoilStatus_0x)
            {
                AddButton(LangProvider.Get("Sel_Btn_Switch"), WidgetMode.Switch, Color.AliceBlue);
            }

            // 3. 数值设定：仅支持 4x 寄存器，且不是 Bool 类型 (V0.9限制)
            if (point.Zone == StorageZone.HoldingRegister_4x && point.DataType != DataType.Bool)
            {
                AddButton(LangProvider.Get("Sel_Btn_Control"), WidgetMode.Control, Color.Lavender);
            }
        }

        private void AddButton(string text, WidgetMode mode, Color color)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Size = new Size(260, 45); // 按钮大一点，方便点
            btn.BackColor = color;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Margin = new Padding(5);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Font = new Font("Microsoft YaHei UI", 10); // 建议统一用微软雅黑，英文显示也好

            btn.Click += (s, e) =>
            {
                SelectedMode = mode;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            panelButtons.Controls.Add(btn);
        }

        // --- Designer Code ---


       
    }
}