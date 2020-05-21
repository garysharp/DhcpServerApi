namespace Dhcp.Proxy.Models
{
    public class GetAuditLogModel
    {
        public string AuditLogDirectory { get; set; }
        public int DiskCheckInterval { get; set; }
        public int MaxLogFilesSize { get; set; }
        public int MinSpaceOnDisk { get; set; }

        public static GetAuditLogModel FromAuditLog(IDhcpServerAuditLog auditLog)
        {
            return new GetAuditLogModel()
            {
                AuditLogDirectory = auditLog.AuditLogDirectory,
                DiskCheckInterval = auditLog.DiskCheckInterval,
                MaxLogFilesSize = auditLog.MaxLogFilesSize,
                MinSpaceOnDisk = auditLog.MinSpaceOnDisk,
            };
        }
    }
}
