using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_OPTION_SCOPE_INFO structure defines information about the options provided for a certain DHCP scope.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_OPTION_SCOPE_INFO_Managed_Global
    {
        private readonly IntPtr scopeType;

        /// <summary>
        /// <see cref="DHCP_OPTION_SCOPE_TYPE"/> enumeration value that defines the scope type of the associated DHCP options, and indicates which of the following fields in the union will be populated.
        /// </summary>
        public DHCP_OPTION_SCOPE_TYPE ScopeType => (DHCP_OPTION_SCOPE_TYPE)scopeType;

        public readonly IntPtr GlobalScopeInfo;

        public DHCP_OPTION_SCOPE_INFO_Managed_Global(IntPtr globalScopeInfo)
        {
            scopeType = (IntPtr)DHCP_OPTION_SCOPE_TYPE.DhcpGlobalOptions;
            GlobalScopeInfo = globalScopeInfo;
        }
    }

    /// <summary>
    /// The DHCP_OPTION_SCOPE_INFO structure defines information about the options provided for a certain DHCP scope.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_OPTION_SCOPE_INFO_Managed_Subnet
    {
        private readonly IntPtr scopeType;

        /// <summary>
        /// <see cref="DHCP_OPTION_SCOPE_TYPE"/> enumeration value that defines the scope type of the associated DHCP options, and indicates which of the following fields in the union will be populated.
        /// </summary>
        public DHCP_OPTION_SCOPE_TYPE ScopeType => (DHCP_OPTION_SCOPE_TYPE)scopeType;

        /// <summary>
        /// DHCP_IP_ADDRESS value that contains the subnet ID as a DWORD.
        /// </summary>
        public readonly DHCP_IP_ADDRESS SubnetScopeInfo;

        public DHCP_OPTION_SCOPE_INFO_Managed_Subnet(DHCP_IP_ADDRESS subnetScopeInfo)
        {
            scopeType = (IntPtr)DHCP_OPTION_SCOPE_TYPE.DhcpSubnetOptions;
            SubnetScopeInfo = subnetScopeInfo;
        }
    }

    /// <summary>
    /// The DHCP_OPTION_SCOPE_INFO structure defines information about the options provided for a certain DHCP scope.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_OPTION_SCOPE_INFO_Managed_Reserved
    {
        private readonly IntPtr scopeType;

        /// <summary>
        /// <see cref="DHCP_OPTION_SCOPE_TYPE"/> enumeration value that defines the scope type of the associated DHCP options, and indicates which of the following fields in the union will be populated.
        /// </summary>
        public DHCP_OPTION_SCOPE_TYPE ScopeType => (DHCP_OPTION_SCOPE_TYPE)scopeType;

        /// <summary>
        /// DHCP_IP_ADDRESS value that contains an IP address used to identify the reservation.
        /// </summary>
        public readonly DHCP_IP_ADDRESS ReservedIpAddress;

        /// <summary>
        /// DHCP_IP_ADDRESS value that specifies the subnet ID of the subnet containing the reservation.
        /// </summary>
        public readonly DHCP_IP_ADDRESS ReservedIpSubnetAddress;

        public DHCP_OPTION_SCOPE_INFO_Managed_Reserved(DHCP_IP_ADDRESS reservedIpSubnetAddress, DHCP_IP_ADDRESS reservedIpAddress)
        {
            scopeType = (IntPtr)DHCP_OPTION_SCOPE_TYPE.DhcpReservedOptions;
            ReservedIpAddress = reservedIpAddress;
            ReservedIpSubnetAddress = reservedIpSubnetAddress;
        }
    }

    /// <summary>
    /// The DHCP_OPTION_SCOPE_INFO structure defines information about the options provided for a certain DHCP scope.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_OPTION_SCOPE_INFO_Managed_MScope
    {
        private readonly IntPtr scopeType;

        /// <summary>
        /// <see cref="DHCP_OPTION_SCOPE_TYPE"/> enumeration value that defines the scope type of the associated DHCP options, and indicates which of the following fields in the union will be populated.
        /// </summary>
        public DHCP_OPTION_SCOPE_TYPE ScopeType => (DHCP_OPTION_SCOPE_TYPE)scopeType;

        /// <summary>
        /// Pointer to a Unicode string that contains the multicast scope name (usually represented as the IP address of the multicast router).
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public readonly string MScopeInfo;

        public DHCP_OPTION_SCOPE_INFO_Managed_MScope(string mScopeInfo)
        {
            scopeType = (IntPtr)DHCP_OPTION_SCOPE_TYPE.DhcpMScopeOptions;
            MScopeInfo = mScopeInfo;
        }
    }
}
