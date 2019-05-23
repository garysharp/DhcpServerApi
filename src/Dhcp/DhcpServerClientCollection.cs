using System.Collections;
using System.Collections.Generic;

namespace Dhcp
{
    public class DhcpServerClientCollection : IEnumerable<DhcpServerClient>
    {
        public DhcpServer Server { get; }

        internal DhcpServerClientCollection(DhcpServer server)
        {
            Server = server;
        }

        public IEnumerator<DhcpServerClient> GetEnumerator()
            => DhcpServerClient.GetClients(Server).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public void RemoveClient(DhcpServerClient client)
            => client.Delete();
    }
}
