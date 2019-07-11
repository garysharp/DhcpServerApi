using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerAuditLog : IDhcpServerAuditLog
    {
        /// <summary>
        /// The associated DHCP Server
        /// </summary>
        public IDhcpServer Server { get; }

        /// <summary>
        /// The directory where the audit log is stored as an absolute path within the file system.
        /// </summary>
        public string AuditLogDirectory { get; }

        /// <summary>
        /// The disk check interval for attempting to write the audit log to the specified file as the number of logged DHCP server events that should occur between checks.
        /// </summary>
        public int DiskCheckInterval { get; }

        /// <summary>
        /// The maximum log file size, in bytes.
        /// </summary>
        public int MaxLogFilesSize { get; }

        /// <summary>
        /// The minimum required disk space, in bytes, for audit log storage.
        /// </summary>
        public int MinSpaceOnDisk { get; }

        private DhcpServerAuditLog(DhcpServer server, string auditLogDir, int diskCheckInterval, int maxLogFilesSize, int minSpaceOnDisk)
        {
            Server = server;
            AuditLogDirectory = auditLogDir;
            DiskCheckInterval = diskCheckInterval;
            MaxLogFilesSize = maxLogFilesSize;
            MinSpaceOnDisk = minSpaceOnDisk;
        }

        internal static DhcpServerAuditLog GetAuditLog(DhcpServer server)
        {
            var result = Api.DhcpAuditLogGetParams(ServerIpAddress: server.Address,
                                                   Flags: 0,
                                                   AuditLogDir: out var auditLogDir,
                                                   DiskCheckInterval: out var diskCheckInterval,
                                                   MaxLogFilesSize: out var maxLogFilesSize,
                                                   MinSpaceOnDisk: out var minSpaceOnDisk);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpAuditLogGetParams), result);

            return new DhcpServerAuditLog(server, auditLogDir, diskCheckInterval, maxLogFilesSize, minSpaceOnDisk);
        }
    }
}
