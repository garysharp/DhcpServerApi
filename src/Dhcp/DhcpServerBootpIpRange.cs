using System;
using System.ComponentModel;
using System.Net;
using Dhcp.Native;

namespace Dhcp
{
    public struct DhcpServerBootpIpRange
    {
        private readonly DhcpServerIpAddress startAddress;
        private readonly DhcpServerIpAddress endAddress;
        private readonly int bootpClientsAllocated;
        private readonly int maxBootpAllowed;

        public DhcpServerIpAddress StartAddress => startAddress;
        [Obsolete("Use StartAddress.Native instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public int StartAddressNative => (int)startAddress.Native;

        public DhcpServerIpAddress EndAddress => endAddress;
        [Obsolete("Use EndAddress.Native instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public int EndAddressNative => (int)endAddress.Native;

        /// <summary>
        /// Specifies the number of BOOTP clients with addresses served from this range.
        /// </summary>
        public int BootpClientsAllocated => bootpClientsAllocated;
        /// <summary>
        /// Specifies the maximum number of BOOTP clients this range is allowed to serve.
        /// </summary>
        public int MaxBootpAllowed => maxBootpAllowed;

        private DhcpServerBootpIpRange(DhcpServerIpAddress startAddress, DhcpServerIpAddress endAddress, int bootpClientsAllocated, int maxBootpAllowed)
        {
            this.startAddress = startAddress;
            this.endAddress = endAddress;
            this.bootpClientsAllocated = bootpClientsAllocated;
            this.maxBootpAllowed = maxBootpAllowed;
        }

        public DhcpServerIpMask GetSmallestIpMask()
        {
            var dif = StartAddress.Native ^ endAddress.Native;
            var bits = BitHelper.HighInsignificantBits(dif);

            return DhcpServerIpMask.FromSignificantBits(bits);
        }

        public bool Contains(IPAddress ipAddress) => Contains((DhcpServerIpAddress)ipAddress);

        public bool Contains(string ipAddress) => Contains(DhcpServerIpAddress.FromString(ipAddress));

        public bool Contains(int ipAddress) => Contains((DhcpServerIpAddress)ipAddress);

        public bool Contains(uint ipAddress) => Contains((DhcpServerIpAddress)ipAddress);

        internal bool Contains(DhcpServerIpAddress ipAddress) => ipAddress >= StartAddress && ipAddress <= endAddress;

        internal static DhcpServerBootpIpRange FromNative(ref DHCP_BOOTP_IP_RANGE native)
        {
            return new DhcpServerBootpIpRange(
                native.StartAddress.AsNetworkToIpAddress(),
                native.EndAddress.AsNetworkToIpAddress(),
                native.BootpAllocated,
                native.MaxBootpAllowed);
        }

        public static explicit operator DhcpServerBootpIpRange(DhcpServerIpRange range)
            => new DhcpServerBootpIpRange(range.StartAddress, range.EndAddress, 0, 0);

        public override string ToString() => $"{StartAddress} - {endAddress}";
    }
}
