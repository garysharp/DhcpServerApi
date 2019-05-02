using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_ALL_OPTIONS structure defines the set of all options available on a DHCP server.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_ALL_OPTIONS
    {
        /// <summary>
        /// Reserved. This value should be set to 0.
        /// </summary>
        public readonly int Flags;
        /// <summary>
        /// DHCP_OPTION_ARRAY structure that contains the set of non-vendor options.
        /// </summary>
        public readonly IntPtr NonVendorOptionsPointer;
        /// <summary>
        /// Specifies the number of vendor options listed in VendorOptions.
        /// </summary>
        public readonly int NumVendorOptions;
        /// <summary>
        /// Pointer to a list of structures that contain the following fields.
        /// - Option: DHCP_OPTION structure that contains specific information describing the option.
        /// - VendorName: Unicode string that contains the name of the vendor for the option.
        /// - ClassName: Unicode string that contains the name of the DHCP class for the option.
        /// </summary>
        public readonly IntPtr VendorOptionsPointer;

        /// <summary>
        /// DHCP_OPTION_ARRAY structure that contains the set of non-vendor options.
        /// </summary>
        public DHCP_OPTION_ARRAY NonVendorOptions => NonVendorOptionsPointer.MarshalToStructure<DHCP_OPTION_ARRAY>();

        /// <summary>
        /// Pointer to a list of <see cref="DHCP_VENDOR_OPTION"/> structures containing DHCP server options and the associated data.
        /// </summary>
        public IEnumerable<DHCP_VENDOR_OPTION> VendorOptions
        {
            get
            {
                var instanceIter = VendorOptionsPointer;
                var instanceSize = Marshal.SizeOf(typeof(DHCP_VENDOR_OPTION));
                for (var i = 0; i < NumVendorOptions; i++)
                {
                    yield return instanceIter.MarshalToStructure<DHCP_VENDOR_OPTION>();
                    instanceIter += instanceSize;
                }
            }
        }
    }
}
