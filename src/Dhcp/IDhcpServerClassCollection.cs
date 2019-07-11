using System.Collections.Generic;

namespace Dhcp
{
    public interface IDhcpServerClassCollection : IEnumerable<IDhcpServerClass>
    {
        IDhcpServer Server { get; }

        IDhcpServerClass GetClass(string name);
    }
}
