using System;
using System.Net;
using Dhcp.Native;

namespace Dhcp
{
    public struct DhcpServerIpRange
    {
        public const int DefaultBootpClientsAllocated = 0;
        public const int DefaultMaxBootpAllowed = -1;

#pragma warning disable IDE0032 // Use auto property
        private readonly DhcpServerIpAddress startAddress;
        private readonly DhcpServerIpAddress endAddress;
        private readonly DhcpServerIpRangeType type;
        private readonly int bootpClientsAllocated;
        private readonly int maxBootpAllowed;
#pragma warning restore IDE0032 // Use auto property

        public DhcpServerIpAddress StartAddress => startAddress;
        public DhcpServerIpAddress EndAddress => endAddress;
        public DhcpServerIpRangeType Type => type;

        /// <summary>
        /// Specifies the number of BOOTP clients with addresses served from this range.
        /// </summary>
        public int BootpClientsAllocated => bootpClientsAllocated;
        /// <summary>
        /// Specifies the maximum number of BOOTP clients this range is allowed to serve.
        /// </summary>
        public int MaxBootpAllowed => maxBootpAllowed;

        private DhcpServerIpRange(DhcpServerIpAddress startAddress, DhcpServerIpAddress endAddress, DhcpServerIpRangeType type, int bootpClientsAllocated, int maxBootpAllowed)
        {
            if (startAddress > endAddress)
                throw new ArgumentOutOfRangeException(nameof(endAddress), "The ip range start address cannot be greater than the end address");

            this.startAddress = startAddress;
            this.endAddress = endAddress;
            this.type = type;
            this.bootpClientsAllocated = bootpClientsAllocated;
            this.maxBootpAllowed = maxBootpAllowed;
        }

        private DhcpServerIpRange(DhcpServerIpAddress startAddress, DhcpServerIpAddress endAddress, DhcpServerIpRangeType type)
            : this(startAddress, endAddress, type, DefaultBootpClientsAllocated, DefaultMaxBootpAllowed) { }

        public static DhcpServerIpRange AsDhcpAndBootpScope(DhcpServerIpAddress startAddress, DhcpServerIpAddress endAddress, int bootpClientsAllocated = DefaultBootpClientsAllocated, int maxBootpAllowed = DefaultMaxBootpAllowed)
            => new DhcpServerIpRange(startAddress, endAddress, DhcpServerIpRangeType.ScopeDhcpAndBootp, bootpClientsAllocated, maxBootpAllowed);
        public static DhcpServerIpRange AsDhcpAndBootpScope(DhcpServerIpAddress address, DhcpServerIpMask mask, int bootpClientsAllocated = DefaultBootpClientsAllocated, int maxBootpAllowed = DefaultMaxBootpAllowed)
            => FromMask(address, mask, DhcpServerIpRangeType.ScopeDhcpAndBootp, bootpClientsAllocated, maxBootpAllowed);
        public static DhcpServerIpRange AsDhcpAndBootpScope(string cidrSubnet, int bootpClientsAllocated = DefaultBootpClientsAllocated, int maxBootpAllowed = DefaultMaxBootpAllowed)
            => FromCidr(cidrSubnet, DhcpServerIpRangeType.ScopeDhcpAndBootp, bootpClientsAllocated, maxBootpAllowed);

        public static DhcpServerIpRange AsBootpScope(DhcpServerIpAddress startAddress, DhcpServerIpAddress endAddress, int bootpClientsAllocated = DefaultBootpClientsAllocated, int maxBootpAllowed = DefaultMaxBootpAllowed)
            => new DhcpServerIpRange(startAddress, endAddress, DhcpServerIpRangeType.ScopeBootpOnly, bootpClientsAllocated, maxBootpAllowed);
        public static DhcpServerIpRange AsBootpScope(DhcpServerIpAddress address, DhcpServerIpMask mask, int bootpClientsAllocated = DefaultBootpClientsAllocated, int maxBootpAllowed = DefaultMaxBootpAllowed)
            => FromMask(address, mask, DhcpServerIpRangeType.ScopeBootpOnly, bootpClientsAllocated, maxBootpAllowed);
        public static DhcpServerIpRange AsBootpScope(string cidrSubnet, int bootpClientsAllocated = DefaultBootpClientsAllocated, int maxBootpAllowed = DefaultMaxBootpAllowed)
            => FromCidr(cidrSubnet, DhcpServerIpRangeType.ScopeBootpOnly, bootpClientsAllocated, maxBootpAllowed);

