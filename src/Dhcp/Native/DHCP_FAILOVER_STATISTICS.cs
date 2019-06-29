namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_FAILOVER_STATISTICS structure defines DHCP server scope statistics that are part of a failover relationship.
    /// </summary>
    internal readonly struct DHCP_FAILOVER_STATISTICS
    {
        /// <summary>
        /// This member is of type DWORD and contains the total number of IPv4 addresses that can be leased out to DHCPv4 clients on an IPv4 subnet that is part of a failover relationship.
        /// </summary>
        public readonly int NumAddr;
        /// <summary>
        /// This member is of type DWORD and contains the total number of IPv4 addresses that are free and can be leased to DHCPv4 clients on an IPv4 subnet that is part of a failover relationship.
        /// </summary>
        public readonly int AddrFree;
        /// <summary>
        /// This member is of type DWORD and contains the total number of IPv4 addresses leased to DHCPv4 clients on an IPv4 subnet that is part of a failover relationship.
        /// </summary>
        public readonly int AddrInUse;
        /// <summary>
        /// This member is of type DWORD and contains the total number of IPv4 addresses that are free and can be leased to DHCPv4 clients on an IPv4 subnet that is part of a failover relationship on the partner server.
        /// </summary>
        public readonly int PartnerAddrFree;
        /// <summary>
        /// This member is of type DWORD and contains the total number of IPv4 addresses that are free and can be leased to DHCPv4 clients on an IPv4 subnet that is part of a failover relationship on the local DHCP server.
        /// </summary>
        public readonly int ThisAddrFree;
        /// <summary>
        /// This member is of type DWORD and contains the total number of IPv4 addresses leased to DHCPv4 clients on an IPv4 subnet that is part of a failover relationship on the partner server.
        /// </summary>
        public readonly int PartnerAddrInUse;
        /// <summary>
        /// This member is of type DWORD and contains the total number of IPv4 addresses leased to DHCPv4 clients on an IPv4 subnet that is part of a failover relationship on the local DHCP server.
        /// </summary>
        public readonly int ThisAddrInUse;
    }
}
