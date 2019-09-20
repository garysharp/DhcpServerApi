using System;
using System.Collections.Generic;
using System.Linq;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerScope : IDhcpServerScope
    {
        private SubnetInfo info = null;
        private DhcpServerIpRange? ipRange = null;
        private DhcpServerDnsSettings dnsSettings = null;
        private bool failoverRelationshipFetched = false;
        private DhcpServerFailoverRelationship failoverRelationship = null;

        /// <summary>
        /// Default time delay offer of 0 milliseconds
        /// </summary>
        public static TimeSpan DefaultTimeDelayOffer => TimeSpan.FromMilliseconds(0);
        /// <summary>
        /// Default lease duration of 8 days
        /// </summary>
        public static TimeSpan DefaultLeaseDuration => TimeSpan.FromDays(8);

        public DhcpServer Server { get; }
        IDhcpServer IDhcpServerScope.Server => Server;
        public DhcpServerIpAddress Address { get; }

        private SubnetInfo Info => info ??= GetInfo(Server, Address);

        public DhcpServerIpMask Mask => Info.Mask;
        public string Name
        {
            get => Info.Name;
            set => SetName(value);
        }
        public string Comment
        {
            get => Info.Comment;
            set => SetComment(value);
        }
        public DhcpServerScopeState State => Info.State;
        
        public DhcpServerIpRange IpRange
        {
            get => (ipRange ??= GetIpRange()).Value;
            set => SetIpRange(value);
        }

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

        public IDhcpServerHost PrimaryHost => Info.PrimaryHost;
        public IDhcpServerDnsSettings DnsSettings => (dnsSettings ??= DhcpServerDnsSettings.GetScopeDnsSettings(this)).Clone();
        public bool QuarantineOn => Info.QuarantineOn;

        public IDhcpServerFailoverRelationship FailoverRelationship
        {
            get
            {
                if (!failoverRelationshipFetched)
                {
                    failoverRelationship = DhcpServerFailoverRelationship.GetFailoverRelationship(Server, Address);
                    failoverRelationshipFetched = true;
                }
                return failoverRelationship;
            }
        }
        public IDhcpServerScopeFailoverStatistics FailoverStatistics
            => DhcpServerScopeFailoverStatistics.GetScopeFailoverStatistics(Server, this);

        /// <summary>
        /// Scope Excluded IP Ranges
        /// </summary>
        public IDhcpServerScopeExcludedIpRangeCollection ExcludedIpRanges { get; }

        /// <summary>
        /// Scope Option Values
        /// </summary>
        public IDhcpServerScopeOptionValueCollection Options { get; }

        /// <summary>
        /// Scope Reservations
        /// </summary>
        public IDhcpServerScopeReservationCollection Reservations { get; }

        /// <summary>
        /// Scope Clients
        /// </summary>
        public IDhcpServerScopeClientCollection Clients { get; }

        private DhcpServerScope(DhcpServer server, DhcpServerIpAddress address)
        {
            Server = server;
            Address = address;

            ExcludedIpRanges = new DhcpServerScopeExcludedIpRangeCollection(this);
            Options = new DhcpServerScopeOptionValueCollection(this);
            Reservations = new DhcpServerScopeReservationCollection(this);
            Clients = new DhcpServerScopeClientCollection(this);
        }

        private DhcpServerScope(DhcpServer server, DhcpServerIpAddress address, DhcpServerFailoverRelationship failoverRelationship)
            : this(server, address)
        {
            this.failoverRelationship = failoverRelationship;
            failoverRelationshipFetched = true;
        }

        private DhcpServerScope(DhcpServer server, DhcpServerIpAddress address, SubnetInfo info)
            : this(server, address)
        {
            this.info = info;
        }

        public void Activate()
        {
            if (Info.State != DhcpServerScopeState.Enabled)
            {
                var proposedInfo = Info.UpdateState(DhcpServerScopeState.Enabled);
                SetInfo(proposedInfo);
            }
        }

        public void Deactivate()
        {
            if (Info.State != DhcpServerScopeState.Disabled)
            {
                var proposedInfo = Info.UpdateState(DhcpServerScopeState.Disabled);
                SetInfo(proposedInfo);
            }
        }

        public IDhcpServerDnsSettings ConfigureDnsSettings(IDhcpServerDnsSettings dnsSettings)
        {
            if (dnsSettings == null)
            {
                // remove DNS settings at this level (scope - returns to global)
                DhcpServerDnsSettings.RemoveScopeDnsSettings(this);
                return (this.dnsSettings = (DhcpServerDnsSettings)Server.DnsSettings).Clone();
            }
            else
            {
                return (this.dnsSettings = DhcpServerDnsSettings.SetScopeDnsSettings(this, (DhcpServerDnsSettings)dnsSettings)).Clone();
            }
        }

        /// <summary>
        /// Deletes this scope from the server
        /// </summary>
        /// <param name="retainClientDnsRecords">If true registered client DNS records are not removed. Useful in failover scenarios. Default = false</param>
        public void Delete(bool retainClientDnsRecords = false)
        {
            var flag = retainClientDnsRecords ? DHCP_FORCE_FLAG.DhcpFailoverForce : DHCP_FORCE_FLAG.DhcpFullForce;

            var result = Api.DhcpDeleteSubnet(ServerIpAddress: Server.Address,
                                              SubnetAddress: Address.ToNativeAsNetwork(),
                                              ForceFlag: flag);

            if (result != DhcpServerNativeErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpDeleteSubnet), result);
        }

        public void ReplicateFailoverPartner()
            => DhcpServerFailoverRelationship.ReplicateScopeRelationship((DhcpServerFailoverRelationship)FailoverRelationship, this);

        /// <summary>
        /// Configures a failover relationship for this scope
        /// </summary>
        /// <param name="partnerServer">The failover partner server</param>
        /// <param name="sharedSecret">Secret used by the relationship</param>
        /// <param name="mode">Failover mode to configure</param>
        /// <returns>The created failover relationship</returns>
        public IDhcpServerFailoverRelationship ConfigureFailover(IDhcpServer partnerServer, string sharedSecret, DhcpServerFailoverMode mode)
        {
            failoverRelationship = DhcpServerFailoverRelationship.CreateFailoverRelationship(this, (DhcpServer)partnerServer, name: null, sharedSecret, mode);
            failoverRelationshipFetched = true;
            return failoverRelationship;
        }

        /// <summary>
        /// Configures a failover relationship for this scope
        /// </summary>
        /// <param name="partnerServer">The failover partner server</param>
        /// <param name="name">Name of the failover relationship</param>
        /// <param name="sharedSecret">Secret used by the relationship</param>
        /// <param name="mode">Failover mode to configure</param>
        /// <returns>The created failover relationship</returns>
        public IDhcpServerFailoverRelationship ConfigureFailover(IDhcpServer partnerServer, string name, string sharedSecret, DhcpServerFailoverMode mode)
        {
            failoverRelationship = DhcpServerFailoverRelationship.CreateFailoverRelationship(this, (DhcpServer)partnerServer, name, sharedSecret, mode);
            failoverRelationshipFetched = true;
            return failoverRelationship;
        }


        /// <summary>
        /// Configures a failover relationship for this scope
        /// </summary>
        /// <param name="partnerServer">The failover partner server</param>
        /// <param name="sharedSecret">Secret used by the relationship</param>
        /// <param name="mode">Failover mode to configure</param>
        /// <param name="modePercentage">Percentage argument for the failover mode</param>
        /// <returns>The created failover relationship</returns>
        public IDhcpServerFailoverRelationship ConfigureFailover(IDhcpServer partnerServer, string sharedSecret, DhcpServerFailoverMode mode, byte modePercentage)
        {
            failoverRelationship = DhcpServerFailoverRelationship.CreateFailoverRelationship(this, (DhcpServer)partnerServer, name: null, sharedSecret, mode, modePercentage);
            failoverRelationshipFetched = true;
            return failoverRelationship;
        }

        /// <summary>
        /// Configures a failover relationship for this scope
        /// </summary>
        /// <param name="partnerServer">The failover partner server</param>
        /// <param name="name">Name of the failover relationship</param>
        /// <param name="sharedSecret">Secret used by the relationship</param>
        /// <param name="mode">Failover mode to configure</param>
        /// <param name="modePercentage">Percentage argument for the failover mode</param>
        /// <returns>The created failover relationship</returns>
        public IDhcpServerFailoverRelationship ConfigureFailover(IDhcpServer partnerServer, string name, string sharedSecret, DhcpServerFailoverMode mode, byte modePercentage)
        {
            failoverRelationship = DhcpServerFailoverRelationship.CreateFailoverRelationship(this, (DhcpServer)partnerServer, name, sharedSecret, mode, modePercentage);
            failoverRelationshipFetched = true;
            return failoverRelationship;
        }

        /// <summary>
        /// Configures a failover relationship for this scope
        /// </summary>
        /// <param name="partnerServer">The failover partner server</param>
        /// <param name="name">Name of the failover relationship</param>
        /// <param name="sharedSecret">Secret used by the relationship</param>
        /// <param name="mode">Failover mode to configure</param>
        /// <param name="modePercentage">Percentage argument for the failover mode</param>
        /// <param name="maximumClientLeadTime">Maximum client lead time</param>
        /// <param name="stateSwitchInterval">State switch interval or null to disable</param>
        /// <returns>The created failover relationship</returns>
        public IDhcpServerFailoverRelationship ConfigureFailover(IDhcpServer partnerServer, string sharedSecret, DhcpServerFailoverMode mode, byte modePercentage, TimeSpan maximumClientLeadTime, TimeSpan? stateSwitchInterval)
        {
            failoverRelationship = DhcpServerFailoverRelationship.CreateFailoverRelationship(this, (DhcpServer)partnerServer, name: null, sharedSecret, mode, modePercentage, maximumClientLeadTime, stateSwitchInterval);
            failoverRelationshipFetched = true;
            return failoverRelationship;
        }

        /// <summary>
        /// Configures a failover relationship for this scope
        /// </summary>
        /// <param name="partnerServer">The failover partner server</param>
        /// <param name="name">Name of the failover relationship</param>
        /// <param name="sharedSecret">Secret used by the relationship</param>
        /// <param name="mode">Failover mode to configure</param>
        /// <param name="modePercentage">Percentage argument for the failover mode</param>
        /// <param name="maximumClientLeadTime">Maximum client lead time</param>
        /// <param name="stateSwitchInterval">State switch interval or null to disable</param>
        /// <returns>The created failover relationship</returns>
        public IDhcpServerFailoverRelationship ConfigureFailover(IDhcpServer partnerServer, string name, string sharedSecret, DhcpServerFailoverMode mode, byte modePercentage, TimeSpan maximumClientLeadTime, TimeSpan? stateSwitchInterval)
        {
            failoverRelationship = DhcpServerFailoverRelationship.CreateFailoverRelationship(this, (DhcpServer)partnerServer, name, sharedSecret, mode, modePercentage, maximumClientLeadTime, stateSwitchInterval);
            failoverRelationshipFetched = true;
            return failoverRelationship;
        }

        /// <summary>
        /// Adds this scope to an existing failover relationship
        /// </summary>
        /// <param name="failoverRelationship">Failover relationship into which this failover relationship is to be added</param>
        public void ConfigureFailover(IDhcpServerFailoverRelationship failoverRelationship)
        {
            DhcpServerFailoverRelationship.AddScopeToFailoverRelationship((DhcpServerFailoverRelationship)failoverRelationship, this);
            this.failoverRelationship = (DhcpServerFailoverRelationship)failoverRelationship;
            failoverRelationshipFetched = true;
        }

        /// <summary>
        /// Removes the scope from its failover relationship. This will delete the scope from the partner server.
        /// </summary>
        public void DeconfigureFailover()
        {
            DhcpServerFailoverRelationship.DeconfigureScopeFailover(this);
            failoverRelationship = null;
            failoverRelationshipFetched = true;
        }

        private void SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (!name.Equals(Info.Name, StringComparison.Ordinal))
            {
                var proposedInfo = Info.UpdateName(name);
                SetInfo(proposedInfo);
            }
        }
        private void SetComment(string comment)
        {
            if (string.IsNullOrEmpty(comment))
                comment = string.Empty;

            if (!comment.Equals(Info.Comment, StringComparison.Ordinal))
            {
                var proposedInfo = Info.UpdateComment(comment);
                SetInfo(proposedInfo);
            }
        }

        private DhcpServerIpRange GetIpRange()
        {
            return EnumSubnetElements(Server, Address, DHCP_SUBNET_ELEMENT_TYPE.DhcpIpRangesDhcpBootp).First();
        }

        private void SetIpRange(DhcpServerIpRange ipRange)
        {
            if (IpRange != ipRange)
            {
                var scopeRange = Mask.GetDhcpIpRange(Address);

                if (!scopeRange.Contains(ipRange))
                    throw new ArgumentOutOfRangeException(nameof(ipRange), "The supplied range is invalid for this subnet");

                AddSubnetScopeIpRangeElement(Server, Address, ipRange);

                // update cache
                this.ipRange = ipRange;
            }
        }

        internal void ReplicateTo(DhcpServerScope destinationScope)
        {
            // can only replicate identical subnets
            if (Address != destinationScope.Address)
                throw new ArgumentException("Scopes are incompatible and cannot be replicated", nameof(destinationScope));

            // replicate properties
            if (!Name.Equals(destinationScope.Name, StringComparison.Ordinal) || !Comment.Equals(destinationScope.Comment, StringComparison.Ordinal) || QuarantineOn != destinationScope.QuarantineOn)
            {
                var scopeInfo = destinationScope.Info.Replicate(Info);
                destinationScope.SetInfo(scopeInfo);
            }

            // replicate address
            if (IpRange != destinationScope.IpRange)
                destinationScope.IpRange = IpRange;

            // replicate exclusions
            var destExclusions = destinationScope.ExcludedIpRanges.ToList();
            var srcExclusions = ExcludedIpRanges.ToList();
            // remove exclusions
            foreach (var range in destExclusions.Except(srcExclusions))
                destinationScope.ExcludedIpRanges.RemoveExcludedIpRange(range);
            // add exclusions
            foreach (var range in srcExclusions.Except(destExclusions))
                destinationScope.ExcludedIpRanges.AddExcludedIpRange(range);

            // replicate option values
            var destOptions = destinationScope.Options.ToDictionary(o => o.OptionId);
            var srcOptions = Options.ToDictionary(o => o.OptionId);
            // remove option values
            foreach (var optionId in destOptions.Keys.Except(srcOptions.Keys))
                destinationScope.Options.RemoveOptionValue(optionId);
            // add option values
            foreach (var optionId in srcOptions.Keys.Except(destOptions.Keys))
                destinationScope.Options.AddOrSetOptionValue(srcOptions[optionId]);
            // update option values
            foreach (var optionId in srcOptions.Keys.Intersect(destOptions.Keys))
            {
                var srcOption = srcOptions[optionId];
                var dstOption = destOptions[optionId];
                if (!Enumerable.SequenceEqual(srcOption.Values, dstOption.Values))
                    destinationScope.Options.AddOrSetOptionValue(srcOption);
            }

            // replicate reservations
            var destReservations = destinationScope.Reservations.ToDictionary(r => r.Address);
            var srcReservations = Reservations.ToDictionary(r => r.Address);
            // remove updated
            foreach (var address in srcReservations.Keys.Intersect(destReservations.Keys))
            {
                // remove from destination (and re-create later)
                if (srcReservations[address].HardwareAddress != destReservations[address].HardwareAddress)
                {
                    var destReservation = destReservations[address];
                    destReservation.Delete();
                    destReservations.Remove(address);
                }
            }
            // remove reservations
            foreach (var address in destReservations.Keys.Except(srcReservations.Keys))
                destReservations[address].Delete();
            // add reservations
            foreach (var address in srcReservations.Keys.Except(destReservations.Keys))
            {
                var srcReservation = srcReservations[address];
                var destReservation = destinationScope.Reservations.AddReservation(srcReservation.Address, srcReservation.HardwareAddress, srcReservation.AllowedClientTypes);
                foreach (var optionValue in srcReservation.Options.ToList())
                    destReservation.Options.AddOrSetOptionValue(optionValue);
            }
            // update reservation options
            foreach (var address in srcReservations.Keys.Intersect(destReservations.Keys))
            {
                var srcRes = srcReservations[address];
                var srcResOptions = srcRes.Options.ToDictionary(o => o.OptionId);
                var destRes = destReservations[address];
                var destResOptions = destRes.Options.ToDictionary(o => o.OptionId);

                // remove option values
                foreach (var optionId in destResOptions.Keys.Except(srcResOptions.Keys))
                    destRes.Options.RemoveOptionValue(optionId);
                // add option values
                foreach (var optionId in srcResOptions.Keys.Except(destResOptions.Keys))
                    destRes.Options.AddOrSetOptionValue(srcResOptions[optionId]);
                // update option values
                foreach (var optionId in srcResOptions.Keys.Intersect(destResOptions.Keys))
                {
                    var srcResOption = srcResOptions[optionId];
                    var destResOption = destResOptions[optionId];
                    if (!Enumerable.SequenceEqual(srcResOption.Values, destResOption.Values))
                        destRes.Options.AddOrSetOptionValue(srcResOption);
                }
            }
        }

        internal static DhcpServerScope CreateScope(DhcpServer server, string name, DhcpServerIpRange ipRange)
            => CreateScope(server, name, comment: null, ipRange, mask: ipRange.SmallestMask, timeDelayOffer: DefaultTimeDelayOffer, leaseDuration: DefaultLeaseDuration);
        internal static DhcpServerScope CreateScope(DhcpServer server, string name, string comment, DhcpServerIpRange ipRange)
            => CreateScope(server, name, comment, ipRange, mask: ipRange.SmallestMask, timeDelayOffer: DefaultTimeDelayOffer, leaseDuration: DefaultLeaseDuration);
        internal static DhcpServerScope CreateScope(DhcpServer server, string name, DhcpServerIpRange ipRange, DhcpServerIpMask mask)
            => CreateScope(server, name, comment: null, ipRange, mask, timeDelayOffer: DefaultTimeDelayOffer, leaseDuration: DefaultLeaseDuration);
        internal static DhcpServerScope CreateScope(DhcpServer server, string name, string comment, DhcpServerIpRange ipRange, DhcpServerIpMask mask)
            => CreateScope(server, name, comment, ipRange, mask, timeDelayOffer: DefaultTimeDelayOffer, leaseDuration: DefaultLeaseDuration);
        internal static DhcpServerScope CreateScope(DhcpServer server, string name, DhcpServerIpRange ipRange, TimeSpan timeDelayOffer, TimeSpan? leaseDuration)
            => CreateScope(server, name, comment: null, ipRange, mask: ipRange.SmallestMask, timeDelayOffer, leaseDuration);
        internal static DhcpServerScope CreateScope(DhcpServer server, string name, string comment, DhcpServerIpRange ipRange, TimeSpan timeDelayOffer, TimeSpan? leaseDuration)
            => CreateScope(server, name, comment, ipRange, mask: ipRange.SmallestMask, timeDelayOffer, leaseDuration);
        internal static DhcpServerScope CreateScope(DhcpServer server, string name, DhcpServerIpRange ipRange, DhcpServerIpMask mask, TimeSpan timeDelayOffer, TimeSpan? leaseDuration)
            => CreateScope(server, name, comment: null, ipRange, mask, timeDelayOffer, leaseDuration);
        internal static DhcpServerScope CreateScope(DhcpServer server, string name, string comment, DhcpServerIpRange ipRange, DhcpServerIpMask mask, TimeSpan timeDelayOffer, TimeSpan? leaseDuration)
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

            var primaryHost = new DHCP_HOST_INFO_Managed(ipAddress: server.Address.ToNativeAsNetwork(), netBiosName: null, serverName: null);
            var scopeInfo = new DHCP_SUBNET_INFO_Managed(subnetAddress: subnetAddress.ToNativeAsNetwork(),
                                                         subnetMask: mask.ToNativeAsNetwork(),
                                                         subnetName: name,
                                                         subnetComment: comment,
                                                         primaryHost: primaryHost,
                                                         subnetState: DHCP_SUBNET_STATE.DhcpSubnetDisabled);

            var result = Api.DhcpCreateSubnet(ServerIpAddress: server.Address,
                                              SubnetAddress: subnetAddress.ToNativeAsNetwork(),
                                              SubnetInfo: in scopeInfo);

            if (result != DhcpServerNativeErrors.SUCCESS)
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

        internal static IEnumerable<DhcpServerScope> GetScopes(DhcpServer server, bool preloadClients, bool preloadFailoverRelationships)
        {
            var failoverRelationships = default(Dictionary<DhcpServerIpAddress, DhcpServerFailoverRelationship>);
            if (preloadFailoverRelationships)
            {
                failoverRelationships = DhcpServerFailoverRelationship.GetFailoverRelationships(server)
                        .SelectMany(r => r.ScopeAddresses, (r, a) => new KeyValuePair<DhcpServerIpAddress, DhcpServerFailoverRelationship>(a, r))
                        .ToDictionary(v => v.Key, v => v.Value);
            }

            var scopes = new List<DhcpServerScope>();
            var resumeHandle = IntPtr.Zero;
            var result = Api.DhcpEnumSubnets(ServerIpAddress: server.Address,
                                             ResumeHandle: ref resumeHandle,
                                             PreferredMaximum: 0xFFFFFFFF,
                                             EnumInfo: out var enumInfoPtr,
                                             ElementsRead: out var elementsRead,
                                             ElementsTotal: out _);

            if (result == DhcpServerNativeErrors.ERROR_NO_MORE_ITEMS || result == DhcpServerNativeErrors.EPT_S_NOT_REGISTERED)
                return scopes;

            if (result != DhcpServerNativeErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpEnumSubnets), result);

            try
            {
                if (elementsRead == 0)
                    return scopes;

                using (var enumInfo = enumInfoPtr.MarshalToStructure<DHCP_IP_ARRAY>())
                {
                    if (preloadFailoverRelationships && failoverRelationships?.Count > 0)
                    {
                        foreach (var scopeAddress in enumInfo.Elements)
                        {
                            var address = scopeAddress.AsNetworkToIpAddress();
                            failoverRelationships.TryGetValue(address, out var failoverRelationship);

                            scopes.Add(new DhcpServerScope(server, address, failoverRelationship));
                        }
                    }
                    else
                    {
                        foreach (var scopeAddress in enumInfo.Elements)
                            scopes.Add(new DhcpServerScope(server, scopeAddress.AsNetworkToIpAddress()));
                    }
                }
            }
            finally
            {
                Api.FreePointer(enumInfoPtr);
            }

            if (preloadClients)
            {
                var scopeLookup = scopes.ToDictionary(s => s.Address);
                foreach (var clientGroup in DhcpServerClient.GetClients(server, scopeLookup).GroupBy(c => c.Scope))
                {
                    var scopeClients = (DhcpServerScopeClientCollection)clientGroup.Key.Clients;
                    scopeClients.clientCache = clientGroup.ToList();
                }
            }

            return scopes;
        }

        internal static DhcpServerScope GetScope(DhcpServer server, DhcpServerIpAddress address)
        {
            // use GetInfo to ensure the scope exists (when loading individual scopes)
            var info = GetInfo(server, address);
            return new DhcpServerScope(server, address, info);
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
            var result = Api.DhcpEnumSubnetElements(ServerIpAddress: server.Address,
                                                    SubnetAddress: address.ToNativeAsNetwork(),
                                                    EnumElementType: enumElementType,
                                                    ResumeHandle: ref resumeHandle,
                                                    PreferredMaximum: 0xFFFFFFFF,
                                                    EnumElementInfo: out var elementsPtr,
                                                    ElementsRead: out var elementsRead,
                                                    ElementsTotal: out _);

            if (result == DhcpServerNativeErrors.ERROR_NO_MORE_ITEMS)
                yield break;

            if (result != DhcpServerNativeErrors.SUCCESS && result != DhcpServerNativeErrors.ERROR_MORE_DATA)
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
            var result = Api.DhcpEnumSubnetElementsV5(ServerIpAddress: server.Address,
                                                      SubnetAddress: address.ToNativeAsNetwork(),
                                                      EnumElementType: enumElementType,
                                                      ResumeHandle: ref resumeHandle,
                                                      PreferredMaximum: 0xFFFFFFFF,
                                                      EnumElementInfo: out var elementsPtr,
                                                      ElementsRead: out var elementsRead,
                                                      ElementsTotal: out _);

            if (result == DhcpServerNativeErrors.ERROR_NO_MORE_ITEMS)
                yield break;

            if (result != DhcpServerNativeErrors.SUCCESS && result != DhcpServerNativeErrors.ERROR_MORE_DATA)
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
            var result = Api.DhcpAddSubnetElementV5(server.Address, address.ToNativeAsNetwork(), in element);

            if (result != DhcpServerNativeErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpAddSubnetElementV5), result);
        }

        private static void AddSubnetElementV0(DhcpServer server, DhcpServerIpAddress address, DHCP_SUBNET_ELEMENT_DATA_Managed element)
        {
            var result = Api.DhcpAddSubnetElement(server.Address, address.ToNativeAsNetwork(), in element);

            if (result != DhcpServerNativeErrors.SUCCESS)
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

        internal static void RemoveSubnetReservationElement(DhcpServer server, DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress)
        {
            using (var element = new DHCP_SUBNET_ELEMENT_DATA_Managed(DHCP_SUBNET_ELEMENT_TYPE.DhcpReservedIps, new DHCP_IP_RESERVATION_Managed(address, hardwareAddress)))
            {
                RemoveSubnetElementV0(server, address, element);
            }
        }

        private static void RemoveSubnetElementV0(DhcpServer server, DhcpServerIpAddress address, DHCP_SUBNET_ELEMENT_DATA_Managed element)
        {
            var result = Api.DhcpRemoveSubnetElement(server.Address, address.ToNativeAsNetwork(), in element, DHCP_FORCE_FLAG.DhcpFullForce);

            if (result != DhcpServerNativeErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpRemoveSubnetElement), result);
        }

        private static TimeSpan GetTimeDelayOffer(DhcpServer server, DhcpServerIpAddress address)
        {
            var result = Api.DhcpGetSubnetDelayOffer(ServerIpAddress: server.Address,
                                                     SubnetAddress: address.ToNativeAsNetwork(),
                                                     TimeDelayInMilliseconds: out var timeDelay);

            if (result != DhcpServerNativeErrors.SUCCESS)
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

            var result = Api.DhcpSetSubnetDelayOffer(ServerIpAddress: server.Address,
                                                     SubnetAddress: address.ToNativeAsNetwork(),
                                                     TimeDelayInMilliseconds: timeDelayOfferMilliseconds);

            if (result != DhcpServerNativeErrors.SUCCESS)
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
            catch (DhcpServerException ex) when (ex.ApiErrorNative == DhcpServerNativeErrors.ERROR_FILE_NOT_FOUND)
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
            var result = Api.DhcpGetSubnetInfo(ServerIpAddress: server.Address,
                                               SubnetAddress: address.ToNativeAsNetwork(),
                                               SubnetInfo: out var subnetInfoPtr);

            if (result != DhcpServerNativeErrors.SUCCESS)
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
            var result = Api.DhcpGetSubnetInfoVQ(ServerIpAddress: server.Address,
                                                 SubnetAddress: address.ToNativeAsNetwork(),
                                                 SubnetInfo: out var subnetInfoPtr);

            if (result != DhcpServerNativeErrors.SUCCESS)
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
            var result = Api.DhcpSetSubnetInfo(ServerIpAddress: Server.Address,
                                               SubnetAddress: Address.ToNativeAsNetwork(),
                                               SubnetInfo: in infoNative);

            if (result != DhcpServerNativeErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpSetSubnetInfo), result);
        }

        private void SetInfoVQ(SubnetInfo info)
        {
            var infoNative = info.ToNativeVQ();
            var result = Api.DhcpSetSubnetInfoVQ(ServerIpAddress: Server.Address,
                                                 SubnetAddress: Address.ToNativeAsNetwork(),
                                                 SubnetInfo: in infoNative);

            if (result != DhcpServerNativeErrors.SUCCESS)
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

            public SubnetInfo Replicate(SubnetInfo partnerSubnetInfo)
                => new SubnetInfo(SubnetAddress, Mask, partnerSubnetInfo.Name, partnerSubnetInfo.Comment, PrimaryHost, State, partnerSubnetInfo.vqQuarantineOn, vqReserved1, vqReserved2, vqReserved3, vqReserved4);

            public DHCP_SUBNET_INFO_Managed ToNativeV0()
                => new DHCP_SUBNET_INFO_Managed(SubnetAddress, Mask.ToNativeAsNetwork(), Name, Comment, PrimaryHost.ToNative(), (DHCP_SUBNET_STATE)State);

            public DHCP_SUBNET_INFO_VQ_Managed ToNativeVQ()
                => new DHCP_SUBNET_INFO_VQ_Managed(SubnetAddress, Mask.ToNativeAsNetwork(), Name, Comment, PrimaryHost.ToNative(), (DHCP_SUBNET_STATE)State, vqQuarantineOn, vqReserved1, vqReserved2, vqReserved3, vqReserved4);
        }
    }
}
