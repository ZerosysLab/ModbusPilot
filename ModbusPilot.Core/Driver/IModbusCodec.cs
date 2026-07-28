using ModbusPilot.Core.Models;
using System;

namespace ModbusPilot.Core.Driver
{
    /// <summary>
    /// Modbus 协议编解码接口
    /// 负责将 ModbusCommand 编码为字节，并将响应字节解码为结果
    /// </summary>
    public interface IModbusCodec
    {
        /// <summary>
        /// 将命令编码为发送字节
        /// </summary>
        byte[] EncodeRequest(ModbusCommand cmd);

        /// <summary>
        /// 计算预期的响应长度（用于 Transport 层知道何时停止接收）
        /// </summary>
        int CalcExpectedResponseLen(ModbusCommand cmd);

        /// <summary>
        /// 解码响应字节为结果
        /// </summary>
        ModbusResponse DecodeResponse(byte[] data, ModbusCommand cmd);

        /// <summary>
        /// 获取协议名称（用于调试）
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Modbus 响应结果
    /// </summary>
    public class ModbusResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public byte[] RawData { get; set; } // 原始响应数据
    }
}