        public static DhcpServerIpRange AsDhcpScope(DhcpServerIpAddress startAddress, DhcpServerIpAddress endAddress)
            => new DhcpServerIpRange(startAddress, endAddress, DhcpServerIpRangeType.ScopeDhcpOnly);
        public static DhcpServerIpRange AsDhcpScope(DhcpServerIpAddress address, DhcpServerIpMask mask)
            => FromMask(address, mask, DhcpServerIpRangeType.ScopeDhcpOnly);
        public static DhcpServerIpRange AsDhcpScope(string cidrSubnet)
            => FromCidr(cidrSubnet, DhcpServerIpRangeType.ScopeDhcpOnly);

        public static DhcpServerIpRange AsExcluded(DhcpServerIpAddress startAddress, DhcpServerIpAddress endAddress)
            => new DhcpServerIpRange(startAddress, endAddress, DhcpServerIpRangeType.Excluded);
        public static DhcpServerIpRange AsExcluded(DhcpServerIpAddress address, DhcpServerIpMask mask)
            => FromMask(address, mask, DhcpServerIpRangeType.Excluded);
        public static DhcpServerIpRange AsExcluded(string cidrSubnet)
            => FromCidr(cidrSubnet, DhcpServerIpRangeType.Excluded);

        internal static DhcpServerIpRange FromMask(DhcpServerIpAddress address, DhcpServerIpMask mask, DhcpServerIpRangeType type)
            => FromMask(address, mask, type, DefaultBootpClientsAllocated, DefaultMaxBootpAllowed);

        private static DhcpServerIpRange FromMask(DhcpServerIpAddress address, DhcpServerIpMask mask, DhcpServerIpRangeType type, int bootpClientsAllocated, int maxBootpAllowed)
        {
            var startAddressNative = address.Native & mask.Native;
            var endAddressNative = (address.Native & mask.Native) | ~mask.Native;

            if (type == DhcpServerIpRangeType.ScopeDhcpOnly ||
                type == DhcpServerIpRangeType.ScopeDhcpAndBootp ||
                type == DhcpServerIpRangeType.ScopeBootpOnly)
            {
                // remove subnet id and broadcast address from range
                startAddressNative++;
                endAddressNative--;
            }

            return new DhcpServerIpRange(startAddress: DhcpServerIpAddress.FromNative(startAddressNative),
                                         endAddress: DhcpServerIpAddress.FromNative(endAddressNative),
                                         type: type,
                                         bootpClientsAllocated: bootpClientsAllocated,
                                         maxBootpAllowed: maxBootpAllowed);
        }

        private static DhcpServerIpRange FromCidr(string cidrSubnet, DhcpServerIpRangeType type)
            => FromCidr(cidrSubnet, type, DefaultBootpClientsAllocated, DefaultMaxBootpAllowed);
        private static DhcpServerIpRange FromCidr(string cidrSubnet, DhcpServerIpRangeType type, int bootpClientsAllocated, int maxBootpAllowed)
        {
            if (string.IsNullOrEmpty(cidrSubnet))
                throw new ArgumentNullException(nameof(cidrSubnet));

            var slashIndex = cidrSubnet.IndexOf('/');
            if (slashIndex < 7 || !BitHelper.TryParseByteFromSubstring(cidrSubnet, ++slashIndex, cidrSubnet.Length - slashIndex, out var significantBits))
                throw new ArgumentException("Invalid CIDR subnet notation format");

            var address = DhcpServerIpAddress.FromNative(BitHelper.StringToIpAddress(cidrSubnet, 0, --slashIndex));
            var mask = DhcpServerIpMask.FromSignificantBits(significantBits);

            return FromMask(address, mask, type, bootpClientsAllocated, maxBootpAllowed);
        }

        public DhcpServerIpMask SmallestMask
        {
            get
            {
                var dif = startAddress.Native ^ endAddress.Native;
                var bits = BitHelper.HighInsignificantBits(dif);

                return DhcpServerIpMask.FromSignificantBits(bits);
            }
        }

        public bool Contains(IPAddress address) => Contains((DhcpServerIpAddress)address);
        public bool Contains(string address) => Contains(DhcpServerIpAddress.FromString(address));
        public bool Contains(int address) => Contains((DhcpServerIpAddress)address);
        public bool Contains(uint address) => Contains((DhcpServerIpAddress)address);

        public bool Contains(DhcpServerIpAddress address) => address >= startAddress && address <= endAddress;

