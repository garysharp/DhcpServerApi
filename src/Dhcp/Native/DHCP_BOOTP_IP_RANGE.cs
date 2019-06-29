using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_BOOTP_IP_RANGE structure defines a suite of IPs for lease to BOOTP-specific clients.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_BOOTP_IP_RANGE
    {
        /// <summary>
        /// DHCP_IP_ADDRESS value that specifies the start of the IP range used for BOOTP service.
        /// </summary>
        public readonly DHCP_IP_ADDRESS StartAddress;
        /// <summary>
        /// DHCP_IP_ADDRESS value that specifies the end of the IP range used for BOOTP service.
        /// </summary>
        public readonly DHCP_IP_ADDRESS EndAddress;
        /// <summary>
        /// Specifies the number of BOOTP clients with addresses served from this range.
        /// </summary>
        public readonly int BootpAllocated;
        /// <summary>
        /// Specifies the maximum number of BOOTP clients this range is allowed to serve.
        /// </summary>
        public readonly int MaxBootpAllowed;

        public DHCP_BOOTP_IP_RANGE(DHCP_IP_ADDRESS startAddress, DHCP_IP_ADDRESS endAddress, int bootpAllocated, int maxBootpAllowed)
        {
            StartAddress = startAddress;
            EndAddress = endAddress;
            BootpAllocated = bootpAllocated;
            MaxBootpAllowed = maxBootpAllowed;
        }
    }
}
