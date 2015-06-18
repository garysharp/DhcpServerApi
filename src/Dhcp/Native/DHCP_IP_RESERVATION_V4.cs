using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_IP_RESERVATION_V4 structure defines a client IP reservation. This structure extends an IP reservation by including the type of client (DHCP or BOOTP) holding the reservation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_IP_RESERVATION_V4
    {
        /// <summary>
        /// DHCP_IP_ADDRESS value that contains the reserved IP address.
        /// </summary>
        public DHCP_IP_ADDRESS ReservedIpAddress;
        /// <summary>
        /// DHCP_CLIENT_UID structure that contains the hardware address (MAC address) of the DHCPv4 client that holds this reservation.
        /// </summary>
        private IntPtr reservedForClient;
        /// <summary>
        /// Value that specifies the DHCPv4 reserved client type.
        /// </summary>
        public ClientTypes bAllowedClientTypes;

        /// <summary>
        /// DHCP_CLIENT_UID structure that contains the hardware address (MAC address) of the DHCPv4 client that holds this reservation.
        /// </summary>
        public DHCP_CLIENT_UID ReservedForClient
        {
            get
            {
                return (DHCP_CLIENT_UID)Marshal.PtrToStructure(reservedForClient, typeof(DHCP_CLIENT_UID));
            }
        }
    }
}
