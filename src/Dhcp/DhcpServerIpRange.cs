using System;
using System.ComponentModel;
using System.Net;
using Dhcp.Native;

namespace Dhcp
{
    public struct DhcpServerIpRange
    {
        private readonly DhcpServerIpAddress startAddress;
        private readonly DhcpServerIpAddress endAddress;

        public DhcpServerIpAddress StartAddress => startAddress;
        [Obsolete("Use StartAddress.Native instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public int StartAddressNative => (int)startAddress.Native;

        public DhcpServerIpAddress EndAddress => endAddress;
        [Obsolete("Use EndAddress.Native instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public int EndAddressNative => (int)endAddress.Native;

        internal DhcpServerIpRange(DhcpServerIpAddress startAddress, DhcpServerIpAddress endAddress)
        {
            this.startAddress = startAddress;
            this.endAddress = endAddress;
        }

        internal DhcpServerIpRange(DhcpServerIpMask mask, DhcpServerIpAddress address)
            :this (DhcpServerIpAddress.FromNative(address.Native & mask.Native),
                 DhcpServerIpAddress.FromNative((address.Native & mask.Native) | ~address.Native))
        {
        }

        public DhcpServerIpMask GetSmallestIpMask()
        {
            var dif = startAddress.Native ^ endAddress.Native;
            var bits = BitHelper.HighInsignificantBits(dif);

            return DhcpServerIpMask.FromSignificantBits(bits);
        }

        public bool Contains(IPAddress ipAddress) => Contains((DhcpServerIpAddress)ipAddress);
        public bool Contains(string ipAddress) => Contains(DhcpServerIpAddress.FromString(ipAddress));
        public bool Contains(int ipAddress) => Contains((DhcpServerIpAddress)ipAddress);
        public bool Contains(uint ipAddress) => Contains((DhcpServerIpAddress)ipAddress);

        internal bool Contains(DhcpServerIpAddress ipAddress) => ipAddress >= startAddress && ipAddress <= endAddress;

        internal static DhcpServerIpRange FromNative(ref DHCP_IP_RANGE native)
        {
            return new DhcpServerIpRange(startAddress: native.StartAddress.AsNetworkToIpAddress(),
                                         endAddress: native.EndAddress.AsNetworkToIpAddress());
        }

        public static explicit operator DhcpServerIpRange(DhcpServerBootpIpRange range)
            => new DhcpServerIpRange(range.StartAddress, range.EndAddress);

        public override string ToString() => $"{startAddress} - {endAddress}";
    }
}
