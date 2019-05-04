namespace Dhcp
{
    public enum DhcpServerClientAddressStates
    {
        /// <summary>
        /// The DHCPv4 client is offered this IPv4 address.
        /// </summary>
        Offered = 0x00,
        /// <summary>
        /// The IPv4 address is active and has an active DHCPv4 client lease record.
        /// </summary>
        Active = 0x01,
        /// <summary>
        /// The IPv4 address request is declined by the DHCPv4 client; hence it is a bad IPv4 address.
        /// </summary>
        Declined = 0x02,
        /// <summary>
        /// The IPv4 address is in DOOMED state and is due to be deleted.
        /// </summary>
        Doomed = 0x03,
        /// <summary>
        /// The Address State is unknown or not supported by the server.
        /// </summary>
        Unknown = -1,
    }
}
