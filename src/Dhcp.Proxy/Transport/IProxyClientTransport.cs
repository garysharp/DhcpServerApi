using System;

namespace Dhcp.Proxy.Transport
{
    public interface IProxyClientTransport : IDisposable
    {
        ArraySegment<byte> Invoke(ArraySegment<byte> request);
        string Invoke(string request);
    }
}
