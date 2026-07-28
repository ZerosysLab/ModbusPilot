using ModbusPilot.Core;
using ModbusPilot.Core.Driver;
using ModbusPilot.Core.Models;
using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ModbusPilot.UI.Common
{
    public partial class UC_WidgetControl : UC_WidgetBase
    {
        // 【新增】缓存前缀，优化 UpdateValue 性能
        private string _strCurrentPrefix = "当前: ";

        public UC_WidgetControl(TrendDragData data, ModbusMaster master)
        {
            InitializeComponent();

            base.Init(data, master);

            // === 3. 样式设置 (不需要再 new 了，直接用) ===

            // 如果你在 Designer 里没设这些样式，可以在这里补全
            // 如果 Designer 里已经设了，这部分可以删除
            lblCurrent.ForeColor = Color.DimGray;

            txtInput.BorderStyle = BorderStyle.FixedSingle;

            btnSet.FlatStyle = FlatStyle.Flat;
            btnSet.BackColor = Color.FromArgb(0, 122, 204);
            btnSet.ForeColor = Color.White;
            btnSet.FlatAppearance.BorderSize = 0;
            btnSet.Cursor = Cursors.Hand;

            ApplyUIText();
        }
        public  void ApplyUIText()
        {
            //base.ApplyUIText();

            //// 1. 按钮文字
            //btnSet.Text = LangProvider.Get("Card_Btn_Set");

            //// 2. 更新缓存的前缀
            //_strCurrentPrefix = LangProvider.Get("Card_Lbl_Curr");

            //// 3. 立即刷新 Label 显示 (如果当前有值的话)
            //// 简单做法：把当前的数值部分抠出来重新拼，或者等下一次 UpdateValue
            //// 这里为了即时响应，尝试提取数值：
            //if (lblCurrent.Text.Contains(":"))
            //{
            //    string[] parts = lblCurrent.Text.Split(':');
            //    if (parts.Length > 1)
            //    {
            //        string val = parts[1].Trim();
            //        lblCurrent.Text = $"{_strCurrentPrefix}{val}";
            //    }
            //}
            //else
            //{
            //    lblCurrent.Text = $"{_strCurrentPrefix}-";
            //}
        }
        public override void UpdateValue(string val)
        {
            lblCurrent.Text = $"{_strCurrentPrefix}{val}";
            //lblCurrent.Text = $"当前: {val}";
        }
        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            lblCurrent.ForeColor = theme.TextPrimary; // 当前值也用亮色

            // 输入框美化
            txtInput.BackColor = ControlPaint.Dark(theme.CardBack, 0.1f); // 比卡片背景稍微深一点
            txtInput.ForeColor = theme.TextPrimary; // 输入文字也是亮色
            txtInput.BorderStyle = BorderStyle.FixedSingle;

            // 按钮保持 Accent 色或者特定色
            btnSet.BackColor = theme.Accent;
            // 如果 Accent 是亮色(如荧光青)，按钮文字最好是黑色；如果是深色，文字是白色
            // 简单判断亮度：
            if (theme.Accent.GetBrightness() > 0.6)
                btnSet.ForeColor = Color.Black;
            else
                btnSet.ForeColor = Color.White;
        }
        private void BtnSet_Click(object sender, EventArgs e)
        {
            if (Master == null)
            {
                //MessageBox.Show("该通道尚未启动，无法下发指令。", "未连接", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                MessageBox.Show(
                   LangProvider.Get("Msg_ChNotStarted"),
                   LangProvider.Get("Title_NotConn"),
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Warning
               ); 
                return;
            }

            // 1. 获取输入
            string inputStr = txtInput.Text;
            if (string.IsNullOrWhiteSpace(inputStr)) return;

            try
            {
                var p = BoundPoint; // 基类属性

                // =================================================================
                // 量化误差预检查 (逻辑复用自 F_DeviceMonitor)
                // =================================================================

                // A. 判断是否为 "整数寄存器" (4x 区且非 Float/Double/Bool)
                bool isIntegerReg = p.Zone == StorageZone.HoldingRegister_4x &&
                                    p.DataType != DataType.Float &&
                                    p.DataType != DataType.Double &&
                                    p.DataType != DataType.Bool;

                // B. 判断是否有 "有效系数" (不等于 1)
                bool hasScaling = Math.Abs(p.Factor - 1.0) > 0.000001;

                if (isIntegerReg && hasScaling)
                {
                    if (double.TryParse(inputStr, out double userVal))
                    {
                        // C. 模拟写入：逆向计算 + 取整
                        // (Val - Offset) / Factor
                        double rawCalc = (userVal - p.Offset) / p.Factor;

                        // 使用 AwayFromZero 保持与 ValueEncoder 一致
                        long rawInt = (long)Math.Round(rawCalc, MidpointRounding.AwayFromZero);

                        // D. 模拟读取：正向计算 (回算实际值)
                        // (Raw * Factor) + Offset
                        double actualVal = (rawInt * p.Factor) + p.Offset;

                        // E. 比较差异
                        // 如果差异 > 0.001 (浮点容差)，说明发生了精度丢失
                        if (Math.Abs(userVal - actualVal) > 0.001)
                        {
                            // 【修改】字典替换 (格式化字符串)
                            string msgFormat = LangProvider.Get("Card_Fix_Msg");
                            string msg = string.Format(msgFormat, p.Factor, userVal, actualVal);

                            string title = LangProvider.Get("Card_Fix_Title");

                            //string msg = $"【精度修正提示】\r\n\r\n" +
                            //             $"由于当前系数 ({p.Factor}) 的限制，设备无法存储 {userVal}。\r\n" +
                            //             $"最接近的有效值为: {actualVal}\r\n\r\n" +
                            //             $"点击 [确定] 将自动修正并写入。\r\n" +
                            //             $"点击 [取消] 放弃操作。";

                            if (MessageBox.Show(msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                            {
                                // 修正输入值
                                inputStr = actualVal.ToString();
                                txtInput.Text = inputStr; // 更新界面显示
                            }
                            else
                            {
                                return; // 用户取消
                            }
                        }
                    }
                }
                // =================================================================

                // 2. 下发指令
                Master.WritePoint(BoundPoint, inputStr, SlaveId);

                // 3. 成功后清空输入框，表示已发送
                txtInput.Text = "";
            }
            catch (Exception ex)
            {
                string errMsg = string.Format(LangProvider.Get("Card_Write_Err"), ex.Message);
                MessageBox.Show(errMsg, LangProvider.Get("Title_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}