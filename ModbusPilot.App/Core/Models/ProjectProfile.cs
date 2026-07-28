using System;
using System.Collections.Generic;

namespace ModbusPilot.Core.Models
{
    /// <summary>
    /// 项目文件根结构 (对应 .json 文件内容)
    /// </summary>
    public class ProjectProfile
    {
        // 版本号 (用于将来处理兼容性问题)
        public string Version { get; set; } = "0.9";

        // 项目名称
        public string ProjectName { get; set; } = "Untitled";

        // 最后修改时间
        public DateTime LastModified { get; set; } = DateTime.Now;

        // 核心数据：所有的通道配置 (包含设备和点位)
        public List<ChannelConfig> Channels { get; set; } = new List<ChannelConfig>();

        // 【新增】仪表盘布局
        public List<DashboardWidgetConfig> DashboardLayout { get; set; } = new List<DashboardWidgetConfig>();
    }
}