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
        public readonly int NumElements;
        /// <summary>
        /// Pointer to a list of <see cref="DHCP_OPTION"/> structures containing DHCP server options and the associated data.
        /// </summary>
        public readonly IntPtr OptionsPointer;

        /// <summary>
        /// Pointer to a list of <see cref="DHCP_OPTION"/> structures containing DHCP server options and the associated data.
        /// </summary>
        public IEnumerable<DHCP_OPTION> Options
        {
            get
            {
                var instanceIter = OptionsPointer;
                var instanceSize = Marshal.SizeOf(typeof(DHCP_OPTION));
                for (var i = 0; i < NumElements; i++)
                {
                    yield return instanceIter.MarshalToStructure<DHCP_OPTION>();
                    instanceIter += instanceSize;
                }
            }
        }
    }
}
