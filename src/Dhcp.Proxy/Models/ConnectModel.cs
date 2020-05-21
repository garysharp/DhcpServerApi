namespace Dhcp.Proxy.Models
{
    public class ConnectModel
    {
        public DhcpServerIpAddress Address { get; set; }
        public string Name { get; set; }
        public int VersionMajor { get; set; }
        public int VersionMinor { get; set; }

        public static ConnectModel FromDhcpServer(IDhcpServer dhcpServer)
        {
            return new ConnectModel()
            {
                Address = dhcpServer.Address,
                Name = dhcpServer.Name,
                VersionMajor = dhcpServer.VersionMajor,
                VersionMinor = dhcpServer.VersionMinor,
            };
        }

    }
}
