using System;
using Dhcp.Native;

namespace Dhcp
{
    [Serializable]
    public struct DhcpServerIpMask
    {
        /// <summary>
        /// IP Mask stored in big-endian (network) order
        /// </summary>
        private readonly uint ipMask;

        public DhcpServerIpMask(uint nativeIpMask)
        {
            ipMask = nativeIpMask;
        }

        /// <summary>
        /// IP Mask in network order
        /// </summary>
        public uint Native => ipMask;

        public static DhcpServerIpMask FromNative(uint nativeIpAddress) =>
            DhcpServerIpAddress.FromNative(nativeIpAddress); // reuse DhcpServerIpAddress method

        public static DhcpServerIpMask FromNative(int nativeIpAddress) 
            => DhcpServerIpAddress.FromNative((uint)nativeIpAddress); // reuse DhcpServerIpAddress method

        internal static DhcpServerIpMask FromNative(IntPtr pointer) 
            => DhcpServerIpAddress.FromNative(pointer); // reuse DhcpServerIpAddress method

        public static DhcpServerIpMask FromString(string ipAddress)
            => DhcpServerIpAddress.FromString(ipAddress); // reuse DhcpServerIpAddress method

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

        public DhcpServerIpRange GetIpRange(DhcpServerIpAddress ipAddress) => new DhcpServerIpRange(this, ipAddress);

        internal DHCP_IP_MASK ToNativeAsHost() => new DHCP_IP_MASK(BitHelper.NetworkToHostOrder(ipMask));
        internal DHCP_IP_MASK ToNativeAsNetwork() => new DHCP_IP_MASK(ipMask);

        public override string ToString() => BitHelper.IpAddressToString(ipMask);

        public static implicit operator DhcpServerIpAddress(DhcpServerIpMask ipMask) => new DhcpServerIpAddress(ipMask.ipMask);
    }
}
