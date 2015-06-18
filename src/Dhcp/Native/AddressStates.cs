
namespace Dhcp.Native
{
    /// <summary>
    /// Possible states of the IPv4 address given to the DHCPv4 client.
    /// </summary>
    internal enum AddressStates : byte
    {
        /// <summary>
        /// The DHCPv4 client has been offered this IPv4 address.
        /// </summary>
        ADDRESS_STATE_OFFERED = 0x00,
        /// <summary>
        /// The IPv4 address is active and has an active DHCPv4 client lease record.
        /// </summary>
        ADDRESS_STATE_ACTIVE = 0x01,
        /// <summary>
        /// The IPv4 address request was declined by the DHCPv4 client; hence, it is a bad IPv4 address.
        /// </summary>
        ADDRESS_STATE_DECLINED = 0x02,
        /// <summary>
        /// The IPv4 address is in DOOMED state and is due to be deleted.
        /// </summary>
        ADDRESS_STATE_DOOM = 0x03,
    }
}
