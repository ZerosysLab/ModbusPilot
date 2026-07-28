using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ModbusPilot.Core.Models; // 确保引用了 DataType 和 StorageZone

namespace ModbusPilot.Core.Utils
{
    /// <summary>
    /// 智能导入辅助类：负责“猜测”和“解析”
    /// </summary>
    public static class ImportHelper
    {
        // ================================================================
        // 1. 核心映射定义 (The Dictionary)
        // ================================================================

        // 目标列定义 (UI显示的选项)
        public enum TargetColumn
        {
            Ignore = 0,     // 忽略
            Name,           // 变量名称
            Address,        // Modbus地址
            Zone,           // 存储区
            DataType,       // 数据类型
            Unit,           // 单位
            Note,           // 备注
            BitIndex,       // 位索引 (可选)
            Factor,         // 系数 (可选)
            Offset          // 偏移 (可选)
        }

        public static string GetColumnText(TargetColumn col)
        {
            switch (col)
            {
                case TargetColumn.Ignore: return "[ 忽略此列 ]";
                case TargetColumn.Name: return "变量名称 (Name)";
                case TargetColumn.Address: return "Modbus地址 (Addr)";
                case TargetColumn.Zone: return "存储区 (Zone)";
                case TargetColumn.DataType: return "数据类型 (Type)";
                case TargetColumn.Unit: return "单位 (Unit)";
                case TargetColumn.Note: return "备注 (Note)";
                case TargetColumn.BitIndex: return "位索引 (Bit)";
                default: return col.ToString();
            }
        }

        // ================================================================
        // 2. 智能猜测逻辑 (Auto-Guess)
        // ================================================================

        /// <summary>
        /// 根据表头文字，猜这一列是干嘛的
        /// </summary>
        public static TargetColumn GuessColumnType(string headerText)
        {
            if (string.IsNullOrWhiteSpace(headerText)) return TargetColumn.Ignore;
            string txt = headerText.Trim().ToUpper();

            // 1. 猜测名称
            if (IsMatch(txt, "NAME", "TAG", "变量", "位号", "名称", "标识")) return TargetColumn.Name;

            // 2. 猜测地址
            if (IsMatch(txt, "ADDR", "REG", "地址", "寄存器", "POINT")) return TargetColumn.Address;

            // 3. 猜测类型
            if (IsMatch(txt, "TYPE", "FMT", "FORMAT", "类型", "格式", "DATATYPE")) return TargetColumn.DataType;

            // 4. 猜测存储区
            if (IsMatch(txt, "ZONE", "AREA", "存储区", "区域", "FC", "FUNCTION")) return TargetColumn.Zone;

            // 5. 猜测单位
            if (IsMatch(txt, "UNIT", "单位", "量纲")) return TargetColumn.Unit;

            // 6. 猜测备注
            if (IsMatch(txt, "NOTE", "DESC", "备注", "描述", "说明", "COMMENT")) return TargetColumn.Note;

            return TargetColumn.Ignore;
        }

        private static bool IsMatch(string input, params string[] keywords)
        {
            foreach (var k in keywords)
            {
                if (input.Contains(k)) return true;
            }
            return false;
        }

        // ================================================================
        // 3. 数据内容解析逻辑 (The Brain)
        // ================================================================

