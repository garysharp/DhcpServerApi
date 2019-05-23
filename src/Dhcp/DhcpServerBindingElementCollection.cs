using System.Collections;
using System.Collections.Generic;

namespace Dhcp
{
    public class DhcpServerBindingElementCollection : IEnumerable<DhcpServerBindingElement>
    {
        public DhcpServer Server { get; }

        internal DhcpServerBindingElementCollection(DhcpServer server)
        {
            Server = server;
        }

        public IEnumerator<DhcpServerBindingElement> GetEnumerator()
            => DhcpServerBindingElement.GetBindingInfo(Server).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
