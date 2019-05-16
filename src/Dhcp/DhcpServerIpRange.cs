using System;
using System.ComponentModel;
using System.Net;
using Dhcp.Native;

namespace Dhcp
{
    public struct DhcpServerIpRange
    {
        public const int DefaultBootpClientsAllocated = 0;
        public const int DefaultMaxBootpAllowed = -1;

        private readonly DhcpServerIpAddress startAddress;
        private readonly DhcpServerIpAddress endAddress;
        private readonly DhcpServerIpRangeType type;
        private readonly int bootpClientsAllocated;
        private readonly int maxBootpAllowed;

        public DhcpServerIpAddress StartAddress => startAddress;
        [Obsolete("Use StartAddress.Native instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public int StartAddressNative => (int)startAddress.Native;

        public DhcpServerIpAddress EndAddress => endAddress;
        [Obsolete("Use EndAddress.Native instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public int EndAddressNative => (int)endAddress.Native;

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
            this.startAddress = startAddress;
            this.endAddress = endAddress;
            this.type = type;
            this.bootpClientsAllocated = bootpClientsAllocated;
            this.maxBootpAllowed = maxBootpAllowed;
        }

        private DhcpServerIpRange(DhcpServerIpAddress startAddress, DhcpServerIpAddress endAddress, DhcpServerIpRangeType type)
            : this(startAddress, endAddress, type, DefaultBootpClientsAllocated, DefaultMaxBootpAllowed) { }

        private DhcpServerIpRange(DhcpServerIpAddress address, DhcpServerIpMask mask, DhcpServerIpRangeType type, int bootpClientsAllocated, int maxBootpAllowed)
            : this(DhcpServerIpAddress.FromNative(address.Native & mask.Native),
                 DhcpServerIpAddress.FromNative((address.Native & mask.Native) | ~address.Native),
                 type, bootpClientsAllocated, maxBootpAllowed) { }

        private DhcpServerIpRange(DhcpServerIpAddress address, DhcpServerIpMask mask, DhcpServerIpRangeType type)
            : this(address, mask, type, DefaultBootpClientsAllocated, DefaultMaxBootpAllowed) { }

        public static DhcpServerIpRange FromAddressesDhcpAndBootpScope(DhcpServerIpAddress startAddress, DhcpServerIpAddress endAddress, int bootpClientsAllocated = DefaultBootpClientsAllocated, int maxBootpAllowed = DefaultMaxBootpAllowed)
            => new DhcpServerIpRange(startAddress, endAddress, DhcpServerIpRangeType.ScopeDhcpAndBootp, bootpClientsAllocated, maxBootpAllowed);

        public static DhcpServerIpRange FromAddressesBootpScope(DhcpServerIpAddress startAddress, DhcpServerIpAddress endAddress, int bootpClientsAllocated = DefaultBootpClientsAllocated, int maxBootpAllowed = DefaultMaxBootpAllowed)
            => new DhcpServerIpRange(startAddress, endAddress, DhcpServerIpRangeType.ScopeBootpOnly, bootpClientsAllocated, maxBootpAllowed);

        public static DhcpServerIpRange FromAddressesDhcpScope(DhcpServerIpAddress startAddress, DhcpServerIpAddress endAddress)
            => new DhcpServerIpRange(startAddress, endAddress, DhcpServerIpRangeType.ScopeDhcpOnly);

        public static DhcpServerIpRange FromAddressesExcluded(DhcpServerIpAddress startAddress, DhcpServerIpAddress endAddress)
            => new DhcpServerIpRange(startAddress, endAddress, DhcpServerIpRangeType.Excluded);

        public static DhcpServerIpRange FromMaskDhcpAndBootpScope(DhcpServerIpAddress address, DhcpServerIpMask mask, int bootpClientsAllocated = DefaultBootpClientsAllocated, int maxBootpAllowed = DefaultMaxBootpAllowed)
            => new DhcpServerIpRange(address, mask, DhcpServerIpRangeType.ScopeDhcpAndBootp, bootpClientsAllocated, maxBootpAllowed);

        public static DhcpServerIpRange FromMaskBootpScope(DhcpServerIpAddress address, DhcpServerIpMask mask, int bootpClientsAllocated = DefaultBootpClientsAllocated, int maxBootpAllowed = DefaultMaxBootpAllowed)
            => new DhcpServerIpRange(address, mask, DhcpServerIpRangeType.ScopeBootpOnly, bootpClientsAllocated, maxBootpAllowed);

        public static DhcpServerIpRange FromMaskDhcpScope(DhcpServerIpAddress address, DhcpServerIpMask mask)
            => new DhcpServerIpRange(address, mask, DhcpServerIpRangeType.ScopeDhcpOnly);

        public static DhcpServerIpRange FromMaskExcluded(DhcpServerIpAddress address, DhcpServerIpMask mask)
            => new DhcpServerIpRange(address, mask, DhcpServerIpRangeType.Excluded);

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

        public bool Contains(DhcpServerIpAddress ipAddress) => ipAddress >= startAddress && ipAddress <= endAddress;

        internal static DhcpServerIpRange FromNative(ref DHCP_SUBNET_ELEMENT_DATA native)
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

        internal static DhcpServerIpRange FromNative(ref DHCP_SUBNET_ELEMENT_DATA_V5 native)
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
