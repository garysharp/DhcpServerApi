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
        private readonly uint address;
#pragma warning restore IDE0032 // Use auto property

        public DhcpServerIpAddress(uint nativeAddress)
        {
            address = nativeAddress;
        }

        public DhcpServerIpAddress(string address)
        {
            this.address = BitHelper.StringToIpAddress(address);
        }

        public DhcpServerIpAddress(IPAddress address)
        {
            if (address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            {
                throw new ArgumentOutOfRangeException(nameof(address), "Only IPv4 addresses are supported");
            }

#pragma warning disable CS0618 // Type or member is obsolete
            var ip = (uint)address.Address;
#pragma warning restore CS0618 // Type or member is obsolete

            // DhcpServerIpAddress always stores in network order
            // IPAddress stores in host order
            this.address = BitHelper.HostToNetworkOrder(ip);
        }

        /// <summary>
        /// IP Address in network order
        /// </summary>
        public uint Native => address;

        public byte[] GetBytes()
        {
            var buffer = new byte[4];
            BitHelper.Write(buffer, 0, address);
            return buffer;
        }

        public byte GetByte(int index)
        {
            if (index < 0 || index > 3)
                throw new ArgumentOutOfRangeException(nameof(index));

            return (byte)(address >> ((3 - index) * 8));
        }

        public static DhcpServerIpAddress Empty => new DhcpServerIpAddress(0);
        public static DhcpServerIpAddress FromNative(uint nativeAddress) => new DhcpServerIpAddress(nativeAddress);
        public static DhcpServerIpAddress FromNative(int nativeAddress) => new DhcpServerIpAddress((uint)nativeAddress);
        internal static DhcpServerIpAddress FromNative(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero)
                throw new ArgumentNullException(nameof(pointer));

            return new DhcpServerIpAddress((uint)BitHelper.HostToNetworkOrder(Marshal.ReadInt32(pointer)));
        }

        public static DhcpServerIpAddress FromString(string address) => new DhcpServerIpAddress(address);

        public override string ToString() => BitHelper.IpAddressToString(address);

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

        public override int GetHashCode() => (int)address;

        public bool Equals(DhcpServerIpAddress other) => address == other.address;

        public bool Equals(IPAddress other)
        {
            if (other == null || other.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                return false;

#pragma warning disable CS0618 // Type or member is obsolete
            var otherIp = (uint)other.Address;
#pragma warning restore CS0618 // Type or member is obsolete

            return address == BitHelper.HostToNetworkOrder(otherIp);
        }

        public static bool operator ==(DhcpServerIpAddress lhs, DhcpServerIpAddress rhs) => lhs.Equals(rhs);

        public static bool operator !=(DhcpServerIpAddress lhs, DhcpServerIpAddress rhs) => !lhs.Equals(rhs);

        public static bool operator ==(DhcpServerIpAddress lhs, IPAddress rhs) => lhs.Equals(rhs);

        public static bool operator !=(DhcpServerIpAddress lhs, IPAddress rhs) => !lhs.Equals(rhs);

        internal DHCP_IP_ADDRESS ToNativeAsHost() => new DHCP_IP_ADDRESS(BitHelper.NetworkToHostOrder(address));

        internal DHCP_IP_ADDRESS ToNativeAsNetwork() => new DHCP_IP_ADDRESS(address);

        public static explicit operator uint(DhcpServerIpAddress address) => address.address;
        public static explicit operator DhcpServerIpAddress(uint address) => new DhcpServerIpAddress(address);
        public static explicit operator int(DhcpServerIpAddress address) => (int)address.address;
        public static explicit operator DhcpServerIpAddress(int address) => new DhcpServerIpAddress((uint)address);

        public static implicit operator DhcpServerIpAddress(IPAddress address) => new DhcpServerIpAddress(address);
        public static implicit operator IPAddress(DhcpServerIpAddress address)
            // IPAddress stores in host order; DhcpServerIpAddress always stores in network order
            => new IPAddress(BitHelper.NetworkToHostOrder(address.address));

        public static explicit operator DhcpServerIpMask(DhcpServerIpAddress address) => new DhcpServerIpMask(address.address);
        public static implicit operator string(DhcpServerIpAddress address) => address.ToString();
        public static implicit operator DhcpServerIpAddress(string address) => FromString(address);

        public static bool operator >(DhcpServerIpAddress a, DhcpServerIpAddress b) => a.address > b.address;
        public static bool operator >=(DhcpServerIpAddress a, DhcpServerIpAddress b) => a.address >= b.address;
        public static bool operator <(DhcpServerIpAddress a, DhcpServerIpAddress b) => a.address < b.address;
        public static bool operator <=(DhcpServerIpAddress a, DhcpServerIpAddress b) => a.address <= b.address;
    }
}
