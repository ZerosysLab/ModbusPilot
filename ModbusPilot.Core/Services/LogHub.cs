using System;
using ModbusPilot.Core.Models;

namespace ModbusPilot.Core.Services
{
    [System.Reflection.Obfuscation(Exclude = true, ApplyToMembers = true)]
    public static class LogHub
    {
        // UI 层订阅这个事件： LogHub.OnLog += (entry) => { ... };
        public static event Action<LogEntry> OnLog;

        /// <summary>
        /// 写入日志
        /// </summary>
        public static void Write(string channel, LogType type, string msg, byte[] data = null, bool isTcp = true)
        {
            // 创建日志对象
            var entry = new LogEntry(channel, type, msg, data, isTcp);

            // 广播事件 (使用 ?.Invoke 防止无订阅时报错)
            OnLog?.Invoke(entry);
        }
    }
}