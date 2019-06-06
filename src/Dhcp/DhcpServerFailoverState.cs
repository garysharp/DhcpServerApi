using System;

namespace Dhcp
{
    public enum DhcpServerFailoverState
    {
        /// <summary>
        /// Indicates that no state is configured for the DHCPv4 failover relationship.
        /// </summary>
        None,
        /// <summary>
        /// Indicates that the failover relationship on the DHCPv4 server is in the initialization state.
        /// </summary>
        Initialization,
        /// <summary>
        /// Indicates that each server participating in the failover relationship can probe its partner server before starting the DHCP client service. A DHCPv4 server moves into the STARTUP state after INIT.
        /// </summary>
        Startup,
        /// <summary>
        /// Indicates that each server in the failover relationship can service DHCPDISCOVER messages and all other DHCP requests as defined in RFC2131. DHCPv4 servers in the NORMAL state can not service DHCPREQUEST/RENEWAL or DHCPREQUEST/REBINDING requests from the client set defined according to the load balancing algorithm in RFC3074. However, each server can service DHCPREQUEST/RENEWAL or DHCPDISCOVER/REBINDING requests from any client.
        /// </summary>
        Normal,
        /// <summary>
        /// Indicates that each server in a failover relationship is operating independently, but neither assumes that their partner is not operating. The partner server might be operating and simply unable to communicate with this server, or it might not be operating at all.
        /// </summary>
        CommunicationInterupted,
        /// <summary>
        /// Indicates that a server assumes its partner is not currently operating.
        /// </summary>
        PartnerDown,
        /// <summary>
        /// Indicates that a failover relationship between two DHCPv4 servers is attempting to reestablish itself.
        /// </summary>
        PotentialConflict,
        /// <summary>
        /// Indicates that the primary server has received all updates from the secondary server during the failover relationship reintegration process.
        /// </summary>
        ConflictDone,
        /// <summary>
        /// Indicates that two servers in the POTENTIAL_CONFLICT state were attempting to reintegrate their failover relationship with each other, but communications between them failed prior to completion of the reintegration.
        /// </summary>
        ResolutionInterupted,
        /// <summary>
        /// Indicates that a server in a failover relationship has no information in its stable storage facility or that it is reintegrating with a server in the PARTNER_DOWN state.
        /// </summary>
        Recover,
        /// <summary>
        /// Indicates that the DHCPv4 server should wait for a time period equal to Maximum Client Lead Time (MCLT) before moving to the RECOVER_DONE state. The MCLT is the maximum time, in seconds, that one server can extend a lease for a client beyond the lease time known by the partner server.
        /// </summary>
        RecoverWait,
        /// <summary>
        /// This value enables an interlocked transition of one server from the RECOVER state and another server from the PARTNER_DOWN or COMMUNICATION-INT state to the NORMAL state.
        /// </summary>
        RecoverDone,
        /// <summary>
        /// Reserved. Do not use.
        /// </summary>
        [Obsolete]
        Paused,
        /// <summary>
        /// Reserved. Do not use.
        /// </summary>
        [Obsolete]
        Shutdown
    }
}
