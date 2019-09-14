using System;
using System.Collections.Generic;

namespace Dhcp
{
    public interface IDhcpServerScopeCollection : IEnumerable<IDhcpServerScope>
    {
        IDhcpServer Server { get; }

        IDhcpServerScope AddScope(string name, DhcpServerIpRange ipRange);
        IDhcpServerScope AddScope(string name, DhcpServerIpRange ipRange, DhcpServerIpMask mask);
        IDhcpServerScope AddScope(string name, DhcpServerIpRange ipRange, DhcpServerIpMask mask, TimeSpan timeDelayOffer, TimeSpan? leaseDuration);
        IDhcpServerScope AddScope(string name, string comment, DhcpServerIpRange ipRange);
        IDhcpServerScope AddScope(string name, string comment, DhcpServerIpRange ipRange, DhcpServerIpMask mask);
        IDhcpServerScope AddScope(string name, string comment, DhcpServerIpRange ipRange, DhcpServerIpMask mask, TimeSpan timeDelayOffer, TimeSpan? leaseDuration);
        IEnumerable<IDhcpServerScope> GetAllScopes(bool preloadClients, bool preloadFailoverRelationships);
        IDhcpServerScope GetScope(DhcpServerIpAddress scopeAddress);
        void RemoveScope(IDhcpServerScope scope, bool retainClientDnsRecords = false);
    }
}
