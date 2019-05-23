using System;
using System.Collections.Generic;
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

        private SubnetInfo info;
        public DhcpServerIpMask Mask => info.Mask;
        public string Name
        {
            get => info.Name;
            set => SetName(value);
        }
        public string Comment
        {
            get => info.Comment;
            set => SetComment(value);
        }
        public DhcpServerScopeState State => info.State;
        public DhcpServerIpRange IpRange { get; }
        
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

        public DhcpServerHost PrimaryHost => info.PrimaryHost;
        public DhcpServerDnsSettings DnsSettings { get; }
        public bool QuarantineOn => info.QuarantineOn;

        /// <summary>
        /// Scope Excluded IP Ranges
        /// </summary>
        public DhcpServerScopeExcludedIpRangeCollection ExcludedIpRanges { get; }

        /// <summary>
        /// Scope Option Values
        /// </summary>
        public DhcpServerScopeOptionValueCollection Options { get; }

        /// <summary>
        /// Scope Reservations
        /// </summary>
        public DhcpServerScopeReservationCollection Reservations { get; }

        /// <summary>
        /// Scope Clients
        /// </summary>
        public DhcpServerScopeClientCollection Clients { get; }

        private DhcpServerScope(DhcpServer server, DhcpServerIpAddress address, DhcpServerIpRange ipRange, DhcpServerDnsSettings dnsSettings, SubnetInfo info)
        {
            Server = server;
            Address = address;
            IpRange = ipRange;
            DnsSettings = dnsSettings;
            this.info = info;

            ExcludedIpRanges = new DhcpServerScopeExcludedIpRangeCollection(this);
            Options = new DhcpServerScopeOptionValueCollection(this);
            Reservations = new DhcpServerScopeReservationCollection(this);
            Clients = new DhcpServerScopeClientCollection(this);
        }

        public void Activate()
        {
            if (info.State != DhcpServerScopeState.Enabled)
            {
                var proposedInfo = info.UpdateState(DhcpServerScopeState.Enabled);
                SetInfo(proposedInfo);
            }
        }

        public void Deactivate()
        {
            if (info.State != DhcpServerScopeState.Disabled)
            {
                var proposedInfo = info.UpdateState(DhcpServerScopeState.Disabled);
                SetInfo(proposedInfo);
            }
        }

        /// <summary>
        /// Deletes this scope from the server
        /// </summary>
        /// <param name="retainClientDnsRecords">If true registered client DNS records are not removed. Useful in failover scenarios. Default = false</param>
        public void Delete(bool retainClientDnsRecords = false)
        {
            var flag = retainClientDnsRecords ? DHCP_FORCE_FLAG.DhcpFailoverForce : DHCP_FORCE_FLAG.DhcpFullForce;

            var result = Api.DhcpDeleteSubnet(ServerIpAddress: Server.IpAddress,
                                              SubnetAddress: Address.ToNativeAsNetwork(),
                                              ForceFlag: flag);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpDeleteSubnet), result);
        }

        private void SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (!name.Equals(info.Name, StringComparison.Ordinal))
            {
                var proposedInfo = info.UpdateName(name);
                SetInfo(proposedInfo);
            }
        }
        private void SetComment(string comment)
        {
            if (string.IsNullOrEmpty(comment))
                comment = string.Empty;

            if (!comment.Equals(info.Comment, StringComparison.Ordinal))
            {
                var proposedInfo = info.UpdateComment(comment);
                SetInfo(proposedInfo);
            }
        }

        internal static DhcpServerScope CreateScope(DhcpServer server, string name, DhcpServerIpRange ipRange)
            => CreateScope(server, name, description: null, ipRange, mask: ipRange.SmallestIpMask, timeDelayOffer: DefaultTimeDelayOffer, leaseDuration: DefaultLeaseDuration);
        internal static DhcpServerScope CreateScope(DhcpServer server, string name, string description, DhcpServerIpRange ipRange)
            => CreateScope(server, name, description, ipRange, mask: ipRange.SmallestIpMask, timeDelayOffer: DefaultTimeDelayOffer, leaseDuration: DefaultLeaseDuration);
        internal static DhcpServerScope CreateScope(DhcpServer server, string name, DhcpServerIpRange ipRange, DhcpServerIpMask mask)
            => CreateScope(server, name, description: null, ipRange, mask, timeDelayOffer: DefaultTimeDelayOffer, leaseDuration: DefaultLeaseDuration);
        internal static DhcpServerScope CreateScope(DhcpServer server, string name, string description, DhcpServerIpRange ipRange, DhcpServerIpMask mask)
            => CreateScope(server, name, description, ipRange, mask, timeDelayOffer: DefaultTimeDelayOffer, leaseDuration: DefaultLeaseDuration);
        internal static DhcpServerScope CreateScope(DhcpServer server, string name, DhcpServerIpRange ipRange, TimeSpan timeDelayOffer, TimeSpan? leaseDuration)
            => CreateScope(server, name, description: null, ipRange, mask: ipRange.SmallestIpMask, timeDelayOffer, leaseDuration);
        internal static DhcpServerScope CreateScope(DhcpServer server, string name, string description, DhcpServerIpRange ipRange, TimeSpan timeDelayOffer, TimeSpan? leaseDuration)
            => CreateScope(server, name, description, ipRange, mask: ipRange.SmallestIpMask, timeDelayOffer, leaseDuration);
        internal static DhcpServerScope CreateScope(DhcpServer server, string name, DhcpServerIpRange ipRange, DhcpServerIpMask mask, TimeSpan timeDelayOffer, TimeSpan? leaseDuration)
            => CreateScope(server, name, description: null, ipRange, mask, timeDelayOffer, leaseDuration);
        internal static DhcpServerScope CreateScope(DhcpServer server, string name, string description, DhcpServerIpRange ipRange, DhcpServerIpMask mask, TimeSpan timeDelayOffer, TimeSpan? leaseDuration)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (leaseDuration.HasValue && leaseDuration.Value.TotalMinutes < 1)
                throw new ArgumentOutOfRangeException(nameof(leaseDuration), "Lease duration can be unlimited (null) or at least 1 minute");
            if (ipRange.Type != DhcpServerIpRangeType.ScopeDhcpOnly && ipRange.Type != DhcpServerIpRangeType.ScopeDhcpAndBootp && ipRange.Type != DhcpServerIpRangeType.ScopeBootpOnly)
                throw new ArgumentOutOfRangeException(nameof(ipRange), "The IP Range must be of scope type (ScopeDhcpOnly, ScopeDhcpAndBootp or ScopeBootpOnly)");

            var maskRange = mask.GetIpRange(ipRange.StartAddress, DhcpServerIpRangeType.Excluded); // only for validation; use excluded range so the first and last addresses are included
            var subnetAddress = maskRange.StartAddress;

            if (maskRange.StartAddress == ipRange.StartAddress)
                throw new ArgumentOutOfRangeException(nameof(ipRange), "The starting address is not valid for this range. Subnet ID address cannot be included in the range.");
            if (maskRange.EndAddress == ipRange.EndAddress)
                throw new ArgumentOutOfRangeException(nameof(ipRange), "The ending address is not valid for this range. Subnet broadcast addresses cannot be included in the range.");
            if (maskRange.EndAddress < ipRange.EndAddress)
                throw new ArgumentOutOfRangeException(nameof(ipRange), "The range is not valid for this subnet mask.");

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

            // add ip range
            AddSubnetScopeIpRangeElement(server, subnetAddress, ipRange);

            // set time delay offer
            if (timeDelayOffer.TotalMilliseconds != 0)
                SetTimeDelayOffer(server, subnetAddress, timeDelayOffer);

            // set lease duration
            SetLeaseDuration(server, subnetAddress, leaseDuration);

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
            var info = GetInfo(server, address);
            var ipRange = EnumSubnetElements(server, address, DHCP_SUBNET_ELEMENT_TYPE.DhcpIpRangesDhcpBootp).First();
            var dnsSettings = DhcpServerDnsSettings.GetScopeDnsSettings(server, address);

            return new DhcpServerScope(server, address, ipRange, dnsSettings, info);
        }

        internal static IEnumerable<DhcpServerIpRange> EnumSubnetElements(DhcpServer server, DhcpServerIpAddress address, DHCP_SUBNET_ELEMENT_TYPE enumElementType)
        {
            if (server.IsCompatible(DhcpServerVersions.Windows2008))
                return EnumSubnetElementsV5(server, address, enumElementType);
            else
                return EnumSubnetElementsV0(server, address, enumElementType);
        }

        private static IEnumerable<DhcpServerIpRange> EnumSubnetElementsV0(DhcpServer server, DhcpServerIpAddress address, DHCP_SUBNET_ELEMENT_TYPE enumElementType)
        {
            var resumeHandle = IntPtr.Zero;
            var result = Api.DhcpEnumSubnetElements(ServerIpAddress: server.IpAddress,
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
                throw new DhcpServerException(nameof(Api.DhcpEnumSubnetElements), result);

            try
            {
                if (elementsRead == 0)
                    yield break;

                using (var elements = elementsPtr.MarshalToStructure<DHCP_SUBNET_ELEMENT_INFO_ARRAY>())
                {
                    foreach (var element in elements.Elements)
                        yield return DhcpServerIpRange.FromNative(element);
                }
            }
            finally
            {
                Api.FreePointer(elementsPtr);
            }
        }

        private static IEnumerable<DhcpServerIpRange> EnumSubnetElementsV5(DhcpServer server, DhcpServerIpAddress address, DHCP_SUBNET_ELEMENT_TYPE enumElementType)
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
                        yield return DhcpServerIpRange.FromNative(element);
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

        internal static void AddSubnetExcludedIpRangeElement(DhcpServer server, DhcpServerIpAddress address, DhcpServerIpRange range)
        {
            if (range.Type != DhcpServerIpRangeType.Excluded)
                throw new ArgumentOutOfRangeException($"{nameof(range)}.{nameof(range.Type)}", $"The expected range type is '{DhcpServerIpRangeType.Excluded}'");

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

        internal static void AddSubnetReservationElement(DhcpServer server, DhcpServerIpAddress scopeAddress, DhcpServerIpAddress reservationAddress, DhcpServerHardwareAddress hardwareAddress, DhcpServerClientTypes allowedClientTypes)
        {
            

            if (server.IsCompatible(DhcpServerVersions.Windows2003))
            {
                var reservation = new DHCP_IP_RESERVATION_V4_Managed(reservationAddress, hardwareAddress, allowedClientTypes);
                using (var element = new DHCP_SUBNET_ELEMENT_DATA_V5_Managed(DHCP_SUBNET_ELEMENT_TYPE.DhcpReservedIps, reservation))
                {
                    AddSubnetElementV5(server, scopeAddress, element);
                }
            }
            else
            {
                var reservation = new DHCP_IP_RESERVATION_Managed(reservationAddress, hardwareAddress);
                using (var element = new DHCP_SUBNET_ELEMENT_DATA_Managed(DHCP_SUBNET_ELEMENT_TYPE.DhcpReservedIps, reservation))
                {
                    AddSubnetElementV0(server, scopeAddress, element);
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

        internal static void RemoveSubnetExcludedIpRangeElement(DhcpServer server, DhcpServerIpAddress address, DhcpServerIpRange range)
        {
            if (range.Type != DhcpServerIpRangeType.Excluded)
                throw new ArgumentOutOfRangeException($"{nameof(range)}.{nameof(range.Type)}", $"The expected range type is '{DhcpServerIpRangeType.Excluded}'");

            using (var element = new DHCP_SUBNET_ELEMENT_DATA_Managed((DHCP_SUBNET_ELEMENT_TYPE)range.Type, range.ToNativeIpRange()))
            {
                RemoveSubnetElementV0(server, address, element);
            }
        }

        //private static void RemoveSubnetElementV5(DhcpServer server, DhcpServerIpAddress address, DHCP_SUBNET_ELEMENT_DATA_V5_Managed element)
        //{
        //    var elementRef = element;
        //    var result = Api.DhcpRemoveSubnetElementV5(server.IpAddress, address.ToNativeAsNetwork(), ref elementRef, DHCP_FORCE_FLAG.DhcpFullForce);

        //    if (result != DhcpErrors.SUCCESS)
        //        throw new DhcpServerException(nameof(Api.DhcpRemoveSubnetElementV5), result);
        //}

        private static void RemoveSubnetElementV0(DhcpServer server, DhcpServerIpAddress address, DHCP_SUBNET_ELEMENT_DATA_Managed element)
        {
            var elementRef = element;
            var result = Api.DhcpRemoveSubnetElement(server.IpAddress, address.ToNativeAsNetwork(), ref elementRef, DHCP_FORCE_FLAG.DhcpFullForce);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpRemoveSubnetElement), result);
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

        private static SubnetInfo GetInfo(DhcpServer server, DhcpServerIpAddress address)
        {
            if (server.IsCompatible(DhcpServerVersions.Windows2008R2))
                return GetInfoVQ(server, address);
            else
                return GetInfoV0(server, address);
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

        private void SetInfo(SubnetInfo info)
        {
            if (Server.IsCompatible(DhcpServerVersions.Windows2008R2))
                SetInfoVQ(info);
            else
                SetInfoV0(info);

            // update cache
            this.info = info;
        }

        private void SetInfoV0(SubnetInfo info)
        {
            var infoNative = info.ToNativeV0();
            var result = Api.DhcpSetSubnetInfo(ServerIpAddress: Server.IpAddress,
                                               SubnetAddress: Address.ToNativeAsNetwork(),
                                               SubnetInfo: ref infoNative);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpSetSubnetInfo), result);
        }

        private void SetInfoVQ(SubnetInfo info)
        {
            var infoNative = info.ToNativeVQ();
            var result = Api.DhcpSetSubnetInfoVQ(ServerIpAddress: Server.IpAddress,
                                                 SubnetAddress: Address.ToNativeAsNetwork(),
                                                 SubnetInfo: ref infoNative);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpSetSubnetInfoVQ), result);
        }

        public override string ToString() => $"Scope [{Address}] {Name} ({Comment})";

        private class SubnetInfo
        {
            public readonly DHCP_IP_ADDRESS SubnetAddress;
            public readonly DhcpServerIpMask Mask;
            public readonly string Name;
            public readonly string Comment;
            public readonly DhcpServerHost PrimaryHost;
            public readonly DhcpServerScopeState State;
            public bool QuarantineOn => vqQuarantineOn != 0;

            private readonly uint vqQuarantineOn;
            private readonly uint vqReserved1;
            private readonly uint vqReserved2;
            private readonly ulong vqReserved3;
            private readonly ulong vqReserved4;

            private SubnetInfo(DHCP_IP_ADDRESS subnetAddress, DhcpServerIpMask mask, string name, string comment, DhcpServerHost primaryHost, DhcpServerScopeState state)
            {
                SubnetAddress = subnetAddress;
                Mask = mask;
                Name = name;
                Comment = comment;
                PrimaryHost = primaryHost;
                State = state;
                vqQuarantineOn = 0;
            }

            private SubnetInfo(DHCP_IP_ADDRESS subnetAddress, DhcpServerIpMask mask, string name, string comment, DhcpServerHost primaryHost, DhcpServerScopeState state, uint quarantineOn, uint reserved1, uint reserved2, ulong reserved3, ulong reserved4)
                : this(subnetAddress, mask, name, comment, primaryHost, state)
            {
                vqQuarantineOn = quarantineOn;
                vqReserved1 = reserved1;
                vqReserved2 = reserved2;
                vqReserved3 = reserved3;
                vqReserved4 = reserved4;
            }

            public static SubnetInfo FromNative(DHCP_SUBNET_INFO info)
            {
                return new SubnetInfo(subnetAddress: info.SubnetAddress,
                                      mask: info.SubnetMask.AsNetworkToIpMask(),
                                      name: info.SubnetName,
                                      comment: info.SubnetComment,
                                      primaryHost: DhcpServerHost.FromNative(info.PrimaryHost),
                                      state: (DhcpServerScopeState)info.SubnetState);
            }

            public static SubnetInfo FromNative(DHCP_SUBNET_INFO_VQ info)
            {
                return new SubnetInfo(subnetAddress: info.SubnetAddress,
                                      mask: info.SubnetMask.AsNetworkToIpMask(),
                                      name: info.SubnetName,
                                      comment: info.SubnetComment,
                                      primaryHost: DhcpServerHost.FromNative(info.PrimaryHost),
                                      state: (DhcpServerScopeState)info.SubnetState,
                                      quarantineOn: info.QuarantineOn,
                                      reserved1: info.Reserved1,
                                      reserved2: info.Reserved2,
                                      reserved3: info.Reserved3,
                                      reserved4: info.Reserved4);
            }

            public SubnetInfo UpdateName(string name)
                => new SubnetInfo(SubnetAddress, Mask, name, Comment, PrimaryHost, State, vqQuarantineOn, vqReserved1, vqReserved2, vqReserved3, vqReserved4);
            public SubnetInfo UpdateComment(string comment)
                => new SubnetInfo(SubnetAddress, Mask, Name, comment, PrimaryHost, State, vqQuarantineOn, vqReserved1, vqReserved2, vqReserved3, vqReserved4);
            public SubnetInfo UpdateState(DhcpServerScopeState state)
                => new SubnetInfo(SubnetAddress, Mask, Name, Comment, PrimaryHost, state, vqQuarantineOn, vqReserved1, vqReserved2, vqReserved3, vqReserved4);

            public DHCP_SUBNET_INFO_Managed ToNativeV0()
                => new DHCP_SUBNET_INFO_Managed(SubnetAddress, Mask.ToNativeAsNetwork(), Name, Comment, PrimaryHost.ToNative(), (DHCP_SUBNET_STATE)State);

            public DHCP_SUBNET_INFO_VQ_Managed ToNativeVQ()
                => new DHCP_SUBNET_INFO_VQ_Managed(SubnetAddress, Mask.ToNativeAsNetwork(), Name, Comment, PrimaryHost.ToNative(), (DHCP_SUBNET_STATE)State, vqQuarantineOn, vqReserved1, vqReserved2, vqReserved3, vqReserved4);
        }
    }
}
