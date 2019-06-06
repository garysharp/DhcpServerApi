using System;
using System.Collections.Generic;
using System.Linq;
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
        public byte? StandbyAddressesReservedPercentage { get; }

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
                    StandbyAddressesReservedPercentage = modePercentage;
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

        public override string ToString()
            => $"{Name}: [{Mode}; {ServerType}; {State}]";
    }
}
