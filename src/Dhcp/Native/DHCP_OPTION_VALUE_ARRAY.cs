using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_OPTION_VALUE_ARRAY structure defines a list of DHCP option values (just the option data with associated ID tags).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_OPTION_VALUE_ARRAY
    {
        /// <summary>
        /// Specifies the number of option values listed in Values.
        /// </summary>
        public int NumElements;
        /// <summary>
        /// Pointer to a list of DHCP_OPTION_VALUE structures containing DHCP option values.
        /// </summary>
        private IntPtr ValuesPointer;

        /// <summary>
        /// Pointer to a list of DHCP_OPTION_VALUE structures containing DHCP option values.
        /// </summary>
        public IEnumerable<DHCP_OPTION_VALUE> Values
        {
            get
            {
                var instanceIter = this.ValuesPointer;
                var instanceSize = Marshal.SizeOf(typeof(DHCP_OPTION_VALUE));
                for (int i = 0; i < this.NumElements; i++)
                {
                    yield return (DHCP_OPTION_VALUE)Marshal.PtrToStructure(instanceIter, typeof(DHCP_OPTION_VALUE));

                    instanceIter += instanceSize;
                }
            }
        }
    }
}