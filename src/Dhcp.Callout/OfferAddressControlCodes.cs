namespace Dhcp.Callout
{
    public enum OfferAddressControlCodes : int
    {
        /// <summary>
        /// Specifies a new lease is being approved
        /// </summary>
        GiveNewAddress = 0x30000001,
        /// <summary>
        /// Specifies the renewal of an existing lease
        /// </summary>
        GiveOldAddress = 0x30000002,
    }
}
