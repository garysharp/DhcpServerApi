using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_CLIENT_INFO_VQ structure defines information about the DHCPv4 client.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_CLIENT_INFO_VQ : IDisposable
    {
        /// <summary>
        /// DHCP_IP_ADDRESS type value that contains the DHCPv4 client's IPv4 address. 
        /// </summary>
        public readonly DHCP_IP_ADDRESS ClientIpAddress;
        /// <summary>
        /// DHCP IP_MASK type value that contains the DHCPv4 client's IPv4 subnet mask address.
        /// </summary>
        public readonly DHCP_IP_MASK SubnetMask;
        /// <summary>
        /// GUID value that contains the hardware address (MAC address) of the DHCPv4 client.
        /// </summary>
        public readonly DHCP_BINARY_DATA ClientHardwareAddress;
        /// <summary>
        /// Pointer to a null-terminated Unicode string that represents the DHCPv4 client's machine name.
        /// </summary>
        private IntPtr ClientNamePointer;
        /// <summary>
        /// Pointer to a null-terminated Unicode string that represents the description given to the DHCPv4 client.
        /// </summary>
        private IntPtr ClientCommentPointer;
        /// <summary>
        /// DATE_TIME structure that contains the lease expiry time for the DHCPv4 client. This is UTC time represented in the FILETIME format.
        /// </summary>
        public readonly DATE_TIME ClientLeaseExpires;
        /// <summary>
        /// DHCP_HOST_INFO structure that contains information about the host machine (DHCPv4 server machine) that has provided a lease to the DHCPv4 client.
        /// </summary>
        public readonly DHCP_HOST_INFO OwnerHost;
        /// <summary>
        /// Possible types of the DHCPv4 client.
        /// </summary>
        public readonly DHCP_CLIENT_TYPE ClientType;
        /// <summary>
        /// Possible states of the IPv4 address given to the DHCPv4 client.
        /// </summary>
        public readonly byte AddressState;
        /// <summary>
        /// QuarantineStatus enumeration that specifies possible health status values for the DHCPv4 client, as validated at the NAP server.
        /// </summary>
        public readonly QuarantineStatus Status;
        /// <summary>
        /// This is of type DATE_TIME, containing the end time of the probation if the DHCPv4 client is on probation. For this time period, the DHCPv4 client has full access to the network.
        /// </summary>
        public readonly DATE_TIME ProbationEnds;
        /// <summary>
        /// If TRUE, the DHCPv4 client is quarantine-enabled; if FALSE, it is not.
        /// </summary>
        public readonly bool QuarantineCapable;

        /// <summary>
        /// Pointer to a null-terminated Unicode string that represents the DHCPv4 client's machine name.
        /// </summary>
        public string ClientName => Marshal.PtrToStringUni(ClientNamePointer);

        /// <summary>
        /// Pointer to a null-terminated Unicode string that represents the description given to the DHCPv4 client.
        /// </summary>
        public string ClientComment => Marshal.PtrToStringUni(ClientCommentPointer);

        public DhcpServerIpAddress SubnetAddress => (ClientIpAddress & SubnetMask).AsNetworkToIpAddress();

        public void Dispose()
        {
            ClientHardwareAddress.Dispose();
            Api.FreePointer(ref ClientNamePointer);
            Api.FreePointer(ref ClientCommentPointer);
            OwnerHost.Dispose();
        }
    }

    /// <summary>
    /// The DHCP_CLIENT_INFO_VQ structure defines information about the DHCPv4 client.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DHCP_CLIENT_INFO_VQ_Managed : IDisposable
    {
        /// <summary>
        /// DHCP_IP_ADDRESS type value that contains the DHCPv4 client's IPv4 address. 
        /// </summary>
        public readonly DHCP_IP_ADDRESS ClientIpAddress;
        /// <summary>
        /// DHCP IP_MASK type value that contains the DHCPv4 client's IPv4 subnet mask address.
        /// </summary>
        public readonly DHCP_IP_MASK SubnetMask;
        /// <summary>
        /// GUID value that contains the hardware address (MAC address) of the DHCPv4 client.
        /// </summary>
        public readonly DHCP_BINARY_DATA_Managed ClientHardwareAddress;
        /// <summary>
        /// Pointer to a null-terminated Unicode string that represents the DHCPv4 client's machine name.
        /// </summary>
        private readonly string ClientName;
        /// <summary>
        /// Pointer to a null-terminated Unicode string that represents the description given to the DHCPv4 client.
        /// </summary>
        private readonly string ClientComment;
        /// <summary>
        /// DATE_TIME structure that contains the lease expiry time for the DHCPv4 client. This is UTC time represented in the FILETIME format.
        /// </summary>
        public readonly DATE_TIME ClientLeaseExpires;
        /// <summary>
        /// DHCP_HOST_INFO structure that contains information about the host machine (DHCPv4 server machine) that has provided a lease to the DHCPv4 client.
        /// </summary>
        public readonly DHCP_HOST_INFO_Managed OwnerHost;
        /// <summary>
        /// Possible types of the DHCPv4 client.
        /// </summary>
        public readonly DHCP_CLIENT_TYPE ClientType;
        /// <summary>
        /// Possible states of the IPv4 address given to the DHCPv4 client.
        /// </summary>
        public readonly byte AddressState;
        /// <summary>
        /// QuarantineStatus enumeration that specifies possible health status values for the DHCPv4 client, as validated at the NAP server.
        /// </summary>
        public readonly QuarantineStatus Status;
        /// <summary>
        /// This is of type DATE_TIME, containing the end time of the probation if the DHCPv4 client is on probation. For this time period, the DHCPv4 client has full access to the network.
        /// </summary>
        public readonly DATE_TIME ProbationEnds;
        /// <summary>
        /// If TRUE, the DHCPv4 client is quarantine-enabled; if FALSE, it is not.
        /// </summary>
        public readonly bool QuarantineCapable;

        public DHCP_CLIENT_INFO_VQ_Managed(DHCP_IP_ADDRESS clientIpAddress, DHCP_IP_MASK subnetMask, DHCP_BINARY_DATA_Managed clientHardwareAddress, string clientName, string clientComment, DATE_TIME clientLeaseExpires, DHCP_HOST_INFO_Managed ownerHost, DHCP_CLIENT_TYPE clientType, byte addressState, QuarantineStatus status, DATE_TIME probationEnds, bool quarantineCapable)
        {
            ClientIpAddress = clientIpAddress;
            SubnetMask = subnetMask;
            ClientHardwareAddress = clientHardwareAddress;
            ClientName = clientName;
            ClientComment = clientComment;
            ClientLeaseExpires = clientLeaseExpires;
            OwnerHost = ownerHost;
            ClientType = clientType;
            AddressState = addressState;
            Status = status;
            ProbationEnds = probationEnds;
            QuarantineCapable = quarantineCapable;
        }

        public void Dispose()
        {
            ClientHardwareAddress.Dispose();
        }
    }
}
