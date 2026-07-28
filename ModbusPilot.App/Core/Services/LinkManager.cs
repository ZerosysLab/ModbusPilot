using Newtonsoft.Json;
using System.Diagnostics;
using ModbusPilot.Core.Models;

namespace ModbusPilot.Core.Services
{
    internal static class LinkManager
    {
        // 保存本地配置 (告诉我们要去哪里下配置)
        private static LocalConfig _localConfig;

        // 保存远程配置 (实际的业务链接)
        private static ServerConfig _serverConfig = new ServerConfig(); // 默认实例化，确保有保底值

        // 标记：同步是否彻底失败
        public static bool IsConfigSyncFailed { get; private set; } = false;

        // --- 静态构造函数：程序启动瞬间加载本地配置 ---
        static LinkManager()
        {
            InitLocalConfig();
        }

        private static void InitLocalConfig()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            try
            {
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    _localConfig = JsonConvert.DeserializeObject<LocalConfig>(json);
                }
            }
            catch { /* 忽略错误 */ }

            // 如果读取失败或文件不存在，创建默认文件
            if (_localConfig == null)
            {
                _localConfig = new LocalConfig();
                try
                {
                    string json = JsonConvert.SerializeObject(_localConfig, Formatting.Indented);
                    File.WriteAllText(path, json);
                }
                catch { /* 写入失败忽略 */ }
            }
        }
        // 1. 公测到期日期 (增加安全解析)
        public static DateTime BetaExpiryDate
        {
            get
            {
                if (DateTime.TryParse(_serverConfig.BetaExpiry, out DateTime dt))
                    return dt;
                return new DateTime(2026, 5, 1); // 如果云端没配或配错，默认到5月1日
            }
        }
        // 2. 联系邮箱
        public static string ContactEmail =>
            string.IsNullOrEmpty(_serverConfig.ContactEmail) ? "modbuspilot@163.com" : _serverConfig.ContactEmail;
        // 3. 全局公告 (UI可以判断是否为空来决定是否显示)
        public static string Announcement => _serverConfig.Announcement;

        public static string BuyUrl =>
           string.IsNullOrEmpty(_serverConfig.BuyUrl) ? _serverConfig.Urls.Cn.Homepage : _serverConfig.BuyUrl;

        // --- 核心属性：动态获取当前语言对应的链接 ---
        private static ChannelUrls CurrentUrls
        {
            get
            {
                // 根据 LangProvider 状态判断
                bool isCn = LangProvider.CurrentLang == "zh";
                return isCn ? _serverConfig.Urls.Cn : _serverConfig.Urls.En;
            }
        }

        // 对外公开的属性 (UI直接调用这些)
        public static string HomeUrl => CurrentUrls.Homepage;
        public static string DocUrl => CurrentUrls.Docs;
        public static string IssueUrl => CurrentUrls.Issues;
        public static string DownloadUrl => CurrentUrls.Download;

        // --- 功能 1: 异步同步远程配置 ---
        public static async Task SyncConfigAsync()
        {
            IsConfigSyncFailed = false;

            // 1. 决定首选和备选源 (根据当前语言)
            bool isCn = LangProvider.CurrentLang == "zh";

            // 从 _localConfig 中读取 Raw 地址
            string primaryUrl = isCn ? _localConfig.RemoteConfigUrl_Cn : _localConfig.RemoteConfigUrl_En;
            string backupUrl = isCn ? _localConfig.RemoteConfigUrl_En : _localConfig.RemoteConfigUrl_Cn;

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(5); // 5秒超时

                try
                {
                    await FetchAndApply(client, primaryUrl);
                    return; // 成功即退出
                }
                catch { /* 首选失败 */ }

                try
                {
                    await FetchAndApply(client, backupUrl);
                    return; // 备选成功即退出
                }
                catch
                {
                    // 双路全挂
                    IsConfigSyncFailed = true;
                }
            }
        }

        /// <summary>
        /// 检查是否有更新
        /// 返回值: (是否有更新, 新版本号, 更新日志, 是否强制)
        /// </summary>
        public static (bool HasUpdate, string Version, string Log, bool IsForce) CheckUpdate()
        {
            // 防御：如果没有配置或版本号为空
            if (_serverConfig == null || string.IsNullOrEmpty(_serverConfig.LatestVersion))
                return (false, null, null, false);

            try
            {
                Version serverVer = new Version(_serverConfig.LatestVersion);

                // 获取本地版本 (App.exe 的版本)
                Version localVer = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;

                // 对比
                if (serverVer > localVer)
                {
                    return (true, _serverConfig.LatestVersion, _serverConfig.UpdateLog, _serverConfig.ForceUpdate);
                }
            }
            catch
            {
                // 忽略版本号解析错误
            }

            // 没更新或出错，默认返回 false
            return (false, null, null, false);
        }
        private static async Task FetchAndApply(HttpClient client, string url)
        {
            client.DefaultRequestHeaders.Remove("Cache-Control");
            // 增加这两个 Header，告诉服务器和中间代理：不要给我缓存数据
            client.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
            {
                NoCache = true,
                NoStore = true,
                MustRevalidate = true
            };

            // 也可以加上这一行，有些老的代理服务器认这个
            client.DefaultRequestHeaders.Remove("Pragma");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Pragma", "no-cache");

            // 2. 修正 URL 参数 (去掉空格，并判断是否已经有问号)
            string connector = url.Contains("?") ? "&" : "?";
            string finalUrl = $"{url}{connector}t={DateTime.Now.Ticks}";

            // 【修正点】必须请求 finalUrl 才能绕过缓存
            string json = await client.GetStringAsync(finalUrl);
            var config = JsonConvert.DeserializeObject<ServerConfig>(json);
            if (config != null)
            {
                _serverConfig = config;
            }
        }

        // --- 功能 2: 通用打开浏览器方法 ---
        public static void Open(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;
            try
            {
                Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message); // 抛出给 UI 处理弹窗
            }
        }
    }
}