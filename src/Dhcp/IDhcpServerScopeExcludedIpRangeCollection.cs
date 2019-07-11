using System.Collections.Generic;

namespace Dhcp
{
    public interface IDhcpServerScopeExcludedIpRangeCollection : IEnumerable<DhcpServerIpRange>
    {
        IDhcpServerScope Scope { get; }
        IDhcpServer Server { get; }

        void AddExcludedIpRange(DhcpServerIpAddress startAddress, DhcpServerIpAddress endAddress);
        void AddExcludedIpRange(DhcpServerIpAddress address, DhcpServerIpMask mask);
        void AddExcludedIpRange(DhcpServerIpRange range);
        void AddExcludedIpRange(string cidrRange);
        void RemoveExcludedIpRange(DhcpServerIpRange range);
    }
}
