using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_IP_RANGE structure defines a range of IP addresses.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_IP_RANGE
    {
        /// <summary>
        /// DHCP_IP_ADDRESS value that contains the first IP address in the range.
        /// </summary>
        public readonly DHCP_IP_ADDRESS StartAddress;
        /// <summary>
        /// DHCP_IP_ADDRESS value that contains the last IP address in the range.
        /// </summary>
        public readonly DHCP_IP_ADDRESS EndAddress;

        public DHCP_IP_RANGE(DHCP_IP_ADDRESS StartAddress, DHCP_IP_ADDRESS EndAddress)
        {
            this.StartAddress = StartAddress;
            this.EndAddress = EndAddress;
        }
    }
}
