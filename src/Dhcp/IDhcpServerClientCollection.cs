using System.Collections.Generic;

namespace Dhcp
{
    public interface IDhcpServerClientCollection : IEnumerable<IDhcpServerClient>
    {
        IDhcpServer Server { get; }

        void RemoveClient(IDhcpServerClient client);
    }
}
