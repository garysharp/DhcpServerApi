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
                    m >>= 16;

                if ((m & 0xFF00) == 0xFF00)
                    sb += 8;
                else
                    m >>= 8;

                if ((m & 0xF0) == 0xF0)
                    sb += 4;
                else
                    m >>= 4;

                if ((m & 0xC) == 0xC)
                    sb += 2;
                else
                    m >>= 2;

                if ((m & 2) == 2)
                    sb += 1;
                else
                    m >>= 1;

                if ((m & 1) == 1)
                    sb += 1;

                return sb;
            }
        }

        public DHCP_IP_MASK ToReverseOrder()
        {
            return new DHCP_IP_MASK()
            {
                ipMask = ((ipMask << 24) & 0xFF00_0000) |
                        ((ipMask << 08) & 0x00FF_0000) |
                        ((ipMask >> 08) & 0x0000_FF00) |
                        ((ipMask >> 24) & 0x0000_00FF)
            };
        }

        public string ToHostOrderString()
        {
            var builder = new StringBuilder(15);
            builder.Append((ipMask >> 24) & 0xFF);
            builder.Append('.');
            builder.Append((ipMask >> 16) & 0xFF);
            builder.Append('.');
            builder.Append((ipMask >> 8) & 0xFF);
            builder.Append('.');
            builder.Append(ipMask & 0xFF);
            return builder.ToString();
        }

        public string ToNetworkOrderString()
        {
            var builder = new StringBuilder(15);
            builder.Append(ipMask & 0xFF);
            builder.Append('.');
            builder.Append((ipMask >> 8) & 0xFF);
            builder.Append('.');
            builder.Append((ipMask >> 16) & 0xFF);
            builder.Append('.');
            builder.Append((ipMask >> 24) & 0xFF);
            return builder.ToString();
        }

        public IPAddress ToIPAddress() => new IPAddress(ToReverseOrder().ipMask);

        /// <summary>
        /// Converts the IP Mask to a Host-Order string
        /// </summary>
        public override string ToString() => ToHostOrderString();

        public static explicit operator uint(DHCP_IP_MASK ipMask) => ipMask.ipMask;

        public static explicit operator DHCP_IP_MASK(uint ipMask)
        {
            return new DHCP_IP_MASK()
            {
                ipMask = ipMask
            };
        }

        public static explicit operator int(DHCP_IP_MASK ipMask) => (int)ipMask.ipMask;

        public static explicit operator DHCP_IP_MASK(int ipMask)
        {
            return new DHCP_IP_MASK()
            {
                ipMask = (uint)ipMask
            };
        }

        public static explicit operator DHCP_IP_ADDRESS(DHCP_IP_MASK ipMask) => (DHCP_IP_ADDRESS)ipMask.ipMask;

        public static explicit operator DHCP_IP_MASK(DHCP_IP_ADDRESS ipMask) => (DHCP_IP_MASK)(uint)ipMask;
    }
}
