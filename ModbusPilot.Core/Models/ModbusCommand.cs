using System;
using System.Collections.Generic;
using System.Text;

namespace ModbusPilot.Core.Models
{
    /// <summary>
    /// 代表一条实际发送的 Modbus 请求
    /// </summary>
    /// <summary>
    /// 通讯状态枚举
    /// </summary>
    public enum CommStatus
    {
        Pending,    // 待发送
        Success,    // 成功
        Timeout,    // 超时无响应
        Error       // 异常(如CRC错误)
    }

    /// <summary>
    /// 指令对象：既包含怎么发(Request)，也包含收到了啥(Response)
    /// </summary>
    public class ModbusCommand
    {
        // === 请求部分 ===
        public byte SlaveId { get; set; } = 1;
        public byte FunctionCode { get; set; }
        public int StartAddress { get; set; }

        /// <summary>
        /// 读指令：代表读取长度 (寄存器数/线圈数)
        /// 写指令：代表写入长度
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// [新增] 仅用于写指令 (FC05, 06, 15, 16)。
        /// 存放要写入的原始字节数据 (大端序)。
        /// </summary>
        public byte[] WritePayload { get; set; }

        // 关联的点位（用于解析）
        public List<ModbusPoint> RelatedPoints { get; set; } = new List<ModbusPoint>();

        // === 响应部分 ===
        public byte[] ResponseData { get; set; }
        public CommStatus ResultStatus { get; set; } = CommStatus.Pending;
        public string ErrorMessage { get; set; }
        public DateTime LastResponseTime { get; set; }

        public long ExecutionTimeMs { get; set; } // 执行耗时
        public override string ToString()
        {
            return $"Slave:{SlaveId} FC:{FunctionCode:00} Addr:{StartAddress} Len:{Count}";
        }
    }
}
