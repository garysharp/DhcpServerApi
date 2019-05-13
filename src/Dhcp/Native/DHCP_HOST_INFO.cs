using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_HOST_INFO structure defines information on a DHCP server (host).
    /// </summary>
    /// <remarks>
    /// When this structure is populated by the DHCP Server, the HostName and NetBiosName members may be set to NULL.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_HOST_INFO : IDisposable
    {
        /// <summary>
        /// DHCP_IP_ADDRESS value that contains the IP address of the DHCP server.
        /// </summary>
        public readonly DHCP_IP_ADDRESS IpAddress;
        /// <summary>
        /// Unicode string that contains the NetBIOS name of the DHCP server.
        /// </summary>
        private IntPtr NetBiosNamePointer;
        /// <summary>
        /// Unicode string that contains the network name of the DHCP server.
        /// </summary>
        private IntPtr ServerNamePointer;

        /// <summary>
        /// Unicode string that contains the NetBIOS name of the DHCP server.
        /// </summary>
        public string NetBiosName => Marshal.PtrToStringUni(NetBiosNamePointer);

        /// <summary>
        /// Unicode string that contains the network name of the DHCP server.
        /// </summary>
        public string ServerName => Marshal.PtrToStringUni(ServerNamePointer);

        public void Dispose()
        {
            Api.FreePointer(ref NetBiosNamePointer);

            // Freeing ServerName causes heap corruption ?!?!?
            // Api.FreePointer(ref ServerNamePointer);
        }
    }

    /// <summary>
    /// The DHCP_HOST_INFO structure defines information on a DHCP server (host).
    /// </summary>
    /// <remarks>
    /// When this structure is populated by the DHCP Server, the HostName and NetBiosName members may be set to NULL.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DHCP_HOST_INFO_Managed
    {
        /// <summary>
        /// DHCP_IP_ADDRESS value that contains the IP address of the DHCP server.
        /// </summary>
        public readonly DHCP_IP_ADDRESS IpAddress;
        /// <summary>
        /// Unicode string that contains the NetBIOS name of the DHCP server.
        /// </summary>
        private readonly string NetBiosName;
        /// <summary>
        /// Unicode string that contains the network name of the DHCP server.
        /// </summary>
        private readonly string ServerName;

        public DHCP_HOST_INFO_Managed(DHCP_IP_ADDRESS ipAddress, string netBiosName, string serverName)
        {
            IpAddress = ipAddress;
            NetBiosName = netBiosName;
            ServerName = serverName;
        }
    }
}
