using ModbusPilot.Core.Models;
using System;
using System.Collections.Generic;

namespace ModbusPilot.Core.Driver
{
    public class ModbusTcpCodec : IModbusCodec
    {
        private ushort _transactionId = 0;
        private readonly object _lock = new object();

        public string Name => "Modbus TCP";

        public byte[] EncodeRequest(ModbusCommand cmd)
        {
            List<byte> frame = new List<byte>();

            ushort currentTxId;
            lock (_lock) { currentTxId = _transactionId++; }

            // [0-1] Transaction ID
            frame.Add((byte)(currentTxId >> 8));
            frame.Add((byte)(currentTxId & 0xFF));
            // [2-3] Protocol ID
            frame.Add(0x00); frame.Add(0x00);
            // [4-5] Length (占位)
            frame.Add(0x00); frame.Add(0x00);
            // [6] Unit ID
            frame.Add(cmd.SlaveId);

            // PDU
            frame.Add(cmd.FunctionCode);
            switch (cmd.FunctionCode)
            {
                case 01:
                case 02:
                case 03:
                case 04:
                    frame.Add((byte)(cmd.StartAddress >> 8));
                    frame.Add((byte)(cmd.StartAddress & 0xFF));
                    frame.Add((byte)(cmd.Count >> 8));
                    frame.Add((byte)(cmd.Count & 0xFF));
                    break;
                case 05:
                case 06:
                    frame.Add((byte)(cmd.StartAddress >> 8));
                    frame.Add((byte)(cmd.StartAddress & 0xFF));
                    if (cmd.WritePayload != null && cmd.WritePayload.Length >= 2)
                    {
                        frame.Add(cmd.WritePayload[0]);
                        frame.Add(cmd.WritePayload[1]);
                    }
                    else { frame.Add(0x00); frame.Add(0x00); }
                    break;
                case 15:
                case 16:
                    frame.Add((byte)(cmd.StartAddress >> 8));
                    frame.Add((byte)(cmd.StartAddress & 0xFF));
                    frame.Add((byte)(cmd.Count >> 8));
                    frame.Add((byte)(cmd.Count & 0xFF));
                    byte byteCount = (byte)(cmd.WritePayload?.Length ?? 0);
                    frame.Add(byteCount);
                    if (byteCount > 0) frame.AddRange(cmd.WritePayload);
                    break;
                default:
                    throw new NotSupportedException($"ModbusTCP 不支持功能码 {cmd.FunctionCode}");
            }

            // 回填 Length (总长度 - 6)
            int length = frame.Count - 6;
            frame[4] = (byte)(length >> 8);
            frame[5] = (byte)(length & 0xFF);

            return frame.ToArray();
        }

        public int CalcExpectedResponseLen(ModbusCommand cmd)
        {
            // 【关键修复】必须计算精确长度！
            // 之前的 return 256 会导致 TcpTransport 死等直到超时

            // 1. 先计算 RTU 理论长度 (SlaveID + FC + Data + CRC)
            int rtuLen = ModbusProtocol.CalcResponseLen(cmd);
            if (rtuLen == 0) return 0;

            // 2. 转换为 TCP 长度
            // TCP 结构: [MBAP(6)] + [UnitID(1)] + [FC(1)] + [Data]
            // RTU 结构:             [UnitID(1)] + [FC(1)] + [Data] + [CRC(2)]
            // 
            // 公式: TCP长度 = RTU长度 - CRC(2) + MBAP头(6)
            // 解释: MBAP头是指前6个字节 (TransID 2 + ProtoID 2 + Len 2)

            return rtuLen - 2 + 6;
        }

        public ModbusResponse DecodeResponse(byte[] data, ModbusCommand cmd)
        {
            // 1. 基础长度检查 (MBAP 7 + FC 1)
            if (data == null || data.Length < 8)
                return new ModbusResponse { IsSuccess = false, ErrorMessage = "响应数据过短" };

            // 2. 协议标识符
            if (data[2] != 0x00 || data[3] != 0x00)
                return new ModbusResponse { IsSuccess = false, ErrorMessage = "协议标识符错误" };

            // 3. 长度校验
            int lengthField = (data[4] << 8) | data[5];
            if (data.Length < 6 + lengthField)
                return new ModbusResponse { IsSuccess = false, ErrorMessage = "接收数据包不完整" };

            // 4. 站号校验 (Byte 6)
            if (data[6] != cmd.SlaveId)
                return new ModbusResponse { IsSuccess = false, ErrorMessage = $"站号不匹配 Exp:{cmd.SlaveId} Act:{data[6]}" };

            // 5. 功能码校验 (Byte 7)
            byte rxFc = data[7];
            if (rxFc == (cmd.FunctionCode | 0x80))
            {
                byte code = data.Length > 8 ? data[8] : (byte)0;
                return new ModbusResponse { IsSuccess = false, ErrorMessage = $"Modbus异常: Code {code:X2}" };
            }
            if (rxFc != cmd.FunctionCode)
                return new ModbusResponse { IsSuccess = false, ErrorMessage = "功能码不匹配" };

            // 6. 提取纯数据 (Payload)
            // 如果是写指令，无数据段
            if (cmd.FunctionCode >= 0x05)
                return new ModbusResponse { IsSuccess = true, RawData = new byte[0] };

            // 读指令结构: [MBAP(7)] + [FC(1)] + [ByteCount(1)] + [Data...]
            // 数据起始索引 = 9
            int headerSize = 9;
            int payloadLen = data.Length - headerSize;

            if (payloadLen < 0)
                return new ModbusResponse { IsSuccess = false, ErrorMessage = "数据长度不足" };

            byte[] payload = new byte[payloadLen];
            Array.Copy(data, headerSize, payload, 0, payloadLen);

            return new ModbusResponse { IsSuccess = true, RawData = payload };
        }

        public void SetTransactionId(ushort txId) { lock (_lock) _transactionId = txId; }
        public void ResetTransactionId() { lock (_lock) _transactionId = 0; }
    }
}