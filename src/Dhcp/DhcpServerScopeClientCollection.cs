using System;
using System.Collections;
using System.Collections.Generic;

namespace Dhcp
{
    public class DhcpServerScopeClientCollection : IEnumerable<DhcpServerClient>
    {
        public DhcpServer Server { get; }
        public DhcpServerScope Scope { get; }

        internal DhcpServerScopeClientCollection(DhcpServerScope scope)
        {
            Server = scope.Server;
            Scope = scope;
        }

        public IEnumerator<DhcpServerClient> GetEnumerator()
            => DhcpServerClient.GetScopeClients(Scope).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public void RemoveClient(DhcpServerClient client)
            => client.Delete();

        public DhcpServerClient AddClient(DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress)
            => DhcpServerClient.CreateClient(Scope, address, hardwareAddress);
        public DhcpServerClient AddClient(DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name)
            => DhcpServerClient.CreateClient(Scope, address, hardwareAddress, name);

        public DhcpServerClient AddClient(DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name, string comment)
            => DhcpServerClient.CreateClient(Scope, address, hardwareAddress, name, comment);

        public DhcpServerClient AddClient(DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name, string comment, DateTime leaseExpires)
            => DhcpServerClient.CreateClient(Scope, address, hardwareAddress, name, comment, leaseExpires);

        public DhcpServerClient AddClient(DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name, string comment, DateTime leaseExpires, DhcpServerHost ownerHost)
            => DhcpServerClient.CreateClient(Scope, address, hardwareAddress, name, comment, leaseExpires, ownerHost);

        public DhcpServerClient AddClient(DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name, string comment, DateTime leaseExpires, DhcpServerHost ownerHost, DhcpServerClientTypes clientType, DhcpServerClientAddressStates addressState, DhcpServerClientQuarantineStatuses quarantineStatus, DateTime probationEnds, bool quarantineCapable)
            => DhcpServerClient.CreateClient(Scope, address, hardwareAddress, name, comment, leaseExpires, ownerHost, clientType, addressState, quarantineStatus, probationEnds, quarantineCapable);

    }
}
