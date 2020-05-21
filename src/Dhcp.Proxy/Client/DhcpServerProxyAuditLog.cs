namespace Dhcp.Proxy.Client
{
    internal class DhcpServerProxyAuditLog : IDhcpServerAuditLog
    {
        private readonly DhcpServerProxyClient proxyClient;

        public string AuditLogDirectory { get; }
        public int DiskCheckInterval { get; }
        public int MaxLogFilesSize { get; }
        public int MinSpaceOnDisk { get; }
        public IDhcpServer Server => proxyClient;

        public DhcpServerProxyAuditLog(DhcpServerProxyClient proxyClient, IProxy proxy)
        {
            this.proxyClient = proxyClient;
            
            var model = proxy.GetAuditLog();
            AuditLogDirectory = model.AuditLogDirectory;
            DiskCheckInterval = model.DiskCheckInterval;
            MaxLogFilesSize = model.MaxLogFilesSize;
            MinSpaceOnDisk = model.MinSpaceOnDisk;
        }
    }
}
