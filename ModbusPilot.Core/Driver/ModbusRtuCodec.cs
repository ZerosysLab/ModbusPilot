using ModbusPilot.Core.Models;
using System;

namespace ModbusPilot.Core.Driver
{
    public class ModbusRtuCodec : IModbusCodec
    {
        public string Name => "Modbus RTU";

        public byte[] EncodeRequest(ModbusCommand cmd)
        {
            // 直接调用之前写好的静态方法 (包含CRC)
            return ModbusProtocol.BuildMessage(cmd);
        }

        public int CalcExpectedResponseLen(ModbusCommand cmd)
        {
            return ModbusProtocol.CalcResponseLen(cmd);
        }

        public ModbusResponse DecodeResponse(byte[] data, ModbusCommand cmd)
        {
            // 1. 基础长度校验 (至少包含 ID+FC+CRC = 4字节)
            if (data == null || data.Length < 4)
                return new ModbusResponse { IsSuccess = false, ErrorMessage = "数据长度不足" };

            // 2. CRC 校验
            if (!ModbusProtocol.CheckCRC(data))
                return new ModbusResponse { IsSuccess = false, ErrorMessage = "CRC 校验失败" };

            // 3. 校验站号
            if (data[0] != cmd.SlaveId)
                return new ModbusResponse { IsSuccess = false, ErrorMessage = $"站号不匹配 (Exp:{cmd.SlaveId} Act:{data[0]})" };

            // 4. 校验异常响应 (FC | 0x80)
            if (data[1] == (cmd.FunctionCode | 0x80))
            {
                byte errCode = data.Length > 2 ? data[2] : (byte)0;
                return new ModbusResponse { IsSuccess = false, ErrorMessage = $"Modbus异常: Code {errCode:X2}" };
            }

            // 5. 提取纯数据 Payload (剥离头部和CRC)
            // RTU 读响应结构: [SlaveId(1)] [FC(1)] [ByteCount(1)] [Data...] [CRC(2)]
            // 写响应结构: [SlaveId(1)] [FC(1)] [Addr(2)] [Value(2)] [CRC(2)]

            // 写指令没有数据段，返回空
            if (cmd.FunctionCode >= 0x05)
                return new ModbusResponse { IsSuccess = true, RawData = new byte[0] };

            // 读指令：头部 3 字节，尾部 2 字节
            int headerSize = 3;
            int footerSize = 2;
            int payloadLen = data.Length - headerSize - footerSize;

            if (payloadLen < 0)
                return new ModbusResponse { IsSuccess = false, ErrorMessage = "数据解析长度错误" };

            byte[] payload = new byte[payloadLen];
            Array.Copy(data, headerSize, payload, 0, payloadLen);

            return new ModbusResponse { IsSuccess = true, RawData = payload };
        }
    }
}