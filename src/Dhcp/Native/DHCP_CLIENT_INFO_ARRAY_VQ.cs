using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_CLIENT_INFO_ARRAY_VQ structure specifies an array of DHCP_CLIENT_INFO_VQ structures.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_CLIENT_INFO_ARRAY_VQ
    {
        public int NumElements;
        /// <summary>
        /// Pointer to the first element in the array of DHCP_CLIENT_INFO_VQ structures.
        /// </summary>
        private IntPtr ClientsPointer;

        /// <summary>
        /// Pointer to the first element in the array of DHCP_CLIENT_INFO_VQ structures.
        /// </summary>
        public IEnumerable<DHCP_CLIENT_INFO_VQ> Clients
        {
            get
            {
                var iter = this.ClientsPointer;
                for (int i = 0; i < this.NumElements; i++)
                {
                    var clientPtr = Marshal.ReadIntPtr(iter);

                    yield return (DHCP_CLIENT_INFO_VQ)Marshal.PtrToStructure(clientPtr, typeof(DHCP_CLIENT_INFO_VQ));

                    iter += IntPtr.Size;
                }
            }
        }
    }
}
