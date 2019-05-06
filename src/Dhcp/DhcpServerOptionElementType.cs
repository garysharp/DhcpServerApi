namespace Dhcp
{
    /// <summary>
    /// Defines the set of formats that represent DHCP option data
    /// </summary>
    public enum DhcpServerOptionElementType
    {
        /// <summary>
        /// The option data is stored as a BYTE value.
        /// </summary>
        Byte,
        /// <summary>
        /// The option data is stored as a WORD value.
        /// </summary>
        Word,
        /// <summary>
        /// The option data is stored as a DWORD value.
        /// </summary>
        DWord,
        /// <summary>
        /// The option data is stored as a DWORD_DWORD value.
        /// </summary>
        DWordDWord,
        /// <summary>
        /// The option data is an IP address, stored as a DHCP_IP_ADDRESS value (DWORD).
        /// </summary>
        IpAddress,
        /// <summary>
        /// The option data is stored as a Unicode string.
        /// </summary>
        StringData,
        /// <summary>
        /// The option data is stored as a binary structure.
        /// </summary>
        BinaryData,
        /// <summary>
        /// The option data is encapsulated and stored as a binary structure.
        /// </summary>
        EncapsulatedData,
        /// <summary>
        /// The option data is stored as a Unicode string.
        /// </summary>
        Ipv6Address
    }
}
