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

        public DhcpServerScope CreateScope(string name, string description, DhcpServerIpRange ipRange)
            => DhcpServerScope.CreateScope(Server, name, description, ipRange);
        public DhcpServerScope CreateScope(string name, string description, DhcpServerIpRange ipRange, DhcpServerIpMask mask)
            => DhcpServerScope.CreateScope(Server, name, description, ipRange, mask);
        public DhcpServerScope CreateScope(string name, string description, DhcpServerIpRange ipRange, DhcpServerIpMask mask, TimeSpan timeDelayOffer, TimeSpan? leaseDuration)
            => DhcpServerScope.CreateScope(Server, name, description, ipRange, mask, timeDelayOffer, leaseDuration);

        /// <summary>
        /// Deletes the specified scope
        /// </summary>
        /// <param name="scope">The scope to be deleted</param>
        /// <param name="retainClientDnsRecords">If true registered client DNS records are not removed. Useful in failover scenarios. Default = false</param>
        public void DeleteScope(DhcpServerScope scope, bool retainClientDnsRecords = false)
            => scope.Delete(retainClientDnsRecords);

    }
}
