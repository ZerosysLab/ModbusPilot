using System;
using System.Text;

namespace ModbusPilot.Core.Utils
{
    public static class ProtocolInterpreter
    {
        /// <summary>
        /// 翻译 Modbus 报文为人类可读的描述
        /// </summary>
        /// <param name="frame">完整报文 (含 MBAP 或 CRC)</param>
        /// <param name="isRequest">true=发送(TX), false=接收(RX)</param>
        /// <param name="isTcp">true=ModbusTCP, false=ModbusRTU</param>
        public static string Interpret(byte[] frame, bool isRequest, bool isTcp)
        {
            try
            {
                if (frame == null || frame.Length == 0) return "空报文";

                // 【问题回答】收到 1 个字节通常是噪声
                if (frame.Length < 3)
                {
                    return $"数据残缺/噪声 (Len:{frame.Length})";
                }

                // 1. 确定协议偏移量 (TCP 头 6 字节，RTU 头 0 字节)
                int offset = isTcp ? 6 : 0;

                // 安全检查：如果 TCP 报文甚至连头都不全，或者 RTU 连 ID+FC 都不全
                if (frame.Length <= offset + 1) return "报文头不完整";

                // 2. 提取核心字段
                byte slaveId = frame[offset];
                byte fc = frame[offset + 1];

                StringBuilder sb = new StringBuilder();
                sb.Append($"[ID:{slaveId}] ");

                // 3. 处理 TCP 特有的 MBAP 头信息 (仅调试用，一般不打印)
                // if (isTcp) { ... }

                // 4. 处理异常响应 (最高位为 1，例如 0x83)
                if ((fc & 0x80) != 0)
                {
                    byte originalFc = (byte)(fc & 0x7F);
                    byte errCode = (frame.Length > offset + 2) ? frame[offset + 2] : (byte)0;

                    sb.Append($"❌ 异常响应 (FC:{originalFc:X2}) - {GetErrorDescription(errCode)}");
                    return sb.ToString();
                }

                // 5. 处理正常功能码
                switch (fc)
                {
                    case 0x01: // Read Coils
                    case 0x02: // Read Discrete Inputs
                    case 0x03: // Read Holding Registers
                    case 0x04: // Read Input Registers
                        // 传入 isTcp 以便计算 CRC 长度
                        sb.Append(InterpretRead(frame, offset, isRequest, fc, isTcp));
                        break;

                    case 0x05: // Write Single Coil
                        sb.Append(InterpretWriteSingle(frame, offset, isRequest, "线圈", isTcp));
                        break;

                    case 0x06: // Write Single Register
                        sb.Append(InterpretWriteSingle(frame, offset, isRequest, "寄存器", isTcp));
                        break;

                    case 0x0F: // Write Multiple Coils
                    case 0x10: // Write Multiple Registers
                        sb.Append(InterpretWriteMultiple(frame, offset, isRequest, fc, isTcp));
                        break;

                    default:
                        sb.Append($"功能码 {fc:X2} (未知/未解析)");
                        break;
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"解析异常: {ex.Message}";
            }
        }

        // --- 内部辅助方法 (已优化长度检查) ---

        private static string InterpretRead(byte[] f, int offset, bool isRequest, byte fc, bool isTcp)
        {
            string area = fc switch
            {
                0x01 => "线圈(0x)",
                0x02 => "离散(1x)",
                0x03 => "保持(4x)",
                0x04 => "输入(3x)",
                _ => "??"
            };

            if (isRequest)
            {
                // TX: [ID] [FC] [AddrHi] [AddrLo] [QtyHi] [QtyLo]
                int expectedLen = offset + 6;
                if (!isTcp) expectedLen += 2; // RTU 有 CRC

                if (f.Length < expectedLen) return $"读{area}请求 - 长度不足";

                ushort addr = (ushort)((f[offset + 2] << 8) | f[offset + 3]);
                ushort qty = (ushort)((f[offset + 4] << 8) | f[offset + 5]);
                return $"读{area}: 地址 {addr}, 数量 {qty}";
            }
            else
            {
                // RX: [ID] [FC] [Bytes] [Data...] [CRC?]
                if (f.Length < offset + 3) return $"读{area}响应 - 头不完整";

                byte bytes = f[offset + 2];
                int expectedDataEnd = offset + 3 + bytes;

                // 检查数据是否收全
                if (f.Length < expectedDataEnd)
                    return $"读{area}响应 - 数据残缺 ({f.Length}/{expectedDataEnd})";

                // RTU 额外检查 CRC 长度
                if (!isTcp && f.Length < expectedDataEnd + 2)
                    return $"读{area}响应 - 缺少CRC";

                // 【优化】响应不打印具体值，太冗余了，只打印收到的字节数
                return $"读{area}响应: 成功返回 {bytes} 字节";
            }
        }

        private static string InterpretWriteSingle(byte[] f, int offset, bool isRequest, string type, bool isTcp)
        {
            // Write Single 的 TX 和 RX 长度固定
            // PDU = 6 bytes (Addr:2 + Val:2)
            int expectedLen = offset + 6;
            if (!isTcp) expectedLen += 2; // RTU CRC

            if (f.Length < expectedLen) return $"写{type} - 长度不足";

            ushort addr = (ushort)((f[offset + 2] << 8) | f[offset + 3]);
            ushort val = (ushort)((f[offset + 4] << 8) | f[offset + 5]);

            string valStr = type == "线圈" ? (val == 0xFF00 ? "ON" : "OFF") : val.ToString();
            string action = isRequest ? "请求写入" : "写入确认";

            return $"{action} {type}: 地址 {addr} -> {valStr}";
        }

        private static string InterpretWriteMultiple(byte[] f, int offset, bool isRequest, byte fc, bool isTcp)
        {
            string type = fc == 0x0F ? "多线圈" : "多寄存器";

            if (isRequest)
            {
                // TX: [Addr:2] [Qty:2] [Bytes:1] [Data...]
                if (f.Length < offset + 7) return $"写{type}请求 - 头不完整";

                byte bytes = f[offset + 6];
                int expectedLen = offset + 7 + bytes;
                if (!isTcp) expectedLen += 2; // CRC

                if (f.Length < expectedLen) return $"写{type}请求 - 数据残缺";

                ushort addr = (ushort)((f[offset + 2] << 8) | f[offset + 3]);
                ushort qty = (ushort)((f[offset + 4] << 8) | f[offset + 5]);
                return $"写{type}: 地址 {addr}, 数量 {qty} ({bytes}字节)";
            }
            else
            {
                // RX: [Addr:2] [Qty:2] (固定长度)
                int expectedLen = offset + 6;
                if (!isTcp) expectedLen += 2; // CRC

                if (f.Length < expectedLen) return $"写{type}响应 - 长度不足";

                ushort addr = (ushort)((f[offset + 2] << 8) | f[offset + 3]);
                ushort qty = (ushort)((f[offset + 4] << 8) | f[offset + 5]);
                return $"写{type}成功: 地址 {addr}, 数量 {qty}";
            }
        }

        private static string GetErrorDescription(byte code)
        {
            return code switch
            {
                0x01 => "功能码不支持",
                0x02 => "地址越界",
                0x03 => "数据值非法",
                0x04 => "设备故障",
                0x05 => "确认",
                0x06 => "设备忙",
                0x0B => "网关超时",
                _ => $"未知错误 {code:X2}"
            };
        }
    }
}