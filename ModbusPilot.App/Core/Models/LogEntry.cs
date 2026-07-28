using System;
using System.Text;

namespace ModbusPilot.Core.Models
{
    // 日志类型：决定了 UI 显示的颜色
    public enum LogType
    {
        Send,       // 发送 (TX) - 通常绿色
        Receive,    // 接收 (RX) - 通常黑色
        Info,       // 系统信息 (SYS) - 通常蓝色
        Warning,    // 警告 - 橙色
        Error       // 错误 - 红色
    }

    public class LogEntry
    {
        public DateTime Time { get; set; }
        public string ChannelName { get; set; } // 关键：区分 COM1, COM2, TCP-Client
        public bool IsTcp { get; set; } // 新增属性

        public LogType Type { get; set; }
        public string Message { get; set; }     // 文本描述
        public byte[] Data { get; set; }        // 原始报文 (可选)

        public LogEntry(string channel, LogType type, string msg, byte[] data = null, bool istcp = true)
        {
            Time = DateTime.Now;
            ChannelName = channel;
            Type = type;
            Message = msg;
            Data = data;
            IsTcp = istcp;
        }

        // 辅助方法：把 byte[] 转成 "01 03 00 00 ..." 字符串
        public string GetDataHex()
        {
            if (Data == null || Data.Length == 0) return "";
            return BitConverter.ToString(Data).Replace("-", " ");
        }
    }
}