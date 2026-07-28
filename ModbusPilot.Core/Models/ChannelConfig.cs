using System.Collections.Generic;
using System.IO.Ports; // 需要引用 System.IO.Ports

namespace ModbusPilot.Core.Models
{
    public enum CommType { Serial, Tcp }

    /// <summary>
    /// 通道配置：定义物理连接方式
    /// 一个通道对应一个 ModbusMaster 实例
    /// </summary>
    public class ChannelConfig
    {
        public string ChannelName { get; set; } = "默认通道";
        public CommType Type { get; set; } = CommType.Serial;

        public string PortName { get; set; } = "COM1";
        public int BaudRate { get; set; } = 9600;
        public int DataBits { get; set; } = 8;
        public StopBits StopBits { get; set; } = StopBits.One;
        public Parity Parity { get; set; } = Parity.None;

        // === 2. TCP 参数 ===
        public string IpAddress { get; set; } = "127.0.0.1";
        public int TcpPort { get; set; } = 502;
        // 如果为 true，则在 TCP 上传输 Modbus TCP (MBAP)，否则使用 RTU-over-TCP 透传
        public bool UseModbusTcp { get; set; } = true;

        // 新增：指令轮询间隔 (毫秒)
        public int MinInterval { get; set; } = 20;

        // === 3. 挂载的设备列表  ===
        public List<DeviceConfig> Devices { get; set; } = new List<DeviceConfig>();
    }
}