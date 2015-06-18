using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_BIND_ELEMENT_ARRAY structure defines an array of network binding elements used by a DHCP server.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_BIND_ELEMENT_ARRAY
    {
        /// <summary>
        /// Specifies the number of network binding elements listed in Elements.
        /// </summary>
        public int NumElements;
        /// <summary>
        /// Specifies an array of DHCP_BIND_ELEMENT structures
        /// </summary>
        private IntPtr ElementsPointer;

        /// <summary>
        /// Specifies an array of DHCP_BIND_ELEMENT structures
        /// </summary>
        public IEnumerable<DHCP_BIND_ELEMENT> Elements
        {
            get
            {
                var instanceIter = this.ElementsPointer;
                var instanceSize = Marshal.SizeOf(typeof(DHCP_BIND_ELEMENT));
                for (int i = 0; i < this.NumElements; i++)
                {
                    yield return (DHCP_BIND_ELEMENT)Marshal.PtrToStructure(instanceIter, typeof(DHCP_BIND_ELEMENT));

                    instanceIter += instanceSize;
                }
            }
        }
    }
}
