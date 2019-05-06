using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    internal struct DHCP_IP_MASK
    {
        private readonly uint ipMask;

        public DHCP_IP_MASK(uint IpMask)
        {
            ipMask = IpMask;
        }

        public override string ToString() => BitHelper.IpAddressToString(ipMask);

        public static explicit operator uint(DHCP_IP_MASK ipMask) => ipMask.ipMask;
        public static explicit operator DHCP_IP_MASK(uint ipMask) => new DHCP_IP_MASK(ipMask);
        public static explicit operator int(DHCP_IP_MASK ipMask) => (int)ipMask.ipMask;
        public static explicit operator DHCP_IP_MASK(int ipMask) => new DHCP_IP_MASK((uint)ipMask);

        public DhcpServerIpMask AsHostToIpMask() => new DhcpServerIpMask(BitHelper.HostToNetworkOrder(ipMask));

        public DhcpServerIpMask AsNetworkToIpMask() => new DhcpServerIpMask(ipMask);
    }
}
