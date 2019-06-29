using System.Collections;
using System.Collections.Generic;

namespace Dhcp
{
    public class DhcpServerScopeExcludedIpRangeCollection : IDhcpServerScopeExcludedIpRangeCollection
    {
        public DhcpServer Server { get; }
        IDhcpServer IDhcpServerScopeExcludedIpRangeCollection.Server => Server;
        public DhcpServerScope Scope { get; }
        IDhcpServerScope IDhcpServerScopeExcludedIpRangeCollection.Scope => Scope;

        internal DhcpServerScopeExcludedIpRangeCollection(DhcpServerScope scope)
        {
            Server = scope.Server;
            Scope = scope;
        }

        /// <summary>
        /// Enumerates a list of excluded IP ranges associated with the DHCP Scope
        /// </summary>
        public IEnumerator<DhcpServerIpRange> GetEnumerator()
            => DhcpServerScope.EnumSubnetElements(Server, Scope.Address, Native.DHCP_SUBNET_ELEMENT_TYPE.DhcpExcludedIpRanges).GetEnumerator();

        /// <summary>
        /// Enumerates a list of excluded IP ranges associated with the DHCP Scope
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public void AddExcludedIpRange(DhcpServerIpRange range)
            => DhcpServerScope.AddSubnetExcludedIpRangeElement(Server, Scope.Address, range);
        public void AddExcludedIpRange(DhcpServerIpAddress startAddress, DhcpServerIpAddress endAddress)
            => DhcpServerScope.AddSubnetExcludedIpRangeElement(Server, Scope.Address, DhcpServerIpRange.AsExcluded(startAddress, endAddress));
        public void AddExcludedIpRange(DhcpServerIpAddress address, DhcpServerIpMask mask)
            => DhcpServerScope.AddSubnetExcludedIpRangeElement(Server, Scope.Address, DhcpServerIpRange.AsExcluded(address, mask));
        public void AddExcludedIpRange(string cidrRange)
            => DhcpServerScope.AddSubnetExcludedIpRangeElement(Server, Scope.Address, DhcpServerIpRange.AsExcluded(cidrRange));

        public void RemoveExcludedIpRange(DhcpServerIpRange range)
            => DhcpServerScope.RemoveSubnetExcludedIpRangeElement(Server, Scope.Address, range);
    }
}
