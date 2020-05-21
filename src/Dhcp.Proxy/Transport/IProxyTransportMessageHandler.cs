using System;

namespace Dhcp.Proxy.Transport
{
    public interface IProxyTransportMessageHandler : IDisposable
    {
        byte[] HandleMessage(ArraySegment<byte> request);
        string HandleMessage(string request);
    }
}
