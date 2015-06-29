using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerAuditLog
    {
        /// <summary>
        /// The associated DHCP Server
        /// </summary>
        public DhcpServer Server { get; private set; }

        /// <summary>
        /// The directory where the audit log is stored as an absolute path within the file system.
        /// </summary>
        public string AuditLogDirectory { get; private set; }

        /// <summary>
        /// The disk check interval for attempting to write the audit log to the specified file as the number of logged DHCP server events that should occur between checks.
        /// </summary>
        public int DiskCheckInterval { get; private set; }

        /// <summary>
        /// The maximum log file size, in bytes.
        /// </summary>
        public int MaxLogFilesSize { get; private set; }
        
        /// <summary>
        /// The minimum required disk space, in bytes, for audit log storage.
        /// </summary>
        public int MinSpaceOnDisk { get; private set; }

        private DhcpServerAuditLog(string AuditLogDir, int DiskCheckInterval, int MaxLogFilesSize, int MinSpaceOnDisk)
        {
            this.AuditLogDirectory = AuditLogDir;
            this.DiskCheckInterval = DiskCheckInterval;
            this.MaxLogFilesSize = MaxLogFilesSize;
            this.MinSpaceOnDisk = MinSpaceOnDisk;
        }

        internal static DhcpServerAuditLog GetParams(DhcpServer Server)
        {
            string auditLogDir;
            int diskCheckInterval, maxLogFilesSize, minSpaceOnDisk;

            DhcpErrors result = Api.DhcpAuditLogGetParams(Server.ipAddress.ToString(), 0, out auditLogDir, out diskCheckInterval, out maxLogFilesSize, out minSpaceOnDisk);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException("DhcpAuditLogGetParams", result);

            return new DhcpServerAuditLog(auditLogDir, diskCheckInterval, maxLogFilesSize, minSpaceOnDisk);
        }
    }
}
