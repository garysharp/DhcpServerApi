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

        /// <summary>
        /// Enumerates a list of scopes associated with the DHCP server
        /// </summary>
        public IEnumerator<DhcpServerScope> GetEnumerator()
            => DhcpServerScope.GetScopes(Server).GetEnumerator();

        /// <summary>
        /// Enumerates a list of scopes associated with the DHCP server
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Gets the DHCP scope with the associated scope address
        /// </summary>
        /// <param name="scopeAddress"></param>
        /// <returns></returns>
        public DhcpServerScope GetScope(DhcpServerIpAddress scopeAddress)
            => DhcpServerScope.GetScope(Server, scopeAddress);

        /// <summary>
        /// Creates a new DHCP Scope
        /// </summary>
        /// <param name="name">Name of the DHCP Scope</param>
        /// <param name="ipRange">IP Range to be associated with the DHCP Scope from which the scope mask is inferred</param>
        /// <returns>Newly created DHCP Scope</returns>
        public DhcpServerScope AddScope(string name, DhcpServerIpRange ipRange)
            => DhcpServerScope.CreateScope(Server, name, ipRange);
        /// <summary>
        /// Creates a new DHCP Scope
        /// </summary>
        /// <param name="name">Name of the DHCP Scope</param>
        /// <param name="comment">Comment for the DHCP Scope</param>
        /// <param name="ipRange">IP Range to be associated with the DHCP Scope from which the scope mask is inferred</param>
        /// <returns>Newly created DHCP Scope</returns>
        public DhcpServerScope AddScope(string name, string comment, DhcpServerIpRange ipRange)
            => DhcpServerScope.CreateScope(Server, name, comment, ipRange);
        /// <summary>
        /// Creates a new DHCP Scope
        /// </summary>
        /// <param name="name">Name of the DHCP Scope</param>
        /// <param name="ipRange">IP Range to be associated with the DHCP Scope</param>
        /// <param name="mask">Subnet mask to be associated with the DHCP Scope</param>
        /// <returns>Newly created DHCP Scope</returns>
        public DhcpServerScope AddScope(string name, DhcpServerIpRange ipRange, DhcpServerIpMask mask)
            => DhcpServerScope.CreateScope(Server, name, ipRange, mask);
        /// <summary>
        /// Creates a new DHCP Scope
        /// </summary>
        /// <param name="name">Name of the DHCP Scope</param>
        /// <param name="comment">Comment for the DHCP Scope</param>
        /// <param name="ipRange">IP Range to be associated with the DHCP Scope</param>
        /// <param name="mask">Subnet mask to be associated with the DHCP Scope</param>
        /// <returns>Newly created DHCP Scope</returns>
        public DhcpServerScope AddScope(string name, string comment, DhcpServerIpRange ipRange, DhcpServerIpMask mask)
            => DhcpServerScope.CreateScope(Server, name, comment, ipRange, mask);
        /// <summary>
        /// Creates a new DHCP Scope
        /// </summary>
        /// <param name="name">Name of the DHCP Scope</param>
        /// <param name="ipRange">IP Range to be associated with the DHCP Scope</param>
        /// <param name="mask">Subnet mask to be associated with the DHCP Scope</param>
        /// <param name="timeDelayOffer">Milliseconds to wait before sending a lease offer (maximum 1 second)</param>
        /// <param name="leaseDuration">Number of seconds the lease is held for</param>
        /// <returns></returns>
        public DhcpServerScope AddScope(string name, DhcpServerIpRange ipRange, DhcpServerIpMask mask, TimeSpan timeDelayOffer, TimeSpan? leaseDuration)
            => DhcpServerScope.CreateScope(Server, name, ipRange, mask, timeDelayOffer, leaseDuration);
        /// <summary>
        /// Creates a new DHCP Scope
        /// </summary>
        /// <param name="name">Name of the DHCP Scope</param>
        /// <param name="comment">Comment for the DHCP Scope</param>
        /// <param name="ipRange">IP Range to be associated with the DHCP Scope</param>
        /// <param name="mask">Subnet mask to be associated with the DHCP Scope</param>
        /// <param name="timeDelayOffer">Milliseconds to wait before sending a lease offer (maximum 1 second)</param>
        /// <param name="leaseDuration">Number of seconds the lease is held for</param>
        /// <returns></returns>
        public DhcpServerScope AddScope(string name, string comment, DhcpServerIpRange ipRange, DhcpServerIpMask mask, TimeSpan timeDelayOffer, TimeSpan? leaseDuration)
            => DhcpServerScope.CreateScope(Server, name, comment, ipRange, mask, timeDelayOffer, leaseDuration);

        /// <summary>
        /// Deletes the specified scope
        /// </summary>
        /// <param name="scope">The scope to be deleted</param>
        /// <param name="retainClientDnsRecords">If true registered client DNS records are not removed. Useful in failover scenarios. Default = false</param>
        public void RemoveScope(DhcpServerScope scope, bool retainClientDnsRecords = false)
            => scope.Delete(retainClientDnsRecords);

    }
}
