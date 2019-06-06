using System.Collections;
using System.Collections.Generic;

namespace Dhcp
{
    public class DhcpServerFailoverRelationshipCollection : IEnumerable<DhcpServerFailoverRelationship>
    {
        public DhcpServer Server { get; }

        internal DhcpServerFailoverRelationshipCollection(DhcpServer server)
        {
            Server = server;
        }

        public DhcpServerFailoverRelationship GetRelationship(string relationshipName)
            => DhcpServerFailoverRelationship.GetFailoverRelationship(Server, relationshipName);

        public IEnumerator<DhcpServerFailoverRelationship> GetEnumerator()
            => DhcpServerFailoverRelationship.GetFailoverRelationships(Server).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
