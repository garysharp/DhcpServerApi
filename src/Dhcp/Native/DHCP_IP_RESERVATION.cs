using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_IP_RESERVATION structure defines a client IP reservation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_IP_RESERVATION : IDisposable
    {
        /// <summary>
        /// DHCP_IP_ADDRESS value that contains the reserved IP address.
        /// </summary>
        public readonly DHCP_IP_ADDRESS ReservedIpAddress;
        /// <summary>
        /// DHCP_CLIENT_UID structure that contains information on the client holding this IP reservation.
        /// </summary>
        private IntPtr ReservedForClientPointer;

        /// <summary>
        /// DHCP_CLIENT_UID structure that contains information on the client holding this IP reservation.
        /// </summary>
        public DHCP_CLIENT_UID ReservedForClient => ReservedForClientPointer.MarshalToStructure<DHCP_CLIENT_UID>();

        public void Dispose()
        {
            if (ReservedForClientPointer != IntPtr.Zero)
            {
                ReservedForClient.Dispose();
                Api.FreePointer(ref ReservedForClientPointer);
            }
        }
    }
}
