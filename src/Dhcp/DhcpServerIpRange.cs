using System.Net;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerIpRange
    {
        private DHCP_IP_ADDRESS startAddress;
        private DHCP_IP_ADDRESS endAddress;

        public IPAddress StartAddress => startAddress.ToIPAddress();
        public int StartAddressNative => (int)startAddress;
        public IPAddress EndAddress => endAddress.ToIPAddress();
        public int EndAddressNative => (int)endAddress;

        public int BootpClientsAllocated { get; }
        public int MaxBootpAllowed { get; }

        private DhcpServerIpRange(DHCP_IP_ADDRESS startAddress, DHCP_IP_ADDRESS endAddress, int bootpClientsAllocated, int maxBootpAllowed)
        {
            this.startAddress = startAddress;
            this.endAddress = endAddress;
            BootpClientsAllocated = bootpClientsAllocated;
            MaxBootpAllowed = maxBootpAllowed;
        }

        public bool Contains(IPAddress ipAddress) => Contains(DHCP_IP_ADDRESS.FromIPAddress(ipAddress));

        public bool Contains(string ipAddress) => Contains(DHCP_IP_ADDRESS.FromString(ipAddress));

        public bool Contains(int ipAddress) => Contains((DHCP_IP_ADDRESS)ipAddress);

        public bool Contains(uint ipAddress) => Contains((DHCP_IP_ADDRESS)ipAddress);

        internal bool Contains(DHCP_IP_ADDRESS ipAddress) => ipAddress >= startAddress && ipAddress <= endAddress;

        internal static DhcpServerIpRange FromNative(DHCP_IP_RANGE native)
            => new DhcpServerIpRange(native.StartAddress, native.EndAddress, 0, 0);

        internal static DhcpServerIpRange FromNative(DHCP_BOOTP_IP_RANGE native)
            => new DhcpServerIpRange(native.StartAddress, native.EndAddress, native.BootpAllocated, native.MaxBootpAllowed);

        public override string ToString() => $"{startAddress} - {endAddress}";
    }
}
