using System.Collections;
using System.Collections.Generic;

namespace Dhcp
{
    public class DhcpServerClassCollection : IDhcpServerClassCollection
    {
        public DhcpServer Server { get; }
        IDhcpServer IDhcpServerClassCollection.Server => Server;

        internal DhcpServerClassCollection(DhcpServer server)
        {
            Server = server;
        }

        /// <summary>
        /// Queries the DHCP Server for the specified User or Vendor Class
        /// </summary>
        /// <param name="name">The name of the User or Vendor Class to retrieve</param>
        /// <returns>A <see cref="DhcpServerClass"/>.</returns>
        public IDhcpServerClass GetClass(string name)
            => DhcpServerClass.GetClass(Server, name);

        public IEnumerator<IDhcpServerClass> GetEnumerator()
            => DhcpServerClass.GetClasses(Server).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
