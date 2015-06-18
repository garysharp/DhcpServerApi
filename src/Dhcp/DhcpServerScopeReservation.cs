using Dhcp.Native;
using System;
using System.Collections.Generic;
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

        public string IpAddress { get { return ipAddress.ToString(); } }

        public string IpAddressMask { get { return ipAddressMask.ToString(); } }

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

        public DhcpServerClientTypes AllowedClientTypes { get; private set; }

        public DhcpServerClient Client { get { return this.client.Value; } }

        /// <summary>
        /// Enumerates a list of Global Option Values associated with the DHCP Server
        /// </summary>
        public IEnumerable<DhcpServerOptionValue> OptionValues
        {
            get
            {
                return DhcpServerOptionValue.EnumScopeReservationOptionValues(this);
            }
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

            var result = Api.DhcpEnumSubnetElementsV5(Scope.Server.IpAddress, Scope.address, DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpReservedIps, ref resumeHandle, 0xFFFFFFFF, out reservationsPtr, out elementsRead, out elementsTotal);

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
