using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_SUBNET_INFO
    {
        /// <summary>
        /// DHCP_IP_ADDRESS value that specifies the subnet ID.
        /// </summary>
        public DHCP_IP_ADDRESS SubnetAddress;
        /// <summary>
        /// DHCP_IP_MASK value that specifies the subnet IP mask.
        /// </summary>
        public DHCP_IP_MASK SubnetMask;
        /// <summary>
        /// Unicode string that specifies the network name of the subnet.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string SubnetName;
        /// <summary>
        /// Unicode string that contains an optional comment particular to this subnet.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string SubnetComment;
        /// <summary>
        /// DHCP_HOST_INFO structure that contains information about the DHCP server servicing this subnet.
        /// </summary>
        public DHCP_HOST_INFO PrimaryHost;
        /// <summary>
        /// DHCP_SUBNET_STATE enumeration value indicating the current state of the subnet (enabled/disabled).
        /// </summary>
        public uint SubnetState;
    }
}
