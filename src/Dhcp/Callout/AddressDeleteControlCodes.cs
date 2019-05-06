namespace Dhcp.Callout
{
    public enum AddressDeleteControlCodes : int
    {
        /// <summary>
        /// The address attempted to be offered, as provided in LeaseAddress, is already in use on the network.
        /// </summary>
        Conflict = 0x20000001,
        /// <summary>
        /// The packet was a DECLINE message for the address specified in LeaseAddress.
        /// </summary>
        Decline = 0x20000002,
        /// <summary>
        /// The packet was a RELEASE message for the address specified in LeaseAddress.
        /// </summary>
        Release = 0x20000003,
        /// <summary>
        /// The packet was a REQUEST message for the address specified in LeaseAddress, and the request was declined by Microsoft DHCP Server.
        /// </summary>
        Nacked = 0x20000004,
    }
}
