using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCPDS_SERVER structure defines information on a DHCP server in the context of directory services.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCPDS_SERVER
    {
        /// <summary>
        /// Reserved. This value should be set to 0.
        /// </summary>
        public UInt32 Version;
        /// <summary>
        /// Unicode string that contains the unique name of the DHCP server.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string ServerName;
        /// <summary>
        /// Specifies the IP address of the DHCP server as an unsigned 32-bit integer.
        /// </summary>
        public DHCP_IP_ADDRESS ServerAddress;
        /// <summary>
        /// Specifies a set of bit flags that describe active directory settings for the DHCP server.
        /// </summary>
        public UInt32 Flags;
        /// <summary>
        /// Reserved. This value should be set to 0.
        /// </summary>
        public UInt32 State;
        /// <summary>
        /// Unicode string that contains the active directory path to the DHCP server.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string DsLocation;
        /// <summary>
        /// Reserved. This value should be set to 0.
        /// </summary>
        public UInt32 DsLocType;
    }
}
