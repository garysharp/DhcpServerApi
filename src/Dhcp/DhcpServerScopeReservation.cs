using Dhcp.Native;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Dhcp
{
    public class DhcpServerScopeReservation
    {
        public DhcpServer Server { get { return Scope.Server; } }
        public DhcpServerScope Scope { get; private set; }

        internal DHCP_IP_ADDRESS ipAddress;
        internal DHCP_IP_ADDRESS ipAddressMask;
        private byte[] macAddress;
        private Lazy<DhcpServerClient> client;

        public IPAddress IpAddress { get { return ipAddress.ToIPAddress(); } }
        public int IpAddressNative { get { return (int)ipAddress; } }

        public IPAddress IpAddressMask { get { return ipAddressMask.ToIPAddress(); } }
        public int IpAddressMaskNative { get { return (int)ipAddressMask; } }

        public string HardwareAddress
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                foreach (var b in macAddress)
                {
                    builder.Append(b.ToString("X2"));
                }
                return builder.ToString();
            }
        }
        public long HardwareAddressNative
        {
            get
            {
                if (macAddress.Length == 6)
                {
                    return (long)macAddress[0] << 40 |
                        (long)macAddress[1] << 32 |
                        (long)macAddress[2] << 24 |
                        (long)macAddress[3] << 16 |
                        (long)macAddress[4] << 08 |
                        (long)macAddress[5];
                }
                else
                {
                    return -1;
                }
            }
        }
        public byte[] HardwareAddressBytes { get { return macAddress; } }

        public DhcpServerClientTypes AllowedClientTypes { get; private set; }

        public DhcpServerClient Client { get { return this.client.Value; } }

        /// <summary>
        /// Enumerates a list of Default Global Option Values associated with the DHCP Server
        /// </summary>
        public IEnumerable<DhcpServerOptionValue> OptionValues
        {
            get
            {
                return DhcpServerOptionValue.EnumScopeReservationDefaultOptionValues(this);
            }
        }

        /// <summary>
        /// Enumerates a list of All Option Values, including vendor/user class option values, associated with the DHCP Server
        /// </summary>
        public IEnumerable<DhcpServerOptionValue> AllOptionValues
        {
            get
            {
                return DhcpServerOptionValue.GetAllScopeReservationOptionValues(this);
            }
        }

        /// <summary>
        /// Retrieves the Option Value assoicated with the Option and Reservation Scope
        /// </summary>
        /// <param name="Option">The associated option to retrieve the option value for</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetOptionValue(DhcpServerOption Option)
        {
            return Option.GetScopeReservationValue(this);
        }

        /// <summary>
        /// Retrieves the Option Value assoicated with the Option and Reservation Scope from the Default options
        /// </summary>
        /// <param name="OptionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetOptionValue(int OptionId)
        {
            return DhcpServerOptionValue.GetScopeReservationDefaultOptionValue(this, OptionId);
        }

        /// <summary>
        /// Retrieves the Option Value assoicated with the Option and Reservation Scope within a Vendor Class
        /// </summary>
        /// <param name="VendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="OptionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetVendorOptionValue(string VendorName, int OptionId)
        {
            return DhcpServerOptionValue.GetScopeReservationVendorOptionValue(this, OptionId, VendorName);
        }

        /// <summary>
        /// Retrieves the Option Value assoicated with the Option and Reservation Scope within a User Class
        /// </summary>
        /// <param name="ClassName">The name of the User Class to retrieve the Option from</param>
        /// <param name="OptionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetUserOptionValue(string ClassName, int OptionId)
        {
            return DhcpServerOptionValue.GetScopeReservationUserOptionValue(this, OptionId, ClassName);
        }

        private DhcpServerScopeReservation(DhcpServerScope Scope)
        {
            this.Scope = Scope;

            this.client = new Lazy<DhcpServerClient>(GetClient);
        }

        private DhcpServerClient GetClient()
        {
            return DhcpServerClient.GetClient(Server, ipAddress);
        }

        internal static IEnumerable<DhcpServerScopeReservation> GetReservations(DhcpServerScope Scope)
        {
            IntPtr reservationsPtr;
            int elementsRead, elementsTotal;
            IntPtr resumeHandle = IntPtr.Zero;

            var result = Api.DhcpEnumSubnetElementsV5(Scope.Server.ipAddress.ToString(), Scope.address, DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpReservedIps, ref resumeHandle, 0xFFFFFFFF, out reservationsPtr, out elementsRead, out elementsTotal);

            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS || result == DhcpErrors.EPT_S_NOT_REGISTERED)
                yield break;

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                throw new DhcpServerException("DhcpEnumSubnetElementsV5", result);

            if (elementsRead == 0)
                yield break;

            try
            {
                var reservations = (DHCP_SUBNET_ELEMENT_INFO_ARRAY_V5)Marshal.PtrToStructure(reservationsPtr, typeof(DHCP_SUBNET_ELEMENT_INFO_ARRAY_V5));

                foreach (var element in reservations.Elements)
                {
                    yield return FromNative(Scope, element.ReadReservedIp());
                }
            }
            finally
            {
                Api.DhcpRpcFreeMemory(reservationsPtr);
            }
        }

        private static DhcpServerScopeReservation FromNative(DhcpServerScope Scope, DHCP_IP_RESERVATION_V4 Native)
        {
            var reservedForClient = Native.ReservedForClient;

            return new DhcpServerScopeReservation(Scope)
            {
                ipAddress = Native.ReservedIpAddress,
                ipAddressMask = reservedForClient.ClientIpAddressMask,
                macAddress = reservedForClient.ClientMacAddress,
                AllowedClientTypes = (DhcpServerClientTypes)Native.bAllowedClientTypes
            };
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}] ({2})", this.IpAddress, this.HardwareAddress, this.AllowedClientTypes);
        }
    }
}
