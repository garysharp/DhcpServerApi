using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_CLIENT_INFO_V4 structure defines a client information record used by the DHCP server, extending the definition provided in DHCP_CLIENT_INFO by including client type information. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_CLIENT_INFO_V4 : IDisposable
    {
        /// <summary>
        /// DHCP_IP_ADDRESS value that contains the assigned IP address of the DHCP client.
        /// </summary>
        public readonly DHCP_IP_ADDRESS ClientIpAddress;
        /// <summary>
        /// DHCP_IP_MASK value that contains the subnet mask value assigned to the DHCP client.
        /// </summary>
        public readonly DHCP_IP_MASK SubnetMask;
        /// <summary>
        /// DHCP_CLIENT_UID structure containing the MAC address of the client's network interface device.
        /// </summary>
        public readonly DHCP_BINARY_DATA ClientHardwareAddress;
        /// <summary>
        /// Unicode string that specifies the network name of the DHCP client. This member is optional.
        /// </summary>
        private IntPtr ClientNamePointer;
        /// <summary>
        /// Unicode string that contains a comment associated with the DHCP client. This member is optional.
        /// </summary>
        private IntPtr ClientCommentPointer;
        /// <summary>
        /// DATE_TIME structure that contains the date and time the DHCP client lease will expire, in UTC time.
        /// </summary>
        public readonly DATE_TIME ClientLeaseExpires;
        /// <summary>
        /// DHCP_HOST_INFO structure that contains information on the DHCP server that assigned the IP address to the client. 
        /// </summary>
        public readonly DHCP_HOST_INFO OwnerHost;
        /// <summary>
        /// Specifies the types of dynamic IP address service used by the client.
        /// </summary>
        public readonly ClientTypes bClientType;

        /// <summary>
        /// Unicode string that specifies the network name of the DHCP client. This member is optional.
        /// </summary>
        public string ClientName => Marshal.PtrToStringUni(ClientNamePointer);

        /// <summary>
        /// Unicode string that contains a comment associated with the DHCP client. This member is optional.
        /// </summary>
        public string ClientComment => Marshal.PtrToStringUni(ClientCommentPointer);

        public void Dispose()
        {
            ClientHardwareAddress.Dispose();
            Api.FreePointer(ref ClientNamePointer);
            Api.FreePointer(ref ClientCommentPointer);
            OwnerHost.Dispose();
        }
    }
}
