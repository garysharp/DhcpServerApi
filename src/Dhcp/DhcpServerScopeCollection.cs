using System;
using System.Collections;
using System.Collections.Generic;

namespace Dhcp
{
    public class DhcpServerScopeCollection : IEnumerable<DhcpServerScope>
    {
        public DhcpServer Server { get; }

        internal DhcpServerScopeCollection(DhcpServer server)
        {
            Server = server;
        }

        public IEnumerator<DhcpServerScope> GetEnumerator()
            => DhcpServerScope.GetScopes(Server).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public DhcpServerScope GetScope(DhcpServerIpAddress scopeAddress)
            => DhcpServerScope.GetScope(Server, scopeAddress);

        public DhcpServerScope CreateScope(string name, string description, DhcpServerIpRange ipRange, DhcpServerIpMask mask, bool enable)
            => DhcpServerScope.CreateScope(Server, name, description, ipRange, mask, enable);

        public DhcpServerScope CreateScope(string name, string description, DhcpServerIpRange ipRange, DhcpServerIpMask mask, IEnumerable<DhcpServerIpRange> excludedRanges, TimeSpan timeDelayOffer, TimeSpan? leaseDuration, bool enable)
            => DhcpServerScope.CreateScope(Server, name, description, ipRange, mask, excludedRanges, timeDelayOffer, leaseDuration, enable);
    }
}
