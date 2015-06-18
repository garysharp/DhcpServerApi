using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerIpRange
    {
        private DHCP_IP_ADDRESS startAddress;
        private DHCP_IP_ADDRESS endAddress;

        public string StartAddress { get { return startAddress.ToString(); } }
        public string EndAddress { get { return endAddress.ToString(); } }

        public int BootpClientsAllocated { get; private set; }
        public int MaxBootpAllowed { get; private set; }

        private DhcpServerIpRange(DHCP_IP_ADDRESS StartAddress, DHCP_IP_ADDRESS EndAddress)
        {
            this.startAddress = StartAddress;
            this.endAddress = EndAddress;
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
