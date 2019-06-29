using System;

namespace Dhcp
{
    public interface IDhcpServerConfiguration
    {
        DhcpServerApiProtocol ApiProtocolSupport { get; }
        TimeSpan BackupInterval { get; }
        string BackupPath { get; }
        TimeSpan DatabaseCleanupInterval { get; }
        bool DatabaseLoggingEnabled { get; }
        string DatabaseName { get; }
        string DatabasePath { get; }
        IDhcpServer Server { get; }
    }
}
