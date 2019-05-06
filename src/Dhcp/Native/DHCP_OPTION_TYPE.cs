namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_OPTION_TYPE enumeration defines the set of possible DHCP option types.
    /// </summary>
    internal enum DHCP_OPTION_TYPE : uint
    {
        /// <summary>
        /// The option has a single data item associated with it.
        /// </summary>
        DhcpUnaryElementTypeOption,
        /// <summary>
        /// The option is an array of data items associated with it.
        /// </summary>
        DhcpArrayTypeOption
    }
}
