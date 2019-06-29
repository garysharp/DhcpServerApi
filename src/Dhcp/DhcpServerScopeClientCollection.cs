using System;
using System.Collections;
using System.Collections.Generic;

namespace Dhcp
{
    public class DhcpServerScopeClientCollection : IDhcpServerScopeClientCollection
    {
        public DhcpServer Server { get; }
        IDhcpServer IDhcpServerScopeClientCollection.Server => Server;
        public DhcpServerScope Scope { get; }
        IDhcpServerScope IDhcpServerScopeClientCollection.Scope => Scope;

        internal DhcpServerScopeClientCollection(DhcpServerScope scope)
        {
            Server = scope.Server;
            Scope = scope;
        }

        public IEnumerator<IDhcpServerClient> GetEnumerator()
            => DhcpServerClient.GetScopeClients(Scope).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public void RemoveClient(IDhcpServerClient client)
            => client.Delete();

        public IDhcpServerClient AddClient(DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress)
            => DhcpServerClient.CreateClient(Scope, address, hardwareAddress);
        public IDhcpServerClient AddClient(DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name)
            => DhcpServerClient.CreateClient(Scope, address, hardwareAddress, name);

        public IDhcpServerClient AddClient(DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name, string comment)
            => DhcpServerClient.CreateClient(Scope, address, hardwareAddress, name, comment);

        public IDhcpServerClient AddClient(DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name, string comment, DateTime leaseExpires)
            => DhcpServerClient.CreateClient(Scope, address, hardwareAddress, name, comment, leaseExpires);

        public IDhcpServerClient AddClient(DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name, string comment, DateTime leaseExpires, IDhcpServerHost ownerHost)
            => DhcpServerClient.CreateClient(Scope, address, hardwareAddress, name, comment, leaseExpires, (DhcpServerHost)ownerHost);

        public IDhcpServerClient AddClient(DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name, string comment, DateTime leaseExpires, IDhcpServerHost ownerHost, DhcpServerClientTypes clientType, DhcpServerClientAddressStates addressState, DhcpServerClientQuarantineStatuses quarantineStatus, DateTime probationEnds, bool quarantineCapable)
            => DhcpServerClient.CreateClient(Scope, address, hardwareAddress, name, comment, leaseExpires, (DhcpServerHost)ownerHost, clientType, addressState, quarantineStatus, probationEnds, quarantineCapable);

    }
}
