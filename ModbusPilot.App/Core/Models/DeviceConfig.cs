using System.Collections.Generic;

namespace ModbusPilot.Core.Models
{
    /// <summary>
    /// 设备配置：定义总线上的一个逻辑站点
    /// </summary>
    public class DeviceConfig
    {
        public string DeviceName { get; set; } = "新设备";

        // 关键：站号
        public byte SlaveId { get; set; } = 1;

        // 点位表
        public List<ModbusPoint> Points { get; set; } = new List<ModbusPoint>();

        // (可选) 设备的特殊配置，比如字节序是否特殊，可以在这里加
        public DataFormat ByteOrder { get; set; } = DataFormat.ABCD;

        // 【新增】启用状态，默认必须为 true
        public bool IsEnabled { get; set; } = true;
    }
}