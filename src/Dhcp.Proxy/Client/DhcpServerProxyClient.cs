using System;

namespace Dhcp.Proxy.Client
{
    public class DhcpServerProxyClient : IDhcpServer
    {
        private readonly IProxy proxy;
        private readonly Lazy<DhcpServerProxyAuditLog> auditLog;

        public DhcpServerIpAddress Address { get; }
        public IDhcpServerAuditLog AuditLog => auditLog.Value;
        public IDhcpServerBindingElementCollection BindingElements => throw new NotImplementedException();
        public IDhcpServerClassCollection Classes => throw new NotImplementedException();
        public IDhcpServerClientCollection Clients => throw new NotImplementedException();
        public IDhcpServerConfiguration Configuration => throw new NotImplementedException();
        public IDhcpServerDnsSettings DnsSettings => throw new NotImplementedException();
        public IDhcpServerFailoverRelationshipCollection FailoverRelationships => throw new NotImplementedException();
        public string Name { get; }
        public IDhcpServerOptionCollection Options => throw new NotImplementedException();
        public IDhcpServerScopeCollection Scopes => throw new NotImplementedException();
        public IDhcpServerSpecificStrings SpecificStrings => throw new NotImplementedException();
        public DhcpServerVersions Version => (DhcpServerVersions)(((ulong)VersionMajor << 16) | (uint)VersionMinor);
        public int VersionMajor { get; }
        public int VersionMinor { get; }

        public DhcpServerProxyClient(IProxy proxy, string hostNameOrAddress)
        {
            var response = proxy.Connect(hostNameOrAddress);

            this.proxy = proxy;
            
            Address = response.Address;
            Name = response.Name;
            VersionMajor = response.VersionMajor;
            VersionMinor = response.VersionMinor;

            auditLog = new Lazy<DhcpServerProxyAuditLog>(() => new DhcpServerProxyAuditLog(this, proxy));
        }

        public IDhcpServerDnsSettings ConfigureDnsSettings(IDhcpServerDnsSettings dnsSettings)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates if this server version is greater than or equal to the supplied <paramref name="version"/>
        /// </summary>
        /// <param name="version">Version to test compatibility against</param>
        /// <returns>True if the server version is greater than or equal to the supplied <paramref name="version"/></returns>
        public bool IsCompatible(DhcpServerVersions version) => ((long)version <= (long)Version);

        #region IDisposable Support
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    proxy.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose() => Dispose(true);
        #endregion

    }
}
