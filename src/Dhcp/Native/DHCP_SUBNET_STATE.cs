namespace Dhcp.Native
{
    internal enum DHCP_SUBNET_STATE : uint
    {
        /// <summary>
        /// The subnet is enabled; the server will distribute addresses, extend leases, and release addresses within the subnet range to clients.
        /// </summary>
        DhcpSubnetEnabled = 0,
        /// <summary>
        /// The subnet is disabled; the server will not distribute addresses or extend leases within the subnet range to clients. However, the server will still release addresses within the subnet range.
        /// </summary>
        DhcpSubnetDisabled,
        /// <summary>
        /// The subnet is enabled; the server will distribute addresses, extend leases, and release addresses within the subnet range to clients. The default gateway is set to the local machine itself.
        /// </summary>
        DhcpSubnetEnabledSwitched,
        /// <summary>
        /// The subnet is disabled; the server will not distribute addresses or extend leases within the subnet range to clients. However, the server will still release addresses within the subnet range. The default gateway is set to the local machine itself.
        /// </summary>
        DhcpSubnetDisabledSwitched,
        /// <summary>
        /// The subnet is in an invalid state.
        /// </summary>
        DhcpSubnetInvalidState,
    }
}
