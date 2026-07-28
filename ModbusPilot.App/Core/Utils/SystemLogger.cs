using System;
using System.IO;
using System.Text;

namespace ModbusPilot.Core.Utils
{
    public static class SystemLogger
    {
        private static readonly string LogDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        private static readonly object _lock = new object();

        /// <summary>
        /// 记录纯文本日志 (适用于通讯异常、状态变更等)
        /// </summary>
        public static void WriteLog(string message, string level = "INFO")
        {
            WriteToFile(level, message, null);
        }

        /// <summary>
        /// 记录异常对象 (适用于 Try-Catch 捕获的硬错误)
        /// </summary>
        public static void WriteError(Exception ex, string context = "System")
        {
            if (ex == null) return;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"[信息]: {ex.Message}");
            sb.AppendLine("[堆栈]:");
            sb.AppendLine(ex.StackTrace);

            if (ex.InnerException != null)
            {
                sb.AppendLine("--- Inner Exception ---");
                sb.AppendLine($"[信息]: {ex.InnerException.Message}");
                sb.AppendLine(ex.InnerException.StackTrace);
            }

            WriteToFile("ERROR", sb.ToString(), context);
        }

        /// <summary>
        /// 核心写入逻辑 (私有封装)
        /// </summary>
        private static void WriteToFile(string level, string content, string context)
        {
            try
            {
                if (!Directory.Exists(LogDir)) Directory.CreateDirectory(LogDir);

                // 按天生成文件名
                string fileName = $"Log_{DateTime.Now:yyyyMMdd}.log";
                string filePath = Path.Combine(LogDir, fileName);

                StringBuilder sb = new StringBuilder();

                sb.AppendLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}  {level}  {content}  {context}");
                lock (_lock)
                {
                    // 使用追加模式写入
                    File.AppendAllText(filePath, sb.ToString(), Encoding.UTF8);
                }
            }
            catch
            {
                // 防御性编程：日志写入失败不应导致业务程序崩溃
            }
        }
    }
}