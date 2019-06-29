using System.Collections.Generic;

namespace Dhcp
{
    public interface IDhcpServerFailoverRelationshipCollection : IEnumerable<IDhcpServerFailoverRelationship>
    {
        IDhcpServer Server { get; }

        IDhcpServerFailoverRelationship GetRelationship(string relationshipName);
        void RemoveRelationship(IDhcpServerFailoverRelationship relationship);
    }
}
