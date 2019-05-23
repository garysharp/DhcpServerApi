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
        private readonly uint ipMask;
#pragma warning restore IDE0032 // Use auto property

        public DhcpServerIpMask(uint ipMask)
        {
            this.ipMask = ipMask;
        }
        public DhcpServerIpMask(string ipMask)
        {
            this.ipMask = BitHelper.StringToIpAddress(ipMask);
        }

        /// <summary>
        /// IP Mask in network order
        /// </summary>
        public uint Native => ipMask;

        public static DhcpServerIpMask Empty => new DhcpServerIpMask(0);
        public static DhcpServerIpMask FromNative(uint nativeIpMask)
            => new DhcpServerIpMask(nativeIpMask);

        public static DhcpServerIpMask FromNative(int nativeIpMask)
            => new DhcpServerIpMask((uint)nativeIpMask);

        internal static DhcpServerIpMask FromNative(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero)
                throw new ArgumentNullException(nameof(pointer));

            return new DhcpServerIpMask((uint)BitHelper.HostToNetworkOrder(Marshal.ReadInt32(pointer)));
        }

        public static DhcpServerIpMask FromString(string ipMask)
            => new DhcpServerIpMask(ipMask);

        public static DhcpServerIpMask FromSignificantBits(int bitCount)
        {
            if (bitCount > 32 || bitCount < 0)
                throw new ArgumentOutOfRangeException(nameof(bitCount));

            if (bitCount == 0)
                return new DhcpServerIpMask(0U);

            var m = unchecked((int)0x8000_0000) >> (bitCount - 1); // signed int

            return new DhcpServerIpMask((uint)m);
        }

        public int SignificantBits => BitHelper.HighSignificantBits(ipMask);

        public DhcpServerIpRange GetDhcpIpRange(DhcpServerIpAddress ipAddress) => DhcpServerIpRange.AsDhcpScope(ipAddress, this);
        public DhcpServerIpRange GetDhcpAndBootpIpRange(DhcpServerIpAddress ipAddress) => DhcpServerIpRange.AsDhcpAndBootpScope(ipAddress, this);
        public DhcpServerIpRange GetBootpIpRange(DhcpServerIpAddress ipAddress) => DhcpServerIpRange.AsBootpScope(ipAddress, this);
        internal DhcpServerIpRange GetIpRange(DhcpServerIpAddress ipAddress, DhcpServerIpRangeType type) => DhcpServerIpRange.FromMask(ipAddress, this, type);

        internal DHCP_IP_MASK ToNativeAsHost() => new DHCP_IP_MASK(BitHelper.NetworkToHostOrder(ipMask));
        internal DHCP_IP_MASK ToNativeAsNetwork() => new DHCP_IP_MASK(ipMask);

        public override string ToString() => BitHelper.IpAddressToString(ipMask);

        public static explicit operator DhcpServerIpAddress(DhcpServerIpMask ipMask) => new DhcpServerIpAddress(ipMask.ipMask);
        public static explicit operator DhcpServerIpMask(string ipMask) => new DhcpServerIpMask(ipMask);
    }
}
