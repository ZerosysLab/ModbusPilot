using System;

namespace ModbusPilot.Core.Models
{
    [Serializable]
    public class TrendDragData
    {
        public TrendDragData() { }
        public TrendDragData(string chName,string deviceName,byte slaveid,ModbusPoint point) {
            ChannelName = chName;
            DeviceName = deviceName;
            SlaveId = slaveid;
            Point = point;
        }
        public string DeviceName { get; set; }
        public byte SlaveId { get; set; }

        public string ChannelName { get; set; }
        public ModbusPoint Point { get; set; }
    }
}
