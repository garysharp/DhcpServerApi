using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_IP_RESERVATION structure defines a client IP reservation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_IP_RESERVATION : IDisposable
    {
        /// <summary>
        /// DHCP_IP_ADDRESS value that contains the reserved IP address.
        /// </summary>
        public readonly DHCP_IP_ADDRESS ReservedIpAddress;
        /// <summary>
        /// DHCP_CLIENT_UID structure that contains information on the client holding this IP reservation.
        /// </summary>
        private readonly IntPtr ReservedForClientPointer;

        /// <summary>
        /// DHCP_CLIENT_UID structure that contains information on the client holding this IP reservation.
        /// </summary>
        public DHCP_CLIENT_UID ReservedForClient => ReservedForClientPointer.MarshalToStructure<DHCP_CLIENT_UID>();

        public void Dispose()
        {
            if (ReservedForClientPointer != IntPtr.Zero)
            {
                ReservedForClient.Dispose();
                Api.FreePointer(ReservedForClientPointer);
            }
        }
    }

    /// <summary>
    /// The DHCP_IP_RESERVATION structure defines a client IP reservation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_IP_RESERVATION_Managed : IDisposable
    {
        /// <summary>
        /// DHCP_IP_ADDRESS value that contains the reserved IP address.
        /// </summary>
        public readonly DHCP_IP_ADDRESS ReservedIpAddress;
        /// <summary>
        /// DHCP_CLIENT_UID structure that contains information on the client holding this IP reservation.
        /// </summary>
        private readonly IntPtr ReservedForClientPointer;

        public DHCP_IP_RESERVATION_Managed(DHCP_IP_ADDRESS reservedIpAddress, DHCP_CLIENT_UID_Managed reservedForClient)
        {
            ReservedIpAddress = reservedIpAddress;
            ReservedForClientPointer = Marshal.AllocHGlobal(Marshal.SizeOf(reservedForClient));
            Marshal.StructureToPtr(reservedForClient, ReservedForClientPointer, false);
        }

        public DHCP_IP_RESERVATION_Managed(DhcpServerIpAddress address, DhcpServerHardwareAddress reservedForClient)
            : this(reservedIpAddress: address.ToNativeAsNetwork(),
                  reservedForClient: reservedForClient.ToNativeClientUid())
        { }

        public void Dispose()
        {
            if (ReservedForClientPointer != IntPtr.Zero)
            {
                var reservedForClient = ReservedForClientPointer.MarshalToStructure<DHCP_CLIENT_UID_Managed>();
                reservedForClient.Dispose();

                Marshal.FreeHGlobal(ReservedForClientPointer);
            }
        }
    }
}
