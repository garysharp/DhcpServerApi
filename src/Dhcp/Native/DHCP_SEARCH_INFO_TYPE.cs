namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_SEARCH_INFO_TYPE enumeration defines the set of possible attributes used to search DHCP client information records.
    /// </summary>
    internal enum DHCP_SEARCH_INFO_TYPE : int
    {
        /// <summary>
        /// The search will be performed against the assigned DHCP client IP address, represented as a 32-bit unsigned integer value.
        /// </summary>
        DhcpClientIpAddress = 0,
        /// <summary>
        /// The search will be performed against the MAC address of the DHCP client network interface device, represented as a DHCP_BINARY_DATA structure.
        /// </summary>
        DhcpClientHardwareAddress = 1,
        /// <summary>
        /// The search will be performed against the DHCP client's network name, represented as a Unicode string.
        /// </summary>
        DhcpClientName = 2
    }
}