        /// <summary>
        /// 智能解析 Modbus 地址 (兼容 40001, 00241, 100, 0x64)
        /// </summary>
        /// <param name="rawAddr">Excel里的原始字符串</param>
        /// <returns>解析出的区域和纯地址(0-based)</returns>
        public static (StorageZone Zone, int Addr) ParseAddress(string rawAddr)
        {
            if (string.IsNullOrWhiteSpace(rawAddr)) return (StorageZone.HoldingRegister_4x, 0);

            string cleanStr = rawAddr.Trim().ToUpper();

            // --- A. 处理 Hex 格式 (0x64, 64H) ---
            if (cleanStr.StartsWith("0X") || cleanStr.EndsWith("H"))
            {
                try
                {
                    cleanStr = cleanStr.Replace("0X", "").Replace("H", "");
                    int val = Convert.ToInt32(cleanStr, 16);
                    return (StorageZone.HoldingRegister_4x, val); // Hex 默认归为 4x
                }
                catch { return (StorageZone.HoldingRegister_4x, 0); }
            }

            // --- B. 处理纯数字 (PLC 逻辑地址 40001 vs 协议地址 100) ---
            if (int.TryParse(cleanStr, out int num))
            {
                // 规则：如果数字 >= 10000，认为是 PLC 逻辑地址，尝试拆解
                // 常见的 5 位或 6 位地址
                if (cleanStr.Length >= 5)
                {
                    int prefix = cleanStr[0] - '0'; // 获取首位
                    int offset = int.Parse(cleanStr.Substring(1)); // 后面的数字

                    // PLC 地址通常是从 1 开始的，转协议地址要 -1
                    // 比如 40001 -> 0
                    int protocolAddr = offset > 0 ? offset - 1 : 0;

                    switch (prefix)
                    {
                        case 0: return (StorageZone.CoilStatus_0x, protocolAddr);
                        case 1: return (StorageZone.InputStatus_1x, protocolAddr);
                        case 3: return (StorageZone.InputRegister_3x, protocolAddr);
                        case 4: return (StorageZone.HoldingRegister_4x, protocolAddr);
                    }
                }

                // 如果数字很小 (比如 100)，或者解析不出前缀
                // 默认归为 4x 区，且认为这就是协议地址
                return (StorageZone.HoldingRegister_4x, num);
            }

            // --- C. 处理带前缀的杂乱格式 (如 %MW100, W40001) ---
            // 简单粗暴：提取所有数字
            var match = Regex.Match(cleanStr, @"\d+");
            if (match.Success)
            {
                int val = int.Parse(match.Value);
                // 再次根据大小判断
                if (val >= 40000) return (StorageZone.HoldingRegister_4x, val - 40001);
                return (StorageZone.HoldingRegister_4x, val);
            }

            return (StorageZone.HoldingRegister_4x, 0);
        }

        // ModbusPilot.Core/Utils/ImportHelper.cs

        /// <summary>
        /// 尝试解析数据类型 (严格模式)
        /// </summary>
        /// <param name="rawType">原始字符串</param>
        /// <param name="result">解析结果</param>
        /// <returns>是否识别成功</returns>
        public static bool TryParseDataType(string rawType, out DataType result)
        {
            result = DataType.Int16; // 默认值
            if (string.IsNullOrWhiteSpace(rawType)) return false; // 空也是失败

            // 利用之前的逻辑，但加一个判断
            // 先记录之前的逻辑结果
            var guess = ParseDataType(rawType);

            // 怎么判断是否真的解析成功了？
            // 笨办法：检查 rawType 是否包含关键字。
            string t = rawType.Trim().ToUpper();

            // 如果 ParseDataType 返回了 Int16，但字符串里并没有 INT/WORD/SHORT 等关键字，说明是瞎猜的
            if (guess == DataType.Int16)
            {
                if (IsMatch(t, "INT", "WORD", "SHORT", "INTEGER", "16BIT"))
                {
                    result = guess;
                    return true;
                }
                return false; // 比如 "UnknownType"，ParseDataType返回Int16，但这里判错
            }

            // 其他类型既然 ParseDataType 匹配上了，肯定是对的
            result = guess;
            return true;
        }
        /// <summary>
        /// 模糊匹配数据类型
        /// </summary>
        public static DataType ParseDataType(string rawType)
        {
            if (string.IsNullOrWhiteSpace(rawType)) return DataType.Int16; // 默认

            string t = rawType.Trim().ToUpper();

            // =========================================================
            // 1. 布尔量 (1 Bit)
            // =========================================================
            // 关键字: BOOL, BIT, DIGITAL, 开关, DO, DI, 0/1
            if (t.Contains("BOOL") || t.Contains("BIT") || t.Contains("DIGITAL") || t.Contains("开关") ||
                t == "DO" || t == "DI")
                return DataType.Bool;

            // =========================================================
            // 2. 有符号整数 (16 Bit) - Int16
            // =========================================================
            // 关键字: INT, SHORT, INTEGER, 16BIT
            // 注意：因为 UINT/DINT 也包含 "INT"，所以必须排除它们，防止误判
            if (t.Contains("SHORT") || t.Contains("INTEGER") || t.Contains("16BIT") ||
               (t.Contains("INT") && !t.Contains("UINT") && !t.Contains("DINT") && !t.Contains("LINT")))
                return DataType.Int16;

            // =========================================================
            // 3. 无符号整数 (16 Bit) - UInt16
            // =========================================================
            // 关键字: UINT, USHORT, WORD, UNSIGNED, UWORD
            if (t.Contains("UINT") || t.Contains("USHORT") || t.Contains("WORD") || t.Contains("UNSIGNED"))
                return DataType.UInt16;

            // =========================================================
            // 4. 有符号长整型 (32 Bit) - Int32
            // =========================================================
            // 关键字: DINT, INT32, LONG
            if (t.Contains("DINT") || t.Contains("INT32") || t.Contains("LONG"))
                return DataType.Int32;

            // =========================================================
            // 5. 无符号长整型 (32 Bit) - UInt32
            // =========================================================
            // 关键字: UDINT, UINT32, ULONG, DWORD
            if (t.Contains("UDINT") || t.Contains("UINT32") || t.Contains("ULONG") || t.Contains("DWORD"))
                return DataType.UInt32;

            // =========================================================
            // 6. 单精度浮点 (32 Bit) - Float
            // =========================================================
            // 关键字: REAL, FLOAT, SINGLE, 浮点
            if (t.Contains("REAL") || t.Contains("FLOAT") || t.Contains("SINGLE") || t.Contains("浮点"))
                return DataType.Float;

            // =========================================================
            // 7. 双精度浮点 (64 Bit) - Double
            // =========================================================
            // 关键字: LREAL, DOUBLE, 双精度
            if (t.Contains("LREAL") || t.Contains("DOUBLE") || t.Contains("双精度"))
                return DataType.Double;

            // =========================================================
            // 兜底默认值
            // =========================================================
            return DataType.Int16;
        }

