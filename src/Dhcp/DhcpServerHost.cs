using Dhcp.Native;
using System.Text;

namespace Dhcp
{
    public class DhcpServerHost
    {
        private DHCP_IP_ADDRESS ipAddress;

        public string IpAddress { get { return ipAddress.ToString(); } }
        public string NetBiosName { get; private set; }
        public string ServerName { get; private set; }

        private DhcpServerHost(DHCP_IP_ADDRESS IpAddress)
        {
            this.ipAddress = IpAddress;
        }

        internal static DhcpServerHost FromNative(DHCP_HOST_INFO Native)
        {
            return new DhcpServerHost(Native.IpAddress)
            {
                NetBiosName = Native.NetBiosName,
                ServerName = Native.ServerName
            };
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(ipAddress.ToString());

            if (!string.IsNullOrEmpty(ServerName))
                builder.Append(" [").Append(ServerName).Append("]");
            else if (!string.IsNullOrEmpty(NetBiosName))
                builder.Append(" [").Append(NetBiosName).Append("]");

            return builder.ToString();
        }
    }
}
