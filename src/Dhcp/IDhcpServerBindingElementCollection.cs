using System.Collections.Generic;

namespace Dhcp
{
    public interface IDhcpServerBindingElementCollection : IEnumerable<IDhcpServerBindingElement>
    {
        IDhcpServer Server { get; }
    }
}
