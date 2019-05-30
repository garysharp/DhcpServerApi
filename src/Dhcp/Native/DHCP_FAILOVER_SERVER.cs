namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_FAILOVER_SERVER enumeration defines whether the DHCP server is the primary or secondary server in a DHCPv4 failover relationship.
    /// </summary>
    internal enum DHCP_FAILOVER_SERVER
    {
        /// <summary>
        /// The server is a primary server in the failover relationship.
        /// </summary>
        PrimaryServer,
        /// <summary>
        /// The server is a secondary server in the failover relationship.
        /// </summary>
        SecondaryServer
    }
}
