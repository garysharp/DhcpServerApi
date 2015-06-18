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
        public int NumElements;
        /// <summary>
        /// Pointer to a list of DHCP_CLIENT_INFO structures that contain information on specific DHCP subnet clients.).
        /// </summary>
        private IntPtr ClientsPointer;

        /// <summary>
        /// Pointer to a list of DHCP_CLIENT_INFO structures that contain information on specific DHCP subnet clients.).
        /// </summary>
        public IEnumerable<DHCP_CLIENT_INFO> Clients
        {
            get
            {
                var iter = this.ClientsPointer;
                for (int i = 0; i < this.NumElements; i++)
                {
                    var clientPtr = Marshal.ReadIntPtr(iter);
                    
                    yield return (DHCP_CLIENT_INFO)Marshal.PtrToStructure(clientPtr, typeof(DHCP_CLIENT_INFO));
                    
                    iter += IntPtr.Size;
                }
            }
        }
    }
}
