using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_ALL_OPTIONS_VALUE_ITEM structure contain the option values for specific class/vendor pairs.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_ALL_OPTION_VALUE_ITEM
    {
        /// <summary>
        /// Unicode string that contains the name of the DHCP class for the option list.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public readonly string ClassName;
        /// <summary>
        /// Unicode string that contains the name of the vendor for the option list.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public readonly string VendorName;
        /// <summary>
        /// Specifies whether or not this set of options is vendor-specific. This value is TRUE if it is, and FALSE if it is not.
        /// </summary>
        public readonly bool IsVendor;
        /// <summary>
        /// DHCP_OPTION_VALUE_ARRAY structure that contains the option values for the specified vendor/class pair.
        /// </summary>
        public readonly IntPtr OptionsArrayPointer;

        /// <summary>
        /// DHCP_OPTION_VALUE_ARRAY structure that contains the option values for the specified vendor/class pair.
        /// </summary>
        public DHCP_OPTION_VALUE_ARRAY OptionsArray
        {
            get
            {
                if (OptionsArrayPointer == IntPtr.Zero)
                {
                    return new DHCP_OPTION_VALUE_ARRAY()
                    {
                        NumElements = 0
                    };
                }
                else
                {
                    return OptionsArrayPointer.MarshalToStructure<DHCP_OPTION_VALUE_ARRAY>();
                }
            }
        }

    }
}
