using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Dhcp.Native
{
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    internal struct DHCP_IP_MASK
    {
        private uint ipMask;

        public int SignificantBits
        {
            get
            {
                var m = ipMask;
                var sb = 0;

                if ((m & 0xFFFF0000) == 0xFFFF0000)
                    sb = 16;
                else
                    m = m >> 16;

                if ((m & 0xFF00) == 0xFF00)
                    sb += 8;
                else
                    m = m >> 8;

                if ((m & 0xF0) == 0xF0)
                    sb += 4;
                else
                    m = m >> 4;

                if ((m & 0xC) == 0xC)
                    sb += 2;
                else
                    m = m >> 2;

                if ((m & 2) == 2)
                    sb += 1;
                else
                    m = m >> 1;

                if ((m & 1) == 1)
                    sb += 1;

                return sb;
            }
        }

        public DHCP_IP_MASK ToReverseOrder()
        {
            unchecked
            {
                return new DHCP_IP_MASK()
                {
                    ipMask = (ipMask << 24) |
                            ((ipMask << 8) & 0xFF0000) |
                            ((ipMask >> 8) & 0xFF00) |
                            (ipMask >> 24)
                };
            }
        }

        public string ToHostOrderString()
        {
            StringBuilder builder = new StringBuilder();
            unchecked
            {
                builder
                    .Append(ipMask >> 24).Append('.')
                    .Append((ipMask >> 16) & 0xFF).Append('.')
                    .Append((ipMask >> 8) & 0xFF).Append('.')
                    .Append(ipMask & 0xFF);
            }
            return builder.ToString();
        }

        public string ToNetworkOrderString()
        {
            StringBuilder builder = new StringBuilder();
            unchecked
            {
                builder
                    .Append(ipMask & 0xFF).Append('.')
                    .Append((ipMask >> 8) & 0xFF).Append('.')
                    .Append((ipMask >> 16) & 0xFF).Append('.')
                    .Append(ipMask >> 24);
            }
            return builder.ToString();
        }

        public IPAddress ToIPAddress()
        {
            return new IPAddress(this.ToReverseOrder().ipMask);
        }

        /// <summary>
        /// Converts the IP Mask to a Host-Order string
        /// </summary>
        public override string ToString()
        {
            return ToHostOrderString();
        }

        public static explicit operator uint(DHCP_IP_MASK ipMask)
        {
            return ipMask.ipMask;
        }

        public static explicit operator DHCP_IP_MASK(uint ipMask)
        {
            return new DHCP_IP_MASK()
            {
                ipMask = ipMask
            };
        }

        public static explicit operator int(DHCP_IP_MASK ipMask)
        {
            return (int)ipMask.ipMask;
        }

        public static explicit operator DHCP_IP_MASK(int ipMask)
        {
            return new DHCP_IP_MASK()
            {
                ipMask = (uint)ipMask
            };
        }

        public static explicit operator DHCP_IP_ADDRESS(DHCP_IP_MASK ipMask)
        {
            return (DHCP_IP_ADDRESS)ipMask.ipMask;
        }

        public static explicit operator DHCP_IP_MASK(DHCP_IP_ADDRESS ipMask)
        {
            return (DHCP_IP_MASK)(uint)ipMask;
        }
    }
}
