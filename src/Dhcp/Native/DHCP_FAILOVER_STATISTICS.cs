namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_FAILOVER_STATISTICS structure defines DHCP server scope statistics that are part of a failover relationship.
    /// </summary>
    internal struct DHCP_FAILOVER_STATISTICS
    {
        /// <summary>
        /// This member is of type DWORD and contains the total number of IPv4 addresses that can be leased out to DHCPv4 clients on an IPv4 subnet that is part of a failover relationship.
        /// </summary>
        public int NumAddr;
        /// <summary>
        /// This member is of type DWORD and contains the total number of IPv4 addresses that are free and can be leased to DHCPv4 clients on an IPv4 subnet that is part of a failover relationship.
        /// </summary>
        public int AddrFree;
        /// <summary>
        /// This member is of type DWORD and contains the total number of IPv4 addresses leased to DHCPv4 clients on an IPv4 subnet that is part of a failover relationship.
        /// </summary>
        public int AddrInUse;
        /// <summary>
        /// This member is of type DWORD and contains the total number of IPv4 addresses that are free and can be leased to DHCPv4 clients on an IPv4 subnet that is part of a failover relationship on the partner server.
        /// </summary>
        public int PartnerAddrFree;
        /// <summary>
        /// This member is of type DWORD and contains the total number of IPv4 addresses that are free and can be leased to DHCPv4 clients on an IPv4 subnet that is part of a failover relationship on the local DHCP server.
        /// </summary>
        public int ThisAddrFree;
        /// <summary>
        /// This member is of type DWORD and contains the total number of IPv4 addresses leased to DHCPv4 clients on an IPv4 subnet that is part of a failover relationship on the partner server.
        /// </summary>
        public int PartnerAddrInUse;
        /// <summary>
        /// This member is of type DWORD and contains the total number of IPv4 addresses leased to DHCPv4 clients on an IPv4 subnet that is part of a failover relationship on the local DHCP server.
        /// </summary>
        public int ThisAddrInUse;
    }
}
