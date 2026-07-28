using ModbusPilot.Core.Models;
using ModbusPilot.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ModbusPilot.Core.Driver
{
    public class RtuTransport : ITransport
    {
        private SerialPort _port;
        public string ChannelName { get; private set; }

        private string _portName;
        private int _baudRate;

        // 构造函数
        public RtuTransport(string channelName, string portName, int baudRate = 9600, int dataBits = 8, StopBits stopBits = StopBits.One, Parity parity = Parity.None)
        {
            ChannelName = channelName; // 保存名字
            _portName = portName;
            _baudRate = baudRate;

            _port = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            _port.ReadTimeout = 1000;
            _port.WriteTimeout = 1000;
            // 【关键补充】
            _port.RtsEnable = true;  // 必须开启，很多 485 模块靠这个信号供电或切换方向
            _port.DtrEnable = true;  // 建议开启
        }

        public bool IsConnected => _port != null && _port.IsOpen;

        public void Connect()
        {
            if (_port != null && !_port.IsOpen)
            {
                _port.Open();
                // [日志]
                LogHub.Write(ChannelName, LogType.Info, $"串口 {_portName} 已打开");
            }
        }

        public void Disconnect()
        {
            if (_port != null && _port.IsOpen)
            {
                try
                {
                    // 有时候直接 Close 会卡死，先 Discard
                    _port.DiscardInBuffer();
                    _port.DiscardOutBuffer();
                    _port.Close();
                }
                catch { }
            }
        }

        public void DiscardBuffer()
        {
            if (IsConnected)
            {
                _port.DiscardInBuffer();
                _port.DiscardOutBuffer();
            }
        }

        public byte[] SendAndReceive(byte[] request, int expectedLen)
        {
            if (!IsConnected) throw new Exception("SerialPort is closed.");

            // 【关键优化 1】发送前清空接收缓冲区
            // 防止上一次通讯残留的垃圾字节干扰本次识别
            _port.DiscardInBuffer();

            // 1. 发送
            _port.Write(request, 0, request.Length);
            LogHub.Write(ChannelName, LogType.Send, "TX", request, isTcp: false);

            // 2. 接收循环
            List<byte> buffer = new List<byte>();
            Stopwatch sw = Stopwatch.StartNew(); // 使用 Stopwatch 比 DateTime.Now 更精确

            while (buffer.Count < expectedLen)
            {
                // 超时检查
                if (sw.ElapsedMilliseconds > _port.ReadTimeout)
                {
                    if (buffer.Count > 0)
                        LogHub.Write(ChannelName, LogType.Warning, $"RX (Incomplete: {buffer.Count}/{expectedLen})", buffer.ToArray(), isTcp: false);

                    throw new TimeoutException($"Recv Timeout. Got {buffer.Count}/{expectedLen} bytes.");
                }

                int bytesToRead = _port.BytesToRead;
                if (bytesToRead > 0)
                {
                    byte[] chunk = new byte[bytesToRead];
                    int readCount = _port.Read(chunk, 0, bytesToRead);
                    for (int i = 0; i < readCount; i++) buffer.Add(chunk[i]);

                    // 【关键优化 2】智能识别 Modbus 异常报文 (Exception Response)
                    // Modbus 标准：如果第 2 个字节（功能码）大于 0x80，说明是一个错误响应
                    // 错误响应的固定长度是 5 字节
                    if (buffer.Count >= 2)
                    {
                        byte fc = buffer[1];
                        if (fc > 0x80)
                        {
                            expectedLen = 5; // 强制缩短预期长度，不再死等
                        }
                    }
                }
                else
                {
                    // 适当缩短 Sleep，RTU 通讯 5ms 有点久，可以改为 1-2ms
                    Thread.Sleep(2);
                }
            }

            byte[] finalData = buffer.ToArray();
            LogHub.Write(ChannelName, LogType.Receive, "RX", finalData, isTcp: false);
            return finalData;
        }

        public void Dispose()
        {
            _port?.Dispose();
        }
    }
}

