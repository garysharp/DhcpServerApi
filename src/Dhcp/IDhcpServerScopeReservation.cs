namespace Dhcp
{
    public interface IDhcpServerScopeReservation
    {
        DhcpServerIpAddress Address { get; }
        DhcpServerClientTypes AllowedClientTypes { get; }
        IDhcpServerClient Client { get; }
        IDhcpServerDnsSettings DnsSettings { get; }
        DhcpServerHardwareAddress HardwareAddress { get; }
        IDhcpServerScopeReservationOptionValueCollection Options { get; }
        IDhcpServerScope Scope { get; }
        IDhcpServer Server { get; }

        void Delete();
        IDhcpServerDnsSettings ConfigureDnsSettings(IDhcpServerDnsSettings dnsSettings);
    }
}
