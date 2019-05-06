using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_IP_ARRAY structure defines an array of IP addresses.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_IP_ARRAY : IDisposable
    {
        /// <summary>
        /// Specifies the number of IP addresses in Elements.
        /// </summary>
        public readonly uint NumElements;
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
                if (NumElements == 0 || ElementsPointer == IntPtr.Zero)
                    yield break;

                var iter = ElementsPointer;
                var size = Marshal.SizeOf(typeof(DHCP_IP_ADDRESS));
                for (var i = 0; i < NumElements; i++)
                {
                    yield return iter.MarshalToStructure<DHCP_IP_ADDRESS>();
                    iter += size;
                }
            }
        }

        public void Dispose()
        {
            Api.FreePointer(ref ElementsPointer);
        }
    }
}
