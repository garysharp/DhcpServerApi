using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_OPTION structure defines a single DHCP option and any data elements associated with it.
    /// Additional fields are added to support Vendor Options in DHCP_ALL_OPTIONS
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_VENDOR_OPTION : IDisposable
    {
        /// <summary>
        /// DHCP_OPTION_ID value that specifies a unique ID number (also called a "code") for this option.
        /// </summary>
        public readonly int OptionID;
        /// <summary>
        /// Unicode string that contains the name of this option.
        /// </summary>
        private readonly IntPtr OptionNamePointer;
        /// <summary>
        /// Unicode string that contains a comment about this option.
        /// </summary>
        private readonly IntPtr OptionCommentPointer;
        /// <summary>
        /// <see cref="DHCP_OPTION_DATA"/> structure that contains the data associated with this option.
        /// </summary>
        public readonly DHCP_OPTION_DATA DefaultValue;
        /// <summary>
        /// DHCP_OPTION_TYPE enumeration value that indicates whether this option is a single unary item or an element in an array of options.
        /// </summary>
        public readonly DHCP_OPTION_TYPE OptionType;
        /// <summary>
        /// Unicode string that contains the name of the vendor for the option.
        /// </summary>
        private readonly IntPtr VendorNamePointer;
        /// <summary>
        /// Unicode string that contains the name of the DHCP class for the option.
        /// </summary>
        private readonly IntPtr ClassNamePointer;

        /// <summary>
        /// Unicode string that contains the name of this option.
        /// </summary>
        public string OptionName => Marshal.PtrToStringUni(OptionNamePointer);
        /// <summary>
        /// Unicode string that contains a comment about this option.
        /// </summary>
        public string OptionComment => Marshal.PtrToStringUni(OptionCommentPointer);
        /// <summary>
        /// Unicode string that contains the name of the vendor for the option.
        /// </summary>
        public string VendorName => Marshal.PtrToStringUni(VendorNamePointer);
        /// <summary>
        /// Unicode string that contains the name of the DHCP class for the option.
        /// </summary>
        public string ClassName => Marshal.PtrToStringUni(ClassNamePointer);

        public void Dispose()
        {
            Api.FreePointer(OptionNamePointer);
            Api.FreePointer(OptionCommentPointer);

            DefaultValue.Dispose();

            Api.FreePointer(VendorNamePointer);
            Api.FreePointer(ClassNamePointer);
        }
    }
}
