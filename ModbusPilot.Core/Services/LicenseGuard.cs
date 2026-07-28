using ModbusPilot.Core.Models;
using System;

namespace ModbusPilot.Core.Services
{
    public static class LicenseGuard
    {
        /// <summary>
        /// 挂载 UI 弹窗逻辑（由 MainForm 注入：输入理由，返回是否激活成功）
        /// </summary>
        public static Func<string, bool> LicenseRequestHandler { get; set; }

        // === 免费版硬限制常量 ===
        public const int MAX_FREE_CHANNELS = 2;
        public const int MAX_FREE_DEVICES = 4;
        public const int MAX_FREE_WIDGETS = 20;
        public const int MAX_FREE_TAGS = 1000;
        public const int MAX_FREE_TREND_CURVES = 4;      // 趋势图最多4条
        public const int MAX_FREE_TREND_MINUTES = 30;    // 趋势图最多回溯30分钟

        // =========================================================
        // 1. 属性转发 (UI 想要读数据，找我拿)
        // =========================================================
        /// <summary>
        /// 获取机器码
        /// </summary>
        public static string MachineCode => LicenseService.CachedMachineCode;

        /// <summary>
        /// 是否处于公测活动期
        /// </summary>
        public static bool IsBetaMode => LicenseService.IsBetaActive;

        /// <summary>
        /// 获取当前的详细授权状态对象
        /// </summary>
        public static LicenseStatus CurrentStatus => LicenseService.Current;


        // =========================================================
        // 2. 行为转发 (UI 想要执行操作，找我办)
        // =========================================================

        /// <summary>
        /// 初始化 (在 Program.cs 启动时调用)
        /// </summary>
        public static void Initialize()
        {
            LicenseService.Initialize();
        }

        /// <summary>
        /// 内部判断：当前环境是否拥有“专业版”级别的访问权
        /// (规则：或者是已激活的专业版，或者是未过期的公测版)
        /// </summary>
        private static bool HasProAccess()
        {
            // 逻辑很简单：
            // 1. 已经是专业版 (RSA校验过)
            // 2. 或者当前处于公测活动期
            return (LicenseService.Current.Type == LicenseType.Professional) || LicenseService.IsBetaActive;
        }
        /// <summary>
        /// 判断是否为正式激活的专业版（排除公测期的临时授权）
        /// </summary>
        public static bool IsProUser()
        {
            return LicenseService.Current.Type == ModbusPilot.Core.Models.LicenseType.Professional;
        }
        /// <summary>
        /// 核心拦截方法：如果无权限则弹出注册窗
        /// </summary>
        /// <param name="featureName">功能描述名称</param>
        public static bool Check(string featureName)
        {
            if (HasProAccess()) return true;

            // 拦截并触发弹窗
            if (LicenseRequestHandler != null)
            {
                return LicenseRequestHandler.Invoke(featureName);
            }
            return false;
        }

        /// <summary>
        /// 尝试激活软件
        /// </summary>
        /// <param name="licenseKey">用户输入的激活码</param>
        /// <param name="message">返回的提示信息</param>
        /// <returns>是否激活成功</returns>
        public static bool TryActivate(string licenseKey, out string message)
        {
            // 1. 调用内部隐身的 LicenseService 进行验证
            var status = LicenseService.ValidateLicense(licenseKey);

            message = status.Message;

            if (status.Type == ModbusPilot.Core.Models.LicenseType.Professional)
            {
                // 2. 验证成功，保存授权 (内部方法)
                LicenseService.SaveLicense(licenseKey);

                // 3. 刷新一下初始化状态
                LicenseService.Initialize();
                return true;
            }

            return false;
        }
        /// <summary>
        /// 获取当前的机器码 (UI显示用)
        /// </summary>
        public static string GetMachineCode()
        {
            return LicenseService.CachedMachineCode ?? "LOADING...";
        }

