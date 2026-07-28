using System.Drawing;

namespace ModbusPilot.UI.Common
{
    public class UITheme
    {
        public string Name { get; set; }

        // 卡片背景
        public Color CardBack { get; set; }
        // 边框颜色
        public Color Border { get; set; }
        // 装饰条颜色 (Accent)
        public Color Accent { get; set; }
        // 主文字颜色 (数值)
        public Color TextPrimary { get; set; }
        // 副文字颜色 (标题/单位)
        public Color TextSecondary { get; set; }
        // 容器背景 (FlowLayoutPanel 的背景)
        public Color DashboardBack { get; set; }

        // 【新增】状态颜色定义
        public Color StatusOnText { get; set; }  // ON 状态文字色
        public Color StatusOnBack { get; set; }  // ON 状态背景色 (用于开关按钮)
        public Color StatusOffText { get; set; } // OFF 状态文字色
        public Color StatusOffBack { get; set; } // OFF 状态背景色

        // 【新增】离线状态的专属配色
        public Color OfflineBack { get; set; }
        public Color OfflineText { get; set; }
        public Color OfflineBorder { get; set; }

        // 重写 ToString，让 ComboBox 直接显示名字
        public override string ToString()
        {
            return Name;
        }

        // =============================================================
        // 1. 简约白 (默认)
        // =============================================================
        public static UITheme DefaultWhite => new UITheme
        {
            Name = "简约白",
            DashboardBack = Color.FromArgb(240, 242, 245),
            CardBack = Color.White,
            Border = Color.FromArgb(220, 220, 220),
            Accent = Color.FromArgb(0, 122, 204),
            TextPrimary = Color.FromArgb(32, 32, 32),
            TextSecondary = Color.Gray,

            // 新增状态色
            StatusOnText = Color.White,
            StatusOnBack = Color.LimeGreen,
            StatusOffText = Color.Gray,
            StatusOffBack = Color.WhiteSmoke,

                // 【新增】离线配色
                // 背景：变成很浅的灰，不再是纯白，表示不可用
    OfflineBack = Color.FromArgb(245, 245, 245),
            // 文字：变成浅银灰，看起来像禁用状态
            OfflineText = Color.FromArgb(180, 180, 180),
            // 边框：几乎看不见的浅灰
            OfflineBorder = Color.FromArgb(230, 230, 230)
        };

        // =============================================================
        // 2. 极客黑 (改进版：高对比度荧光色)
        // =============================================================
        public static UITheme DarkMode => new UITheme
        {
            Name = "极客黑",
            // 背景：改为接近纯黑的深色，去除"雾蒙蒙"的感觉
            DashboardBack = Color.FromArgb(18, 18, 18),

            // 卡片：深灰黑色，与背景稍微区分即可
            CardBack = Color.FromArgb(30, 30, 30),

            // 边框：稍微亮一点的灰，形成轮廓
            Border = Color.FromArgb(60, 60, 60),

            // 装饰条 & 主文字：【关键】使用荧光青 (Cyan) 或 亮天蓝
            // 这种颜色在黑底上穿透力极强
            Accent = Color.FromArgb(0, 255, 255),
            TextPrimary = Color.FromArgb(0, 255, 255),

            // 副标题：银白色，保证清晰度
            TextSecondary = Color.FromArgb(200, 200, 200),

             // 新增状态色 (荧光绿 vs 暗灰)
            StatusOnText = Color.Black,             // 亮底黑字更清晰，或者用荧光绿
            StatusOnBack = Color.FromArgb(57, 255, 20), // 赛博朋克霓虹绿
            StatusOffText = Color.Silver,
            StatusOffBack = Color.FromArgb(50, 50, 50),

            // 【新增】暗黑模式下的离线配色 (熄灯效果)
            OfflineBack = Color.FromArgb(15, 15, 15), // 比背景更黑，接近纯黑
            OfflineText = Color.FromArgb(60, 60, 60), // 很暗的灰色文字，仿佛没开背光
            OfflineBorder = Color.FromArgb(60, 0, 0) // 暗红色边框 (警示但刺眼)
        };

        // =============================================================
        // 3. 工业灰 (改进版：低反差，耐看)
        // =============================================================
        public static UITheme Industrial => new UITheme
        {
            Name = "工业灰",
            DashboardBack = Color.FromArgb(192, 192, 192), // 【改动】经典 Windows/PLC 灰
            CardBack = Color.FromArgb(224, 224, 224),      // 稍微亮一点的灰
            Border = Color.DimGray,
            Accent = Color.FromArgb(255, 69, 0),           // 橙红色 (OrangeRed)，工业警示感
            TextPrimary = Color.Black,
            TextSecondary = Color.FromArgb(64, 64, 64),

            // 新增状态色 (经典指示灯风格)
            StatusOnText = Color.White,
            StatusOnBack = Color.ForestGreen, // 深绿
            StatusOffText = Color.Black,
            StatusOffBack = Color.LightGray,

             // 【新增】离线配色
             // 背景：变暗沉，像没有背光的 LCD 屏幕
    OfflineBack = Color.FromArgb(200, 200, 200),
            // 文字：深灰色，对比度降低
            OfflineText = Color.FromArgb(128, 128, 128),
            // 边框：暗淡的灰色
            OfflineBorder = Color.Gray
        };

        // 4. 科技蓝 (重制版：赛博科技 / 数据中心风格)
        // =============================================================
        public static UITheme TechBlue => new UITheme
        {
            Name = "科技蓝",

            // 背景：深邃的午夜蓝，营造空间感
            DashboardBack = Color.FromArgb(32, 56, 100),

            // 卡片：比背景稍亮的深蓝，带一点透明感的效果
            CardBack = Color.FromArgb(20, 40, 70),

            // 边框：亮眼的青蓝色，勾勒出卡片的轮廓
            Border = Color.FromArgb(0, 191, 255), // DeepSkyBlue

            // 装饰条 & 主文字：高亮的蓝绿色 (青色)，这就是你想要的"科技感核心"
            // 这种颜色在深蓝背景上会有发光的感觉
            Accent = Color.FromArgb(0, 255, 230),       // 荧光青 (Turquoise)
            TextPrimary = Color.FromArgb(0, 255, 230),  // 数值也是荧光青

            // 副标题：浅天蓝，保证可读性但又不抢镜
            TextSecondary = Color.FromArgb(135, 206, 250), // LightSkyBlue

            // === 状态色 (高亮 LED 风格) ===

            // ON：背景用极亮的青色，文字用深色，模拟发光的灯泡
            StatusOnText = Color.FromArgb(0, 20, 40),      // 深蓝字
            StatusOnBack = Color.FromArgb(0, 255, 255),    // 亮青底 (Cyan)

            // OFF：背景用暗淡的深蓝，文字用灰蓝
            StatusOffText = Color.FromArgb(100, 149, 237), // 矢车菊蓝
            StatusOffBack = Color.FromArgb(15, 30, 50),     // 很暗的深蓝
             // 【新增】离线配色
             // 背景：极深的深渊蓝，几乎融入背景
    OfflineBack = Color.FromArgb(10, 20, 35),
            // 文字：非常暗淡的蓝灰色，像幽灵信号
            OfflineText = Color.FromArgb(40, 60, 90),
            // 边框：几乎看不见的深蓝
            OfflineBorder = Color.FromArgb(0, 0, 50)
        };


    }
}