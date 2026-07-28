using ModbusPilot.Core.Models;
using ModbusPilot.Core.Services;
using ModbusPilot.Core.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace ModbusPilot.Core.Driver
{
    public class ModbusMaster : IDisposable
    {
        public int Interval { get; set; } = 50;

        private ITransport _transport;
        private IModbusCodec _codec;
        private Thread _workerThread;
        private volatile bool _isRunning = false;

        private ConcurrentQueue<ModbusCommand> _writeQueue = new ConcurrentQueue<ModbusCommand>();
        private List<ModbusCommand> _readCycleList = new List<ModbusCommand>();
        private object _readListLock = new object();

        // 【新增 1】记录最后一次成功通信的时间
        public DateTime LastActiveTime { get; private set; } = DateTime.MinValue;
        private bool _wasLastCommSuccessful = true; // 记录上一次通讯状态，默认为成功

        // 【新增】公开的统计计数器 (使用 long 防止溢出)
        public long TxCount { get; private set; } = 0;
        public long RxCount { get; private set; } = 0;
        public long ErrCount { get; private set; } = 0;

        // 【新增】诊断专用统计
        public int ConsecutiveErrors { get; private set; } = 0;      // 连续错误计数
        public long TotalResponseTime { get; private set; } = 0;     // 累计成功响应时间(ms)，用于算平均值
        public long SuccessCount => RxCount;                        // 成功总数

        // 【新增】实时诊断属性
        public long LastResponseTimeMs { get; private set; } = 0; // 最近一次成功的耗时

        // 如果需要近期丢包率，可以使用简单的滑动窗口思想
        private Queue<bool> _recentResults = new Queue<bool>(); // 记录最近 20 次的成败

        private readonly object _recentResultsLock = new object(); // 专门给近期结果队列加的锁

        public event Action<ModbusCommand> OnResponseReceived;

        public bool IsOnline
        {
            get
            {
                // 1. 物理层检查 (TCP断开直接 false)
                if (_transport == null || !_transport.IsConnected) return false;

                // 2. 逻辑层检查 (针对 RTU 或 TCP 假死)
                // 如果超过 5 秒没有更新 LastActiveTime，视为离线
                // (刚启动时给 5 秒宽限期)
                if ((DateTime.Now - LastActiveTime).TotalSeconds > 5) return false;

                return true;
            }
        }

        public ModbusMaster(ITransport transport, IModbusCodec codec)
        {
            _transport = transport;
            _codec = codec;
        }

        public void Start()
        {
            if (_isRunning) return;

            _isRunning = true;
            try
            {
                _transport.Connect();
                // 【新增 3】连接刚建立时，重置活跃时间，防止一启动就显示离线
                LastActiveTime = DateTime.Now;
                _wasLastCommSuccessful = true; // 启动时重置
                                               // 【新增日志】
                LogHub.Write(_transport.ChannelName, LogType.Info, "驱动已启动，物理连接建立。");
            }
            catch (Exception ex)
            {
                _isRunning = false;
                LogHub.Write(_transport.ChannelName, LogType.Error, $"驱动启动失败: {ex.Message}");
                throw new Exception("Connection failed: " + ex.Message);
            }

            _workerThread = new Thread(WorkLoop)
            {
                IsBackground = true,
                Name = "ModbusWorker"
            };
            _workerThread.Start();
        }

        public void Stop()
        {
            _isRunning = false;
            // A. 先断开连接！(这会强制让 socket.Receive 抛出异常，从而打断阻塞)
            try
            {
                _transport.Disconnect();
                // 【新增日志】
                LogHub.Write(_transport.ChannelName, LogType.Info, "驱动已停止，物理连接断开。");
            }
            catch { }

            // B. 再等待线程结束
            // 如果 Transport 断开了，WorkLoop 里的 Receive 应该会立即报错并退出循环
            // 2. 再等待线程结束
            try
            {
                if (_workerThread != null && _workerThread.IsAlive)
                {
                    // 给它 1秒时间收尸，不行就算了（反正 Socket 已经断了，它也没法作妖了）
                    _workerThread.Join(1000);
                }
            }
            catch { }

            _workerThread = null; // 置空
        }

        /// <summary>
        /// 设置需要周期性轮询的读指令列表
        /// </summary>
        public void SetReadCommands(List<ModbusCommand> cmds)
        {
            lock (_readListLock)
            {
                _readCycleList.Clear();
                _readCycleList.AddRange(cmds);
            }
        }

        /// <summary>
        /// 插队发送一条写指令
        /// </summary>
        public void EnqueueWrite(ModbusCommand cmd)
        {
            _writeQueue.Enqueue(cmd);
        }

        /// <summary>
        /// 高级 API：直接写入点位值 (异步插队)
        /// </summary>
        public void WritePoint(ModbusPoint point, object value, byte slaveId)
        {
            // 1. 调用编码器生成 Payload 和 功能码
            byte[] payload = ValueEncoder.Encode(value, point, out byte fc);

            // 2. 构建写指令
            var cmd = new ModbusCommand
            {
                SlaveId = slaveId,
                FunctionCode = fc,
                StartAddress = point.Address,
                WritePayload = payload,

                // 【关键】如果是 FC16，需要计算寄存器数量
                // Payload长度 / 2 = 寄存器数
                Count = (fc == 0x10) ? (payload.Length / 2) : 0
            };

            // 3. 插队发送
            EnqueueWrite(cmd);
        }

        private void WorkLoop()
        {
            int readIndex = 0;

            while (_isRunning)
            {
                // ============================================================
                // 1. 连接看门狗 (Watchdog)
                // ============================================================
                if (!_transport.IsConnected)
                {
                    try
                    {
                        _transport.Connect();
                        // 连接成功后稍微歇一下，防止设备未就绪
                        Thread.Sleep(500);
                    }
                    catch
                    {
                        // 如果是用户点了停止，导致连接断开，直接退出
                        if (!_isRunning) return;

                        Thread.Sleep(2000); // 连不上就多歇会再试
                        continue;
                    }
                }

                // ============================================================
                // 2. 指令调度 (Scheduler)
                // ============================================================
                ModbusCommand currentCmd = null;

                // A. 优先处理写队列 (插队)
                if (!_writeQueue.TryDequeue(out currentCmd))
                {
                    // B. 如果没写的，从轮询列表里取一个
                    lock (_readListLock)
                    {
                        if (_readCycleList.Count > 0)
                        {
                            if (readIndex >= _readCycleList.Count) readIndex = 0;
                            currentCmd = _readCycleList[readIndex];
                            readIndex++;
                            // 再次检查越界，防止列表在别处被清空
                            if (readIndex >= _readCycleList.Count) readIndex = 0;
                        }

                    }
                }

                // C. 既没写也没读，休息一下
                if (currentCmd == null)
                {
                    Thread.Sleep(50);
                    continue;
                }

                // ============================================================
                // 3. 执行指令 (Execution)
                // ============================================================
                try
                {
                    ExecuteCommand(currentCmd);

                    // 成功后触发回调
                    OnResponseReceived?.Invoke(currentCmd);
                }
                catch (Exception ex)
                {
                    // 【核心修复】如果是 Stop() 强制断开连接导致的异常，直接退出线程
                    if (!_isRunning)
                    {
                        return; // 优雅退出
                    }

                    // 如果是正常运行时的异常 (如超时)，可以在这里记录日志
                    // 具体的错误处理其实 ExecuteCommand 内部已经做了 (设置 ResultStatus)，
                    // 这里主要是为了防止线程崩掉。
                }

                // ============================================================
                // 4. 轮询间隔
                // ============================================================
                // 如果 Stop 被调用，立即退出，不再 Sleep
                if (!_isRunning) return;

                Thread.Sleep(Interval);
            }
        }

        private void ExecuteCommand(ModbusCommand cmd)
        {
            string chName = _transport.ChannelName; // 简写方便调用
            try
            {
                // A. 清空缓存
                _transport.DiscardBuffer();

                // B. 使用 Codec 编码请求
                byte[] request = _codec.EncodeRequest(cmd);


                // 【计数】发送前 TX + 1
                TxCount++;

                // C. 计算预期响应长度
                int expectedLen = _codec.CalcExpectedResponseLen(cmd);

                // 在发送逻辑中
                Stopwatch sw = Stopwatch.StartNew();
                // D. 发送并接收
                byte[] response = _transport.SendAndReceive(request, expectedLen);
                sw.Stop();
                cmd.ExecutionTimeMs = sw.ElapsedMilliseconds;
                // E. 使用 Codec 解码响应
                ModbusResponse decodeResult = _codec.DecodeResponse(response, cmd);

                if (decodeResult.IsSuccess)
                {
                    RxCount++;
                    ConsecutiveErrors = 0;                  // 【重要】成功则重置连续错误
                    TotalResponseTime += cmd.ExecutionTimeMs; // 【重要】累加响应时间
                    LastResponseTimeMs = cmd.ExecutionTimeMs;
                    UpdateRecentResults(true);
                  
                    if (!_wasLastCommSuccessful)
                    {
                        LogHub.Write(chName, LogType.Info, $"[√] 通讯已恢复正常 (Slave:{cmd.SlaveId})");
                        SystemLogger.WriteLog($"CH:{_transport.ChannelName} Slave:{cmd.SlaveId}  [√] 通讯已恢复正常.", "COMM_WARN");
                        _wasLastCommSuccessful = true;
                    }

                    cmd.ResponseData = decodeResult.RawData;
                    cmd.ResultStatus = CommStatus.Success;
                    cmd.ErrorMessage = null;
                    cmd.LastResponseTime = DateTime.Now;
                    LastActiveTime = DateTime.Now;

                    // ============================================================
                    // 【核心修改】数据解析逻辑下沉到 Driver 层
                    // 这样 UI 和 自动化引擎 拿到的 cmd.RelatedPoints 里的值已经是解析好的了
                    // ============================================================
                    try
                    {
                        foreach (var point in cmd.RelatedPoints)
                        {
                            // 调用静态解析服务，直接更新内存中的点位值
                            object val = DataResolution.Parse(cmd.ResponseData, cmd, point);
                            point.CurrentValue = val;
                            point.LastUpdateTime = DateTime.Now;
                        }
                    }
                    catch (Exception ex)
                    {
                        // 如果解析出错（比如配置的长度越界），记录日志但不中断通讯
                        LogHub.Write(_transport.ChannelName, LogType.Error, $"Parse Error: {ex.Message}");
                    }
                }
                else
                {
                    // === 情况 B：逻辑错误 (例如从站返回 01/02 异常码) ===
                    // 这种错误通常需要立即知道，因为它代表配置错了
                    LogHub.Write(chName, LogType.Error, $"从站协议错误: {decodeResult.ErrorMessage} (Slave:{cmd.SlaveId} FC:{cmd.FunctionCode})");
                    ProcessCommandError(cmd, LogType.Error, $"Logic Err: {decodeResult.ErrorMessage}");
                }
            }
            catch (TimeoutException ex)
            {
                ProcessCommandError(cmd, LogType.Warning, $"Timeout (Slave:{cmd.SlaveId} FC:{cmd.FunctionCode} {ex.Message})", CommStatus.Timeout);
            }
            catch (IOException ioex)
            {
                ProcessCommandError(cmd, LogType.Error, $"IO Error: {ioex.Message}", CommStatus.Error, true);
            }
            catch (Exception ex)
            {
                ProcessCommandError(cmd, LogType.Error, $"System Error: {ex.Message}", CommStatus.Error);
            }
        }
        private void ProcessCommandError(ModbusCommand cmd, LogType type, string msg, CommStatus status = CommStatus.Error, bool disconnect = false)
        {
            ErrCount++;
            ConsecutiveErrors++; // 【重要】错误则累加

            // 只有在“从成功转为失败”的那一刻，才记录 Warning 日志
            if (_wasLastCommSuccessful)
            {
                //string temp = $"[!] 通讯故障开始: 响应超时 (Slave:{cmd.SlaveId})";
                LogHub.Write(_transport.ChannelName, LogType.Warning, msg);
                msg = $"CH: {_transport.ChannelName} Slave:{cmd.SlaveId} msg:{msg}";
                SystemLogger.WriteLog(msg, "COMM_WARN");
                _wasLastCommSuccessful = false;
            }

            UpdateRecentResults(false);

            cmd.ResultStatus = status;
            cmd.ErrorMessage = msg;

            if (disconnect) { try { _transport.Disconnect(); } catch { } }

            LogHub.Write(_transport.ChannelName, type, msg);
        }
        private void UpdateRecentResults(bool success)
        {
            lock (_recentResultsLock) // 写入时加锁
            {
                _recentResults.Enqueue(success);
                if (_recentResults.Count > 20) _recentResults.Dequeue();
            }
        }
        // 获取近期丢包率 (0-100)
        public double RecentErrorRate
        {
            get
            {
                lock (_recentResultsLock) // 读取时加锁
                {
                    if (_recentResults.Count == 0) return 0;

                    // 方案：将 LINQ 逻辑放在锁内部，或者先 ToArray()
                    // 对于 20 个元素的队列，直接在锁内计算是非常快的
                    int errorCount = 0;
                    foreach (var res in _recentResults)
                    {
                        if (!res) errorCount++;
                    }
                    return (double)errorCount / _recentResults.Count * 100;
                }
            }
        }
        /// <summary>
        /// 重置所有诊断计数器
        /// </summary>
        public void ResetCounters()
        {
            TxCount = 0;
            RxCount = 0;
            ErrCount = 0;
            ConsecutiveErrors = 0;
            TotalResponseTime = 0;
            // 不建议重置 LastActiveTime，那是判定在线的物理依据
        }
        public void Dispose()
        {
            Stop();
            try { (_transport as IDisposable)?.Dispose(); } catch { }
        }

        public enum MessageDirection
        {
            Sent,
            Received
        }
    }
}