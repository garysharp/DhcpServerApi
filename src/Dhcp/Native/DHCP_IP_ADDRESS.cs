using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    internal readonly struct DHCP_IP_ADDRESS
    {
        private readonly uint ipAddress;

        public DHCP_IP_ADDRESS(uint IpAddress)
        {
            ipAddress = IpAddress;
        }

        public static DHCP_IP_ADDRESS FromString(string IpAddress) => new DHCP_IP_ADDRESS(BitHelper.StringToIpAddress(IpAddress));
        public override string ToString() => BitHelper.IpAddressToString(ipAddress);

        public DhcpServerIpAddress AsHostToIpAddress() => new DhcpServerIpAddress(BitHelper.HostToNetworkOrder(ipAddress));
        public DhcpServerIpAddress AsNetworkToIpAddress() => new DhcpServerIpAddress(ipAddress);

        public static explicit operator DHCP_IP_ADDRESS(int ipAddress) => new DHCP_IP_ADDRESS((uint)ipAddress);
        public static explicit operator int(DHCP_IP_ADDRESS ipAddress) => (int)ipAddress.ipAddress;

        public static DHCP_IP_ADDRESS operator &(DHCP_IP_ADDRESS address, DHCP_IP_MASK mask)
            => (DHCP_IP_ADDRESS)((int)address & (int)mask);
    }
}
