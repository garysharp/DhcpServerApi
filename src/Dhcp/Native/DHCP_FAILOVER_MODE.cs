namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_FAILOVER_MODE enumeration defines the DHCPv4 server mode operation in a failover relationship.
    /// </summary>
    internal enum DHCP_FAILOVER_MODE
    {
        /// <summary>
        /// The DHCPv4 server failover relationship is in Load Balancing mode.
        /// </summary>
        LoadBalance,
        /// <summary>
        /// The DHCPv4 server failover relationship is in Hot Standby mode.
        /// </summary>
        HotStandby
    }
}
