namespace ModbusPilot.Core.Models
{
    // 根配置对象
    public class ServerConfig
    {
        public string LatestVersion { get; set; } = "0.9.5.0";
        public bool ForceUpdate { get; set; } = false; //代表是否强制更新，即有新版本则停用旧版本

        // --- 新增字段 ---
        public string BetaExpiry { get; set; }    // 公测到期时间
        public string ContactEmail { get; set; } // 作者邮箱
        public string Announcement { get; set; } // 全局公告内容
        // 嵌套的链接集合
        public UrlSet Urls { get; set; } = new UrlSet();

        public string UpdateLog { get; set; }
        public string BuyUrl { get; set; }
    }

    public class UrlSet
    {
        public ChannelUrls Cn { get; set; } = new ChannelUrls();
        public ChannelUrls En { get; set; } = new ChannelUrls();
    }

    public class ChannelUrls
    {
        // 这里的默认值是“保底值”，防止云端同步失败时链接为空
        // 既然决定 All in Gitee，默认值全填 Gitee
        public string Homepage { get; set; } = "https://gitee.com/ZerosysLab/ModbusPilot";
        public string Docs { get; set; } = "https://gitee.com/ZerosysLab/ModbusPilot/wikis";
        public string Issues { get; set; } = "https://gitee.com/ZerosysLab/ModbusPilot/issues";
        public string Download { get; set; } = "https://gitee.com/ZerosysLab/ModbusPilot/releases";
    }
}