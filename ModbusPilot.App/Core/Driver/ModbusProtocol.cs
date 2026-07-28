using ModbusPilot.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModbusPilot.Core.Driver
{
    public static class ModbusProtocol
    {
        /// <summary>
        /// 构建发送报文 (包含 CRC)
        /// 支持 FC: 01, 02, 03, 04, 05, 06, 15, 16
        /// </summary>
        public static byte[] BuildMessage(ModbusCommand cmd)
        {
            List<byte> frame = new List<byte>();

            // 1. 站号 & 功能码
            frame.Add(cmd.SlaveId);
            frame.Add(cmd.FunctionCode);

            // 2. 数据区 (根据功能码不同而不同)
            switch (cmd.FunctionCode)
            {
                case 01: // Read Coils
                case 02: // Read Discrete Inputs
                case 03: // Read Holding Registers
                case 04: // Read Input Registers
                    // 格式：Addr(2) + Count(2)
                    frame.Add((byte)(cmd.StartAddress >> 8));
                    frame.Add((byte)(cmd.StartAddress & 0xFF));
                    frame.Add((byte)(cmd.Count >> 8));
                    frame.Add((byte)(cmd.Count & 0xFF));
                    break;

                case 05: // Write Single Coil
                case 06: // Write Single Register
                    // 格式：Addr(2) + Value(2)
                    frame.Add((byte)(cmd.StartAddress >> 8));
                    frame.Add((byte)(cmd.StartAddress & 0xFF));
                    // 单个写入的数据在 WritePayload 中，必须是2个字节
                    if (cmd.WritePayload != null && cmd.WritePayload.Length >= 2)
                    {
                        frame.Add(cmd.WritePayload[0]);
                        frame.Add(cmd.WritePayload[1]);
                    }
                    else
                    {
                        frame.Add(0x00); frame.Add(0x00); // 防崩
                    }
                    break;

                case 15: // Write Multiple Coils
                case 16: // Write Multiple Registers
                    // 格式：Addr(2) + Count(2) + ByteCount(1) + Data(n)
                    frame.Add((byte)(cmd.StartAddress >> 8));
                    frame.Add((byte)(cmd.StartAddress & 0xFF));
                    frame.Add((byte)(cmd.Count >> 8));
                    frame.Add((byte)(cmd.Count & 0xFF));

                    byte byteCount = (byte)(cmd.WritePayload?.Length ?? 0);
                    frame.Add(byteCount);

                    if (byteCount > 0)
                    {
                        frame.AddRange(cmd.WritePayload);
                    }
                    break;

                default:
                    throw new NotSupportedException($"Function Code {cmd.FunctionCode} not supported");
            }

            // 3. 计算并附加 CRC
            byte[] crc = CalculateCRC(frame.ToArray());
            frame.Add(crc[0]);
            frame.Add(crc[1]);

            return frame.ToArray();
        }

        /// <summary>
        /// 构建 PDU（不含 UnitId 与 CRC），用于 Modbus TCP
        /// 返回：FunctionCode + Data
        /// </summary>
        public static byte[] BuildPdu(ModbusCommand cmd)
        {
            List<byte> pdu = new List<byte>();
            // Function code
            pdu.Add(cmd.FunctionCode);

            switch (cmd.FunctionCode)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    pdu.Add((byte)(cmd.StartAddress >> 8));
                    pdu.Add((byte)(cmd.StartAddress & 0xFF));
                    pdu.Add((byte)(cmd.Count >> 8));
                    pdu.Add((byte)(cmd.Count & 0xFF));
                    break;
                case 5:
                case 6:
                    pdu.Add((byte)(cmd.StartAddress >> 8));
                    pdu.Add((byte)(cmd.StartAddress & 0xFF));
                    if (cmd.WritePayload != null && cmd.WritePayload.Length >= 2)
                    {
                        pdu.Add(cmd.WritePayload[0]);
                        pdu.Add(cmd.WritePayload[1]);
                    }
                    else
                    {
                        pdu.Add(0x00); pdu.Add(0x00);
                    }
                    break;
                case 15:
                case 16:
                    pdu.Add((byte)(cmd.StartAddress >> 8));
                    pdu.Add((byte)(cmd.StartAddress & 0xFF));
                    pdu.Add((byte)(cmd.Count >> 8));
                    pdu.Add((byte)(cmd.Count & 0xFF));
                    byte byteCount = (byte)(cmd.WritePayload?.Length ?? 0);
                    pdu.Add(byteCount);
                    if (byteCount > 0) pdu.AddRange(cmd.WritePayload);
                    break;
                default:
                    throw new NotSupportedException($"Function Code {cmd.FunctionCode} not supported");
            }

            return pdu.ToArray();
        }

        /// <summary>
        /// 为 Modbus TCP 构建 MBAP + PDU 报文（不含 CRC）
        /// </summary>
        public static byte[] BuildTcpMessage(ModbusCommand cmd, ushort transactionId)
        {
            byte[] pdu = BuildPdu(cmd);
            ushort len = (ushort)(1 + pdu.Length); // unit id + pdu
            byte[] mbap = new byte[7 + pdu.Length];
            mbap[0] = (byte)((transactionId >> 8) & 0xFF);
            mbap[1] = (byte)(transactionId & 0xFF);
            mbap[2] = 0; mbap[3] = 0; // protocol id
            mbap[4] = (byte)((len >> 8) & 0xFF);
            mbap[5] = (byte)(len & 0xFF);
            mbap[6] = cmd.SlaveId;
            if (pdu.Length > 0) Array.Copy(pdu, 0, mbap, 7, pdu.Length);
            return mbap;
        }

        /// <summary>
        /// 计算预期响应长度 (用于接收时判断是否读完了)
        /// 此方法返回 RTU 风格长度（SlaveID + FC + Data + CRC）
        /// </summary>
        public static int CalcResponseLen(ModbusCommand cmd)
        {
            // 基础开销：SlaveID(1) + FC(1) + CRC(2) = 4字节
            int baseLen = 4;

            switch (cmd.FunctionCode)
            {
                case 01:
                case 02:
                    // 响应：ByteCount(1) + Data(N)
                    // N = (Count + 7) / 8
                    int bytesCoil = (cmd.Count + 7) / 8;
                    return baseLen + 1 + bytesCoil;

                case 03:
                case 04:
                    // 响应：ByteCount(1) + Data(N*2)
                    return baseLen + 1 + (cmd.Count * 2);

                case 05:
                case 06:
                case 15:
                case 16:
                    // 写响应固定：Addr(2) + Value/Count(2)
                    // 总长度：1+1+2+2+2 = 8字节
                    return 8;

                default:
                    return 0; // 未知
            }
        }

        /// <summary>
        /// 计算 Modbus TCP (MBAP) 响应的预期长度（MBAP 头 + UnitId + PDU）
        /// 注意：由于我们需要先读 MBAP 头才能知道真正的长度，这里返回一个足够大的值
        /// Transport 会根据实际接收到的数据动态调整
        /// </summary>
        public static int CalcTcpResponseLen(ModbusCommand cmd)
        {
            // 标准 Modbus TCP 响应的最大长度
            // MBAP(7) + UnitId(1) + FC(1) + ByteCount(1) + MaxData(252) = 262 字节
            // 但为了简单起见，返回一个合理的上界
            // 对于读寄存器 FC03/04：MBAP(7) + Unit(1) + FC(1) + ByteCount(1) + Data(Count*2) = 7 + 1 + 1 + 1 + Count*2
            // 对于写操作 FC05/06/15/16：MBAP(7) + Unit(1) + FC(1) + Addr(2) + Value/Count(2) = 13
            
            // 为了安全起见，对于所有情况返回一个足够大的值
            // 实际长度会由 Transport 根据接收到的数据确定
            return 256;  // 足够覆盖所有常见的 Modbus TCP 响应
        }

        /// <summary>
        /// 校验 CRC
        /// </summary>
        public static bool CheckCRC(byte[] response)
        {
            if (response == null || response.Length < 4) return false;

            // 提取最后两位 CRC
            byte receivedLow = response[response.Length - 2];
            byte receivedHigh = response[response.Length - 1];

            // 计算前面所有数据的 CRC
            // 注意：要排除最后两个字节
            byte[] dataToCheck = new byte[response.Length - 2];
            Array.Copy(response, 0, dataToCheck, 0, response.Length - 2);

            byte[] calculated = CalculateCRC(dataToCheck);

            // Modbus CRC 是低字节在前 (Little Endian)
            return calculated[0] == receivedLow && calculated[1] == receivedHigh;
        }

        /// <summary>
        /// 标准 Modbus CRC16 算法 (Poly: 0xA001)
        /// </summary>
        public static byte[] CalculateCRC(byte[] data)
        {
            ushort crc = 0xFFFF;

            for (int i = 0; i < data.Length; i++)
            {
                crc ^= data[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 1) != 0)
                    {
                        crc >>= 1;
                        crc ^= 0xA001;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }

            // 返回 Low, High (Modbus 报文中 CRC 是低字节在前)
            return new byte[] { (byte)(crc & 0xFF), (byte)(crc >> 8) };
        }
    }
}
