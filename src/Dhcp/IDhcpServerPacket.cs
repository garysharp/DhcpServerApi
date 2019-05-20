using System.Collections.ObjectModel;

namespace Dhcp
{
    public interface IDhcpServerPacket
    {
        /// <summary>
        /// Packet OP Code/Message Type (OP)
        /// </summary>
        DhcpServerMessageTypes MessageType { get; }

        /// <summary>
        /// Type of Hardware Address (HTYPE)
        /// </summary>
        DhcpServerHardwareType HardwareAddressType { get; }
        
        /// <summary>
        /// Length (in bytes) of the Hardware Address (HLEN)
        /// </summary>
        byte HardwareAddressLength { get; }
        
        /// <summary>
        /// Optionally used by relay agents when booting via a relay agent (HOPS)
        /// </summary>
        byte GatewayHops { get; }
        
        /// <summary>
        /// Transaction Id, a random number, used to match this boot request with the responses it generates (XID)
        /// </summary>
        int TransactionId { get; }
        
        /// <summary>
        /// Seconds elapsed since client started trying to boot (SECS)
        /// </summary>
        ushort SecondsElapsed { get; }
        
        /// <summary>
        /// Flags (FLAGS)
        /// </summary>
        DhcpServerPacketFlags Flags { get; }
        
        /// <summary>
        /// Client IP Address; only filled in if client is in BOUND, RENEW or REBINDING state and can respond to ARP requests (CIADDR)
        /// </summary>
        DhcpServerIpAddress ClientIpAddress { get; }
        
        /// <summary>
        /// 'your' (client) IP address (YIADDR)
        /// </summary>
        DhcpServerIpAddress YourIpAddress { get; }
        
        /// <summary>
        /// IP address of next server to use in bootstrap; returned in DHCPOFFER, DHCPACK by server (SIADDR)
        /// </summary>
        DhcpServerIpAddress NextServerIpAddress { get; }

        /// <summary>
        /// Relay agent IP address, used in booting via a relay agent (GIADDR)
        /// </summary>
        DhcpServerIpAddress RelayAgentIpAddress { get; }
        
        /// <summary>
        /// Client hardware address (CHADDR)
        /// </summary>
        DhcpServerHardwareAddress ClientHardwareAddress { get; }
        
        /// <summary>
        /// Optional server host name (SNAME)
        /// </summary>
        string ServerHostName { get; }
        
        /// <summary>
        /// Boot file name; "generic" name or null in DHCPDISCOVER, fully qualified directory-path name in DHCPOFFER (FILE)
        /// </summary>
        string FileName { get; }
        
        int OptionsMagicCookie { get; }
        
        DhcpServerPacketMessageTypes DhcpMessageType { get; }

        ReadOnlyCollection<DhcpServerPacketOption> Options { get; }
        bool TryGetOption(DhcpServerOptionIds optionId, out DhcpServerPacketOption option);
        bool TryGetOption(byte optionId, out DhcpServerPacketOption option);

    }
}
