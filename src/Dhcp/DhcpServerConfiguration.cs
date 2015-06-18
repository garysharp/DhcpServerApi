using Dhcp.Native;
using System;
using System.Runtime.InteropServices;

namespace Dhcp
{
    public class DhcpServerConfiguration
    {
        public DhcpServer Server { get; private set; }

        /// <summary>
        /// Specifies a set of bit flags that contain the RPC protocols supported by the DHCP server.
        /// </summary>
        public DhcpServerApiProtocol ApiProtocolSupport { get; private set; }

        /// <summary>
        /// Unicode string that specifies the file name of the client lease JET database.
        /// </summary>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// Unicode string that specifies the absolute path to DatabaseName.
        /// </summary>
        public string DatabasePath { get; private set; }

        /// <summary>
        /// Unicode string that specifies the absolute path and file name of the backup client lease JET database.
        /// </summary>
        public string BackupPath { get; private set; }

        /// <summary>
        /// Specifies the interval between backups of the client lease database.
        /// </summary>
        public TimeSpan BackupInterval { get; private set; }

        /// <summary>
        /// Specifies a bit flag that indicates whether or not database actions should be logged.
        /// </summary>
        public bool DatabaseLoggingEnabled { get; private set; }

        /// <summary>
        /// Specifies the interval between cleanup operations performed on the client lease database.
        /// </summary>
        public TimeSpan DatabaseCleanupInterval { get; private set; }

        private DhcpServerConfiguration(DhcpServer Server)
        {
            this.Server = Server;
        }

        internal static DhcpServerConfiguration GetConfiguration(DhcpServer Server)
        {
            IntPtr configInfoPtr;

            var result = Api.DhcpServerGetConfig(Server.IpAddress, out configInfoPtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException("DhcpServerGetConfig", result);

            try
            {
                var configInfo = (DHCP_SERVER_CONFIG_INFO)Marshal.PtrToStructure(configInfoPtr, typeof(DHCP_SERVER_CONFIG_INFO));
                
                return FromNative(Server, configInfo);
            }
            finally
            {
                Api.DhcpRpcFreeMemory(configInfoPtr);
            }
        }

        private static DhcpServerConfiguration FromNative(DhcpServer Server, DHCP_SERVER_CONFIG_INFO Native)
        {
            return new DhcpServerConfiguration(Server)
            {
                ApiProtocolSupport = (DhcpServerApiProtocol)Native.APIProtocolSupport,
                DatabaseName = Native.DatabaseName,
                DatabasePath = Native.DatabasePath,
                BackupPath = Native.BackupPath,
                BackupInterval = TimeSpan.FromMinutes(Native.BackupInterval),
                DatabaseLoggingEnabled = (Native.DatabaseLoggingFlag & 0x1) == 0x1,
                DatabaseCleanupInterval = TimeSpan.FromMinutes(Native.DatabaseCleanupInterval)
            };
        }

        public override string ToString()
        {
            return string.Format("DHCP Configuration: {0} ({1})", this.Server.Name, this.Server.IpAddress);
        }
    }
}
