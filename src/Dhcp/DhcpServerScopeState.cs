namespace Dhcp
{
    public enum DhcpServerScopeState : uint
    {
        /// <summary>
        /// The subnet is enabled; the server will distribute addresses, extend leases, and release addresses within the subnet range to clients.
        /// </summary>
        Enabled = 0,
        /// <summary>
        /// The subnet is disabled; the server will not distribute addresses or extend leases within the subnet range to clients. However, the server will still release addresses within the subnet range.
        /// </summary>
        Disabled,
        /// <summary>
        /// The subnet is enabled; the server will distribute addresses, extend leases, and release addresses within the subnet range to clients. The default gateway is set to the local machine itself. 
        /// </summary>
        EnabledSwitched,
        /// <summary>
        /// The subnet is disabled; the server will not distribute addresses or extend leases within the subnet range to clients. However, the server will still release addresses within the subnet range. The default gateway is set to the local machine itself.
        /// </summary>
        DisabledSwitched,
        /// <summary>
        /// The subnet is in an invalid state.
        /// </summary>
        InvalidState
    }
}
