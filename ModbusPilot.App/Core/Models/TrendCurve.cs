using System;
using System.Drawing;
using System.Linq;
using ModbusPilot.Core.Utils;

namespace ModbusPilot.Core.Models
{
    public class TrendCurve
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string DeviceName { get; set; }
        public byte SlaveId { get; set; }
        public ModbusPoint Point { get; set; }

        public Color LineColor { get; set; }
        public YAxisSide AxisSide { get; set; } = YAxisSide.Left;
        public int LineWidth { get; set; } = 2;

        // 建议：容量参数化，不要写死 100000
        public CircularBuffer<DateTime> TimeData { get; set; }
        public CircularBuffer<double> ValueData { get; set; }

        public string DisplayName => $"[{DeviceName}] {Point.Name}";

        // 优化：不再去查 Buffer，而是由 F_TrendChart 在采样时直接赋值
        // 因为 Buffer.LastOrDefault() 在 10万数据量下是 O(N) 操作，每秒刷新一次会导致界面卡顿
        public double CurrentValue { get; set; }

        public DateTime LastSampleTime { get; set; }

        // 构造函数：接收最大点数
        public TrendCurve(int maxCapacity = 100000)
        {
            TimeData = new CircularBuffer<DateTime>(maxCapacity);
            ValueData = new CircularBuffer<double>(maxCapacity);
        }
        public void ResizeBuffer(int newCapacity)
        {
            // 直接替换为新的实例，旧的数据会被 GC 回收
            // 既然点击了 Start，清空历史数据也是符合逻辑的
            TimeData = new CircularBuffer<DateTime>(newCapacity);
            ValueData = new CircularBuffer<double>(newCapacity);
        }
    }

    public enum YAxisSide { Left, Right }
}
    
