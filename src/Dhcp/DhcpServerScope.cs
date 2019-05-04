using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerScope
    {
        internal DHCP_IP_ADDRESS address;
        private DHCP_SUBNET_INFO? info;
        private TimeSpan? timeDelayOffer;
        private DhcpServerIpRange ipRange;
        private List<DhcpServerIpRange> excludedIpRanges;
        private TimeSpan? leaseDuration;
        private DhcpServerDnsSettings dnsSettings;

        public DhcpServer Server { get; }

        public IPAddress Address => address.ToIPAddress();
        public int AddressNative => (int)address;

        private DHCP_SUBNET_INFO Info => (DHCP_SUBNET_INFO)(info ??= GetInfo());

        public IPAddress Mask => Info.SubnetMask.ToIPAddress();
        public int MaskNative => (int)Info.SubnetMask;

        public string Name => Info.SubnetName;
        public string Comment => Info.SubnetComment;

        public IPAddress PrimaryHostIpAddress => Info.PrimaryHost.IpAddress.ToReverseOrder().ToIPAddress();
        public int PrimaryHostIpAddressNative => (int)Info.PrimaryHost.IpAddress.ToReverseOrder();

        public DhcpServerScopeState State => (DhcpServerScopeState)Info.SubnetState;

        public TimeSpan TimeDelayOffer => (TimeSpan)(timeDelayOffer ??= GetTimeDelayOffer());

        internal DhcpServerScope(DhcpServer server, DHCP_IP_ADDRESS subnetAddress)
        {
            Server = server;
            address = subnetAddress;
        }

        /// <summary>
        /// Enumerates a list of Default Option Values associated with the DHCP Scope
        /// </summary>
        public IEnumerable<DhcpServerOptionValue> OptionValues => DhcpServerOptionValue.EnumScopeDefaultOptionValues(this);

        /// <summary>
        /// Enumerates a list of All Option Values, including vendor/user class option values, associated with the DHCP Scope
        /// </summary>
        public IEnumerable<DhcpServerOptionValue> AllOptionValues => DhcpServerOptionValue.GetAllScopeOptionValues(this);

        public IEnumerable<DhcpServerClient> Clients => DhcpServerClient.GetClients(this);

        public IEnumerable<DhcpServerScopeReservation> Reservations => DhcpServerScopeReservation.GetReservations(this);

        public DhcpServerIpRange IpRange => ipRange ??= GetIpRange();

        public IEnumerable<DhcpServerIpRange> ExcludedIpRanges => excludedIpRanges ??= GetExcludedIpRanges();

        public TimeSpan LeaseDuration => (leaseDuration ??= GetLeaseDuration()).GetValueOrDefault();

        public DhcpServerDnsSettings DnsSettings => dnsSettings ??= DhcpServerDnsSettings.GetScopeDnsSettings(this);

        /// <summary>
        /// Retrieves the Option Value assoicated with the Option and Scope
        /// </summary>
        /// <param name="option">The associated option to retrieve the option value for</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetOptionValue(DhcpServerOption option) => option.GetScopeValue(this);

        /// <summary>
        /// Retrieves the Option Value assoicated with the Option and Scope from the Default options
        /// </summary>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetOptionValue(int optionId) => DhcpServerOptionValue.GetScopeDefaultOptionValue(this, optionId);

        /// <summary>
        /// Retrieves the Option Value assoicated with the Option and Scope within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetVendorOptionValue(string vendorName, int optionId) 
            => DhcpServerOptionValue.GetScopeVendorOptionValue(this, optionId, vendorName);

        /// <summary>
        /// Retrieves the Option Value assoicated with the Option and Scope within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetUserOptionValue(string className, int optionId) 
            => DhcpServerOptionValue.GetScopeUserOptionValue(this, optionId, className);

        internal static IEnumerable<DhcpServerScope> GetScopes(DhcpServer server)
        {
            var resumeHandle = IntPtr.Zero;
            var result = Api.DhcpEnumSubnets(ServerIpAddress: server.ipAddress,
                                             ResumeHandle: ref resumeHandle,
                                             PreferredMaximum: 0xFFFFFFFF,
                                             EnumInfo: out var enumInfoPtr,
                                             ElementsRead: out var elementsRead,
                                             ElementsTotal: out _);

            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS || result == DhcpErrors.EPT_S_NOT_REGISTERED)
                yield break;

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpEnumSubnets), result);

            if (elementsRead > 0)
            {
                var enumInfo = enumInfoPtr.MarshalToStructure<DHCP_IP_ARRAY>();

                try
                {
                    foreach (var scopeAddress in enumInfo.Elements)
                        yield return new DhcpServerScope(server, scopeAddress);
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(enumInfoPtr);
                }
            }
        }

        private DHCP_SUBNET_INFO GetInfo()
        {
            var result = Api.DhcpGetSubnetInfo(ServerIpAddress: Server.ipAddress,
                                               SubnetAddress: address,
                                               SubnetInfo: out var subnetInfoPtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetSubnetInfo), result);

            try
            {
                return subnetInfoPtr.MarshalToStructure<DHCP_SUBNET_INFO>();
            }
            finally
            {
                Api.DhcpRpcFreeMemory(subnetInfoPtr);
            }
        }

        private List<DhcpServerIpRange> GetExcludedIpRanges()
        {
            return EnumSubnetElements(DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpExcludedIpRanges)
                .Select(e => DhcpServerIpRange.FromNative(e.ReadExcludeIpRange()))
                .ToList();
        }

        private DhcpServerIpRange GetIpRange()
        {
            return EnumSubnetElements(DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpIpRangesDhcpBootp)
                .Select(e => DhcpServerIpRange.FromNative(e.ReadIpRange()))
                .First();
        }

        private IEnumerable<DHCP_SUBNET_ELEMENT_DATA_V5> EnumSubnetElements(DHCP_SUBNET_ELEMENT_TYPE_V5 enumElementType)
        {
            var resumeHandle = IntPtr.Zero;
            var result = Api.DhcpEnumSubnetElementsV5(ServerIpAddress: Server.ipAddress,
                                                      SubnetAddress: address,
                                                      EnumElementType: enumElementType,
                                                      ResumeHandle: ref resumeHandle,
                                                      PreferredMaximum: 0xFFFFFFFF,
                                                      EnumElementInfo: out var elementsPtr,
                                                      ElementsRead: out var elementsRead,
                                                      ElementsTotal: out _);

            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS)
                yield break;

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                throw new DhcpServerException(nameof(Api.DhcpEnumSubnetElementsV5), result);

            if (elementsRead == 0)
                yield break;

            try
            {
                var elements = elementsPtr.MarshalToStructure<DHCP_SUBNET_ELEMENT_INFO_ARRAY_V5>();

                foreach (var element in elements.Elements)
                    yield return element;
            }
            finally
            {
                Api.DhcpRpcFreeMemory(elementsPtr);
            }
        }

        private TimeSpan GetTimeDelayOffer()
        {
            var result = Api.DhcpGetSubnetDelayOffer(ServerIpAddress: Server.ipAddress,
                                                     SubnetAddress: address,
                                                     TimeDelayInMilliseconds: out var timeDelay);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetSubnetDelayOffer), result);

            return TimeSpan.FromMilliseconds(timeDelay);
        }

        private TimeSpan GetLeaseDuration()
        {
            var option = DhcpServerOptionValue.GetScopeDefaultOptionValue(this, 51);

            if (option == null)
                return default;
            else
                return TimeSpan.FromSeconds(((DhcpServerOptionElementDWord)option.Values.First()).RawValue);
        }

        public override string ToString() => $"DHCP Scope: {Address} ({Server.Name} ({Server.IpAddress}))";
    }
}
