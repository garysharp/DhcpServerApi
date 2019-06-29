using System;

namespace Dhcp
{
    public interface IDhcpServerScope
    {
        DhcpServerIpAddress Address { get; }
        IDhcpServerScopeClientCollection Clients { get; }
        string Comment { get; set; }
        IDhcpServerDnsSettings DnsSettings { get; }
        IDhcpServerScopeExcludedIpRangeCollection ExcludedIpRanges { get; }
        DhcpServerIpRange IpRange { get; set; }
        TimeSpan? LeaseDuration { get; set; }
        DhcpServerIpMask Mask { get; }
        string Name { get; set; }
        IDhcpServerScopeOptionValueCollection Options { get; }
        IDhcpServerHost PrimaryHost { get; }
        bool QuarantineOn { get; }
        IDhcpServerScopeReservationCollection Reservations { get; }
        IDhcpServer Server { get; }
        DhcpServerScopeState State { get; }
        TimeSpan TimeDelayOffer { get; set; }

        void Activate();
        IDhcpServerFailoverRelationship ConfigureFailover(IDhcpServer partnerServer, string sharedSecret, DhcpServerFailoverMode mode);
        IDhcpServerFailoverRelationship ConfigureFailover(IDhcpServer partnerServer, string sharedSecret, DhcpServerFailoverMode mode, byte modePercentage);
        IDhcpServerFailoverRelationship ConfigureFailover(IDhcpServer partnerServer, string sharedSecret, DhcpServerFailoverMode mode, byte modePercentage, TimeSpan maximumClientLeadTime, TimeSpan? stateSwitchInterval);
        IDhcpServerFailoverRelationship ConfigureFailover(IDhcpServer partnerServer, string name, string sharedSecret, DhcpServerFailoverMode mode);
        IDhcpServerFailoverRelationship ConfigureFailover(IDhcpServer partnerServer, string name, string sharedSecret, DhcpServerFailoverMode mode, byte modePercentage);
        IDhcpServerFailoverRelationship ConfigureFailover(IDhcpServer partnerServer, string name, string sharedSecret, DhcpServerFailoverMode mode, byte modePercentage, TimeSpan maximumClientLeadTime, TimeSpan? stateSwitchInterval);
        void ConfigureFailover(IDhcpServerFailoverRelationship failoverRelationship);
        void Deactivate();
        void DeconfigureFailover();
        void Delete(bool retainClientDnsRecords = false);
        IDhcpServerFailoverRelationship GetFailoverRelationship();
        IDhcpServerScopeFailoverStatistics GetFailoverStatistics();
        void ReplicateFailoverPartner();
    }
}
