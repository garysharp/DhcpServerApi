using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_CLIENT_INFO structure defines a client information record used by the DHCP server.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_CLIENT_INFO : IDisposable
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
        private readonly IntPtr ClientNamePointer;
        /// <summary>
        /// Unicode string that contains a comment associated with the DHCP client. This member is optional.
        /// </summary>
        private readonly IntPtr ClientCommentPointer;
        /// <summary>
        /// DATE_TIME structure that contains the date and time the DHCP client lease will expire, in UTC time.
        /// </summary>
        public readonly DATE_TIME ClientLeaseExpires;
        /// <summary>
        /// DHCP_HOST_INFO structure that contains information on the DHCP server that assigned the IP address to the client. 
        /// </summary>
        public readonly DHCP_HOST_INFO OwnerHost;

        /// <summary>
        /// Unicode string that specifies the network name of the DHCP client. This member is optional.
        /// </summary>
        public string ClientName => Marshal.PtrToStringUni(ClientNamePointer);
        /// <summary>
        /// Unicode string that contains a comment associated with the DHCP client. This member is optional.
        /// </summary>
        public string ClientComment => Marshal.PtrToStringUni(ClientCommentPointer);

        public DhcpServerIpAddress SubnetAddress => (ClientIpAddress & SubnetMask).AsNetworkToIpAddress();

        public void Dispose()
        {
            ClientHardwareAddress.Dispose();
            Api.FreePointer(ClientNamePointer);
            Api.FreePointer(ClientCommentPointer);
            OwnerHost.Dispose();
        }
    }

    /// <summary>
    /// The DHCP_CLIENT_INFO structure defines a client information record used by the DHCP server.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal readonly struct DHCP_CLIENT_INFO_Managed : IDisposable
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
        public readonly DHCP_BINARY_DATA_Managed ClientHardwareAddress;
        /// <summary>
        /// Unicode string that specifies the network name of the DHCP client. This member is optional.
        /// </summary>
        public readonly string ClientName;
        /// <summary>
        /// Unicode string that contains a comment associated with the DHCP client. This member is optional.
        /// </summary>
        public readonly string ClientComment;
        /// <summary>
        /// DATE_TIME structure that contains the date and time the DHCP client lease will expire, in UTC time.
        /// </summary>
        public readonly DATE_TIME ClientLeaseExpires;
        /// <summary>
        /// DHCP_HOST_INFO structure that contains information on the DHCP server that assigned the IP address to the client. 
        /// </summary>
        public readonly DHCP_HOST_INFO_Managed OwnerHost;

        public DHCP_CLIENT_INFO_Managed(DHCP_IP_ADDRESS clientIpAddress, DHCP_IP_MASK subnetMask, DHCP_BINARY_DATA_Managed clientHardwareAddress, string clientName, string clientComment, DATE_TIME clientLeaseExpires, DHCP_HOST_INFO_Managed ownerHost)
        {
            ClientIpAddress = clientIpAddress;
            SubnetMask = subnetMask;
            ClientHardwareAddress = clientHardwareAddress;
            ClientName = clientName;
            ClientComment = clientComment;
            ClientLeaseExpires = clientLeaseExpires;
            OwnerHost = ownerHost;
        }

        public void Dispose()
        {
            ClientHardwareAddress.Dispose();
        }
    }
}
