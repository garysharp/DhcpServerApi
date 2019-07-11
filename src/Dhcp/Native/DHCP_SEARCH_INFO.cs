using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_SEARCH_INFO structure defines the DHCP client record data used to search against for particular server operations.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_SEARCH_INFO_Managed_IpAddress
    {
        private readonly IntPtr searchType;

        /// <summary>
        /// DHCP_SEARCH_INFO_TYPE enumeration value that specifies the data included in the subsequent member of this structure.
        /// </summary>
        public DHCP_SEARCH_INFO_TYPE SearchType => (DHCP_SEARCH_INFO_TYPE)searchType;

        /// <summary>
        /// DHCP_IP_ADDRESS value that specifies a client IP address. This field is populated if SearchType is set to DhcpClientIpAddress.
        /// </summary>
        public readonly DHCP_IP_ADDRESS ClientIpAddress;

        public DHCP_SEARCH_INFO_Managed_IpAddress(DHCP_IP_ADDRESS clientIpAddress)
        {
            searchType = (IntPtr)DHCP_SEARCH_INFO_TYPE.DhcpClientIpAddress;
            ClientIpAddress = clientIpAddress;
        }
    }

    /// <summary>
    /// The DHCP_SEARCH_INFO structure defines the DHCP client record data used to search against for particular server operations.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_SEARCH_INFO_Managed_HardwareAddress
    {
        private readonly IntPtr searchType;

        /// <summary>
        /// DHCP_SEARCH_INFO_TYPE enumeration value that specifies the data included in the subsequent member of this structure.
        /// </summary>
        public DHCP_SEARCH_INFO_TYPE SearchType => (DHCP_SEARCH_INFO_TYPE)searchType;

        /// <summary>
        /// DHCP_CLIENT_UID structure that contains a hardware MAC address. This field is populated if SearchType is set to DhcpClientHardwareAddress.
        /// </summary>
        public readonly DHCP_CLIENT_UID ClientHardwareAddress;

        public DHCP_SEARCH_INFO_Managed_HardwareAddress(DHCP_CLIENT_UID clientHardwareAddress)
        {
            searchType = (IntPtr)DHCP_SEARCH_INFO_TYPE.DhcpClientHardwareAddress;
            ClientHardwareAddress = clientHardwareAddress;
        }
    }

    /// <summary>
    /// The DHCP_SEARCH_INFO structure defines the DHCP client record data used to search against for particular server operations.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_SEARCH_INFO_Managed_Name
    {
        private readonly IntPtr searchType;

        /// <summary>
        /// DHCP_SEARCH_INFO_TYPE enumeration value that specifies the data included in the subsequent member of this structure.
        /// </summary>
        public DHCP_SEARCH_INFO_TYPE SearchType => (DHCP_SEARCH_INFO_TYPE)searchType;

        /// <summary>
        /// Unicode string that specifies the network name of the DHCP client. This field is populated if SearchType is set to DhcpClientName.
        /// </summary>
        public readonly IntPtr ClientName;

        public DHCP_SEARCH_INFO_Managed_Name(IntPtr clientName)
        {
            searchType = (IntPtr)DHCP_SEARCH_INFO_TYPE.DhcpClientName;
            ClientName = clientName;
        }
    }
}
