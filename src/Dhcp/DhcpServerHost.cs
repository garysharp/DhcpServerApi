using System.Net;
using System.Text;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerHost
    {
        private DHCP_IP_ADDRESS ipAddress;

        public IPAddress IpAddress => ipAddress.ToIPAddress();
        public int IpAddressNative => (int)ipAddress;
        public string NetBiosName { get; }
        public string ServerName { get; }

        private DhcpServerHost(DHCP_IP_ADDRESS ipAddress, string netBiosName, string serverName)
        {
            this.ipAddress = ipAddress;
            NetBiosName = netBiosName;
            ServerName = serverName;
        }

        internal static DhcpServerHost FromNative(DHCP_HOST_INFO native) 
            => new DhcpServerHost(native.IpAddress.ToReverseOrder(), native.NetBiosName, native.ServerName);

        public override string ToString()
        {
            var builder = new StringBuilder(ipAddress);

            if (!string.IsNullOrEmpty(ServerName))
                builder.Append(" [").Append(ServerName).Append("]");
            else if (!string.IsNullOrEmpty(NetBiosName))
                builder.Append(" [").Append(NetBiosName).Append("]");

            return builder.ToString();
        }
    }
}
