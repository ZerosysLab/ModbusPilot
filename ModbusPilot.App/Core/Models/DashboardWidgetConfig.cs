using System;
using System.Collections.Generic;
using System.Text;

namespace ModbusPilot.Core.Models
{
    // 必须要 public，否则 Json 无法序列化
    public class DashboardWidgetConfig
    {
        // 定位路径
        public string ChannelName { get; set; }
        public byte SlaveId { get; set; }     // 用 ID 比用 DeviceName 更稳
        public int PointAddress { get; set; } // Modbus 地址是唯一的 Key

        // 【新增】存储区，用于区分 0x0000 和 4x0000
        public int Zone { get; set; }

        // 只有加上它，才能区分同一个寄存器下的不同位变量
        public int? BitIndex { get; set; }

        // 卡片类型
        // 注意：你需要确保 WidgetMode 枚举是在 Core 或 UI.Common 公共命名空间下
        // 这里假设 WidgetMode 定义在 UI.Common，为了解耦，建议存 int 或 string，或者把枚举移到 Core
        // 简单起见，这里存 int
        public int Mode { get; set; }
    }
}
