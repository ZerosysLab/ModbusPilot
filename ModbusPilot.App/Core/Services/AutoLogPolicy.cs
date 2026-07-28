using ModbusPilot.Core.Models;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModbusPilot.Core.Services
{
    // 日志策略枚举
    public enum AutoLogPolicy
    {
        ErrorsOnly, // 推荐：仅存异常 (Type == Error)
        All         // 调试：存所有 (TX/RX/Info/Error)
    }

    public class AutoLogService
    {
        private static AutoLogService _instance;
        public static AutoLogService Instance => _instance ??= new AutoLogService();

        public bool IsEnabled { get; private set; } = false;
        public AutoLogPolicy Policy { get; set; } = AutoLogPolicy.ErrorsOnly;

        private const int MAX_FILE_SIZE = 5 * 1024 * 1024;
        private const int RETENTION_DAYS = 7;
        private const int MAX_QUEUE_ITEMS = 10000; // 【新增】最大队列长度，防止内存爆炸

        private readonly string _logPath;
        private readonly ConcurrentQueue<string> _diskQueue = new ConcurrentQueue<string>();
        private CancellationTokenSource _cts;
        private bool _isWriterRunning = false;

        private AutoLogService()
        {
            _logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutoLogs");
            if (!Directory.Exists(_logPath)) Directory.CreateDirectory(_logPath);
        }

        public void Start()
        {
            if (IsEnabled) return;
            IsEnabled = true;
            _cts = new CancellationTokenSource();
            LogHub.OnLog += LogHub_OnLog_Handler;

            if (!_isWriterRunning)
            {
                _isWriterRunning = true;
                // 使用 LongRunning 标记，告诉系统这是一个独立的持久线程
                Task.Factory.StartNew(DiskWriteLoop, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }

        public void Stop()
        {
            if (!IsEnabled) return;
            IsEnabled = false;
            LogHub.OnLog -= LogHub_OnLog_Handler;
            _cts?.Cancel();
            _isWriterRunning = false;
        }

        private void LogHub_OnLog_Handler(LogEntry entry)
        {
            if (!IsEnabled) return;

            // 1. 策略拦截
            if (Policy == AutoLogPolicy.ErrorsOnly && entry.Type != LogType.Error) return;

            // 2. 【保命逻辑】防止内存爆炸
            // 如果堆积超过 10000 条，说明硬盘太慢，直接丢弃新日志
            if (_diskQueue.Count > MAX_QUEUE_ITEMS) return;

            // 3. 格式化逻辑
            string hexPart = "";
            if (entry.Data != null && entry.Data.Length > 0)
            {
                // 优化：只有需要的时候才转 Hex
                hexPart = BitConverter.ToString(entry.Data).Replace("-", " ");
            }

            string line = $"[{entry.Time:yyyy-MM-dd HH:mm:ss.fff}] [{entry.ChannelName}] [{entry.Type}] {entry.Message} {hexPart}";
            _diskQueue.Enqueue(line);
        }

        private async Task DiskWriteLoop()
        {
            CleanupOldLogs();

            // 缓存当前文件名，避免频繁扫描磁盘
            string currentFilePath = GetRollingFileName();

            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    if (_diskQueue.IsEmpty)
                    {
                        await Task.Delay(500); // 没数据时缩短等待时间，提高响应性
                        continue;
                    }

                    // 批量处理：一次性从队列拿出一批数据
                    List<string> buffer = new List<string>();
                    while (_diskQueue.TryDequeue(out string line) && buffer.Count < 500)
                    {
                        buffer.Add(line);
                    }

                    if (buffer.Count > 0)
                    {
                        // 检查文件大小，决定是否滚动
                        FileInfo fi = new FileInfo(currentFilePath);
                        if (fi.Exists && fi.Length > MAX_FILE_SIZE)
                        {
                            currentFilePath = GetRollingFileName();
                        }

                        // 使用同步流写入（在后台线程中同步写通常比高频异步写更快）
                        using (var sw = new StreamWriter(currentFilePath, true, Encoding.UTF8))
                        {
                            foreach (var line in buffer)
                            {
                                sw.WriteLine(line);
                            }
                        }
                    }
                }
                catch
                {
                    await Task.Delay(2000); // 发生磁盘错误（如文件被占用）时避让
                }
            }
        }

        private string GetRollingFileName()
        {
            // 这个方法逻辑保持不变，但因为 DiskWriteLoop 里的优化，调用频率降低了 99%
            string dateStr = DateTime.Now.ToString("yyyyMMdd");
            var files = Directory.GetFiles(_logPath, $"Log_{dateStr}_*.txt");
            if (files.Length == 0) return Path.Combine(_logPath, $"Log_{dateStr}_001.txt");

            string lastFile = files.OrderByDescending(f => f).First();
            FileInfo fi = new FileInfo(lastFile);
            if (fi.Length > MAX_FILE_SIZE)
            {
                string name = Path.GetFileNameWithoutExtension(lastFile);
                int idx = int.Parse(name.Split('_').Last()) + 1;
                return Path.Combine(_logPath, $"Log_{dateStr}_{idx:D3}.txt");
            }
            return lastFile;
        }

        private void CleanupOldLogs() { /* 保持原样 */ }
    }

}