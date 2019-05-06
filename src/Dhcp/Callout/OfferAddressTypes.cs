namespace Dhcp.Callout
{
    public enum OfferAddressTypes : uint
    {
        /// <summary>
        /// The address is a BOOTP-served address.
        /// </summary>
        BOOTP = 0x30000003U,
        /// <summary>
        /// The address is a DHCP-served address.
        /// </summary>
        DHCP = 0x30000004U,
    }
}