        internal static DhcpServerIpRange FromNative(DHCP_SUBNET_ELEMENT_DATA native)
        {
            switch (native.ElementType)
            {
                case DHCP_SUBNET_ELEMENT_TYPE.DhcpIpRanges:
                case DHCP_SUBNET_ELEMENT_TYPE.DhcpExcludedIpRanges:
                    var bootpIpRange = native.ReadIpRange();
                    // translate legacy 'DhcpIpRanges' -> 'DhcpIpRangesDhcpOnly'
                    var type = (DhcpServerIpRangeType)(native.ElementType == DHCP_SUBNET_ELEMENT_TYPE.DhcpIpRanges ? DHCP_SUBNET_ELEMENT_TYPE.DhcpIpRangesDhcpOnly : native.ElementType);
                    return new DhcpServerIpRange(startAddress: bootpIpRange.StartAddress.AsNetworkToIpAddress(),
                                                 endAddress: bootpIpRange.EndAddress.AsNetworkToIpAddress(),
                                                 type: type);
                default:
                    throw new DhcpServerException(nameof(DHCP_SUBNET_ELEMENT_DATA_V5), DhcpErrors.ERROR_INVALID_PARAMETER, "An unexpected subnet element type was encountered");
            }
        }

        internal static DhcpServerIpRange FromNative(DHCP_SUBNET_ELEMENT_DATA_V5 native)
        {
            switch (native.ElementType)
            {
                case DHCP_SUBNET_ELEMENT_TYPE.DhcpIpRanges:
                case DHCP_SUBNET_ELEMENT_TYPE.DhcpIpRangesDhcpOnly:
                case DHCP_SUBNET_ELEMENT_TYPE.DhcpIpRangesDhcpBootp:
                case DHCP_SUBNET_ELEMENT_TYPE.DhcpIpRangesBootpOnly:
                    var bootpIpRange = native.ReadBootpIpRange();
                    // translate legacy 'DhcpIpRanges' -> 'DhcpIpRangesDhcpOnly'
                    var type = (DhcpServerIpRangeType)(native.ElementType == DHCP_SUBNET_ELEMENT_TYPE.DhcpIpRanges ? DHCP_SUBNET_ELEMENT_TYPE.DhcpIpRangesDhcpOnly : native.ElementType);
                    return new DhcpServerIpRange(startAddress: bootpIpRange.StartAddress.AsNetworkToIpAddress(),
                                                 endAddress: bootpIpRange.EndAddress.AsNetworkToIpAddress(),
                                                 type: type,
                                                 bootpClientsAllocated: bootpIpRange.BootpAllocated,
                                                 maxBootpAllowed: bootpIpRange.MaxBootpAllowed);
                case DHCP_SUBNET_ELEMENT_TYPE.DhcpExcludedIpRanges:
                    var ipRange = native.ReadIpRange();
                    return new DhcpServerIpRange(startAddress: ipRange.StartAddress.AsNetworkToIpAddress(),
                                                 endAddress: ipRange.EndAddress.AsNetworkToIpAddress(),
                                                 type: DhcpServerIpRangeType.Excluded);
                default:
                    throw new DhcpServerException(nameof(DHCP_SUBNET_ELEMENT_DATA_V5), DhcpErrors.ERROR_INVALID_PARAMETER, "An unexpected subnet element type was encountered");
            }
        }

        internal static DhcpServerIpRange FromNative(ref DHCP_IP_RANGE native, DhcpServerIpRangeType type)
        {
            return new DhcpServerIpRange(startAddress: native.StartAddress.AsNetworkToIpAddress(),
                                         endAddress: native.EndAddress.AsNetworkToIpAddress(),
                                         type: type);
        }

        internal static DhcpServerIpRange FromNative(ref DHCP_BOOTP_IP_RANGE native, DhcpServerIpRangeType type)
        {
            return new DhcpServerIpRange(startAddress: native.StartAddress.AsNetworkToIpAddress(),
                                         endAddress: native.EndAddress.AsNetworkToIpAddress(),
                                         type: type,
                                         bootpClientsAllocated: native.BootpAllocated,
                                         maxBootpAllowed: native.MaxBootpAllowed);
        }

        internal DHCP_BOOTP_IP_RANGE ToNativeBootpIpRange()
            => new DHCP_BOOTP_IP_RANGE(StartAddress.ToNativeAsNetwork(), EndAddress.ToNativeAsNetwork(), BootpClientsAllocated, MaxBootpAllowed);

        internal DHCP_IP_RANGE ToNativeIpRange()
            => new DHCP_IP_RANGE(StartAddress.ToNativeAsNetwork(), EndAddress.ToNativeAsNetwork());

        public override string ToString() => $"{startAddress} - {endAddress} [{type}]";
    }
}
