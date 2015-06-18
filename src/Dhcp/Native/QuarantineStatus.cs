
namespace Dhcp.Native
{
    /// <summary>
    /// The QuarantineStatus enumeration specifies possible health status values for the DHCPv4 client, as validated at the NAP server. 
    /// </summary>
    internal enum QuarantineStatus
    {
        /// <summary>
        /// The DHCP client is compliant with the health policies defined by the administrator and has normal access to the network.
        /// </summary>
        NOQUARANTINE = 0,
        /// <summary>
        /// The DHCP client is not compliant with the health policies defined by the administrator and is being quarantined with restricted access to the network.
        /// </summary>
        RESTRICTEDACCESS = 1,
        /// <summary>
        /// The DHCP client is not compliant with the health policies defined by the administrator and is being denied access to the network. The DHCP server does not grant an IP address lease to this client.
        /// </summary>
        DROPPACKET = 2,
        /// <summary>
        /// The DHCP client is not compliant with the health policies defined by the administrator and is being granted normal access to the network for a limited time.
        /// </summary>
        PROBATION = 3,
        /// <summary>
        /// The DHCP client is exempt from compliance with the health policies defined by the administrator and is granted normal access to the network.
        /// </summary>
        EXEMPT = 4,
        /// <summary>
        /// The DHCP client is put into the default quarantine state configured on the DHCP NAP server. When a network policy server (NPS) is unavailable, the DHCP client can be put in any of the states NOQUARANTINE, RESTRICTEDACCESS, or DROPPACKET, depending on the default setting on the DHCP NAP server.
        /// </summary>
        DEFAULTQUARSETTING = 5,
        /// <summary>
        /// No quarantine.
        /// </summary>
        NOQUARINFO = 6
    }
}
