using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_OPTION_VALUE_ARRAY structure defines a list of DHCP option values (just the option data with associated ID tags).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_OPTION_VALUE_ARRAY : IDisposable
    {
        /// <summary>
        /// Specifies the number of option values listed in Values.
        /// </summary>
        public readonly int NumElements;
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
                if (NumElements == 0 || ValuesPointer == IntPtr.Zero)
                    yield break;

                var iter = ValuesPointer;
                var size = Marshal.SizeOf(typeof(DHCP_OPTION_VALUE));
                for (var i = 0; i < NumElements; i++)
                {
                    yield return iter.MarshalToStructure<DHCP_OPTION_VALUE>();
                    iter += size;
                }
            }
        }

        public void Dispose()
        {
            foreach (var value in Values)
                value.Dispose();

            Api.FreePointer(ref ValuesPointer);
        }
    }
}
