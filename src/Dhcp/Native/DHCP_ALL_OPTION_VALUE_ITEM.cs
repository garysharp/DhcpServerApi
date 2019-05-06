using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_ALL_OPTIONS_VALUE_ITEM structure contain the option values for specific class/vendor pairs.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_ALL_OPTION_VALUE_ITEM : IDisposable
    {
        /// <summary>
        /// Unicode string that contains the name of the DHCP class for the option list.
        /// </summary>
        private IntPtr ClassNamePointer;
        /// <summary>
        /// Unicode string that contains the name of the vendor for the option list.
        /// </summary>
        private IntPtr VendorNamePointer;
        /// <summary>
        /// Specifies whether or not this set of options is vendor-specific. This value is TRUE if it is, and FALSE if it is not.
        /// </summary>
        public readonly bool IsVendor;
        /// <summary>
        /// DHCP_OPTION_VALUE_ARRAY structure that contains the option values for the specified vendor/class pair.
        /// </summary>
        private IntPtr OptionsArrayPointer;

        /// <summary>
        /// Unicode string that contains the name of the DHCP class for the option list.
        /// </summary>
        public string ClassName => Marshal.PtrToStringUni(ClassNamePointer);
        /// <summary>
        /// Unicode string that contains the name of the vendor for the option list.
        /// </summary>
        public string VendorName => Marshal.PtrToStringUni(VendorNamePointer);
        /// <summary>
        /// DHCP_OPTION_VALUE_ARRAY structure that contains the option values for the specified vendor/class pair.
        /// </summary>
        public DHCP_OPTION_VALUE_ARRAY OptionsArray
        {
            get
            {
                if (OptionsArrayPointer == IntPtr.Zero)
                    return default;
                else
                    return OptionsArrayPointer.MarshalToStructure<DHCP_OPTION_VALUE_ARRAY>();
            }
        }

        public void Dispose()
        {
            Api.FreePointer(ref ClassNamePointer);
            Api.FreePointer(ref VendorNamePointer);

            if (OptionsArrayPointer != IntPtr.Zero)
            {
                OptionsArray.Dispose();
                Api.FreePointer(ref OptionsArrayPointer);
            }
        }
    }
}
