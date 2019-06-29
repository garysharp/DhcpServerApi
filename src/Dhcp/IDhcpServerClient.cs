using System;

namespace Dhcp
{
    public interface IDhcpServerClient
    {
        DhcpServerClientAddressStates AddressState { get; }
        string Comment { get; set; }
        DhcpServerClientDnsStates DnsState { get; }
        DhcpServerHardwareAddress HardwareAddress { get; }
        DhcpServerIpAddress IpAddress { get; }
        bool LeaseExpired { get; }
        DateTime LeaseExpires { get; }
        DateTime LeaseExpiresLocal { get; }
        DateTime LeaseExpiresUtc { get; }
        bool LeaseHasExpiry { get; }
        string Name { get; set; }
        DhcpServerClientNameProtectionStates NameProtectionState { get; }
        DateTime ProbationEnds { get; }
        bool QuarantineCapable { get; }
        DhcpServerClientQuarantineStatuses QuarantineStatus { get; }
        IDhcpServerScope Scope { get; }
        IDhcpServer Server { get; }
        DhcpServerIpMask SubnetMask { get; }
        DhcpServerClientTypes Type { get; }

        IDhcpServerScopeReservation ConvertToReservation();
        void Delete();
        string ToString();
    }
}
