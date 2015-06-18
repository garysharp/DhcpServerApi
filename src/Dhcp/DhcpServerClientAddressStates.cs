
namespace Dhcp
{
    public enum DhcpServerClientAddressStates
    {
        /// <summary>
        /// The DHCPv4 client has been offered this IPv4 address.
        /// </summary>
        Offered = 0x00,
        /// <summary>
        /// The IPv4 address is active and has an active DHCPv4 client lease record.
        /// </summary>
        Active = 0x01,
        /// <summary>
        /// The IPv4 address request was declined by the DHCPv4 client; hence, it is a bad IPv4 address.
        /// </summary>
        Declined = 0x02,
        /// <summary>
        /// The IPv4 address is in DOOMED state and is due to be deleted.
        /// </summary>
        Doomed = 0x03,

        Unknown = 0x100 // For backwards-compatibility
    }
}
