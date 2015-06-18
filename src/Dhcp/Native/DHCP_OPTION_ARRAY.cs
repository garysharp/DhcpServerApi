using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_OPTION_ARRAY structure defines an array of DHCP server options.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_OPTION_ARRAY
    {
        /// <summary>
        /// Specifies the number of option elements in Options.
        /// </summary>
        public int NumElements;
        /// <summary>
        /// Pointer to a list of <see cref="DHCP_OPTION"/> structures containing DHCP server options and the associated data.
        /// </summary>
        public IntPtr OptionsPointer;

        /// <summary>
        /// Pointer to a list of <see cref="DHCP_OPTION"/> structures containing DHCP server options and the associated data.
        /// </summary>
        public IEnumerable<DHCP_OPTION> Options
        {
            get
            {
                var instanceIter = this.OptionsPointer;
                var instanceSize = Marshal.SizeOf(typeof(DHCP_OPTION));
                for (int i = 0; i < this.NumElements; i++)
                {
                    yield return (DHCP_OPTION)Marshal.PtrToStructure(instanceIter, typeof(DHCP_OPTION));

                    instanceIter += instanceSize;
                }
            }
        }
    }
}
