using System;
using System.Net;
using System.Runtime.InteropServices;
using Dhcp.Native;

namespace Dhcp
{
    [Serializable]
    public struct DhcpServerIpAddress : IEquatable<DhcpServerIpAddress>, IEquatable<IPAddress>
    {
#pragma warning disable IDE0032 // Use auto property
        /// <summary>
        /// IP Address stored in big-endian (network) order
        /// </summary>
        private readonly uint ipAddress;
#pragma warning restore IDE0032 // Use auto property

        public DhcpServerIpAddress(uint nativeIpAddress)
        {
            ipAddress = nativeIpAddress;
        }

        public DhcpServerIpAddress(string ipAddress)
        {
            this.ipAddress = BitHelper.StringToIpAddress(ipAddress);
        }

        public DhcpServerIpAddress(IPAddress ipAddress)
        {
            if (ipAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            {
                throw new ArgumentOutOfRangeException(nameof(ipAddress), "Only IPv4 addresses are supported");
            }

#pragma warning disable CS0618 // Type or member is obsolete
            var ip = (uint)ipAddress.Address;
#pragma warning restore CS0618 // Type or member is obsolete

            // DhcpServerIpAddress always stores in network order
            // IPAddress stores in host order
            this.ipAddress = BitHelper.HostToNetworkOrder(ip);
        }

        /// <summary>
        /// IP Address in network order
        /// </summary>
        public uint Native => ipAddress;

        public byte[] GetBytes()
        {
            var buffer = new byte[4];
            BitHelper.Write(buffer, 0, ipAddress);
            return buffer;
        }

        public byte GetByte(int index)
        {
            if (index < 0 || index > 3)
                throw new ArgumentOutOfRangeException(nameof(index));

            return (byte)(ipAddress >> ((3 - index) * 8));
        }

        public static DhcpServerIpAddress Empty => new DhcpServerIpAddress(0);
        public static DhcpServerIpAddress FromNative(uint nativeIpAddress) => new DhcpServerIpAddress(nativeIpAddress);
        public static DhcpServerIpAddress FromNative(int nativeIpAddress) => new DhcpServerIpAddress((uint)nativeIpAddress);
        internal static DhcpServerIpAddress FromNative(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero)
                throw new ArgumentNullException(nameof(pointer));

            return new DhcpServerIpAddress((uint)BitHelper.HostToNetworkOrder(Marshal.ReadInt32(pointer)));
        }

        public static DhcpServerIpAddress FromString(string ipAddress) => new DhcpServerIpAddress(ipAddress);

        public override string ToString() => BitHelper.IpAddressToString(ipAddress);

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is DhcpServerIpAddress sia)
                return Equals(sia);
            else if (obj is IPAddress ia)
                return Equals(ia);

            return false;
        }

        public override int GetHashCode() => (int)ipAddress;

        public bool Equals(DhcpServerIpAddress other) => ipAddress == other.ipAddress;

        public bool Equals(IPAddress other)
        {
            if (other == null || other.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                return false;

#pragma warning disable CS0618 // Type or member is obsolete
            var otherIp = (uint)other.Address;
#pragma warning restore CS0618 // Type or member is obsolete

            return ipAddress == BitHelper.HostToNetworkOrder(otherIp);
        }

        public static bool operator ==(DhcpServerIpAddress lhs, DhcpServerIpAddress rhs) => lhs.Equals(rhs);

        public static bool operator !=(DhcpServerIpAddress lhs, DhcpServerIpAddress rhs) => !lhs.Equals(rhs);

        public static bool operator ==(DhcpServerIpAddress lhs, IPAddress rhs) => lhs.Equals(rhs);

        public static bool operator !=(DhcpServerIpAddress lhs, IPAddress rhs) => !lhs.Equals(rhs);

        internal DHCP_IP_ADDRESS ToNativeAsHost() => new DHCP_IP_ADDRESS(BitHelper.NetworkToHostOrder(ipAddress));

        internal DHCP_IP_ADDRESS ToNativeAsNetwork() => new DHCP_IP_ADDRESS(ipAddress);

        public static explicit operator uint(DhcpServerIpAddress ipAddress) => ipAddress.ipAddress;
        public static explicit operator DhcpServerIpAddress(uint ipAddress) => new DhcpServerIpAddress(ipAddress);
        public static explicit operator int(DhcpServerIpAddress ipAddress) => (int)ipAddress.ipAddress;
        public static explicit operator DhcpServerIpAddress(int ipAddress) => new DhcpServerIpAddress((uint)ipAddress);

        public static implicit operator DhcpServerIpAddress(IPAddress ipAddress) => new DhcpServerIpAddress(ipAddress);
        public static implicit operator IPAddress(DhcpServerIpAddress ipAddress)
            // IPAddress stores in host order; DhcpServerIpAddress always stores in network order
            => new IPAddress(BitHelper.NetworkToHostOrder(ipAddress.ipAddress));

        public static explicit operator DhcpServerIpMask(DhcpServerIpAddress ipAddress) => new DhcpServerIpMask(ipAddress.ipAddress);
        public static implicit operator string(DhcpServerIpAddress ipAddress) => ipAddress.ToString();
        public static implicit operator DhcpServerIpAddress(string ipAddress) => FromString(ipAddress);

        public static bool operator >(DhcpServerIpAddress a, DhcpServerIpAddress b) => a.ipAddress > b.ipAddress;
        public static bool operator >=(DhcpServerIpAddress a, DhcpServerIpAddress b) => a.ipAddress >= b.ipAddress;
        public static bool operator <(DhcpServerIpAddress a, DhcpServerIpAddress b) => a.ipAddress < b.ipAddress;
        public static bool operator <=(DhcpServerIpAddress a, DhcpServerIpAddress b) => a.ipAddress <= b.ipAddress;
    }
}
