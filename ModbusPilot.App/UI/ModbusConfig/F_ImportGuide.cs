using System;
using System.Drawing;
using System.Windows.Forms;

namespace ModbusPilot.UI.Common
{
    // 如果继承 F_BaseForm，改为 : F_BaseForm
    public partial class F_ImportGuide : Form
    {
        public F_ImportGuide()
        {
            InitializeComponent();

            // 如果继承了 BaseForm，图标会自动处理
            // this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            InitGuideContent();
        }

        private void InitGuideContent()
        {
            rtbGuide.Clear();

            // === 顶部标题 ===
            AppendText("📖 智能导入操作指南\n", Color.FromArgb(0, 102, 204), 14, true);
            AppendText("Smart Import Quick Guide\n\n", Color.LightSlateGray, 9, false);

            // === 步骤 1: 加载 ===
            AppendText("1. 加载文件：", Color.Black, 11, true);
            AppendText("请直接加载待导入的 Excel (.xlsx / .xls) 或 CSV 文件。\n\n", Color.DimGray, 10);

            // === 步骤 2: 表头设置 ===
            AppendText("2. 锁定表头：", Color.Black, 11, true);
            AppendText("观察预览表格，调整左上角的 ", Color.DimGray, 10);
            AppendText("[表头所在行]", Color.FromArgb(255, 128, 0), 10, true);
            AppendText("，确保表格的第一行显示的是正确的列名称（而非标题或空行）。\n\n", Color.DimGray, 10);

            // === 步骤 3: 映射操作 ===
            AppendText("3. 建立映射 (关键)：", Color.Black, 11, true);
            AppendText("👆 点击表格列头", Color.FromArgb(0, 102, 204), 10, true);
            AppendText(" 可循环切换该列的映射类型。\n", Color.DimGray, 10);

            // 基础必选
            AppendText("   ⚠️ 基础必选：", Color.DimGray, 10);
            AppendText("[变量名称]、[Modbus地址]、[数据类型]\n", Color.Red, 10, true);

            // 特殊情况说明
            AppendText("   💡 特殊情况：", Color.DimGray, 10);
            AppendText("若使用协议纯数字地址 (如 0, 100) 而非 PLC 地址 (如 40001)，", Color.DimGray, 10);
            AppendText("必须额外映射 [存储区] 列", Color.DarkOrange, 10, true); // 用橙色强调
            AppendText(" 以区分寄存器类型。\n\n", Color.DimGray, 10);

            // === 步骤 4: 状态图例 ===
            AppendText("4. 状态图例：\n", Color.Black, 11, true);

            AppendText("   ✅ 绿色列", Color.SeaGreen, 10, true);
            AppendText(" : 已成功匹配映射。\n", Color.DimGray, 10);

            AppendText("   ⛔ 红色列", Color.Red, 10, true);
            AppendText(" : 存在映射冲突 (多列选择了同一个类型)。\n", Color.DimGray, 10);

            // 【新增】橙色/黄色警告列
            AppendText("   ⚠️ 橙色列", Color.DarkOrange, 10, true);
            AppendText(" : 数据格式疑似不匹配 (仅警告，允许强行导入)。\n", Color.DimGray, 10);

            AppendText("   ⚪ 灰色列", Color.Gray, 10, true);
            AppendText(" : 该列将被忽略，不进行导入。\n\n", Color.DimGray, 10);

            // 分隔线
            AppendText("------------------------------------------------------------------\n\n", Color.LightGray, 9);

            // === 附录: 数据类型关键字 ===
           
            AppendText("💡 ", Color.DarkSlateGray, 12, true);
            AppendText("[数据类型]", Color.FromArgb(255, 128, 0), 12, true);
            AppendText(" 列智能识别规则 (支持的关键字)：\n", Color.DarkSlateGray, 12, true);

            // 1. Bool
            AppendText("• 布尔 (Bool): ", Color.Black, 10, true);
            AppendText("BOOL, BIT, 开关, Digital, DO, DI\n", Color.DimGray, 10);

            // 2. Int16
            AppendText("• 整数 (Int16): ", Color.Black, 10, true);
            AppendText("INT, SHORT, Integer, 16Bit\n", Color.DimGray, 10);

            // 3. UInt16
            AppendText("• 无符整数 (UInt16): ", Color.Black, 10, true);
            AppendText("UINT, USHORT, WORD, Unsigned\n", Color.DimGray, 10);

            // 4. Int32
            AppendText("• 长整型 (Int32): ", Color.Black, 10, true);
            AppendText("DINT, LONG, INT32\n", Color.DimGray, 10);

            // 5. UInt32
            AppendText("• 无符长整 (UInt32): ", Color.Black, 10, true);
            AppendText("UDINT, DWORD, UINT32\n", Color.DimGray, 10);

            // 6. Float
            AppendText("• 浮点 (Float): ", Color.Black, 10, true);
            AppendText("REAL, FLOAT, Single, 浮点数\n", Color.DimGray, 10);

            // 7. Double
            AppendText("• 双精度 (Double): ", Color.Black, 10, true);
            AppendText("LREAL, DOUBLE, 双精度\n", Color.DimGray, 10);

            // === 底部提示 ===
            AppendText("⚠️ 注意：", Color.OrangeRed, 10, true);
            AppendText("如果您的数据类型不在上述范围，请在 Excel 中将其修改为上述近似的关键字再进行导入。\n\n", Color.DimGray, 9);

            AppendText("💡 ", Color.DarkSlateGray, 12, true);
            AppendText("[存储区]", Color.FromArgb(255, 128, 0), 12, true);
            AppendText("列智能识别规则 (Zone)：", Color.DarkSlateGray, 12, true);

            AppendText("• 优先识别：", Color.Black, 10, true);
            AppendText("如果映射了 [存储区] 列，优先使用该列内容。\n", Color.DimGray, 10);
            AppendText("  - 0x/线圈: 0x, Coil, DO, FC01\n", Color.DimGray, 9);
            AppendText("  - 1x/输入: 1x, DI, Input Status\n", Color.DimGray, 9);
            AppendText("  - 3x/只读: 3x, AI, Input Reg, RO\n", Color.DimGray, 9);
            AppendText("  - 4x/保持: 4x, HR, Holding, RW\n", Color.DimGray, 9);

            AppendText("• 智能推断：", Color.Black, 10, true);
            AppendText("如果未映射或识别失败，将根据地址格式自动推断 (如 40001 -> 4x)。\n", Color.DimGray, 10);

            // =========================================================
            // 【修复】将光标移回起点，防止滚动条卡在最下面，并取消选中
            // =========================================================
            rtbGuide.Select(0, 0);
            rtbGuide.ScrollToCaret();
        }

        // 【修改】增加了 fontSize 参数，默认 9f
        private void AppendText(string text, Color color, float fontSize = 9f, bool bold = false)
        {
            int start = rtbGuide.TextLength;
            rtbGuide.AppendText(text);
            int end = rtbGuide.TextLength;

            rtbGuide.Select(start, end - start);
            rtbGuide.SelectionColor = color;
            rtbGuide.SelectionFont = new Font("Microsoft YaHei UI", fontSize, bold ? FontStyle.Bold : FontStyle.Regular);
        }
    }
}