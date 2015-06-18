using System;

namespace Dhcp.Native
{
    /// <summary>
    /// Possible types of the DHCPv4 client.
    /// </summary>
    [Flags]
    internal enum ClientTypes : byte
    {
        /// <summary>
        /// A DHCPv4 client other than ones defined in this table.
        /// </summary>
        CLIENT_TYPE_UNSPECIFIED = 0x00,
        /// <summary>
        /// The DHCPv4 client supports the DHCP protocol.
        /// </summary>
        CLIENT_TYPE_DHCP = 0x01,
        /// <summary>
        /// The DHCPv4 client supports the BOOTP protocol.
        /// </summary>
        CLIENT_TYPE_BOOTP = 0x02,
        /// <summary>
        /// The DHCPv4 client understands both the DHCPv4 and the BOOTP protocols.
        /// </summary>
        CLIENT_TYPE_BOTH = 0x03,
        /// <summary>
        /// There is an IPv4 reservation created for the DHCPv4 client.
        /// </summary>
        CLIENT_TYPE_RESERVATION_FLAG = 0x04,
        /// <summary>
        /// Backward compatibility for manual addressing.
        /// </summary>
        CLIENT_TYPE_NONE = 0x64,
    }
}
