using System.Collections;
using System.Collections.Generic;

namespace Dhcp
{
    public class DhcpServerBindingElementCollection : IDhcpServerBindingElementCollection
    {
        public DhcpServer Server { get; }
        IDhcpServer IDhcpServerBindingElementCollection.Server => Server;

        internal DhcpServerBindingElementCollection(DhcpServer server)
        {
            Server = server;
        }

        public IEnumerator<IDhcpServerBindingElement> GetEnumerator()
            => DhcpServerBindingElement.GetBindingInfo(Server).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
