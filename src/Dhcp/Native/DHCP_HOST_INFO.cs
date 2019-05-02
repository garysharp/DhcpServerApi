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
        private readonly IntPtr NetBiosNamePointer;
        /// <summary>
        /// Unicode string that contains the network name of the DHCP server.
        /// </summary>
        private readonly IntPtr ServerNamePointer;

        /// <summary>
        /// Unicode string that contains the NetBIOS name of the DHCP server.
        /// </summary>
        public string NetBiosName => (NetBiosNamePointer == IntPtr.Zero) ? null : Marshal.PtrToStringUni(NetBiosNamePointer);

        /// <summary>
        /// Unicode string that contains the network name of the DHCP server.
        /// </summary>
        public string ServerName => (ServerNamePointer == IntPtr.Zero) ? null : Marshal.PtrToStringUni(ServerNamePointer);

        public void Dispose()
        {
            if (NetBiosNamePointer != IntPtr.Zero)
                Api.DhcpRpcFreeMemory(NetBiosNamePointer);

            if (ServerNamePointer != IntPtr.Zero)
                Api.DhcpRpcFreeMemory(ServerNamePointer);
        }
    }
}
