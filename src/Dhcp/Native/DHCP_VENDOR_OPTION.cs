using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_OPTION structure defines a single DHCP option and any data elements associated with it.
    /// Additional fields are added to support Vendor Options in DHCP_ALL_OPTIONS
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_VENDOR_OPTION
    {
        /// <summary>
        /// DHCP_OPTION_ID value that specifies a unique ID number (also called a "code") for this option.
        /// </summary>
        public int OptionID;
        /// <summary>
        /// Unicode string that contains the name of this option.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string OptionName;
        /// <summary>
        /// Unicode string that contains a comment about this option.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string OptionComment;
        /// <summary>
        /// <see cref="DHCP_OPTION_DATA"/> structure that contains the data associated with this option.
        /// </summary>
        public DHCP_OPTION_DATA DefaultValue;
        /// <summary>
        /// DHCP_OPTION_TYPE enumeration value that indicates whether this option is a single unary item or an element in an array of options.
        /// </summary>
        public DHCP_OPTION_TYPE OptionType;
        /// <summary>
        /// Unicode string that contains the name of the vendor for the option.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string VendorName;
        /// <summary>
        /// Unicode string that contains the name of the DHCP class for the option.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string ClassName;
    }
}