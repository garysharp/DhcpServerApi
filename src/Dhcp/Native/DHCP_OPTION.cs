using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_OPTION structure defines a single DHCP option and any data elements associated with it.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_OPTION
    {
        /// <summary>
        /// DHCP_OPTION_ID value that specifies a unique ID number (also called a "code") for this option.
        /// </summary>
        public readonly int OptionID;
        /// <summary>
        /// Unicode string that contains the name of this option.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public readonly string OptionName;
        /// <summary>
        /// Unicode string that contains a comment about this option.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public readonly string OptionComment;
        /// <summary>
        /// <see cref="DHCP_OPTION_DATA"/> structure that contains the data associated with this option.
        /// </summary>
        public readonly DHCP_OPTION_DATA DefaultValue;
        /// <summary>
        /// DHCP_OPTION_TYPE enumeration value that indicates whether this option is a single unary item or an element in an array of options.
        /// </summary>
        public readonly DHCP_OPTION_TYPE OptionType;
    }
}
