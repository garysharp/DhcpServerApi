using Dhcp.Proxy.Models;
using System;
using System.Collections.Generic;

namespace Dhcp.Proxy
{
    public interface IProxy : IDisposable
    {
        int GetProxyVersion();

        IEnumerable<string> GetRemoteServerNames();

        ConnectModel Connect(string hostNameOrAddress);

        GetAuditLogModel GetAuditLog();
    }
}
