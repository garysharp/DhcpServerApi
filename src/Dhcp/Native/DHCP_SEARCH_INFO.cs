using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_SEARCH_INFO structure defines the DHCP client record data used to search against for particular server operations.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_SEARCH_INFO_Managed_IpAddress
    {
        private IntPtr searchType;

        /// <summary>
        /// DHCP_SEARCH_INFO_TYPE enumeration value that specifies the data included in the subsequent member of this structure.
        /// </summary>
        public DHCP_SEARCH_INFO_TYPE SearchType
        {
            get => (DHCP_SEARCH_INFO_TYPE)searchType;
            set => searchType = (IntPtr)value;
        }

        /// <summary>
        /// DHCP_IP_ADDRESS value that specifies a client IP address. This field is populated if SearchType is set to DhcpClientIpAddress.
        /// </summary>
        public DHCP_IP_ADDRESS ClientIpAddress;
    }

    /// <summary>
    /// The DHCP_SEARCH_INFO structure defines the DHCP client record data used to search against for particular server operations.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_SEARCH_INFO_Managed_HardwareAddress
    {
        private IntPtr searchType;

        /// <summary>
        /// DHCP_SEARCH_INFO_TYPE enumeration value that specifies the data included in the subsequent member of this structure.
        /// </summary>
        public DHCP_SEARCH_INFO_TYPE SearchType
        {
            get => (DHCP_SEARCH_INFO_TYPE)searchType;
            set => searchType = (IntPtr)value;
        }

        /// <summary>
        /// DHCP_CLIENT_UID structure that contains a hardware MAC address. This field is populated if SearchType is set to DhcpClientHardwareAddress.
        /// </summary>
        public DHCP_CLIENT_UID ClientHardwareAddress;
    }

    /// <summary>
    /// The DHCP_SEARCH_INFO structure defines the DHCP client record data used to search against for particular server operations.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_SEARCH_INFO_Managed_Name
    {
        private IntPtr searchType;

        /// <summary>
        /// DHCP_SEARCH_INFO_TYPE enumeration value that specifies the data included in the subsequent member of this structure.
        /// </summary>
        public DHCP_SEARCH_INFO_TYPE SearchType
        {
            get => (DHCP_SEARCH_INFO_TYPE)searchType;
            set => searchType = (IntPtr)value;
        }

        /// <summary>
        /// Unicode string that specifies the network name of the DHCP client. This field is populated if SearchType is set to DhcpClientName.
        /// </summary>
        public IntPtr ClientName;
    }
}
