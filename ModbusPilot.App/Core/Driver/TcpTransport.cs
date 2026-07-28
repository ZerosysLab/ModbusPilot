using ModbusPilot.Core.Models;
using ModbusPilot.Core.Services;
using System;
using System.Net.Sockets;
using System.Threading;

namespace ModbusPilot.Core.Driver
{
    public class TcpTransport : ITransport, IDisposable
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private readonly string _ip;
        private readonly int _port;
        private readonly int _timeout = 2000;
        public string ChannelName { get; private set; }

        public TcpTransport(string channelName, string ip, int port)
        {
            ChannelName = channelName;
            _ip = ip;
            _port = port;
        }

        public bool IsConnected => _client != null && _client.Connected;

        public void Connect()
        {
            // 1. 防御性编程：如果已有连接，先杀掉
            Disconnect();

            try
            {
                _client = new TcpClient();

                // 【核心修复 1】设置 LingerOption 为 0
                // 这意味着 Close() 时直接发送 RST 复位包，不等待缓冲区发送，立即释放端口
                _client.LingerState = new LingerOption(true, 0);

                // 设置接收/发送缓冲区，避免小包延迟
                _client.NoDelay = true;
                _client.ReceiveTimeout = _timeout;
                _client.SendTimeout = _timeout;

                // 2. 带超时的连接尝试
                var result = _client.BeginConnect(_ip, _port, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(2000); // 2秒超时

                if (!success)
                {
                    // 必须手动关闭，否则 socket 会悬挂
                    _client.Close();
                    throw new TimeoutException($"Connection to {_ip}:{_port} timed out.");
                }

                _client.EndConnect(result);
                _stream = _client.GetStream();

                // 流的超时也要设
                _stream.ReadTimeout = _timeout;
                _stream.WriteTimeout = _timeout;

                LogHub.Write(ChannelName, LogType.Info, $"TCP Connected to {_ip}:{_port}");
            }
            catch (Exception ex)
            {
                // 连接失败也要确保清理干净
                Disconnect();
                throw new Exception($"Connect Failed: {ex.Message}");
            }
        }

        public void Disconnect()
        {
            // 【核心修复 2】彻底的资源释放顺序
            try
            {
                if (_stream != null)
                {
                    _stream.Close();
                    _stream.Dispose();
                }
            }
            catch { }
            finally { _stream = null; }

            try
            {
                if (_client != null)
                {
                    if (_client.Connected)
                    {
                        // 尝试关闭连接
                        try { _client.Client.Shutdown(SocketShutdown.Both); } catch { }
                    }
                    _client.Close();
                    _client.Dispose();
                }
            }
            catch { }
            finally { _client = null; }
        }

        public void DiscardBuffer()
        {
            try
            {
                if (_stream != null && _stream.DataAvailable && _client.Available > 0)
                {
                    byte[] junk = new byte[_client.Available];
                    _stream.Read(junk, 0, junk.Length);
                }
            }
            catch
            {
                // 忽略丢弃缓存时的错误
            }
        }

        /// <summary>
        /// 智能发送并接收 (自动处理 Modbus TCP 变长报文)
        /// </summary>
        public byte[] SendAndReceive(byte[] request, int expectedLen_UNUSED)
        {
            // 局部变量保护，防止多线程下 _stream 被 Dispose
            var stream = _stream;
            var client = _client;

            if (stream == null || client == null || !client.Connected)
                throw new Exception("Socket is closed.");

            try
            {
                // ---------------------------------------------------------
                // 1. 发送请求
                // ---------------------------------------------------------
                stream.Write(request, 0, request.Length);
                LogHub.Write(ChannelName, LogType.Send, "TX", request, isTcp: true);

                // 设置读取截止时间
                DateTime deadline = DateTime.Now.AddMilliseconds(_timeout + 500);

                // ---------------------------------------------------------
                // 2. 读取 MBAP 报文头 (固定 6 字节)
                //    [TransactionID:2] [ProtocolID:2] [Length:2]
                // ---------------------------------------------------------
                byte[] header = new byte[6];
                int headerRead = 0;

                while (headerRead < 6)
                {
                    if (DateTime.Now > deadline) throw new TimeoutException("Header read timeout.");

                    if (!stream.DataAvailable)
                    {
                        Thread.Sleep(1);
                        continue;
                    }

                    int r = stream.Read(header, headerRead, 6 - headerRead);
                    if (r == 0) throw new System.IO.IOException("Remote closed connection.");
                    headerRead += r;
                }

                // ---------------------------------------------------------
                // 3. 解析后续数据长度
                //    MBAP头部的第4、5字节 (Big Endian) 表示后续字节数
                //    后续包含: [UnitID:1] + [PDU:N]
                // ---------------------------------------------------------
                int bodyLen = (header[4] << 8) | header[5];

                // 安全检查：长度不能为0，也不能太离谱 (Modbus TCP PDU 最大通常 253，加上 UnitID 是 254)
                if (bodyLen <= 0 || bodyLen > 300)
                    throw new Exception($"Invalid Modbus TCP Length: {bodyLen}");

                // ---------------------------------------------------------
                // 4. 读取剩余数据 Body
                // ---------------------------------------------------------
                byte[] body = new byte[bodyLen];
                int bodyRead = 0;

                while (bodyRead < bodyLen)
                {
                    if (DateTime.Now > deadline) throw new TimeoutException("Body read timeout.");

                    // 这里通常不需要判 DataAvailable，因为 TCP 流是连续的，只要 Header 到了，Body 紧接着就到
                    int r = stream.Read(body, bodyRead, bodyLen - bodyRead);
                    if (r == 0) throw new System.IO.IOException("Remote closed connection.");
                    bodyRead += r;
                }

                // ---------------------------------------------------------
                // 5. 拼装完整报文返回
                // ---------------------------------------------------------
                byte[] fullResponse = new byte[6 + bodyLen];
                Array.Copy(header, 0, fullResponse, 0, 6);
                Array.Copy(body, 0, fullResponse, 6, bodyLen);

                LogHub.Write(ChannelName, LogType.Receive, "RX", fullResponse, isTcp: true);
                return fullResponse;
            }
            catch (NullReferenceException)
            {
                throw new Exception("Connection lost.");
            }
            catch (ObjectDisposedException)
            {
                throw new Exception("Socket disposed.");
            }
            catch (Exception ex)
            {
                LogHub.Write(ChannelName, LogType.Error, $"IO Error: {ex.Message}", isTcp: true);
                Disconnect(); // 发生物理错误必须断开
                throw;
            }
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}