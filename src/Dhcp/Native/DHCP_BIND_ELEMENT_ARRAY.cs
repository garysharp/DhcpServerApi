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
        public readonly int NumElements;
        /// <summary>
        /// Specifies an array of DHCP_BIND_ELEMENT structures
        /// </summary>
        private readonly IntPtr ElementsPointer;

        /// <summary>
        /// Specifies an array of DHCP_BIND_ELEMENT structures
        /// </summary>
        public IEnumerable<DHCP_BIND_ELEMENT> Elements
        {
            get
            {
                var instanceIter = ElementsPointer;
                var instanceSize = Marshal.SizeOf(typeof(DHCP_BIND_ELEMENT));
                for (var i = 0; i < NumElements; i++)
                {
                    yield return instanceIter.MarshalToStructure<DHCP_BIND_ELEMENT>();
                    instanceIter += instanceSize;
                }
            }
        }
    }
}
