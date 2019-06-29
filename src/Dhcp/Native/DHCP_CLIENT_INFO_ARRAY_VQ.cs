using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_CLIENT_INFO_ARRAY_VQ structure specifies an array of DHCP_CLIENT_INFO_VQ structures.
    /// </summary>
    internal readonly struct DHCP_CLIENT_INFO_ARRAY_VQ : IDisposable
    {
#pragma warning disable CS0649
        /// <summary>
        /// The number of elements in the array.
        /// </summary>
        public readonly int NumElements;
#pragma warning restore CS0649

        /// <summary>
        /// Pointer to the first element in the array of DHCP_CLIENT_INFO_VQ structures.
        /// </summary>
        private readonly IntPtr ClientsPointer;

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
                        Value = clientPtr.MarshalToStructure<DHCP_CLIENT_INFO_VQ>()
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
            public DHCP_CLIENT_INFO_VQ Value;
        }
    }
}
