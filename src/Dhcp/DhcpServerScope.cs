using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerScope
    {
        /// <summary>
        /// Default time delay offer of 0 milliseconds
        /// </summary>
        public static TimeSpan DefaultTimeDelayOffer => TimeSpan.FromMilliseconds(0);
        /// <summary>
        /// Default lease duration of 8 days
        /// </summary>
        public static TimeSpan DefaultLeaseDuration => TimeSpan.FromDays(8);

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
        public DhcpServerIpRange IpRange { get; }
        public IEnumerable<DhcpServerIpRange> ExcludedIpRanges { get; }
        public TimeSpan? LeaseDuration
        {
            get => GetLeaseDuration(Server, Address);
            set => SetLeaseDuration(Server, Address, value);
        }
        public TimeSpan TimeDelayOffer
        {
            get => GetTimeDelayOffer(Server, Address);
            set => SetTimeDelayOffer(Server, Address, value);
        }
        public DhcpServerHost PrimaryHost { get; }
        public DhcpServerDnsSettings DnsSettings { get; }
        public bool QuarantineOn { get; }

        private DhcpServerScope(DhcpServer server, DhcpServerIpAddress address, DhcpServerIpRange ipRange, List<DhcpServerIpRange> excludedIpRanges, DhcpServerDnsSettings dnsSettings)
        {
            Server = server;
            Address = address;
            IpRange = ipRange;
            ExcludedIpRanges = excludedIpRanges;
            DnsSettings = dnsSettings;
        }

        private DhcpServerScope(DhcpServer server, DhcpServerIpAddress address, DhcpServerIpRange ipRange, List<DhcpServerIpRange> excludedIpRanges, DhcpServerDnsSettings dnsSettings, SubnetInfo info)
            : this(server, address, ipRange, excludedIpRanges, dnsSettings)
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

        internal static DhcpServerScope CreateScope(DhcpServer server, string name, string description, DhcpServerIpRange ipRange, DhcpServerIpMask mask, bool enable)
            => CreateScope(server, name, description, ipRange, mask, excludedRanges: null, timeDelayOffer: DefaultTimeDelayOffer, leaseDuration: DefaultLeaseDuration, enable: enable);

        internal static DhcpServerScope CreateScope(DhcpServer server, string name, string description, DhcpServerIpRange ipRange, DhcpServerIpMask mask, IEnumerable<DhcpServerIpRange> excludedRanges, TimeSpan timeDelayOffer, TimeSpan? leaseDuration, bool enable)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (leaseDuration.HasValue && leaseDuration.Value.TotalMinutes < 1)
                throw new ArgumentOutOfRangeException(nameof(leaseDuration), "Lease duration can be unlimited (null) or at least 1 minute");
            if (ipRange.Type != DhcpServerIpRangeType.ScopeDhcpOnly && ipRange.Type != DhcpServerIpRangeType.ScopeDhcpAndBootp && ipRange.Type != DhcpServerIpRangeType.ScopeBootpOnly)
                throw new ArgumentOutOfRangeException(nameof(ipRange), "The IP Range must be of scope type (ScopeDhcpOnly, ScopeDhcpAndBootp or ScopeBootpOnly)");

            var subnetAddress = mask.GetDhcpIpRange(ipRange.StartAddress).StartAddress;

            var primaryHost = new DHCP_HOST_INFO_Managed(ipAddress: server.IpAddress.ToNativeAsNetwork(), netBiosName: null, serverName: null);
            var scopeInfo = new DHCP_SUBNET_INFO_Managed(subnetAddress: subnetAddress.ToNativeAsNetwork(),
                                                         subnetMask: mask.ToNativeAsNetwork(),
                                                         subnetName: name,
                                                         subnetComment: description,
                                                         primaryHost: primaryHost,
                                                         subnetState: DHCP_SUBNET_STATE.DhcpSubnetDisabled);

            var result = Api.DhcpCreateSubnet(ServerIpAddress: server.IpAddress,
                                              SubnetAddress: subnetAddress.ToNativeAsNetwork(),
                                              SubnetInfo: ref scopeInfo);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpCreateSubnet), result);

            if (timeDelayOffer.TotalMilliseconds != 0)
                SetTimeDelayOffer(server, subnetAddress, timeDelayOffer);

            SetLeaseDuration(server, subnetAddress, leaseDuration);

            AddSubnetScopeIpRangeElement(server, subnetAddress, ipRange);

            if (excludedRanges != null)
            {
                foreach (var excludedRange in excludedRanges)
                    AddSubnetExcludedIpRangeElement(server, subnetAddress, excludedRange);
            }

            if (enable)
            {
                scopeInfo.SubnetState = DHCP_SUBNET_STATE.DhcpSubnetEnabled;
                result = Api.DhcpSetSubnetInfo(ServerIpAddress: server.IpAddress,
                                               SubnetAddress: subnetAddress.ToNativeAsNetwork(),
                                               SubnetInfo: ref scopeInfo);

                if (result != DhcpErrors.SUCCESS)
                    throw new DhcpServerException(nameof(Api.DhcpCreateSubnet), result);
            }
            
            return GetScope(server, subnetAddress);
        }

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

        internal static DhcpServerScope GetScope(DhcpServer server, DhcpServerIpAddress address)
        {
            SubnetInfo info;
            if (server.IsCompatible(DhcpServerVersions.Windows2008R2))
                info = GetInfoVQ(server, address);
            else
                info = GetInfoV0(server, address);

            var ipRange = EnumSubnetElements(server, address, DHCP_SUBNET_ELEMENT_TYPE.DhcpIpRangesDhcpBootp)
                .Select(element => DhcpServerIpRange.FromNative(ref element)).First();
            var excludedIpRanges = EnumSubnetElements(server, address, DHCP_SUBNET_ELEMENT_TYPE.DhcpExcludedIpRanges)
                .Select(element => DhcpServerIpRange.FromNative(ref element)).ToList();

            var dnsSettings = DhcpServerDnsSettings.GetScopeDnsSettings(server, address);

            return new DhcpServerScope(server, address, ipRange, excludedIpRanges, dnsSettings, info);
        }

        private static IEnumerable<DHCP_SUBNET_ELEMENT_DATA_V5> EnumSubnetElements(DhcpServer server, DhcpServerIpAddress address, DHCP_SUBNET_ELEMENT_TYPE enumElementType)
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

        private static void AddSubnetScopeIpRangeElement(DhcpServer server, DhcpServerIpAddress address, DhcpServerIpRange range)
        {
            if (server.IsCompatible(DhcpServerVersions.Windows2003))
            {
                using (var element = new DHCP_SUBNET_ELEMENT_DATA_V5_Managed((DHCP_SUBNET_ELEMENT_TYPE)range.Type, range.ToNativeBootpIpRange()))
                {
                    AddSubnetElementV5(server, address, element);
                }
            }
            else
            {
                using (var element = new DHCP_SUBNET_ELEMENT_DATA_Managed(DHCP_SUBNET_ELEMENT_TYPE.DhcpIpRanges, range.ToNativeIpRange()))
                {
                    AddSubnetElementV0(server, address, element);
                }
            }
        }

        private static void AddSubnetExcludedIpRangeElement(DhcpServer server, DhcpServerIpAddress address, DhcpServerIpRange range)
        {
            if (range.Type != DhcpServerIpRangeType.Excluded)
                throw new ArgumentOutOfRangeException($"{nameof(range)}.{nameof(range.Type)}", "The expected range type is 'Excluded'");

            if (server.IsCompatible(DhcpServerVersions.Windows2003))
            {
                using (var element = new DHCP_SUBNET_ELEMENT_DATA_V5_Managed((DHCP_SUBNET_ELEMENT_TYPE)range.Type, range.ToNativeIpRange()))
                {
                    AddSubnetElementV5(server, address, element);
                }
            }
            else
            {
                using (var element = new DHCP_SUBNET_ELEMENT_DATA_Managed(DHCP_SUBNET_ELEMENT_TYPE.DhcpIpRanges, range.ToNativeIpRange()))
                {
                    AddSubnetElementV0(server, address, element);
                }
            }
        }

        private static void AddSubnetElementV5(DhcpServer server, DhcpServerIpAddress address, DHCP_SUBNET_ELEMENT_DATA_V5_Managed element)
        {
            var elementRef = element;
            var result = Api.DhcpAddSubnetElementV5(server.IpAddress, address.ToNativeAsNetwork(), ref elementRef);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpAddSubnetElementV5), result);
        }

        private static void AddSubnetElementV0(DhcpServer server, DhcpServerIpAddress address, DHCP_SUBNET_ELEMENT_DATA_Managed element)
        {
            var elementRef = element;
            var result = Api.DhcpAddSubnetElement(server.IpAddress, address.ToNativeAsNetwork(), ref elementRef);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpAddSubnetElement), result);
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

        private static void SetTimeDelayOffer(DhcpServer server, DhcpServerIpAddress address, TimeSpan timeDelayOffer)
        {
            if (timeDelayOffer.TotalMilliseconds < 0)
                throw new ArgumentOutOfRangeException(nameof(timeDelayOffer), "Time delay offer must be positive");
            if (timeDelayOffer.TotalMilliseconds > 1000)
                throw new ArgumentOutOfRangeException(nameof(timeDelayOffer), $"Time delay offer must be less than or equal to 1000");

            var timeDelayOfferMilliseconds = (ushort)timeDelayOffer.TotalMilliseconds;

            var result = Api.DhcpSetSubnetDelayOffer(ServerIpAddress: server.IpAddress,
                                                     SubnetAddress: address.ToNativeAsNetwork(),
                                                     TimeDelayInMilliseconds: timeDelayOfferMilliseconds);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetSubnetDelayOffer), result);
        }

        private static TimeSpan? GetLeaseDuration(DhcpServer server, DhcpServerIpAddress address)
        {
            try
            {
                var option = DhcpServerOptionValue.GetScopeDefaultOptionValue(server, address, 51);
                var optionValue = (option.Values.FirstOrDefault() as DhcpServerOptionElementDWord)?.RawValue ?? -1;

                if (optionValue < 0)
                    return null;
                else
                    return TimeSpan.FromSeconds(optionValue);
            }
            catch (DhcpServerException ex) when (ex.ApiErrorId == (uint)DhcpErrors.ERROR_FILE_NOT_FOUND)
            {
                return null;
            }
        }

        private static void SetLeaseDuration(DhcpServer server, DhcpServerIpAddress address, TimeSpan? leaseDuration)
        {
            if (leaseDuration.HasValue && leaseDuration.Value.TotalMinutes < 1)
                throw new ArgumentOutOfRangeException(nameof(leaseDuration), "Lease duration can be unlimited (null) or at least 1 minute");

            // lease duration in seconds (or -1 for unlimited/null)
            var leaseDurationSeconds = leaseDuration.HasValue ? (int)leaseDuration.Value.TotalSeconds : -1;

            var optionValue = new DhcpServerOptionElementDWord(leaseDurationSeconds);

            DhcpServerOptionValue.SetScopeDefaultOptionValue(server, address, 51, new DhcpServerOptionElement[] { optionValue });
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
