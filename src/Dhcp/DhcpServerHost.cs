using System.Text;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerHost
    {
        private static readonly DhcpServerHost emptyInstance = new DhcpServerHost(DhcpServerIpAddress.Empty, null, null);

        public DhcpServerIpAddress IpAddress { get; }
        public string NetBiosName { get; }
        public string ServerName { get; }

        private DhcpServerHost(DhcpServerIpAddress ipAddress, string netBiosName, string serverName)
        {
            IpAddress = ipAddress;
            NetBiosName = netBiosName;
            ServerName = serverName;
        }

        public static DhcpServerHost Empty => emptyInstance;

        internal static DhcpServerHost FromNative(ref DHCP_HOST_INFO native)
        {
            return new DhcpServerHost(ipAddress: native.IpAddress.AsHostToIpAddress(),
                                      netBiosName: native.NetBiosName,
                                      serverName: native.ServerName);
        }

        internal static DhcpServerHost FromNative(DHCP_HOST_INFO native)
        {
            return new DhcpServerHost(ipAddress: native.IpAddress.AsHostToIpAddress(),
                                      netBiosName: native.NetBiosName,
                                      serverName: native.ServerName);
        }

        internal DHCP_HOST_INFO_Managed ToNative()
            => new DHCP_HOST_INFO_Managed(IpAddress.ToNativeAsNetwork(), NetBiosName, ServerName);

        public override string ToString()
        {
            var builder = new StringBuilder(IpAddress);

            if (!string.IsNullOrEmpty(ServerName))
                builder.Append(" [").Append(ServerName).Append("]");
            else if (!string.IsNullOrEmpty(NetBiosName))
                builder.Append(" [").Append(NetBiosName).Append("]");

            return builder.ToString();
        }
    }
}
