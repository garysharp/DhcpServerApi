using System;
using System.Runtime.InteropServices;
using Dhcp.Native;

namespace Dhcp
{
    [Serializable]
    public struct DhcpServerIpMask
    {
#pragma warning disable IDE0032// Use auto property
        /// <summary>
        /// IP Mask stored in big-endian (network) order
        /// </summary>
        private readonly uint mask;
#pragma warning restore IDE0032 // Use auto property

        public DhcpServerIpMask(uint mask)
        {
            this.mask = mask;
        }
        public DhcpServerIpMask(string mask)
        {
            this.mask = BitHelper.StringToIpAddress(mask);
        }

        /// <summary>
        /// IP Mask in network order
        /// </summary>
        public uint Native => mask;

        public static DhcpServerIpMask Empty => new DhcpServerIpMask(0);
        public static DhcpServerIpMask FromNative(uint nativeMask)
            => new DhcpServerIpMask(nativeMask);

        public static DhcpServerIpMask FromNative(int nativeMask)
            => new DhcpServerIpMask((uint)nativeMask);

        internal static DhcpServerIpMask FromNative(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero)
                throw new ArgumentNullException(nameof(pointer));

            return new DhcpServerIpMask((uint)BitHelper.HostToNetworkOrder(Marshal.ReadInt32(pointer)));
        }

        public static DhcpServerIpMask FromString(string mask)
            => new DhcpServerIpMask(mask);

        public static DhcpServerIpMask FromSignificantBits(int bitCount)
        {
            if (bitCount > 32 || bitCount < 0)
                throw new ArgumentOutOfRangeException(nameof(bitCount));

            if (bitCount == 0)
                return new DhcpServerIpMask(0U);

            var m = unchecked((int)0x8000_0000) >> (bitCount - 1); // signed int

            return new DhcpServerIpMask((uint)m);
        }

        public int SignificantBits => BitHelper.HighSignificantBits(mask);

        public DhcpServerIpRange GetDhcpIpRange(DhcpServerIpAddress address) => DhcpServerIpRange.AsDhcpScope(address, this);
        public DhcpServerIpRange GetDhcpAndBootpIpRange(DhcpServerIpAddress address) => DhcpServerIpRange.AsDhcpAndBootpScope(address, this);
        public DhcpServerIpRange GetBootpIpRange(DhcpServerIpAddress address) => DhcpServerIpRange.AsBootpScope(address, this);
        internal DhcpServerIpRange GetIpRange(DhcpServerIpAddress address, DhcpServerIpRangeType type) => DhcpServerIpRange.FromMask(address, this, type);

        internal DHCP_IP_MASK ToNativeAsHost() => new DHCP_IP_MASK(BitHelper.NetworkToHostOrder(mask));
        internal DHCP_IP_MASK ToNativeAsNetwork() => new DHCP_IP_MASK(mask);

        public override string ToString() => BitHelper.IpAddressToString(mask);

        public static explicit operator DhcpServerIpAddress(DhcpServerIpMask mask) => new DhcpServerIpAddress(mask.mask);
        public static explicit operator DhcpServerIpMask(string mask) => new DhcpServerIpMask(mask);
    }
}
