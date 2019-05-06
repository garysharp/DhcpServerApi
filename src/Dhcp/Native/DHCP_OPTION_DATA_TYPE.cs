namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_OPTION_DATA_TYPE enumeration defines the set of formats that represent DHCP option data.
    /// </summary>
    internal enum DHCP_OPTION_DATA_TYPE
    {
        /// <summary>
        /// The option data is stored as a BYTE value.
        /// </summary>
        DhcpByteOption,
        /// <summary>
        /// The option data is stored as a WORD value.
        /// </summary>
        DhcpWordOption,
        /// <summary>
        /// The option data is stored as a DWORD value.
        /// </summary>
        DhcpDWordOption,
        /// <summary>
        /// The option data is stored as a DWORD_DWORD value.
        /// </summary>
        DhcpDWordDWordOption,
        /// <summary>
        /// The option data is an IP address, stored as a DHCP_IP_ADDRESS value (DWORD).
        /// </summary>
        DhcpIpAddressOption,
        /// <summary>
        /// The option data is stored as a Unicode string.
        /// </summary>
        DhcpStringDataOption,
        /// <summary>
        /// The option data is stored as a DHCP_BINARY_DATA structure.
        /// </summary>
        DhcpBinaryDataOption,
        /// <summary>
        /// The option data is encapsulated and stored as a DHCP_BINARY_DATA structure.
        /// </summary>
        DhcpEncapsulatedDataOption,
        /// <summary>
        /// The option data is stored as a Unicode string.
        /// </summary>
        DhcpIpv6AddressOption
    }
}
