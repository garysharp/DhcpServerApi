namespace Dhcp
{
    public interface IDhcpServerPacketWritable : IDhcpServerPacket
    {
        /// <summary>
        /// Packet OP Code/Message Type (OP)
        /// </summary>
        new DhcpServerMessageTypes MessageType { get; set; }

        /// <summary>
        /// Type of Hardware Address (HTYPE)
        /// </summary>
        new DhcpServerHardwareType HardwareAddressType { get; set; }

        /// <summary>
        /// Length (in bytes) of the Hardware Address (HLEN)
        /// </summary>
        new byte HardwareAddressLength { get; set; }

        /// <summary>
        /// Optionally used by relay agents when booting via a relay agent (HOPS)
        /// </summary>
        new byte GatewayHops { get; set; }

        /// <summary>
        /// Transaction Id, a random number, used to match this boot request with the responses it generates (XID)
        /// </summary>
        new int TransactionId { get; set; }

        /// <summary>
        /// Seconds elapsed since client started trying to boot (SECS)
        /// </summary>
        new ushort SecondsElapsed { get; set; }

        /// <summary>
        /// Flags (FLAGS)
        /// </summary>
        new DhcpServerPacketFlags Flags { get; set; }

        /// <summary>
        /// Client IP Address; only filled in if client is in BOUND, RENEW or REBINDING state and can respond to ARP requests (CIADDR)
        /// </summary>
        new DhcpServerIpAddress ClientIpAddress { get; set; }

        /// <summary>
        /// 'your' (client) IP address (YIADDR)
        /// </summary>
        new DhcpServerIpAddress YourIpAddress { get; set; }

        /// <summary>
        /// IP address of next server to use in bootstrap; returned in DHCPOFFER, DHCPACK by server (SIADDR)
        /// </summary>
        new DhcpServerIpAddress NextServerIpAddress { get; set; }

        /// <summary>
        /// Relay agent IP address, used in booting via a relay agent (GIADDR)
        /// </summary>
        new DhcpServerIpAddress RelayAgentIpAddress { get; set; }

        /// <summary>
        /// Client hardware address (CHADDR)
        /// </summary>
        new DhcpServerHardwareAddress ClientHardwareAddress { get; set; }

        /// <summary>
        /// Optional server host name (SNAME)
        /// </summary>
        new string ServerHostName { get; set; }

        /// <summary>
        /// Boot file name; "generic" name or null in DHCPDISCOVER, fully qualified directory-path name in DHCPOFFER (FILE)
        /// </summary>
        new string FileName { get; set; }

        new int OptionsMagicCookie { get; set; }

    }
}