        /// <summary>
        /// 智能解析存储区 (支持：0x, Coil, DO, RW, 3x, Input, RO 等)
        /// 返回 null 表示无法识别
        /// </summary>
        public static StorageZone? ParseStorageZone(string rawZone)
        {
            if (string.IsNullOrWhiteSpace(rawZone)) return null;

            string t = rawZone.Trim().ToUpper();

            // 1. 线圈 (0x)
            // 关键字: 0x, Coil, DO (Digital Output), FC01, FC05, FC15
            if (t.Contains("0X") || t.Contains("COIL") || t == "DO" ||
                t.Contains("FC01") || t.Contains("FC1") || t.Contains("FC05") || t.Contains("FC5"))
                return StorageZone.CoilStatus_0x;

            // 2. 离散输入 (1x)
            // 关键字: 1x, Input Status, DI (Digital Input), FC02, FC2
            if (t.Contains("1X") || (t.Contains("INPUT") && t.Contains("STAT")) || t == "DI" ||
                t.Contains("FC02") || t.Contains("FC2"))
                return StorageZone.InputStatus_1x;

            // 3. 输入寄存器 (3x) - 只读模拟量
            // 关键字: 3x, Input Reg, AI (Analog Input), IR, FC04, FC4
            // 注意：单纯的 "INPUT" 可能有歧义，但在寄存器语境下通常指 3x
            if (t.Contains("3X") || (t.Contains("INPUT") && t.Contains("REG")) || t == "AI" || t == "IR" ||
                t.Contains("FC04") || t.Contains("FC4"))
                return StorageZone.InputRegister_3x;

            // 4. 保持寄存器 (4x) - 读写模拟量
            // 关键字: 4x, Holding, HR, AO (Analog Output), Data, FC03, FC3, FC06, FC16
            if (t.Contains("4X") || t.Contains("HOLD") || t == "HR" || t == "AO" ||
                t.Contains("FC03") || t.Contains("FC3") || t.Contains("FC06") || t.Contains("FC16"))
                return StorageZone.HoldingRegister_4x;

            // 5. 模糊推断：读写属性 (不一定准，作为补充)
            // 如果写了 "R/W" 或 "RW"，通常是 4x 或 0x，优先归为 4x
            if (t == "RW" || t == "R/W") return StorageZone.HoldingRegister_4x;

            // 如果写了 "RO" (ReadOnly)，通常是 3x 或 1x，优先归为 3x
            if (t == "RO") return StorageZone.InputRegister_3x;

            // 无法识别
            return null;
        }
    }
}