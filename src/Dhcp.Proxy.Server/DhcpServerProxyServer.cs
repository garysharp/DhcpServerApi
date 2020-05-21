using Dhcp.Proxy.Models;
using Dhcp.Proxy.Transport;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dhcp.Proxy.Server
{
    public class DhcpServerProxyServer : IProxy
    {
        private string dhcpServerHostNameOrAddress;
        private IDhcpServer dhcpServer;
        private readonly DhcpServerProxyServerConfiguration config;

        public DhcpServerProxyServer(IConfiguration config)
        {
            this.config = config.Get<DhcpServerProxyServerConfiguration>();
        }

        public int GetProxyVersion() => 1;

        public IEnumerable<string> GetRemoteServerNames()
        {
            if ((config.AllowedServers?.Count ?? 0) > 0)
                return config.AllowedServers;
            else
                return DhcpServer.Servers.Select(s => s.Name);
        }

        public ConnectModel Connect(string hostNameOrAddress)
        {
            if (string.IsNullOrWhiteSpace(hostNameOrAddress))
                throw new ArgumentNullException(nameof(hostNameOrAddress));

            if (dhcpServer != null && !hostNameOrAddress.Equals(dhcpServerHostNameOrAddress, StringComparison.OrdinalIgnoreCase))
                throw new ProxyTransportException("The proxy connection is already connected to a different DHCP Server instance.");

            // validate allowed server
            if (!(config.AllowedServers?.Any(s => hostNameOrAddress.Equals(s, StringComparison.OrdinalIgnoreCase)) ?? true))
                throw new DhcpServerException("Connect", DhcpServerNativeErrors.ERROR_ACCESS_DENIED, "DHCP proxy denies access to the specified server");

            dhcpServer = DhcpServer.Connect(hostNameOrAddress);
            dhcpServerHostNameOrAddress = hostNameOrAddress;

            return ConnectModel.FromDhcpServer(dhcpServer);
        }

        public GetAuditLogModel GetAuditLog()
        {
            EnsureConnected();

            var auditLog = dhcpServer.AuditLog;
            return GetAuditLogModel.FromAuditLog(auditLog);
        }

        private void EnsureConnected()
        {
            if (dhcpServer == null)
                throw new ProxyTransportException("The proxy connection must be connected to a DHCP Server instance before this method can be called.");
        }

        #region IDisposable Support
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (dhcpServer != null)
                    {
                        dhcpServer.Dispose();
                        dhcpServer = null;
                    }
                }

                disposedValue = true;
            }
        }
        public void Dispose() => Dispose(true);
        #endregion

    }
}
