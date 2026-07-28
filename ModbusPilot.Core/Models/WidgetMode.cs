using System;
using System.Collections.Generic;
using System.Text;

namespace ModbusPilot.Core.Models
{
    public enum WidgetMode
    {
        // 纯显示 (适用于所有点位)
        Monitor = 0,

        // 开关控制 (仅适用于 0x 线圈)
        Switch = 1,

        // 数值设定 (仅适用于 4x 保持寄存器，且非 Bool 类型)
        Control = 2
    }
}
