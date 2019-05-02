using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_CLIENT_INFO_ARRAY_V4 structure defines an array of DHCP_CLIENT_INFO_V4 structures for use with enumeration functions. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_CLIENT_INFO_ARRAY_V4
    {
        /// <summary>
        /// Specifies the number of elements present in Clients.
        /// </summary>
        public readonly int NumElements;
        /// <summary>
        /// Pointer to a list of DHCP_CLIENT_INFO_V4 structures that contain information on specific DHCP subnet clients, including the dynamic address type (DHCP and/or BOOTP).
        /// </summary>
        private readonly IntPtr ClientsPointer;

        /// <summary>
        /// Pointer to a list of DHCP_CLIENT_INFO_V4 structures that contain information on specific DHCP subnet clients, including the dynamic address type (DHCP and/or BOOTP).
        /// </summary>
        public IEnumerable<DHCP_CLIENT_INFO_V4> Clients
        {
            get
            {
                var iter = ClientsPointer;
                for (var i = 0; i < NumElements; i++)
                {
                    var clientPtr = Marshal.ReadIntPtr(iter);

                    yield return clientPtr.MarshalToStructure<DHCP_CLIENT_INFO_V4>();

                    iter += IntPtr.Size;
                }
            }
        }
    }
}
