using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerScope
    {
        public DhcpServer Server { get; }

        public DhcpServerIpAddress Address { get; }
        [Obsolete("Use Address.Native instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public int AddressNative => (int)Address.Native;


        public DhcpServerIpMask Mask { get; }
        [Obsolete("Use Mask.Native instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public int MaskNative => (int)Mask.Native;

        public string Name { get; }
        public string Comment { get; }
        public DhcpServerScopeState State { get; }
        public DhcpServerBootpIpRange IpRange { get; }
        public IEnumerable<DhcpServerIpRange> ExcludedIpRanges { get; }
        public TimeSpan? LeaseDuration { get; }
        public TimeSpan TimeDelayOffer { get; }
        public DhcpServerHost PrimaryHost { get; }
        public DhcpServerDnsSettings DnsSettings { get; }
        public bool QuarantineOn { get; }

        private DhcpServerScope(DhcpServer server, DhcpServerIpAddress address, DhcpServerBootpIpRange ipRange, List<DhcpServerIpRange> excludedIpRanges, TimeSpan? leaseDuration, TimeSpan timeDelayOffer, DhcpServerDnsSettings dnsSettings)
        {
            Server = server;
            Address = address;
            IpRange = ipRange;
            ExcludedIpRanges = excludedIpRanges;
            LeaseDuration = leaseDuration;
            TimeDelayOffer = timeDelayOffer;
            DnsSettings = dnsSettings;
        }

        private DhcpServerScope(DhcpServer server, DhcpServerIpAddress address, DhcpServerBootpIpRange ipRange, List<DhcpServerIpRange> excludedIpRanges, TimeSpan? leaseDuration, TimeSpan timeDelayOffer, DhcpServerDnsSettings dnsSettings, SubnetInfo info)
            : this(server, address, ipRange, excludedIpRanges, leaseDuration, timeDelayOffer, dnsSettings)
        {
            Mask = info.Mask;
            Name = info.Name;
            Comment = info.Comment;
            State = info.State;
            PrimaryHost = info.PrimaryHost;
            QuarantineOn = info.QuarantineOn;
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
        /// Retrieves the Option Value associated with the Option and Scope
        /// </summary>
        /// <param name="option">The associated option to retrieve the option value for</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetOptionValue(DhcpServerOption option) => option.GetScopeValue(this);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope from the Default options
        /// </summary>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetOptionValue(int optionId) => DhcpServerOptionValue.GetScopeDefaultOptionValue(this, optionId);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetVendorOptionValue(string vendorName, int optionId)
            => DhcpServerOptionValue.GetScopeVendorOptionValue(this, optionId, vendorName);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetUserOptionValue(string className, int optionId)
            => DhcpServerOptionValue.GetScopeUserOptionValue(this, optionId, className);

        internal static IEnumerable<DhcpServerScope> GetScopes(DhcpServer server)
        {
            var resumeHandle = IntPtr.Zero;
            var result = Api.DhcpEnumSubnets(ServerIpAddress: server.IpAddress,
                                             ResumeHandle: ref resumeHandle,
                                             PreferredMaximum: 0xFFFFFFFF,
                                             EnumInfo: out var enumInfoPtr,
                                             ElementsRead: out var elementsRead,
                                             ElementsTotal: out _);

            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS || result == DhcpErrors.EPT_S_NOT_REGISTERED)
                yield break;

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpEnumSubnets), result);

            try
            {
                if (elementsRead == 0)
                    yield break;

                using (var enumInfo = enumInfoPtr.MarshalToStructure<DHCP_IP_ARRAY>())
                {
                    foreach (var scopeAddress in enumInfo.Elements)
                        yield return GetScope(server, scopeAddress.AsNetworkToIpAddress());
                }
            }
            finally
            {
                Api.FreePointer(enumInfoPtr);
            }
        }

        private static DhcpServerScope GetScope(DhcpServer server, DhcpServerIpAddress address)
        {
            var leaseDuration = GetLeaseDuration(server, address);
            var timeDelayOffer = GetTimeDelayOffer(server, address);

            var ipRange = EnumSubnetElements(server, address, DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpIpRangesDhcpBootp)
                .Select(element =>
                {
                    var ipRange = element.ReadIpRange();
                    return DhcpServerBootpIpRange.FromNative(ref ipRange);
                }).First();
            var excludedIpRanges = EnumSubnetElements(server, address, DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpExcludedIpRanges)
                .Select(element =>
                {
                    var ipRange = element.ReadExcludeIpRange();
                    return DhcpServerIpRange.FromNative(ref ipRange);
                }).ToList();

            var dnsSettings = DhcpServerDnsSettings.GetScopeDnsSettings(server, address);

            SubnetInfo info;
            if (server.IsCompatible(DhcpServerVersions.Windows2008R2))
                info = GetInfoVQ(server, address);
            else
                info = GetInfoV0(server, address);

            return new DhcpServerScope(server, address, ipRange, excludedIpRanges, leaseDuration, timeDelayOffer, dnsSettings, info);
        }

        private static IEnumerable<DHCP_SUBNET_ELEMENT_DATA_V5> EnumSubnetElements(DhcpServer server, DhcpServerIpAddress address, DHCP_SUBNET_ELEMENT_TYPE_V5 enumElementType)
        {
            var resumeHandle = IntPtr.Zero;
            var result = Api.DhcpEnumSubnetElementsV5(ServerIpAddress: server.IpAddress,
                                                      SubnetAddress: address.ToNativeAsNetwork(),
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

            try
            {
                if (elementsRead == 0)
                    yield break;

                using (var elements = elementsPtr.MarshalToStructure<DHCP_SUBNET_ELEMENT_INFO_ARRAY_V5>())
                {
                    foreach (var element in elements.Elements)
                        yield return element;
                }
            }
            finally
            {
                Api.FreePointer(elementsPtr);
            }
        }

        private static TimeSpan GetTimeDelayOffer(DhcpServer server, DhcpServerIpAddress address)
        {
            var result = Api.DhcpGetSubnetDelayOffer(ServerIpAddress: server.IpAddress,
                                                     SubnetAddress: address.ToNativeAsNetwork(),
                                                     TimeDelayInMilliseconds: out var timeDelay);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetSubnetDelayOffer), result);

            return TimeSpan.FromMilliseconds(timeDelay);
        }

        private static TimeSpan? GetLeaseDuration(DhcpServer server, DhcpServerIpAddress address)
        {
            var option = DhcpServerOptionValue.GetScopeDefaultOptionValue(server, address, 51);
            var optionValue = (option?.Values.FirstOrDefault() as DhcpServerOptionElementDWord)?.RawValue ?? -1;

            if (optionValue < 0)
                return null;
            else
                return TimeSpan.FromSeconds(optionValue);
        }

        private static SubnetInfo GetInfoV0(DhcpServer server, DhcpServerIpAddress address)
        {
            var result = Api.DhcpGetSubnetInfo(ServerIpAddress: server.IpAddress,
                                               SubnetAddress: address.ToNativeAsNetwork(),
                                               SubnetInfo: out var subnetInfoPtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetSubnetInfo), result);

            try
            {
                using (var subnetInfo = subnetInfoPtr.MarshalToStructure<DHCP_SUBNET_INFO>())
                    return SubnetInfo.FromNative(subnetInfo);
            }
            finally
            {
                Api.FreePointer(subnetInfoPtr);
            }
        }

        private static SubnetInfo GetInfoVQ(DhcpServer server, DhcpServerIpAddress address)
        {
            var result = Api.DhcpGetSubnetInfoVQ(ServerIpAddress: server.IpAddress,
                                                 SubnetAddress: address.ToNativeAsNetwork(),
                                                 SubnetInfo: out var subnetInfoPtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetSubnetInfoVQ), result);

            try
            {
                using (var subnetInfo = subnetInfoPtr.MarshalToStructure<DHCP_SUBNET_INFO_VQ>())
                    return SubnetInfo.FromNative(subnetInfo);
            }
            finally
            {
                Api.DhcpRpcFreeMemory(subnetInfoPtr);
            }
        }

        public override string ToString() => $"DHCP Scope: {Address} ({Server.Name} ({Server.IpAddress}))";

        private class SubnetInfo
        {
            public readonly DhcpServerIpMask Mask;
            public readonly string Name;
            public readonly string Comment;
            public readonly DhcpServerHost PrimaryHost;
            public readonly DhcpServerScopeState State;
            public readonly bool QuarantineOn;

            private SubnetInfo(DhcpServerIpMask mask, string name, string comment, DhcpServerHost primaryHost, DhcpServerScopeState state, bool quarantineOn)
            {
                Mask = mask;
                Name = name;
                Comment = comment;
                PrimaryHost = primaryHost;
                State = state;
                QuarantineOn = quarantineOn;
            }
            public static SubnetInfo FromNative(DHCP_SUBNET_INFO info)
            {
                return new SubnetInfo(mask: info.SubnetMask.AsNetworkToIpMask(),
                                      name: info.SubnetName,
                                      comment: info.SubnetComment,
                                      primaryHost: DhcpServerHost.FromNative(info.PrimaryHost),
                                      state: (DhcpServerScopeState)info.SubnetState,
                                      quarantineOn: false);
            }

            public static SubnetInfo FromNative(DHCP_SUBNET_INFO_VQ info)
            {
                return new SubnetInfo(mask: info.SubnetMask.AsNetworkToIpMask(),
                                      name: info.SubnetName,
                                      comment: info.SubnetComment,
                                      primaryHost: DhcpServerHost.FromNative(info.PrimaryHost),
                                      state: (DhcpServerScopeState)info.SubnetState,
                                      quarantineOn: info.QuarantineOn != 0);
            }
        }
    }
}
