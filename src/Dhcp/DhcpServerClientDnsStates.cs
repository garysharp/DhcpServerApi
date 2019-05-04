using System;

namespace Dhcp
{
    [Flags]
    public enum DhcpServerClientDnsStates
    {
        /// <summary>
        /// The DNS update for the DHCPv4 client lease record needs to be deleted from the DNS server when the lease is deleted.
        /// </summary>
        Cleanup = 0x01,
        /// <summary>
        /// The DNS update needs to be sent for both A and PTR resource records ([RFC1034] section 3.6).
        /// </summary>
        UpdateBothRecords = 0x02,
        /// <summary>
        /// The DNS update is not completed for the lease record.
        /// </summary>
        Unregistered = 0x04,
        /// <summary>
        /// The address lease is expired, but the DNS updates for the lease record have not been deleted from the DNS server.
        /// </summary>
        Deleted = 0x08,
        /// <summary>
        /// The DNS State is unknown or not supported by the server.
        /// </summary>
        Unknown = -1,
    }
}
