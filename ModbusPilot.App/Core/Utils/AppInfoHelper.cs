using System.Reflection;

namespace ModbusPilot.Core.Utils
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public static class AppInfoHelper
    {
        private static readonly Assembly _asm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

        // 对应 <Product>
        public static string Product => _asm.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "ModbusPilot";

        // 对应 <Company>
        public static string Company => _asm.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "Zerosys Lab";

        // 对应 <Copyright>
        public static string Copyright => _asm.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "Copyright © 2026";

        // 对应 <Version> (纯数字版 1.0.0)
        public static string Version
        {
            get
            {
                var v = _asm.GetName().Version;
                return $"{v.Major}.{v.Minor}.{v.Build}";
            }
        }

        // 获取完整标题用于 MainForm
        public static string GetAppTitle() => $"{Product} V{Version}";
    }
}