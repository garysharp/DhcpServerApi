using System;
using System.Collections.Generic;
using System.ComponentModel;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerScopeReservation
    {
        public DhcpServer Server => Scope.Server;
        public DhcpServerScope Scope { get; }

        private DhcpServerClient client;

        public DhcpServerIpAddress IpAddress { get; }
        [Obsolete("Use IpAddress.Native instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public int IpAddressNative => (int)IpAddress.Native;

        public DhcpServerHardwareAddress HardwareAddress { get; }
        [Obsolete("Use HardwareAddress.Native instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public long HardwareAddressNative => throw new NotImplementedException();
        [Obsolete("Use HardwareAddress.Native instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] HardwareAddressBytes => HardwareAddress.Native;

        public DhcpServerClientTypes AllowedClientTypes { get; }

        public DhcpServerClient Client => client ??= DhcpServerClient.GetClient(Server, IpAddress);

        /// <summary>
        /// Enumerates a list of Default Global Option Values associated with the DHCP Server
        /// </summary>
        public IEnumerable<DhcpServerOptionValue> OptionValues => DhcpServerOptionValue.EnumScopeReservationDefaultOptionValues(this);

        /// <summary>
        /// Enumerates a list of All Option Values, including vendor/user class option values, associated with the DHCP Server
        /// </summary>
        public IEnumerable<DhcpServerOptionValue> AllOptionValues => DhcpServerOptionValue.GetAllScopeReservationOptionValues(this);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Reservation Scope
        /// </summary>
        /// <param name="option">The associated option to retrieve the option value for</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetOptionValue(DhcpServerOption option) => option.GetScopeReservationValue(this);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Reservation Scope from the Default options
        /// </summary>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetOptionValue(int optionId) => DhcpServerOptionValue.GetScopeReservationDefaultOptionValue(this, optionId);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Reservation Scope within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetVendorOptionValue(string vendorName, int optionId)
            => DhcpServerOptionValue.GetScopeReservationVendorOptionValue(this, optionId, vendorName);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Reservation Scope within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetUserOptionValue(string className, int optionId)
            => DhcpServerOptionValue.GetScopeReservationUserOptionValue(this, optionId, className);

        private DhcpServerScopeReservation(DhcpServerScope scope, DhcpServerIpAddress ipAddress, DhcpServerHardwareAddress hardwareAddress, DhcpServerClientTypes allowedClientTypes)
        {
            Scope = scope;
            IpAddress = ipAddress;
            HardwareAddress = hardwareAddress;
            AllowedClientTypes = allowedClientTypes;
        }

        internal static IEnumerable<DhcpServerScopeReservation> GetReservations(DhcpServerScope scope)
        {
            var resumeHandle = IntPtr.Zero;
            var result = Api.DhcpEnumSubnetElementsV5(ServerIpAddress: scope.Server.IpAddress,
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
                                                  ipAddress: native.ReservedIpAddress.AsNetworkToIpAddress(),
                                                  hardwareAddress: reservedForClient.ClientHardwareAddress,
                                                  allowedClientTypes: (DhcpServerClientTypes)native.bAllowedClientTypes);
        }

        public override string ToString() => $"{IpAddress} [{HardwareAddress}] ({AllowedClientTypes})";
    }
}
