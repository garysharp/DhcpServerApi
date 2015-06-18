using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_CLIENT_INFO_VQ structure defines information about the DHCPv4 client.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_CLIENT_INFO_VQ
    {
        /// <summary>
        /// DHCP_IP_ADDRESS type value that contains the DHCPv4 client's IPv4 address. 
        /// </summary>
        public DHCP_IP_ADDRESS ClientIpAddress;
        /// <summary>
        /// DHCP IP_MASK type value that contains the DHCPv4 client's IPv4 subnet mask address.
        /// </summary>
        public DHCP_IP_MASK SubnetMask;
        /// <summary>
        /// GUID value that contains the hardware address (MAC address) of the DHCPv4 client.
        /// </summary>
        public DHCP_BINARY_DATA ClientHardwareAddress;
        /// <summary>
        /// Pointer to a null-terminated Unicode string that represents the DHCPv4 client's machine name.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string ClientName;
        /// <summary>
        /// Pointer to a null-terminated Unicode string that represents the description given to the DHCPv4 client.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string ClientComment;
        /// <summary>
        /// DATE_TIME structure that contains the lease expiry time for the DHCPv4 client. This is UTC time represented in the FILETIME format.
        /// </summary>
        public DATE_TIME ClientLeaseExpires;
        /// <summary>
        /// DHCP_HOST_INFO structure that contains information about the host machine (DHCPv4 server machine) that has provided a lease to the DHCPv4 client.
        /// </summary>
        public DHCP_HOST_INFO OwnerHost;
        /// <summary>
        /// Possible types of the DHCPv4 client.
        /// </summary>
        public ClientTypes bClientType;
        /// <summary>
        /// Possible states of the IPv4 address given to the DHCPv4 client.
        /// </summary>
        public AddressStates AddressState;
        /// <summary>
        /// QuarantineStatus enumeration that specifies possible health status values for the DHCPv4 client, as validated at the NAP server.
        /// </summary>
        public QuarantineStatus Status;
        /// <summary>
        /// This is of type DATE_TIME, containing the end time of the probation if the DHCPv4 client is on probation. For this time period, the DHCPv4 client has full access to the network.
        /// </summary>
        public DATE_TIME ProbationEnds;
        /// <summary>
        /// If TRUE, the DHCPv4 client is quarantine-enabled; if FALSE, it is not.
        /// </summary>
        public bool QuarantineCapable;
    }
}
