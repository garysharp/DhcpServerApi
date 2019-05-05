using System;
using System.Collections.Generic;
using System.Net;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerScopeReservation
    {
        public DhcpServer Server => Scope.Server;
        public DhcpServerScope Scope { get; }

        internal readonly DHCP_IP_ADDRESS ipAddress;
        internal readonly DHCP_IP_ADDRESS ipAddressMask;
        private DhcpServerClient client;

        public IPAddress IpAddress => ipAddress.ToIPAddress();
        public int IpAddressNative => (int)ipAddress;

        public IPAddress IpAddressMask => ipAddressMask.ToIPAddress();
        public int IpAddressMaskNative => (int)ipAddressMask;

        /// <summary>
        /// A string representation of the reservation hardware address
        /// </summary>
        public string HardwareAddress => HardwareAddressBytes.ToHexString();

        /// <summary>
        /// A 64-bit representation of the reservation hardware address.
        /// Or -1 if the address is greater than 64-bits.
        /// </summary>
        public long HardwareAddressNative
        {
            get
            {
                if (HardwareAddressBytes.Length == 6)
                {
                    return (long)HardwareAddressBytes[0] << 40 |
                        (long)HardwareAddressBytes[1] << 32 |
                        (long)HardwareAddressBytes[2] << 24 |
                        (long)HardwareAddressBytes[3] << 16 |
                        (long)HardwareAddressBytes[4] << 08 |
                        HardwareAddressBytes[5];
                }
                else
                    return -1;
            }
        }
        public byte[] HardwareAddressBytes { get; }

        public DhcpServerClientTypes AllowedClientTypes { get; }

        public DhcpServerClient Client => client ??= DhcpServerClient.GetClient(Server, ipAddress);

        /// <summary>
        /// Enumerates a list of Default Global Option Values associated with the DHCP Server
        /// </summary>
        public IEnumerable<DhcpServerOptionValue> OptionValues => DhcpServerOptionValue.EnumScopeReservationDefaultOptionValues(this);

        /// <summary>
        /// Enumerates a list of All Option Values, including vendor/user class option values, associated with the DHCP Server
        /// </summary>
        public IEnumerable<DhcpServerOptionValue> AllOptionValues => DhcpServerOptionValue.GetAllScopeReservationOptionValues(this);

        /// <summary>
        /// Retrieves the Option Value assoicated with the Option and Reservation Scope
        /// </summary>
        /// <param name="option">The associated option to retrieve the option value for</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetOptionValue(DhcpServerOption option) => option.GetScopeReservationValue(this);

        /// <summary>
        /// Retrieves the Option Value assoicated with the Option and Reservation Scope from the Default options
        /// </summary>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetOptionValue(int optionId) => DhcpServerOptionValue.GetScopeReservationDefaultOptionValue(this, optionId);

        /// <summary>
        /// Retrieves the Option Value assoicated with the Option and Reservation Scope within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetVendorOptionValue(string vendorName, int optionId)
            => DhcpServerOptionValue.GetScopeReservationVendorOptionValue(this, optionId, vendorName);

        /// <summary>
        /// Retrieves the Option Value assoicated with the Option and Reservation Scope within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetUserOptionValue(string className, int optionId)
            => DhcpServerOptionValue.GetScopeReservationUserOptionValue(this, optionId, className);

        private DhcpServerScopeReservation(DhcpServerScope scope, DHCP_IP_ADDRESS ipAddress, DHCP_IP_ADDRESS ipAddressMask, byte[] hardwareAddressBytes, DhcpServerClientTypes allowedClientTypes)
        {
            Scope = scope;
            this.ipAddress = ipAddress;
            this.ipAddressMask = ipAddressMask;
            HardwareAddressBytes = hardwareAddressBytes;
            AllowedClientTypes = allowedClientTypes;
        }

        internal static IEnumerable<DhcpServerScopeReservation> GetReservations(DhcpServerScope scope)
        {
            var resumeHandle = IntPtr.Zero;
            var result = Api.DhcpEnumSubnetElementsV5(ServerIpAddress: scope.Server.address,
                                                      SubnetAddress: scope.address,
                                                      EnumElementType: DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpReservedIps,
                                                      ResumeHandle: ref resumeHandle, PreferredMaximum: 0xFFFFFFFF,
                                                      EnumElementInfo: out var reservationsPtr,
                                                      ElementsRead: out var elementsRead,
                                                      ElementsTotal: out _);

            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS || result == DhcpErrors.EPT_S_NOT_REGISTERED)
                yield break;

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                throw new DhcpServerException(nameof(Api.DhcpEnumSubnetElementsV5), result);

            if (elementsRead == 0)
                yield break;

            try
            {
                var reservations = reservationsPtr.MarshalToStructure<DHCP_SUBNET_ELEMENT_INFO_ARRAY_V5>();

                foreach (var element in reservations.Elements)
                {
                    yield return FromNative(scope, element.ReadReservedIp());
                }
            }
            finally
            {
                Api.DhcpRpcFreeMemory(reservationsPtr);
            }
        }

        private static DhcpServerScopeReservation FromNative(DhcpServerScope scope, DHCP_IP_RESERVATION_V4 native)
        {
            var reservedForClient = native.ReservedForClient;

            return new DhcpServerScopeReservation(
                scope: scope,
                ipAddress: native.ReservedIpAddress,
                ipAddressMask: reservedForClient.ClientIpAddressMask,
                hardwareAddressBytes: reservedForClient.ClientMacAddress,
                allowedClientTypes: (DhcpServerClientTypes)native.bAllowedClientTypes);
        }

        public override string ToString() => $"{IpAddress} [{HardwareAddress}] ({AllowedClientTypes})";
    }
}
