using System.Drawing;
using System.Windows.Forms;
using ModbusPilot.Core.Models;
using ModbusPilot.Core.Driver;

namespace ModbusPilot.UI.Common
{
    public partial class UC_WidgetMonitor : UC_WidgetBase
    {
       

        public UC_WidgetMonitor(TrendDragData data, ModbusMaster master)
        {
            InitializeComponent();

            base.Init(data, master);

            lblValue = new Label();
            lblValue.Dock = DockStyle.Fill; // 填满 pnlContent
            lblValue.TextAlign = ContentAlignment.MiddleCenter;
            lblValue.Font = new Font("Segoe UI", 22F, FontStyle.Regular); // 更现代的字体
            lblValue.Text = "-";
            lblValue.ForeColor = Color.FromArgb(64, 64, 64); // 深灰

            this.pnlContent.Controls.Add(lblValue);
        }

        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            // 强制触发一次颜色更新逻辑
            string currentVal = lblValue.Text;

            // 如果不是开关量，直接应用主文字色
            if (currentVal != "ON" && currentVal != "OFF")
            {
                lblValue.ForeColor = theme.TextPrimary;
            }
            else
            {
                // 如果是开关量，根据当前字面值重新染色
                if (currentVal == "ON") lblValue.ForeColor = theme.StatusOnBack;
                else lblValue.ForeColor = theme.StatusOffText;
            }
        }

        public override void UpdateValue(string val)
        {
            // 1. 如果是 Bool 类型的变量（线圈、离散输入、或寄存器位）
            if (BoundPoint.DataType == DataType.Bool)
            {
                // 使用更专业的“真值判定”逻辑
                bool isOn = IsTruthful(BoundPoint.CurrentValue);

                string targetText = isOn ? "ON" : "OFF";
                Color targetColor = isOn ? StatusOnBack : StatusOffText;

                // 只有变化时才更新 UI，减少闪烁
                if (lblValue.Text != targetText)
                {
                    lblValue.Text = targetText;
                    lblValue.ForeColor = targetColor;
                }
            }
            else
            {
                // 2. 如果是数值类型（Int, Float, Double）
                if (lblValue.Text != val)
                {
                    lblValue.Text = val;
                    lblValue.ForeColor = CurrentTextColor;
                }
            }
        }

        /// <summary>
        /// 工业级“真值”判定助手：把各种 Modbus 数据统一转换为布尔状态
        /// </summary>
        private bool IsTruthful(object value)
        {
            if (value == null) return false;

            // A. 已经是 bool 类型
            if (value is bool b) return b;

            // B. 数值类型 (0 为 False, 非 0 为 True)
            if (double.TryParse(value.ToString(), out double d))
            {
                return d != 0;
            }

            // C. 字符串兜底
            string s = value.ToString().Trim().ToUpper();
            return s == "TRUE" || s == "ON" || s == "1" || s == "OPEN";
        }
    }
}