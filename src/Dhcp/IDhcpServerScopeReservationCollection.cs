using System.Collections.Generic;

namespace Dhcp
{
    public interface IDhcpServerScopeReservationCollection : IEnumerable<IDhcpServerScopeReservation>
    {
        IDhcpServerScope Scope { get; }
        IDhcpServer Server { get; }

        IDhcpServerScopeReservation AddReservation(IDhcpServerClient client);
        IDhcpServerScopeReservation AddReservation(DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress);
        IDhcpServerScopeReservation AddReservation(DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, DhcpServerClientTypes allowedClientTypes);
    }
}
