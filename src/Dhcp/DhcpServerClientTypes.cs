using System;

namespace Dhcp
{
    [Flags]
    public enum DhcpServerClientTypes
    {
        /// <summary>
        /// A client other than ones defined in this table.
        /// </summary>
        Unspecified = 0x00,
        /// <summary>
        /// Supports the DHCP protocol.
        /// </summary>
        Dhcp = 0x01,
        /// <summary>
        /// Supports the BOOTP protocol.
        /// </summary>
        Bootp = 0x02,
        /// <summary>
        /// Supports the DHCP and BOOTP protocols.
        /// </summary>
        DhcpAndBootp = 0x03,
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
