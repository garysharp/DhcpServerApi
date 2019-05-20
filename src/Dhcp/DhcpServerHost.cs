using System;
using System.ComponentModel;
using System.Text;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerHost
    {
        public DhcpServerIpAddress IpAddress { get; }
        [Obsolete("Use IpAddress.Native instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public int IpAddressNative => (int)IpAddress.Native;

        public string NetBiosName { get; }
        public string ServerName { get; }

        private DhcpServerHost(DhcpServerIpAddress ipAddress, string netBiosName, string serverName)
        {
            IpAddress = ipAddress;
            NetBiosName = netBiosName;
            ServerName = serverName;
        }

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
