using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_CLIENT_INFO_ARRAY_VQ structure specifies an array of DHCP_CLIENT_INFO_VQ structures.
    /// </summary>
    internal struct DHCP_CLIENT_INFO_ARRAY_VQ : IDisposable
    {
        public int NumElements;
        private IntPtr ClientsPointer;

        public List<Tuple<IntPtr, DHCP_CLIENT_INFO_VQ>> Clients;

        public static DHCP_CLIENT_INFO_ARRAY_VQ Read(IntPtr ptr)
        {
            // Number of elements in the array
            var numElements = Marshal.ReadIntPtr(ptr).ToInt32();

            var clients = new List<Tuple<IntPtr, DHCP_CLIENT_INFO_VQ>>(numElements);

            // Pointer to the first element in the array of DHCP_CLIENT_INFO_VQ structures.
            var clientsPointer = Marshal.ReadIntPtr(ptr, IntPtr.Size);

            var iter = clientsPointer;
            for (var i = 0; i < numElements; i++)
            {
                var clientPtr = Marshal.ReadIntPtr(iter);
                var client = clientPtr.MarshalToStructure<DHCP_CLIENT_INFO_VQ>();

                clients.Add(Tuple.Create(clientPtr, client));

                iter += IntPtr.Size;
            }

            return new DHCP_CLIENT_INFO_ARRAY_VQ()
            {
                NumElements = numElements,
                ClientsPointer = clientsPointer,
                Clients = clients
            };
        }

        public void Dispose()
        {
            if (Clients != null)
            {
                foreach (var client in Clients)
                {
                    client.Item2.Dispose();
                    Api.DhcpRpcFreeMemory(client.Item1);
                }
                Clients = null;
            }

            if (ClientsPointer != IntPtr.Zero)
            {
                Api.DhcpRpcFreeMemory(ClientsPointer);
                ClientsPointer = IntPtr.Zero;
            }
        }
    }
}
