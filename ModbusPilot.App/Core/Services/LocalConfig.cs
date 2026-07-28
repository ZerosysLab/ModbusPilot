using System;
using System.Collections.Generic;
using System.Text;

namespace ModbusPilot.Core.Services
{
    public class LocalConfig
    {
        // 这里存放的是 version.json 的“元地址”
        public string RemoteConfigUrl_Cn { get; set; } = "https://gitee.com/ZerosysLab/ModbusPilot/raw/master/version.json";
        public string RemoteConfigUrl_En { get; set; } = "https://raw.githubusercontent.com/ZerosysLab/ModbusPilot/main/version.json";
    }
}
