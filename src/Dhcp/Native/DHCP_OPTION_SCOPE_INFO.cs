using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_OPTION_SCOPE_INFO structure defines information about the options provided for a certain DHCP scope.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_OPTION_SCOPE_INFO_LocalGlobal
    {
        private IntPtr scopeType;

        /// <summary>
        /// <see cref="DHCP_OPTION_SCOPE_TYPE"/> enumeration value that defines the scope type of the associated DHCP options, and indicates which of the following fields in the union will be populated.
        /// </summary>
        public DHCP_OPTION_SCOPE_TYPE ScopeType
        {
            get => (DHCP_OPTION_SCOPE_TYPE)scopeType;
            set => scopeType = (IntPtr)value;
        }

        public IntPtr GlobalScopeInfo;
    }

    /// <summary>
    /// The DHCP_OPTION_SCOPE_INFO structure defines information about the options provided for a certain DHCP scope.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_OPTION_SCOPE_INFO_LocalSubnet
    {
        private IntPtr scopeType;

        /// <summary>
        /// <see cref="DHCP_OPTION_SCOPE_TYPE"/> enumeration value that defines the scope type of the associated DHCP options, and indicates which of the following fields in the union will be populated.
        /// </summary>
        public DHCP_OPTION_SCOPE_TYPE ScopeType
        {
            get => (DHCP_OPTION_SCOPE_TYPE)scopeType;
            set => scopeType = (IntPtr)value;
        }

        /// <summary>
        /// DHCP_IP_ADDRESS value that contains the subnet ID as a DWORD.
        /// </summary>
        public DHCP_IP_ADDRESS SubnetScopeInfo;
    }

    /// <summary>
    /// The DHCP_OPTION_SCOPE_INFO structure defines information about the options provided for a certain DHCP scope.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_OPTION_SCOPE_INFO_LocalReserved
    {
        private IntPtr scopeType;

        /// <summary>
        /// <see cref="DHCP_OPTION_SCOPE_TYPE"/> enumeration value that defines the scope type of the associated DHCP options, and indicates which of the following fields in the union will be populated.
        /// </summary>
        public DHCP_OPTION_SCOPE_TYPE ScopeType
        {
            get => (DHCP_OPTION_SCOPE_TYPE)scopeType;
            set => scopeType = (IntPtr)value;
        }

        /// <summary>
        /// DHCP_IP_ADDRESS value that contains an IP address used to identify the reservation.
        /// </summary>
        public DHCP_IP_ADDRESS ReservedIpAddress;

        /// <summary>
        /// DHCP_IP_ADDRESS value that specifies the subnet ID of the subnet containing the reservation.
        /// </summary>
        public DHCP_IP_ADDRESS ReservedIpSubnetAddress;
    }

    /// <summary>
    /// The DHCP_OPTION_SCOPE_INFO structure defines information about the options provided for a certain DHCP scope.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_OPTION_SCOPE_INFO_LocalMScope
    {
        private IntPtr scopeType;

        /// <summary>
        /// <see cref="DHCP_OPTION_SCOPE_TYPE"/> enumeration value that defines the scope type of the associated DHCP options, and indicates which of the following fields in the union will be populated.
        /// </summary>
        public DHCP_OPTION_SCOPE_TYPE ScopeType
        {
            get => (DHCP_OPTION_SCOPE_TYPE)scopeType;
            set => scopeType = (IntPtr)value;
        }

        /// <summary>
        /// Pointer to a Unicode string that contains the multicast scope name (usually represented as the IP address of the multicast router).
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string MScopeInfo;
    }
}
