using System;

namespace Dhcp
{
    public interface IDhcpServer : IDisposable
    {
        DhcpServerIpAddress Address { get; }
        IDhcpServerAuditLog AuditLog { get; }
        IDhcpServerBindingElementCollection BindingElements { get; }
        IDhcpServerClassCollection Classes { get; }
        IDhcpServerClientCollection Clients { get; }
        IDhcpServerConfiguration Configuration { get; }
        IDhcpServerDnsSettings DnsSettings { get; }
        IDhcpServerFailoverRelationshipCollection FailoverRelationships { get; }
        string Name { get; }
        IDhcpServerOptionCollection Options { get; }
        IDhcpServerScopeCollection Scopes { get; }
        IDhcpServerSpecificStrings SpecificStrings { get; }
        DhcpServerVersions Version { get; }
        int VersionMajor { get; }
        int VersionMinor { get; }

        bool IsCompatible(DhcpServerVersions version);
        IDhcpServerDnsSettings ConfigureDnsSettings(IDhcpServerDnsSettings dnsSettings);
    }
}
