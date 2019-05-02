using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Dhcp.Native
{
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    internal struct DHCP_IP_ADDRESS
    {
        private uint ipAddress;

        public DHCP_IP_ADDRESS ToReverseOrder()
        {
            return new DHCP_IP_ADDRESS()
            {
                ipAddress = ((ipAddress << 24) & 0xFF00_0000) |
                            ((ipAddress << 08) & 0x00FF_0000) |
                            ((ipAddress >> 08) & 0x0000_FF00) |
                            ((ipAddress >> 24) & 0x0000_00FF)
            };
        }

        public string ToHostOrderString()
        {
            var builder = new StringBuilder(15);
            builder.Append((ipAddress >> 24) & 0xFF);
            builder.Append('.');
            builder.Append((ipAddress >> 16) & 0xFF);
            builder.Append('.');
            builder.Append((ipAddress >> 8) & 0xFF);
            builder.Append('.');
            builder.Append(ipAddress & 0xFF);
            return builder.ToString();
        }

        public string ToNetworkOrderString()
        {
            var builder = new StringBuilder(15);
            builder.Append(ipAddress & 0xFF);
            builder.Append('.');
            builder.Append((ipAddress >> 8) & 0xFF);
            builder.Append('.');
            builder.Append((ipAddress >> 16) & 0xFF);
            builder.Append('.');
            builder.Append((ipAddress >> 24) & 0xFF);
            return builder.ToString();
        }

        public IPAddress ToIPAddress() => new IPAddress(ToReverseOrder().ipAddress);

        public static DHCP_IP_ADDRESS FromIPAddress(IPAddress ipAddress)
        {
            var address = ipAddress.GetAddressBytes();

            if (address.Length != 4)
                throw new ArgumentOutOfRangeException(nameof(ipAddress), "Only IPv4 addresses are supported");

            return new DHCP_IP_ADDRESS()
            {
                ipAddress = ((uint)address[0] << 24) |
                            ((uint)address[1] << 16) |
                            ((uint)address[2] << 08) |
                            address[3]
            };
        }

        public static DHCP_IP_ADDRESS FromString(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                throw new ArgumentNullException(nameof(ipAddress));

            var address = 0U;
            int index = 0,
                nextIndex,
                octetCount = 0;

            while (index < ipAddress.Length && ((nextIndex = ipAddress.IndexOf('.', index)) >= 0 || octetCount == 3))
            {
                // handle remainder (last octet)
                if (octetCount == 3 && nextIndex == -1)
                    nextIndex = ipAddress.Length;

                // ensure octet != 0
                if (nextIndex == index)
                    throw new ArgumentOutOfRangeException(nameof(ipAddress), "Invalid IP Address format (empty octet)");

                // parse octet to int (without string allocation)
                if (!Helpers.TryParseByteFromSubstring(ipAddress, index, nextIndex - index, out var octet))
                    throw new ArgumentOutOfRangeException(nameof(ipAddress), "Invalid IP Address format (invalid octet)");

                // shift octet onto address int
                address |= (uint)octet << ((3 - octetCount++) * 8);

                // move index forward
                index = nextIndex + 1;

                // ensure no more than 4 octets
                if (octetCount > 4)
                    throw new ArgumentOutOfRangeException(nameof(ipAddress), "Invalid IP Address format (too many octets)");
            }
            // ensure no remaining characters
            if (index < ipAddress.Length)
                throw new ArgumentOutOfRangeException(nameof(ipAddress), "Invalid IP Address format (too many characters)");
            // ensure no less than 4 octets
            if (octetCount != 4)
                throw new ArgumentOutOfRangeException(nameof(ipAddress), "Invalid IP Address format (too few octets)");

            return new DHCP_IP_ADDRESS()
            {
                ipAddress = address
            };
        }

        /// <summary>
        /// Converts the IP Address to a Host-Order string
        /// </summary>
        public override string ToString() => this;

        public static explicit operator uint(DHCP_IP_ADDRESS ipAddress) => ipAddress.ipAddress;
        public static explicit operator DHCP_IP_ADDRESS(uint ipAddress) => new DHCP_IP_ADDRESS() { ipAddress = ipAddress };
        public static explicit operator int(DHCP_IP_ADDRESS ipAddress) => (int)ipAddress.ipAddress;
        public static explicit operator DHCP_IP_ADDRESS(int ipAddress) => new DHCP_IP_ADDRESS() { ipAddress = (uint)ipAddress };

        public static implicit operator string(DHCP_IP_ADDRESS ipAddress) => ipAddress.ToHostOrderString();

        public static bool operator >(DHCP_IP_ADDRESS a, DHCP_IP_ADDRESS b) => a.ipAddress > b.ipAddress;
        public static bool operator >=(DHCP_IP_ADDRESS a, DHCP_IP_ADDRESS b) => a.ipAddress >= b.ipAddress;
        public static bool operator <(DHCP_IP_ADDRESS a, DHCP_IP_ADDRESS b) => a.ipAddress < b.ipAddress;
        public static bool operator <=(DHCP_IP_ADDRESS a, DHCP_IP_ADDRESS b) => a.ipAddress <= b.ipAddress;
    }
}
