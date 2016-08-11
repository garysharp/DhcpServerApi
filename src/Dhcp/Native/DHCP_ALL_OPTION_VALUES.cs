using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_ALL_OPTION_VALUES structure defines the set of all option values defined on a DHCP server, organized according to class/vendor pairing.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_ALL_OPTION_VALUES
    {
        /// <summary>
        /// Reserved. This value should be set to 0.
        /// </summary>
        public int Flags;
        /// <summary>
        /// Specifies the number of elements in Options.
        /// </summary>
        public int NumElements;
        /// <summary>
        /// Pointer to a list of structures that contain the option values for specific class/vendor pairs.
        /// </summary>
        public IntPtr OptionsPointer;

        /// <summary>
        /// Pointer to a list of <see cref="DHCP_ALL_OPTION_VALUE_ITEM"/> structures containing the option values for specific class/vendor pairs.
        /// </summary>
        public IEnumerable<DHCP_ALL_OPTION_VALUE_ITEM> Options
        {
            get
            {
                var instanceIter = this.OptionsPointer;
                var instanceSize = Marshal.SizeOf(typeof(DHCP_ALL_OPTION_VALUE_ITEM));
                for (int i = 0; i < this.NumElements; i++)
                {
                    yield return (DHCP_ALL_OPTION_VALUE_ITEM)Marshal.PtrToStructure(instanceIter, typeof(DHCP_ALL_OPTION_VALUE_ITEM));

                    instanceIter += instanceSize;
                }
            }
        }
    }
}
