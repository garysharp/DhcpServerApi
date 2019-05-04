
namespace Dhcp.Native
{
    internal static class Constants
    {

        public const uint DHCP_FLAGS_OPTION_IS_VENDOR = 0x03;

        public const uint DHCP_ENDPOINT_FLAG_CANT_MODIFY = 0x01;

        public const uint DNS_FLAG_ENABLED = 0x01;
        public const uint DNS_FLAG_UPDATE_DOWNLEVEL = 0x02;
        public const uint DNS_FLAG_CLEANUP_EXPIRED = 0x04;
        public const uint DNS_FLAG_UPDATE_BOTH_ALWAYS = 0x10;
        public const uint DNS_FLAG_UPDATE_DHCID = 0x20;
        public const uint DNS_FLAG_DISABLE_PTR_UPDATE = 0x40;

        /// <summary>
        /// The DHCPv4 client has been offered this IPv4 address.
        /// </summary>
        public const byte ADDRESS_STATE_OFFERED = 0x00;
        /// <summary>
        /// The IPv4 address is active and has an active DHCPv4 client lease record.
        /// </summary>
        public const byte ADDRESS_STATE_ACTIVE = 0x01;
        /// <summary>
        /// The IPv4 address request was declined by the DHCPv4 client; hence, it is a bad IPv4 address.
        /// </summary>
        public const byte ADDRESS_STATE_DECLINED = 0x02;
        /// <summary>
        /// The IPv4 address is in DOOMED state and is due to be deleted.
        /// </summary>
        public const byte ADDRESS_STATE_DOOM = 0x03;

        /// <summary>
        /// The address is leased to the DHCPv4 client without DHCID (sections 3 and 3.5 of [RFC4701]).
        /// </summary>
        public const byte ADDRESS_BIT_NO_DHCID = 0x00;
        /// <summary>
        /// The address is leased to the DHCPv4 client with DHCID as specified in section 3.5.3 of [RFC4701].
        /// </summary>
        public const byte ADDRESS_BIT_DHCID_NO_CLIENTIDOPTION = 0x01;
        /// <summary>
        /// The address is leased to the DHCPv4 client with DHCID as specified in section 3.5.2 of [RFC4701].
        /// </summary>
        public const byte ADDRESS_BIT_DHCID_WITH_CLIENTIDOPTION = 0x02;
        /// <summary>
        /// The address is leased to the DHCPv4 client with DHCID as specified in section 3.5.1 of [RFC4701].
        /// </summary>
        public const byte ADDRESS_BIT_DHCID_WITH_DUID = 0x03;

        /// <summary>
        /// The DNS update for the DHCPv4 client lease record needs to be deleted from the DNS server when the lease is deleted.
        /// </summary>
        public const byte ADDRESS_BIT_CLEANUP = 0x10;
        /// <summary>
        /// The DNS update needs to be sent for both A and PTR resource records ([RFC1034] section 3.6).
        /// </summary>
        public const byte ADDRESS_BIT_BOTH_REC = 0x20;
        /// <summary>
        /// The DNS update is not completed for the lease record.
        /// </summary>
        public const byte ADDRESS_BIT_UNREGISTERED = 0x40;
        /// <summary>
        /// The address lease is expired, but the DNS updates for the lease record have not been deleted from the DNS server.
        /// </summary>
        public const byte ADDRESS_BIT_DELETED = 0x80;
    }
}
