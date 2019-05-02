using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_CLIENT_INFO_ARRAY
    {
        /// <summary>
        /// Specifies the number of elements present in Clients.
        /// </summary>
        public readonly int NumElements;
        /// <summary>
        /// Pointer to a list of DHCP_CLIENT_INFO structures that contain information on specific DHCP subnet clients.).
        /// </summary>
        private readonly IntPtr ClientsPointer;

        /// <summary>
        /// Pointer to a list of DHCP_CLIENT_INFO structures that contain information on specific DHCP subnet clients.).
        /// </summary>
        public IEnumerable<DHCP_CLIENT_INFO> Clients
        {
            get
            {
                var iter = ClientsPointer;
                for (var i = 0; i < NumElements; i++)
                {
                    var clientPtr = Marshal.ReadIntPtr(iter);
                    
                    yield return clientPtr.MarshalToStructure<DHCP_CLIENT_INFO>();
                    
                    iter += IntPtr.Size;
                }
            }
        }
    }
}
