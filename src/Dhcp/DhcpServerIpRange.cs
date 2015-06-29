using Dhcp.Native;
using System.Net;

namespace Dhcp
{
    public class DhcpServerIpRange
    {
        private DHCP_IP_ADDRESS startAddress;
        private DHCP_IP_ADDRESS endAddress;

        public IPAddress StartAddress { get { return startAddress.ToIPAddress(); } }
        public int StartAddressNative { get { return (int)startAddress; } }
        public IPAddress EndAddress { get { return endAddress.ToIPAddress(); } }
        public int EndAddressNative { get { return (int)endAddress; } }

        public int BootpClientsAllocated { get; private set; }
        public int MaxBootpAllowed { get; private set; }

        private DhcpServerIpRange(DHCP_IP_ADDRESS StartAddress, DHCP_IP_ADDRESS EndAddress)
        {
            this.startAddress = StartAddress;
            this.endAddress = EndAddress;
        }

        public bool Contains(IPAddress IpAddress)
        {
            return Contains(DHCP_IP_ADDRESS.FromIPAddress(IpAddress));
        }

        public bool Contains(string IpAddress)
        {
            return Contains(DHCP_IP_ADDRESS.FromString(IpAddress));
        }

        public bool Contains(int IpAddress)
        {
            return Contains((DHCP_IP_ADDRESS)IpAddress);
        }

        public bool Contains(uint IpAddress)
        {
            return Contains((DHCP_IP_ADDRESS)IpAddress);
        }

        internal bool Contains(DHCP_IP_ADDRESS IpAddress)
        {
            return IpAddress >= startAddress && IpAddress <= endAddress;
        }

        internal static DhcpServerIpRange FromNative(DHCP_IP_RANGE Native)
        {
            return new DhcpServerIpRange(Native.StartAddress, Native.EndAddress);
        }

        internal static DhcpServerIpRange FromNative(DHCP_BOOTP_IP_RANGE Native)
        {
            return new DhcpServerIpRange(Native.StartAddress, Native.EndAddress)
            {
                BootpClientsAllocated = Native.BootpAllocated,
                MaxBootpAllowed = Native.MaxBootpAllowed
            };
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", startAddress, endAddress);
        }
    }
}
