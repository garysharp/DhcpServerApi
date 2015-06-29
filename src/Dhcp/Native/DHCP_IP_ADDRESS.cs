using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Dhcp.Native
{
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    internal struct DHCP_IP_ADDRESS
    {
        private uint ipAddress;

        public DHCP_IP_ADDRESS ToReverseOrder()
        {
            unchecked
            {
                return new DHCP_IP_ADDRESS()
                {
                    ipAddress = (ipAddress << 24) |
                                ((ipAddress << 8) & 0xFF0000) |
                                ((ipAddress >> 8) & 0xFF00) |
                                (ipAddress >> 24)
                };
            }
        }

        public string ToHostOrderString()
        {
            StringBuilder builder = new StringBuilder();
            unchecked
            {
                builder
                    .Append(ipAddress >> 24).Append('.')
                    .Append((ipAddress >> 16) & 0xFF).Append('.')
                    .Append((ipAddress >> 8) & 0xFF).Append('.')
                    .Append(ipAddress & 0xFF);
            }
            return builder.ToString();
        }

        public string ToNetworkOrderString()
        {
            StringBuilder builder = new StringBuilder();
            unchecked
            {
                builder
                    .Append(ipAddress & 0xFF).Append('.')
                    .Append((ipAddress >> 8) & 0xFF).Append('.')
                    .Append((ipAddress >> 16) & 0xFF).Append('.')
                    .Append(ipAddress >> 24);
            }
            return builder.ToString();
        }

        public IPAddress ToIPAddress()
        {
            return new IPAddress(this.ToReverseOrder().ipAddress);
        }

        public static DHCP_IP_ADDRESS FromIPAddress(IPAddress IpAddress)
        {
            var address = IpAddress.GetAddressBytes();

            if (address.Length != 4)
                throw new ArgumentOutOfRangeException("IpAddress", "Only IPv4 addresses are supported");

            return new DHCP_IP_ADDRESS()
            {
                ipAddress = ((uint)address[0] << 24) |
                            ((uint)address[1] << 16) |
                            ((uint)address[2] << 8) |
                            ((uint)address[3])
            };
        }

        public static DHCP_IP_ADDRESS FromString(string IpAddress)
        {
            if (IpAddress == null)
            {
                throw new ArgumentNullException("IpAddress");
            }

            var match = Regex.Match(IpAddress, @"^(\d{1,3}).(\d{1,3}).(\d{1,3}).(\d{1,3})$");

            if (!match.Success)
            {
                throw new ArgumentOutOfRangeException("IpAddress");
            }

            return new DHCP_IP_ADDRESS()
            {
                ipAddress = (uint.Parse(match.Groups[1].Value) << 24) |
                            (uint.Parse(match.Groups[2].Value) << 16) |
                            (uint.Parse(match.Groups[3].Value) << 8) |
                            (uint.Parse(match.Groups[4].Value))
            };
        }

        /// <summary>
        /// Converts the IP Address to a Host-Order string
        /// </summary>
        public override string ToString()
        {
            return ToHostOrderString();
        }

        public static explicit operator uint(DHCP_IP_ADDRESS ipAddress)
        {
            return ipAddress.ipAddress;
        }

        public static explicit operator DHCP_IP_ADDRESS(uint ipAddress)
        {
            return new DHCP_IP_ADDRESS()
            {
                ipAddress = ipAddress
            };
        }

        public static explicit operator int(DHCP_IP_ADDRESS ipAddress)
        {
            return (int)ipAddress.ipAddress;
        }

        public static explicit operator DHCP_IP_ADDRESS(int ipAddress)
        {
            return new DHCP_IP_ADDRESS()
            {
                ipAddress = (uint)ipAddress
            };
        }

        public static bool operator >(DHCP_IP_ADDRESS a, DHCP_IP_ADDRESS b)
        {
            return a.ipAddress > b.ipAddress;
        }

        public static bool operator >=(DHCP_IP_ADDRESS a, DHCP_IP_ADDRESS b)
        {
            return a.ipAddress >= b.ipAddress;
        }

        public static bool operator <(DHCP_IP_ADDRESS a, DHCP_IP_ADDRESS b)
        {
            return a.ipAddress < b.ipAddress;
        }

        public static bool operator <=(DHCP_IP_ADDRESS a, DHCP_IP_ADDRESS b)
        {
            return a.ipAddress <= b.ipAddress;
        }
    }
}
