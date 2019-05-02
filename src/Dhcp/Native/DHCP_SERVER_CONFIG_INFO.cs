using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_SERVER_CONFIG_INFO structure defines the data used to configure the DHCP server.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_SERVER_CONFIG_INFO
    {
        /// <summary>
        /// Specifies a set of bit flags that contain the RPC protocols supported by the DHCP server.
        /// </summary>
        public readonly uint APIProtocolSupport;
        /// <summary>
        /// Unicode string that specifies the file name of the client lease JET database.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public readonly string DatabaseName;
        /// <summary>
        /// Unicode string that specifies the absolute path to DatabaseName.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public readonly string DatabasePath;
        /// <summary>
        /// Unicode string that specifies the absolute path and file name of the backup client lease JET database.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public readonly string BackupPath;
        /// <summary>
        /// Specifies the interval, in minutes, between backups of the client lease database.
        /// </summary>
        public readonly int BackupInterval;
        /// <summary>
        /// Specifies a bit flag that indicates whether or not database actions should be logged.
        /// </summary>
        public readonly uint DatabaseLoggingFlag;
        /// <summary>
        /// Specifies a bit flag that indicates whether or not a database restore operation should be performed.
        /// </summary>
        public readonly uint RestoreFlag;
        /// <summary>
        /// Specifies the interval, in minutes, between cleanup operations performed on the client lease database.
        /// </summary>
        public readonly int DatabaseCleanupInterval;
        /// <summary>
        /// Reserved. This field should be set to 0x00000000.
        /// </summary>
        public readonly uint DebugFlag;
    }
}
