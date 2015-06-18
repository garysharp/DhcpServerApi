using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_IP_ARRAY structure defines an array of IP addresses.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_IP_ARRAY
    {
        /// <summary>
        /// Specifies the number of IP addresses in Elements.
        /// </summary>
        public UInt32 NumElements;
        /// <summary>
        /// Pointer to a list of DHCP_IP_ADDRESS values.
        /// </summary>
        private IntPtr ElementsPointer;

        /// <summary>
        /// Pointer to a list of DHCP_IP_ADDRESS values.
        /// </summary>
        public IEnumerable<DHCP_IP_ADDRESS> Elements
        {
            get
            {
                var instanceIter = this.ElementsPointer;
                var instanceSize = Marshal.SizeOf(typeof(DHCP_IP_ADDRESS));
                for (int i = 0; i < this.NumElements; i++)
                {
                    yield return (DHCP_IP_ADDRESS)Marshal.PtrToStructure(instanceIter, typeof(DHCP_IP_ADDRESS));

                    instanceIter += instanceSize;
                }
            }
        }
    }
}
