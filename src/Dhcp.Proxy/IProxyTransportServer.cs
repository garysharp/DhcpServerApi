using System;

namespace Dhcp.Proxy
{
    public interface IProxyTransportServer : IDisposable
    {
        void Start();
        void Stop();
    }
}
