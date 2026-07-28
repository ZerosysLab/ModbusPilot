using System;

namespace ModbusPilot.Core.Driver
{
    public interface ITransport : IDisposable
    {
        string ChannelName { get; }

        bool IsConnected { get; }
        void Connect();
        void Disconnect();

        byte[] SendAndReceive(byte[] request, int expectedLen);
        void DiscardBuffer();
    }
}