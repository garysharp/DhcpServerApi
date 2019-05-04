namespace Dhcp
{
    /// <summary>
    /// Client Name Protection States
    /// </summary>
    public enum DhcpServerClientNameProtectionStates
    {
        /// <summary>
        /// The address is leased to the DHCPv4 client without DHCID (sections 3 and 3.5 of [RFC4701]).
        /// </summary>
        NoDhcid = 0x00,
        /// <summary>
        /// The address is leased to the DHCPv4 client with DHCID as specified in section 3.5.3 of [RFC4701].
        /// </summary>
        NoClientIdOption = 0x01,
        /// <summary>
        /// The address is leased to the DHCPv4 client with DHCID as specified in section 3.5.2 of [RFC4701].
        /// </summary>
        DhcidWithClientIdOption = 0x02,
        /// <summary>
        /// The address is leased to the DHCPv4 client with DHCID as specified in section 3.5.1 of [RFC4701].
        /// </summary>
        DhcidWithDuid = 0x03,
        /// <summary>
        /// The Name Protection State is unknown or not supported by the server.
        /// </summary>
        Unknown = -1,
    }
}
