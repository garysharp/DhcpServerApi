using System;
using System.Collections.Generic;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerScopeReservation
    {
        public DhcpServer Server => Scope.Server;
        public DhcpServerScope Scope { get; }

        private DhcpServerClient client;
        public DhcpServerIpAddress Address { get; }
        public DhcpServerHardwareAddress HardwareAddress { get; }

        public DhcpServerClientTypes AllowedClientTypes { get; }

        public DhcpServerClient Client => client ??= DhcpServerClient.GetClient(Server, Scope, Address);

        public DhcpServerScopeReservationOptionValueCollection Options { get; }

        private DhcpServerScopeReservation(DhcpServerScope scope, DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, DhcpServerClientTypes allowedClientTypes)
        {
            Scope = scope;
            Address = address;
            HardwareAddress = hardwareAddress;
            AllowedClientTypes = allowedClientTypes;

            Options = new DhcpServerScopeReservationOptionValueCollection(this);
        }

        internal static DhcpServerScopeReservation CreateReservation(DhcpServerScope scope, DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress)
            => CreateReservation(scope, address, hardwareAddress, DhcpServerClientTypes.DhcpAndBootp);
        internal static DhcpServerScopeReservation CreateReservation(DhcpServerScope scope, DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, DhcpServerClientTypes allowedClientTypes)
        {
            if (!scope.IpRange.Contains(address))
                throw new ArgumentOutOfRangeException(nameof(address), "The DHCP scope does not include the provided address");

            DhcpServerScope.AddSubnetReservationElement(scope.Server, scope.Address, address, hardwareAddress, allowedClientTypes);

            return new DhcpServerScopeReservation(scope, address, hardwareAddress, allowedClientTypes);
        }

        internal static IEnumerable<DhcpServerScopeReservation> GetReservations(DhcpServerScope scope)
        {
            var resumeHandle = IntPtr.Zero;
            var result = Api.DhcpEnumSubnetElementsV5(ServerIpAddress: scope.Server.Address,
                                                      SubnetAddress: scope.Address.ToNativeAsNetwork(),
                                                      EnumElementType: DHCP_SUBNET_ELEMENT_TYPE.DhcpReservedIps,
                                                      ResumeHandle: ref resumeHandle,
                                                      PreferredMaximum: 0xFFFFFFFF,
                                                      EnumElementInfo: out var reservationsPtr,
                                                      ElementsRead: out var elementsRead,
                                                      ElementsTotal: out _);

            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS || result == DhcpErrors.EPT_S_NOT_REGISTERED)
                yield break;

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                throw new DhcpServerException(nameof(Api.DhcpEnumSubnetElementsV5), result);

            try
            {
                if (elementsRead == 0)
                    yield break;

                using (var reservations = reservationsPtr.MarshalToStructure<DHCP_SUBNET_ELEMENT_INFO_ARRAY_V5>())
                {
                    foreach (var element in reservations.Elements)
                    {
                        var elementIp = element.ReadReservedIp();
                        yield return FromNative(scope, ref elementIp);
                    }
                }
            }
            finally
            {
                Api.FreePointer(reservationsPtr);
            }
        }

        private static DhcpServerScopeReservation FromNative(DhcpServerScope scope, ref DHCP_IP_RESERVATION_V4 native)
        {
            var reservedForClient = native.ReservedForClient;

            return new DhcpServerScopeReservation(scope: scope,
                                                  address: native.ReservedIpAddress.AsNetworkToIpAddress(),
                                                  hardwareAddress: reservedForClient.ClientHardwareAddress,
                                                  allowedClientTypes: (DhcpServerClientTypes)native.bAllowedClientTypes);
        }

        public override string ToString() => $"{Address} [{HardwareAddress}] ({AllowedClientTypes})";
    }
}
