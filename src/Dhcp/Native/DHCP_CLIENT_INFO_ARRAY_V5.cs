using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_CLIENT_INFO_ARRAY_V5 structure defines an array of DHCP_CLIENT_INFO_V5 structures for use with enumeration functions. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_CLIENT_INFO_ARRAY_V5 : IDisposable
    {
        /// <summary>
        /// Specifies the number of elements present in Clients.
        /// </summary>
        public readonly int NumElements;
        /// <summary>
        /// Pointer to a list of DHCP_CLIENT_INFO_V5 structures that contain information on specific DHCP subnet clients, including the dynamic address type (DHCP and/or BOOTP) and address state information.
        /// </summary>
        private readonly IntPtr ClientsPointer;

        /// <summary>
        /// Pointer to a list of DHCP_CLIENT_INFO_V5 structures that contain information on specific DHCP subnet clients, including the dynamic address type (DHCP and/or BOOTP) and address state information.
        /// </summary>
        public IEnumerable<ClientTuple> Clients
        {
            get
            {
                if (NumElements == 0 || ClientsPointer == IntPtr.Zero)
                    yield break;

                var iter = ClientsPointer;
                for (var i = 0; i < NumElements; i++)
                {
                    var clientPtr = Marshal.ReadIntPtr(iter);
                    yield return new ClientTuple()
                    {
                        Pointer = clientPtr,
                        Value = clientPtr.MarshalToStructure<DHCP_CLIENT_INFO_V5>()
                    };
                    iter += IntPtr.Size;
                }
            }
        }

        public void Dispose()
        {
            foreach (var client in Clients)
            {
                client.Value.Dispose();
                Api.FreePointer(client.Pointer);
            }

            Api.FreePointer(ClientsPointer);
        }

        internal struct ClientTuple
        {
            public IntPtr Pointer;
            public DHCP_CLIENT_INFO_V5 Value;
        }
    }
}
