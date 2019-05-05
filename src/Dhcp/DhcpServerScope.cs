using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerScope
    {
        internal readonly DHCP_IP_ADDRESS address;
        private readonly DHCP_IP_MASK mask;

        public DhcpServer Server { get; }

        public IPAddress Address => address.ToIPAddress();
        public int AddressNative => (int)address;

        public IPAddress Mask => mask.ToIPAddress();
        public int MaskNative => (int)mask;

        public string Name { get; }
        public string Comment { get; }
        public DhcpServerScopeState State { get; }
        public DhcpServerIpRange IpRange { get; }
        public IEnumerable<DhcpServerIpRange> ExcludedIpRanges { get; }
        public TimeSpan? LeaseDuration { get; }
        public TimeSpan TimeDelayOffer { get; }
        public DhcpServerHost PrimaryHost { get; }
        public DhcpServerDnsSettings DnsSettings { get; }
        public bool QuarantineOn { get; }

        private DhcpServerScope(DhcpServer server, DHCP_IP_ADDRESS address, DhcpServerIpRange ipRange, List<DhcpServerIpRange> excludedIpRanges, TimeSpan? leaseDuration, TimeSpan timeDelayOffer, DhcpServerDnsSettings dnsSettings)
        {
            Server = server;
            this.address = address;
            IpRange = ipRange;
            ExcludedIpRanges = excludedIpRanges;
            LeaseDuration = leaseDuration;
            TimeDelayOffer = timeDelayOffer;
            DnsSettings = dnsSettings;
        }

        private DhcpServerScope(DhcpServer server, DHCP_IP_ADDRESS address, DhcpServerIpRange ipRange, List<DhcpServerIpRange> excludedIpRanges, TimeSpan? leaseDuration, TimeSpan timeDelayOffer, DhcpServerDnsSettings dnsSettings, DHCP_SUBNET_INFO_VQ info)
            : this(server, address, ipRange, excludedIpRanges, leaseDuration, timeDelayOffer, dnsSettings)
        {
            mask = info.SubnetMask;
            Name = info.SubnetName;
            Comment = info.SubnetComment;
            State = (DhcpServerScopeState)info.SubnetState;
            PrimaryHost = DhcpServerHost.FromNative(info.PrimaryHost);
            QuarantineOn = info.QuarantineOn != 0;
        }

        private DhcpServerScope(DhcpServer server, DHCP_IP_ADDRESS address, DhcpServerIpRange ipRange, List<DhcpServerIpRange> excludedIpRanges, TimeSpan? leaseDuration, TimeSpan timeDelayOffer, DhcpServerDnsSettings dnsSettings, DHCP_SUBNET_INFO info)
            : this(server, address, ipRange, excludedIpRanges, leaseDuration, timeDelayOffer, dnsSettings)
        {
            Name = info.SubnetName;
            Comment = info.SubnetComment;
            mask = info.SubnetMask;
            PrimaryHost = DhcpServerHost.FromNative(info.PrimaryHost);
            State = (DhcpServerScopeState)info.SubnetState;
            QuarantineOn = false;
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
            var result = Api.DhcpEnumSubnets(ServerIpAddress: server.address,
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
                        yield return GetScope(server, scopeAddress);
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(enumInfoPtr);
                }
            }
        }

        private static DhcpServerScope GetScope(DhcpServer server, DHCP_IP_ADDRESS address)
        {
            var leaseDuration = GetLeaseDuration(server, address);
            var timeDelayOffer = GetTimeDelayOffer(server, address);

            var ipRange = EnumSubnetElements(server, address, DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpIpRangesDhcpBootp)
                .Select(e => DhcpServerIpRange.FromNative(e.ReadIpRange()))
                .First();
            var excludedIpRanges = EnumSubnetElements(server, address, DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpExcludedIpRanges)
                .Select(e => DhcpServerIpRange.FromNative(e.ReadExcludeIpRange()))
                .ToList();

            var dnsSettings = DhcpServerDnsSettings.GetScopeDnsSettings(server, address);

            if (server.IsCompatible(DhcpServerVersions.Windows2008R2))
            {
                var info = GetInfoVQ(server, address);
                return new DhcpServerScope(server, address, ipRange, excludedIpRanges, leaseDuration, timeDelayOffer, dnsSettings, info);
            }
            else
            {
                var info = GetInfo(server, address);
                return new DhcpServerScope(server, address, ipRange, excludedIpRanges, leaseDuration, timeDelayOffer, dnsSettings, info);
            }
        }
        
        private static IEnumerable<DHCP_SUBNET_ELEMENT_DATA_V5> EnumSubnetElements(DhcpServer server, DHCP_IP_ADDRESS address, DHCP_SUBNET_ELEMENT_TYPE_V5 enumElementType)
        {
            var resumeHandle = IntPtr.Zero;
            var result = Api.DhcpEnumSubnetElementsV5(ServerIpAddress: server.address,
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

        private static TimeSpan GetTimeDelayOffer(DhcpServer server, DHCP_IP_ADDRESS address)
        {
            var result = Api.DhcpGetSubnetDelayOffer(ServerIpAddress: server.address,
                                                     SubnetAddress: address,
                                                     TimeDelayInMilliseconds: out var timeDelay);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetSubnetDelayOffer), result);

            return TimeSpan.FromMilliseconds(timeDelay);
        }

        private static TimeSpan? GetLeaseDuration(DhcpServer server, DHCP_IP_ADDRESS address)
        {
            var option = DhcpServerOptionValue.GetScopeDefaultOptionValue(server, address, 51);
            var optionValue = (option?.Values.FirstOrDefault() as DhcpServerOptionElementDWord)?.RawValue ?? -1;

            if (optionValue < 0)
                return null;
            else
                return TimeSpan.FromSeconds(optionValue);
        }

        private static DHCP_SUBNET_INFO GetInfo(DhcpServer server, DHCP_IP_ADDRESS address)
        {
            var result = Api.DhcpGetSubnetInfo(ServerIpAddress: server.address,
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

        private static DHCP_SUBNET_INFO_VQ GetInfoVQ(DhcpServer server, DHCP_IP_ADDRESS address)
        {
            var result = Api.DhcpGetSubnetInfoVQ(ServerIpAddress: server.address,
                                                 SubnetAddress: address,
                                                 SubnetInfo: out var subnetInfoPtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetSubnetInfo), result);

            try
            {
                return subnetInfoPtr.MarshalToStructure<DHCP_SUBNET_INFO_VQ>();
            }
            finally
            {
                Api.DhcpRpcFreeMemory(subnetInfoPtr);
            }
        }

        public override string ToString() => $"DHCP Scope: {Address} ({Server.Name} ({Server.IpAddress}))";
    }
}
