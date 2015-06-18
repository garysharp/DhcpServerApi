using System;

namespace Dhcp
{
    [Flags]
    public enum DhcpServerClientTypes
    {
        /// <summary>
        /// A DHCPv4 client other than ones defined in this table.
        /// </summary>
        Unspecified = 0x00,
        /// <summary>
        /// The DHCPv4 client supports the DHCP protocol.
        /// </summary>
        DHCP = 0x01,
        /// <summary>
        /// The DHCPv4 client supports the BOOTP protocol.
        /// </summary>
        BOOTP = 0x02,
        /// <summary>
        /// There is an IPv4 reservation created for the DHCPv4 client.
        /// </summary>
        Reservation = 0x04,
        /// <summary>
        /// Backward compatibility for manual addressing.
        /// </summary>
        None = 0x64,
    }
}
