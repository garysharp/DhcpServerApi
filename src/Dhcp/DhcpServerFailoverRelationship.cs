using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerFailoverRelationship
    {
        public DhcpServer Server { get; }

        public string Name { get; }

        public DhcpServerFailoverMode Mode { get; }
        public DhcpServerFailoverState State { get; }
        public DhcpServerFailoverState PreviousState { get; }

        public DhcpServerIpAddress PrimaryServerAddress { get; }
        public string PrimaryServerName { get; }
        public DhcpServerIpAddress SecondaryServerAddress { get; }
        public string SecondaryServerName { get; }
        public DhcpServerFailoverServerType ServerType { get; }
        public string SharedSecret { get; }

        public TimeSpan MaximumClientLeadTime { get; }
        public TimeSpan? StateSwitchoverInterval { get; }

        public byte? LoadBalancePercentage { get; }
        public byte? HotStandbyAddressesReservedPercentage { get; }

        private readonly List<DhcpServerIpAddress> scopeAddresses;
        public IEnumerable<DhcpServerScope> Scopes
        {
            get
            {
                // shortcut
                if (scopeAddresses?.Count == 0)
                    yield break;

                foreach (var scopeAddress in scopeAddresses)
                    yield return Server.Scopes.GetScope(scopeAddress);
            }
        }

        private DhcpServerFailoverRelationship(
            DhcpServer server,
            string name,
            DhcpServerFailoverMode mode,
            byte modePercentage,
            DhcpServerFailoverState state,
            DhcpServerFailoverState previousState,
            DhcpServerIpAddress primaryServerAddress,
            string primaryServerName,
            DhcpServerIpAddress secondaryServerAddress,
            string secondaryServerName,
            DhcpServerFailoverServerType serverType,
            string sharedSecret,
            TimeSpan maximumClientLeadTime,
            TimeSpan? stateSwitchInterval,
            List<DhcpServerIpAddress> scopeAddresses
            )
        {
            Server = server;
            Name = name;
            Mode = mode;
            State = state;
            PreviousState = previousState;
            PrimaryServerAddress = primaryServerAddress;
            PrimaryServerName = primaryServerName;
            SecondaryServerAddress = secondaryServerAddress;
            SecondaryServerName = secondaryServerName;
            ServerType = serverType;
            SharedSecret = sharedSecret;
            MaximumClientLeadTime = maximumClientLeadTime;
            StateSwitchoverInterval = stateSwitchInterval;
            this.scopeAddresses = scopeAddresses;

            switch (mode)
            {
                case DhcpServerFailoverMode.LoadBalance:
                    LoadBalancePercentage = modePercentage;
                    break;
                case DhcpServerFailoverMode.HotStandby:
                    HotStandbyAddressesReservedPercentage = modePercentage;
                    break;
            }
        }

        public DhcpServer ConnectToPartner()
        {
            if (ServerType == DhcpServerFailoverServerType.PrimaryServer)
                return DhcpServer.Connect(SecondaryServerAddress.ToString());
            else
                return DhcpServer.Connect(PrimaryServerAddress.ToString());
        }

        public void ReplicateRelationship()
            => ReplicateRelationship(this);

        public void RedistributesFreeAddresses()
        {
            var result = Api.DhcpV4FailoverTriggerAddrAllocation(ServerIpAddress: Server.Address,
                                                                 FailRelName: Name);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpV4FailoverTriggerAddrAllocation), result);
        }

        public void Delete()
            => DeleteFailoverRelationship(this);

        internal static DhcpServerFailoverRelationship GetFailoverRelationship(DhcpServer server, string relationshipName)
        {
            var result = Api.DhcpV4FailoverGetRelationship(ServerIpAddress: server.Address,
                                                           RelationshipName: relationshipName,
                                                           Relationship: out var relationshipPtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpV4FailoverGetRelationship), result);

            try
            {
                using (var relationship = relationshipPtr.MarshalToStructure<DHCP_FAILOVER_RELATIONSHIP>())
                {
                    var relationshipRef = relationship;
                    return FromNative(server, ref relationshipRef);
                }
            }
            finally
            {
                Api.FreePointer(relationshipPtr);
            }

        }

        internal static DhcpServerFailoverRelationship GetFailoverRelationship(DhcpServer server, DhcpServerIpAddress subnetAddress)
        {
            var result = Api.DhcpV4FailoverGetScopeRelationship(ServerIpAddress: server.Address,
                                                                ScopeId: subnetAddress.ToNativeAsNetwork(),
                                                                Relationship: out var relationshipPtr);

            if (result == DhcpErrors.FO_SCOPE_NOT_IN_RELATIONSHIP)
                return null;

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpV4FailoverGetScopeRelationship), result);

            try
            {
                using (var relationship = relationshipPtr.MarshalToStructure<DHCP_FAILOVER_RELATIONSHIP>())
                {
                    var relationshipRef = relationship;
                    return FromNative(server, ref relationshipRef);
                }
            }
            finally
            {
                Api.FreePointer(relationshipPtr);
            }

        }

        internal static IEnumerable<DhcpServerFailoverRelationship> GetFailoverRelationships(DhcpServer server)
        {
            var resultHandle = IntPtr.Zero;
            var result = Api.DhcpV4FailoverEnumRelationship(ServerIpAddress: server.Address,
                                                            ResumeHandle: ref resultHandle,
                                                            PreferredMaximum: 0x10000,
                                                            Relationship: out var relationshipsPtr,
                                                            RelationshipRead: out _,
                                                            RelationshipTotal: out _);

            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS)
                yield break;

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpV4FailoverEnumRelationship), result);

            try
            {
                using (var relationships = relationshipsPtr.MarshalToStructure<DHCP_FAILOVER_RELATIONSHIP_ARRAY>())
                {
                    foreach (var relationship in relationships.Relationships)
                    {
                        var elementRef = relationship;
                        yield return FromNative(server, ref elementRef);
                    }
                }
            }
            finally
            {
                Api.FreePointer(relationshipsPtr);
            }
        }

        internal static void AddScopeToFailoverRelationship(DhcpServerFailoverRelationship relationship, DhcpServerScope scope)
        {
            var partnerServer = relationship.ConnectToPartner();

            // create/replicate
            var partnerScope = CreateOrReplicatePartnerScope(partnerServer, scope);

            // determine relationship primary/secondary servers
            DhcpServer primaryServer;
            DhcpServer secondaryServer;
            if (relationship.ServerType == DhcpServerFailoverServerType.PrimaryServer)
            {
                primaryServer = relationship.Server;
                secondaryServer = partnerServer;
            }
            else
            {
                primaryServer = partnerServer;
                secondaryServer = relationship.Server;
            }

            using (var relationshipNative = new DHCP_FAILOVER_RELATIONSHIP_Managed(relationshipName: relationship.Name,
                                                                                   scopes: new DHCP_IP_ARRAY_Managed(scope.Address.ToNativeAsNetwork())))
            {
                var relationshipNativeRef = relationshipNative;

                // update relationship on primary server
                var result = Api.DhcpV4FailoverAddScopeToRelationship(primaryServer.Address, ref relationshipNativeRef);
                if (result != DhcpErrors.SUCCESS)
                    throw new DhcpServerException(nameof(Api.DhcpV4FailoverAddScopeToRelationship), result, "Failed to add scope to failover relationship on primary server");

                // update relationship on secondary server
                result = Api.DhcpV4FailoverAddScopeToRelationship(secondaryServer.Address, ref relationshipNativeRef);
                if (result != DhcpErrors.SUCCESS)
                    throw new DhcpServerException(nameof(Api.DhcpV4FailoverAddScopeToRelationship), result, "Failed to add scope to failover relationship on secondary server");
            }

            // update local cache
            relationship.scopeAddresses.Add(scope.Address);

            // activate scope on partner (if the scope is active on primary)
            if (scope.State == DhcpServerScopeState.Enabled || scope.State == DhcpServerScopeState.EnabledSwitched)
            {
                partnerScope = partnerServer.Scopes.GetScope(partnerScope.Address);
                partnerScope.Activate();
            }
        }

        internal static DhcpServerFailoverRelationship CreateFailoverRelationship(DhcpServerScope scope, DhcpServer partnerServer, string name, string sharedSecret, DhcpServerFailoverMode mode)
            => CreateFailoverRelationship(scope, partnerServer, name, sharedSecret, mode, mode == DhcpServerFailoverMode.HotStandby ? (byte)5 : (byte)50);

        internal static DhcpServerFailoverRelationship CreateFailoverRelationship(DhcpServerScope scope, DhcpServer partnerServer, string name, string sharedSecret, DhcpServerFailoverMode mode, byte modePercentage)
            => CreateFailoverRelationship(scope, partnerServer, name, sharedSecret, mode, modePercentage, TimeSpan.FromHours(1), null);

        internal static DhcpServerFailoverRelationship CreateFailoverRelationship(DhcpServerScope scope, DhcpServer partnerServer, string name, string sharedSecret, DhcpServerFailoverMode mode, byte modePercentage, TimeSpan maximumClientLeadTime, TimeSpan? stateSwitchInterval)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            if (partnerServer == null)
                throw new ArgumentNullException(nameof(partnerServer));
            if (string.IsNullOrWhiteSpace(sharedSecret))
                throw new ArgumentNullException(nameof(sharedSecret));

            if (string.IsNullOrWhiteSpace(name))
                name = BuildRelationshipName(scope.Server, partnerServer);

            if (!scope.Server.IsCompatible(DhcpServerVersions.Windows2012) || !partnerServer.IsCompatible(DhcpServerVersions.Windows2012))
                throw new DhcpServerException(nameof(Api.DhcpV4FailoverCreateRelationship), DhcpErrors.ERROR_INVALID_PARAMETER, "Windows Server 2012 is required to establish a failover relationship");

            var primaryServerAddress = scope.Server.Address;
            if (primaryServerAddress == (DhcpServerIpAddress)0x7F_00_00_01) // localhost: 127.0.0.1
                primaryServerAddress = scope.Server.BindingElements.First().AdapterPrimaryIpAddress;

            var existingRelationship = scope.GetFailoverRelationship();
            if (existingRelationship != null)
                throw new DhcpServerException(nameof(Api.DhcpV4FailoverCreateRelationship), DhcpErrors.FO_SCOPE_ALREADY_IN_RELATIONSHIP);

            // create/replicate
            var partnerScope = CreateOrReplicatePartnerScope(partnerServer, scope);

            // define relationship
            using (var primaryRelationship = new DHCP_FAILOVER_RELATIONSHIP_Managed(primaryServer: primaryServerAddress.ToNativeAsNetwork(),
                                                                             secondaryServer: partnerServer.Address.ToNativeAsNetwork(),
                                                                             mode: (DHCP_FAILOVER_MODE)mode,
                                                                             serverType: DHCP_FAILOVER_SERVER.PrimaryServer,
                                                                             state: FSM_STATE.NO_STATE,
                                                                             prevState: FSM_STATE.NO_STATE,
                                                                             mclt: (int)maximumClientLeadTime.TotalSeconds,
                                                                             safePeriod: (int)(stateSwitchInterval?.TotalSeconds ?? -1),
                                                                             relationshipName: name,
                                                                             primaryServerName: scope.Server.Name,
                                                                             secondaryServerName: partnerServer.Name,
                                                                             scopes: new DHCP_IP_ARRAY_Managed(scope.Address.ToNativeAsNetwork()),
                                                                             percentage: modePercentage,
                                                                             sharedSecret: sharedSecret))
            {
                var primaryRelationshipRef = primaryRelationship;

                // create relationship on partner server
                var partnerRelationship = primaryRelationship.InvertRelationship();
                var result = Api.DhcpV4FailoverCreateRelationship(partnerServer.Address, ref partnerRelationship);
                if (result != DhcpErrors.SUCCESS)
                    throw new DhcpServerException(nameof(Api.DhcpV4FailoverCreateRelationship), result, "Failed to create relationship on partner server");

                // create relationship on primary server
                result = Api.DhcpV4FailoverCreateRelationship(scope.Server.Address, ref primaryRelationshipRef);
                if (result != DhcpErrors.SUCCESS)
                    throw new DhcpServerException(nameof(Api.DhcpV4FailoverCreateRelationship), result, "Failed to create relationship on primary server");

                // activate scope on partner (if the scope is active on primary)
                if (scope.State == DhcpServerScopeState.Enabled || scope.State == DhcpServerScopeState.EnabledSwitched)
                {
                    partnerScope = partnerServer.Scopes.GetScope(partnerScope.Address);
                    partnerScope.Activate();
                }

                return FromNative(scope.Server, ref primaryRelationshipRef);
            }
        }

        private static DhcpServerScope CreateOrReplicatePartnerScope(DhcpServer partnerServer, DhcpServerScope sourceScope)
        {
            // create scope on partner server (if it doesn't exist)
            DhcpServerScope partnerScope;
            try
            {
                // retrieve scope from partner
                partnerScope = partnerServer.Scopes.GetScope(sourceScope.Address);

                var existingRelationship = partnerScope.GetFailoverRelationship();
                if (existingRelationship != null)
                    throw new DhcpServerException(nameof(Api.DhcpV4FailoverCreateRelationship), DhcpErrors.FO_SCOPE_ALREADY_IN_RELATIONSHIP);

                // deactivate scope
                partnerScope.Deactivate();
            }
            catch (DhcpServerException ex) when (ex.ApiErrorNative == DhcpErrors.SUBNET_NOT_PRESENT)
            {
                // create scope (including excluded ranges)
                partnerScope = partnerServer.Scopes.AddScope(sourceScope.Name, sourceScope.Comment, sourceScope.IpRange, sourceScope.Mask, sourceScope.TimeDelayOffer, sourceScope.LeaseDuration);
            }

            // replicate scope
            sourceScope.ReplicateTo(partnerScope);

            return partnerScope;
        }

        private static void ReplicateRelationship(DhcpServerFailoverRelationship relationship)
        {
            var scopes = relationship.Scopes.ToList();
            var partner = relationship.ConnectToPartner();

            foreach (var sourceScope in scopes)
            {
                var destinationScope = partner.Scopes.GetScope(sourceScope.Address);

                sourceScope.ReplicateTo(destinationScope);
            }
        }

        internal static void ReplicateScopeRelationship(DhcpServerFailoverRelationship relationship, DhcpServerScope sourceScope)
        {
            var partner = relationship.ConnectToPartner();
            var destinationScope = partner.Scopes.GetScope(sourceScope.Address);

            sourceScope.ReplicateTo(destinationScope);
        }

        private static string BuildRelationshipName(DhcpServer primaryServer, DhcpServer secondaryServer)
        {
            var builder = new StringBuilder(primaryServer.Name.Length + secondaryServer.Name.Length + 1);

            var periodLocation = primaryServer.Name.IndexOf('.');
            builder.Append(primaryServer.Name, 0, periodLocation < 0 ? primaryServer.Name.Length : periodLocation);

            builder.Append('-');

            periodLocation = secondaryServer.Name.IndexOf('.');
            builder.Append(secondaryServer.Name, 0, periodLocation < 0 ? secondaryServer.Name.Length : periodLocation);

            return builder.ToString();
        }

        internal static void DeconfigureScopeFailover(DhcpServerScope scope)
        {
            var relationship = scope.GetFailoverRelationship();
            var partner = relationship.ConnectToPartner();
            var partnerScope = partner.Scopes.GetScope(scope.Address);

            // deactivate scope on the failover relationship
            partnerScope.Deactivate();

            using (var relationshipNative = new DHCP_FAILOVER_RELATIONSHIP_Managed(relationshipName: relationship.Name,
                                                                                   scopes: new DHCP_IP_ARRAY_Managed(scope.Address.ToNativeAsNetwork())))
            {
                var relationshipNativeRef = relationshipNative;

                // remove scope from failover relationship on partner server
                var result = Api.DhcpV4FailoverDeleteScopeFromRelationship(partner.Address, ref relationshipNativeRef);
                if (result != DhcpErrors.SUCCESS)
                    throw new DhcpServerException(nameof(Api.DhcpV4FailoverDeleteScopeFromRelationship), result, "Failed to delete scope from relationship on partner server");

                // remove scope from failover relationship on server
                result = Api.DhcpV4FailoverDeleteScopeFromRelationship(scope.Server.Address, ref relationshipNativeRef);
                if (result != DhcpErrors.SUCCESS)
                    throw new DhcpServerException(nameof(Api.DhcpV4FailoverDeleteScopeFromRelationship), result, "Failed to delete scope from relationship");
            }

            // delete scope on partner server
            partnerScope.Delete();
        }

        internal static void DeleteFailoverRelationship(DhcpServerFailoverRelationship relationship)
        {
            // refresh relationship instance
            relationship = relationship.Server.FailoverRelationships.GetRelationship(relationship.Name);

            if (relationship.scopeAddresses.Count > 0)
                throw new InvalidOperationException("This failover relationship contains configured scopes. Deconfigure the scopes before deleting the relationship.");

            var result = Api.DhcpV4FailoverDeleteRelationship(relationship.Server.Address, relationship.Name);
            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpV4FailoverDeleteScopeFromRelationship), result, "Failed to delete relationship");

            var partnerServer = relationship.ConnectToPartner();
            result = Api.DhcpV4FailoverDeleteRelationship(partnerServer.Address, relationship.Name);
            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpV4FailoverDeleteScopeFromRelationship), result, "Failed to delete relationship on partner server");
        }

        private static DhcpServerFailoverRelationship FromNative(DhcpServer server, ref DHCP_FAILOVER_RELATIONSHIP native)
        {
            return new DhcpServerFailoverRelationship(server: server,
                                                      name: native.RelationshipName,
                                                      mode: (DhcpServerFailoverMode)native.Mode,
                                                      modePercentage: native.Percentage,
                                                      state: (DhcpServerFailoverState)native.State,
                                                      previousState: (DhcpServerFailoverState)native.PrevState,
                                                      primaryServerAddress: native.PrimaryServer.AsNetworkToIpAddress(),
                                                      primaryServerName: native.PrimaryServerName,
                                                      secondaryServerAddress: native.SecondaryServer.AsNetworkToIpAddress(),
                                                      secondaryServerName: native.SecondaryServerName,
                                                      serverType: (DhcpServerFailoverServerType)native.ServerType,
                                                      sharedSecret: native.SharedSecret,
                                                      maximumClientLeadTime: TimeSpan.FromSeconds(native.Mclt),
                                                      stateSwitchInterval: native.SafePeriod >= 0 ? TimeSpan.FromSeconds(native.SafePeriod) : (TimeSpan?)null,
                                                      scopeAddresses: native.Scopes.Elements.Select(e => e.AsNetworkToIpAddress()).ToList());
        }

        private static DhcpServerFailoverRelationship FromNative(DhcpServer server, ref DHCP_FAILOVER_RELATIONSHIP_Managed native)
        {
            return new DhcpServerFailoverRelationship(server: server,
                                                      name: native.RelationshipName,
                                                      mode: (DhcpServerFailoverMode)native.Mode,
                                                      modePercentage: native.Percentage,
                                                      state: (DhcpServerFailoverState)native.State,
                                                      previousState: (DhcpServerFailoverState)native.PrevState,
                                                      primaryServerAddress: native.PrimaryServer.AsNetworkToIpAddress(),
                                                      primaryServerName: native.PrimaryServerName,
                                                      secondaryServerAddress: native.SecondaryServer.AsNetworkToIpAddress(),
                                                      secondaryServerName: native.SecondaryServerName,
                                                      serverType: (DhcpServerFailoverServerType)native.ServerType,
                                                      sharedSecret: native.SharedSecret,
                                                      maximumClientLeadTime: TimeSpan.FromSeconds(native.Mclt),
                                                      stateSwitchInterval: native.SafePeriod >= 0 ? TimeSpan.FromSeconds(native.SafePeriod) : (TimeSpan?)null,
                                                      scopeAddresses: native.Scopes.Elements.Select(e => e.AsNetworkToIpAddress()).ToList());
        }

        public override string ToString()
            => $"{Name}: [{Mode}; {ServerType}; {State}]";
    }
}
