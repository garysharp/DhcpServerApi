using System.Text;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerHost : IDhcpServerHost
    {
        public DhcpServerIpAddress Address { get; }
        public string NetBiosName { get; }
        public string ServerName { get; }

        private DhcpServerHost(DhcpServerIpAddress address, string netBiosName, string serverName)
        {
            Address = address;
            NetBiosName = netBiosName;
            ServerName = serverName;
        }

        public static DhcpServerHost Empty { get; } = new DhcpServerHost(DhcpServerIpAddress.Empty, null, null);

        internal static DhcpServerHost FromNative(ref DHCP_HOST_INFO native)
        {
            return new DhcpServerHost(address: native.IpAddress.AsHostToIpAddress(),
                                      netBiosName: native.NetBiosName,
                                      serverName: native.ServerName);
        }

        internal static DhcpServerHost FromNative(DHCP_HOST_INFO native)
        {
            return new DhcpServerHost(address: native.IpAddress.AsHostToIpAddress(),
                                      netBiosName: native.NetBiosName,
                                      serverName: native.ServerName);
        }

        internal DHCP_HOST_INFO_Managed ToNative()
            => new DHCP_HOST_INFO_Managed(Address.ToNativeAsNetwork(), NetBiosName, ServerName);

        public override string ToString()
        {
            var builder = new StringBuilder(Address);

            if (!string.IsNullOrEmpty(ServerName))
                builder.Append(" [").Append(ServerName).Append("]");
            else if (!string.IsNullOrEmpty(NetBiosName))
                builder.Append(" [").Append(NetBiosName).Append("]");

            return builder.ToString();
        }
    }
}