        /// <summary>
        /// 获取当前的授权状态描述 (UI显示用，例如：专业版 / 剩余30天)
        /// </summary>
        public static string GetLicenseStatusMessage()
        {
            return LicenseService.Current.Message;
        }
        // =============================================================
        //  具体的规模/功能限制判定函数
        // =============================================================
        /// <summary>
        /// 检查项目总点位数量是否超限
        /// </summary>
        /// <param name="currentTotalCount">当前项目所有通道下所有设备的总点位数</param>
        /// <returns>如果是专业版或未超限返回true；否则弹出注册窗并返回激活结果</returns>
        public static bool CanSupportTagCount(int currentTotalCount)
        {
            if (HasProAccess()) return true;
            if (currentTotalCount <= MAX_FREE_TAGS) return true;

            // 触发拦截弹窗
            return Check($"海量变量点位支持 (免费版项目总上限 {MAX_FREE_TAGS} 个)");
        }
        /// <summary>
        /// 检查是否可以添加通道
        /// </summary>
        public static bool CanAddChannel(int currentCount)
        {
            if (HasProAccess()) return true;
            if (currentCount < MAX_FREE_CHANNELS) return true;

            return Check($"多通道管理 (免费版上限 {MAX_FREE_CHANNELS} 个)");
        }
        public static bool CanAddChannelSilent(int currentCount)
        {
            if (HasProAccess()) return true;
            return currentCount < MAX_FREE_CHANNELS ;

        }
        /// <summary>
        /// 检查是否可以给指定通道添加设备
        /// </summary>
        public static bool CanAddDevice(int currentCount)
        {
            if (HasProAccess()) return true;
            if (currentCount < MAX_FREE_DEVICES) return true;

            return Check($"多设备并行通讯 (免费版上限 {MAX_FREE_DEVICES} 台)");
        }
        public static bool CanAddDeviceSilent(int currentCount)
        {
            // 如果是专业版或公测期，直接放行
            if (HasProAccess()) return true;

            return (currentCount < MAX_FREE_DEVICES);
        }
        /// <summary>
        /// 检查是否可以添加仪表盘卡片
        /// </summary>
        public static bool CanAddWidget(int currentCount)
        {
            if (HasProAccess()) return true;
            if (currentCount < MAX_FREE_WIDGETS) return true;

            return Check($"多仪表看板布局 (免费版上限 {MAX_FREE_WIDGETS} 个)");
        }
        /// <summary>
        /// 静默检查：只返回结果，不弹窗。用于启动加载等批量场景。
        /// </summary>
        public static bool CanAddWidgetSilent(int currentCount)
        {
            // 如果是专业版或公测期，直接放行
            if (HasProAccess()) return true;

            // 否则检查是否超过免费额度 (20)
            return currentCount < MAX_FREE_WIDGETS;
        }

        /// <summary>
        /// 检查趋势图曲线数量
        /// </summary>
        public static bool CanAddTrendCurve(int currentCount)
        {
            if (HasProAccess()) return true;
            if (currentCount < MAX_FREE_TREND_CURVES) return true;

            return Check($"多路趋势实时比对 (免费版上限 {MAX_FREE_TREND_CURVES} 路)");
        }

        /// <summary>
        /// 检查趋势图回溯时长 (分钟)
        /// </summary>
        /// <param name="minutes">计划设置的时长</param>
        public static bool CanSetTrendDuration(int minutes)
        {
            if (HasProAccess()) return true;
            if (minutes <= MAX_FREE_TREND_MINUTES) return true;

            return Check($"超长历史趋势回溯 (免费版上限 {MAX_FREE_TREND_MINUTES} 分钟)");
        }

        /// <summary>
        /// 检查报文自动留档功能权限
        /// </summary>
        public static bool CanUseAutoLogging()
        {
            // 此功能完全属于专业版/公测版专属，不设免费额度
            return Check("报文诊断自动留档 (Hex Logging)");
        }

        /// <summary>
        /// 检查全屏模式权限
        /// </summary>
        public static bool CanUseFullScreen()
        {
            return Check("F11 全屏沉浸监控模式");
        }

        /// <summary>
        /// 检查 Excel 导出权限
        /// </summary>
        public static bool CanExportData()
        {
            return Check("Excel / CSV 监控趋势数据导出");
        }
    }
}