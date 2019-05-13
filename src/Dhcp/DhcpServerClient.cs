using System;
using System.Collections.Generic;
using System.ComponentModel;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerClient
    {
        public DhcpServer Server { get; }

        public DhcpServerIpAddress IpAddress { get; }
        [Obsolete("Use IpAddress.Native instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public int IpAddressNative => (int)IpAddress.Native;

        public DhcpServerIpMask SubnetMask { get; }
        [Obsolete("Use SubnetMask.Native instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public int SubnetMaskNative => (int)SubnetMask.Native;

        public DhcpServerHardwareAddress HardwareAddress { get; }
        [Obsolete("Use HardwareAddress.Native instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public long HardwareAddressNative => throw new NotImplementedException("Use HardwareAddress.Native to access raw hardware address");
        [Obsolete("Use HardwareAddress.Native instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] HardwareAddressBytes => HardwareAddress.Native;

        public string Name { get; }
        public string Comment { get; }

        [Obsolete("Caused confusion. Use LeaseExpiresUtc or LeaseExpiresLocal instead.")]
        public DateTime LeaseExpires => LeaseExpiresUtc;
        public DateTime LeaseExpiresUtc { get; }
        public DateTime LeaseExpiresLocal => LeaseExpiresUtc.ToLocalTime();

        public bool LeaseExpired => DateTime.UtcNow > LeaseExpiresUtc;
        public bool LeaseHasExpiry => LeaseExpiresUtc != DateTime.MaxValue;

        public DhcpServerClientTypes Type { get; }

        public DhcpServerClientAddressStates AddressState { get; }
        public DhcpServerClientNameProtectionStates NameProtectionState { get; }
        public DhcpServerClientDnsStates DnsState { get; }

        public DhcpServerClientQuarantineStatuses QuarantineStatus { get; }

        public DateTime ProbationEnds { get; }

        public bool QuarantineCapable { get; }

        private DhcpServerClient(DhcpServer server, DhcpServerIpAddress ipAddress, DhcpServerIpMask subnetMask, DhcpServerHardwareAddress hardwareAddress, string name, string comment, DateTime leaseExpires, DhcpServerClientTypes type, DhcpServerClientAddressStates addressState, DhcpServerClientNameProtectionStates nameProtectionState, DhcpServerClientDnsStates dnsState, DhcpServerClientQuarantineStatuses quarantineStatus, DateTime probationEnds, bool quarantineCapable)
        {
            Server = server;
            IpAddress = ipAddress;
            SubnetMask = subnetMask;
            HardwareAddress = hardwareAddress;
            Name = name;
            Comment = comment;
            LeaseExpiresUtc = leaseExpires.ToUniversalTime();
            Type = type;
            AddressState = addressState;
            NameProtectionState = nameProtectionState;
            DnsState = dnsState;
            QuarantineStatus = quarantineStatus;
            ProbationEnds = probationEnds;
            QuarantineCapable = quarantineCapable;
        }

        internal static DhcpServerClient GetClient(DhcpServer server, DhcpServerIpAddress ipAddress)
        {
            var searchInfo = new DHCP_SEARCH_INFO_Managed_IpAddress
            {
                SearchType = DHCP_SEARCH_INFO_TYPE.DhcpClientIpAddress,
                ClientIpAddress = ipAddress.ToNativeAsNetwork()
            };

            using (var searchInfoPtr = BitHelper.StructureToPtr(searchInfo))
            {
                if (server.IsCompatible(DhcpServerVersions.Windows2008R2))
                    return GetClientVQ(server, searchInfoPtr);
                else if (server.IsCompatible(DhcpServerVersions.Windows2000))
                    return GetClientV0(server, searchInfoPtr);
                else
                    throw new PlatformNotSupportedException($"DHCP Server v{server.VersionMajor}.{server.VersionMinor} does not support this feature");
            }
        }

        private static DhcpServerClient GetClientV0(DhcpServer server, IntPtr searchInfo)
        {
            var result = Api.DhcpGetClientInfo(server.IpAddress, searchInfo, out var clientPtr);

            if (result == DhcpErrors.JET_ERROR)
                return null;

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetClientInfo), result);

            try
            {
                using (var client = clientPtr.MarshalToStructure<DHCP_CLIENT_INFO>())
                {
                    var clientRef = client;
                    return FromNative(server, ref clientRef);
                }
            }
            finally
            {
                Api.FreePointer(clientPtr);
            }
        }

        private static DhcpServerClient GetClientVQ(DhcpServer server, IntPtr searchInfo)
        {
            var result = Api.DhcpGetClientInfoVQ(server.IpAddress, searchInfo, out var clientPtr);

            if (result == DhcpErrors.JET_ERROR)
                return null;

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetClientInfoVQ), result);

            try
            {
                using (var client = clientPtr.MarshalToStructure<DHCP_CLIENT_INFO_VQ>())
                {
                    var clientRef = client;
                    return FromNative(server, ref clientRef);
                }
            }
            finally
            {
                Api.FreePointer(clientPtr);
            }
        }

        internal static IEnumerable<DhcpServerClient> GetClients(DhcpServer server)
        {
            if (server.IsCompatible(DhcpServerVersions.Windows2008R2))
                return GetClientsVQ(server, (DhcpServerIpAddress)0);
            else if (server.IsCompatible(DhcpServerVersions.Windows2000))
                return GetClientsV0(server, (DhcpServerIpAddress)0);
            else
                throw new PlatformNotSupportedException($"DHCP Server v{server.VersionMajor}.{server.VersionMinor} does not support this feature");
        }

        internal static IEnumerable<DhcpServerClient> GetClients(DhcpServerScope scope)
        {
            if (scope.Server.IsCompatible(DhcpServerVersions.Windows2008R2))
                return GetClientsVQ(scope.Server, scope.Address);
            else if (scope.Server.IsCompatible(DhcpServerVersions.Windows2000))
                return GetClientsV0(scope.Server, scope.Address);
            else
                throw new PlatformNotSupportedException($"DHCP Server v{scope.Server.VersionMajor}.{scope.Server.VersionMinor} does not support this feature");
        }

        private static IEnumerable<DhcpServerClient> GetClientsV0(DhcpServer server, DhcpServerIpAddress subnetAddress)
        {
            var resultHandle = IntPtr.Zero;
            var result = Api.DhcpEnumSubnetClients(ServerIpAddress: server.IpAddress,
                                                   SubnetAddress: subnetAddress.ToNativeAsNetwork(),
                                                   ResumeHandle: ref resultHandle,
                                                   PreferredMaximum: 0x10000,
                                                   ClientInfo: out var clientsPtr,
                                                   ClientsRead: out _,
                                                   ClientsTotal: out _);

            // shortcut if no subnet clients are returned
            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS)
                yield break;

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClients), result);

            try
            {
                while (result == DhcpErrors.SUCCESS || result == DhcpErrors.ERROR_MORE_DATA)
                {
                    using (var clients = clientsPtr.MarshalToStructure<DHCP_CLIENT_INFO_ARRAY>())
                    {
                        foreach (var client in clients.Clients)
                        {
                            var clientValue = client.Value;
                            yield return FromNative(server, ref clientValue);
                        }
                    }

                    if (result == DhcpErrors.SUCCESS)
                        yield break; // Last results

                    Api.FreePointer(ref clientsPtr);
                    result = Api.DhcpEnumSubnetClients(ServerIpAddress: server.IpAddress,
                                                       SubnetAddress: subnetAddress.ToNativeAsNetwork(),
                                                       ResumeHandle: ref resultHandle,
                                                       PreferredMaximum: 0x10000,
                                                       ClientInfo: out clientsPtr,
                                                       ClientsRead: out _,
                                                       ClientsTotal: out _);

                    if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA && result != DhcpErrors.ERROR_NO_MORE_ITEMS)
                        throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClients), result);
                }
            }
            finally
            {
                Api.FreePointer(clientsPtr);
            }
        }

        private static IEnumerable<DhcpServerClient> GetClientsV4(DhcpServer server, DhcpServerIpAddress subnetAddress)
        {
            var resultHandle = IntPtr.Zero;
            var result = Api.DhcpEnumSubnetClientsV4(ServerIpAddress: server.IpAddress,
                                                     SubnetAddress: subnetAddress.ToNativeAsNetwork(),
                                                     ResumeHandle: ref resultHandle,
                                                     PreferredMaximum: 0x10000,
                                                     ClientInfo: out var clientsPtr,
                                                     ClientsRead: out _,
                                                     ClientsTotal: out _);

            // shortcut if no subnet clients are returned
            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS)
                yield break;

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClientsV4), result);

            try
            {
                while (result == DhcpErrors.SUCCESS || result == DhcpErrors.ERROR_MORE_DATA)
                {
                    using (var clients = clientsPtr.MarshalToStructure<DHCP_CLIENT_INFO_ARRAY_V4>())
                    {
                        foreach (var client in clients.Clients)
                        {
                            var clientValue = client.Value;
                            yield return FromNative(server, ref clientValue);
                        }
                    }

                    if (result == DhcpErrors.SUCCESS)
                        yield break; // Last results

                    Api.FreePointer(ref clientsPtr);
                    result = Api.DhcpEnumSubnetClientsV4(ServerIpAddress: server.IpAddress,
                                                         SubnetAddress: subnetAddress.ToNativeAsNetwork(),
                                                         ResumeHandle: ref resultHandle,
                                                         PreferredMaximum: 0x10000,
                                                         ClientInfo: out clientsPtr,
                                                         ClientsRead: out _,
                                                         ClientsTotal: out _);

                    if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA && result != DhcpErrors.ERROR_NO_MORE_ITEMS)
                        throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClientsV4), result);
                }
            }
            finally
            {
                Api.FreePointer(clientsPtr);
            }
        }

        private static IEnumerable<DhcpServerClient> GetClientsV5(DhcpServer server, DhcpServerIpAddress subnetAddress)
        {
            var resultHandle = IntPtr.Zero;
            var result = Api.DhcpEnumSubnetClientsV5(ServerIpAddress: server.IpAddress,
                                                     SubnetAddress: subnetAddress.ToNativeAsNetwork(),
                                                     ResumeHandle: ref resultHandle,
                                                     PreferredMaximum: 0x10000,
                                                     ClientInfo: out var clientsPtr,
                                                     ClientsRead: out _,
                                                     ClientsTotal: out _);

            // shortcut if no subnet clients are returned
            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS)
                yield break;

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClientsV5), result);

            try
            {
                while (result == DhcpErrors.SUCCESS || result == DhcpErrors.ERROR_MORE_DATA)
                {
                    using (var clients = clientsPtr.MarshalToStructure<DHCP_CLIENT_INFO_ARRAY_V5>())
                    {
                        foreach (var client in clients.Clients)
                        {
                            var clientValue = client.Value;
                            yield return FromNative(server, ref clientValue);
                        }
                    }

                    if (result == DhcpErrors.SUCCESS)
                        yield break; // Last results

                    Api.FreePointer(ref clientsPtr);
                    result = Api.DhcpEnumSubnetClientsV5(ServerIpAddress: server.IpAddress,
                                                         SubnetAddress: subnetAddress.ToNativeAsNetwork(),
                                                         ResumeHandle: ref resultHandle,
                                                         PreferredMaximum: 0x10000,
                                                         ClientInfo: out clientsPtr,
                                                         ClientsRead: out _,
                                                         ClientsTotal: out _);

                    if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA && result != DhcpErrors.ERROR_NO_MORE_ITEMS)
                        throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClientsV5), result);
                }
            }
            finally
            {
                Api.FreePointer(clientsPtr);
            }
        }

        private static IEnumerable<DhcpServerClient> GetClientsVQ(DhcpServer server, DhcpServerIpAddress subnetAddress)
        {
            var resultHandle = IntPtr.Zero;
            var result = Api.DhcpEnumSubnetClientsVQ(ServerIpAddress: server.IpAddress,
                                                     SubnetAddress: subnetAddress.ToNativeAsNetwork(),
                                                     ResumeHandle: ref resultHandle,
                                                     PreferredMaximum: 0x10000,
                                                     ClientInfo: out var clientsPtr,
                                                     ClientsRead: out _,
                                                     ClientsTotal: out _);

            // shortcut if no subnet clients are returned
            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS)
                yield break;

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClientsVQ), result);

            try
            {
                while (result == DhcpErrors.SUCCESS || result == DhcpErrors.ERROR_MORE_DATA)
                {
                    using (var clients = clientsPtr.MarshalToStructure<DHCP_CLIENT_INFO_ARRAY_VQ>())
                    {
                        foreach (var client in clients.Clients)
                        {
                            var clientValue = client.Value;
                            yield return FromNative(server, ref clientValue);
                        }
                    }

                    if (result == DhcpErrors.SUCCESS)
                        yield break; // Last results

                    Api.FreePointer(ref clientsPtr);
                    result = Api.DhcpEnumSubnetClientsVQ(ServerIpAddress: server.IpAddress,
                                                         SubnetAddress: subnetAddress.ToNativeAsNetwork(),
                                                         ResumeHandle: ref resultHandle,
                                                         PreferredMaximum: 0x10000,
                                                         ClientInfo: out clientsPtr,
                                                         ClientsRead: out _,
                                                         ClientsTotal: out _);

                    if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                        throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClientsVQ), result);
                }
            }
            finally
            {
                Api.FreePointer(clientsPtr);
            }
        }

        private static DhcpServerClient FromNative(DhcpServer server, ref DHCP_CLIENT_INFO native)
        {
            return new DhcpServerClient(server: server,
                                        ipAddress: native.ClientIpAddress.AsNetworkToIpAddress(),
                                        subnetMask: native.SubnetMask.AsNetworkToIpMask(),
                                        hardwareAddress: native.ClientHardwareAddress.DataAsHardwareAddress,
                                        name: native.ClientName,
                                        comment: native.ClientComment,
                                        leaseExpires: native.ClientLeaseExpires.ToDateTime(),
                                        type: DhcpServerClientTypes.Unspecified,
                                        addressState: DhcpServerClientAddressStates.Unknown,
                                        nameProtectionState: DhcpServerClientNameProtectionStates.Unknown,
                                        dnsState: DhcpServerClientDnsStates.Unknown,
                                        quarantineStatus: DhcpServerClientQuarantineStatuses.NoQuarantineInformation,
                                        probationEnds: DateTime.MaxValue,
                                        quarantineCapable: false);
        }

        private static DhcpServerClient FromNative(DhcpServer server, ref DHCP_CLIENT_INFO_V4 native)
        {
            return new DhcpServerClient(server: server,
                                        ipAddress: native.ClientIpAddress.AsNetworkToIpAddress(),
                                        subnetMask: native.SubnetMask.AsNetworkToIpMask(),
                                        hardwareAddress: native.ClientHardwareAddress.DataAsHardwareAddress,
                                        name: native.ClientName,
                                        comment: native.ClientComment,
                                        leaseExpires: native.ClientLeaseExpires.ToDateTime(),
                                        type: (DhcpServerClientTypes)native.bClientType,
                                        addressState: DhcpServerClientAddressStates.Unknown,
                                        nameProtectionState: DhcpServerClientNameProtectionStates.Unknown,
                                        dnsState: DhcpServerClientDnsStates.Unknown,
                                        quarantineStatus: DhcpServerClientQuarantineStatuses.NoQuarantineInformation,
                                        probationEnds: DateTime.MaxValue,
                                        quarantineCapable: false);
        }

        private static DhcpServerClient FromNative(DhcpServer server, ref DHCP_CLIENT_INFO_V5 native)
        {
            return new DhcpServerClient(server: server,
                                        ipAddress: native.ClientIpAddress.AsNetworkToIpAddress(),
                                        subnetMask: native.SubnetMask.AsNetworkToIpMask(),
                                        hardwareAddress: native.ClientHardwareAddress.DataAsHardwareAddress,
                                        name: native.ClientName,
                                        comment: native.ClientComment,
                                        leaseExpires: native.ClientLeaseExpires.ToDateTime(),
                                        type: (DhcpServerClientTypes)native.bClientType,
                                        addressState: (DhcpServerClientAddressStates)(native.AddressState & 0x03), // bits 0 & 1
                                        nameProtectionState: (DhcpServerClientNameProtectionStates)((native.AddressState >> 2) & 0x03), // bits 2 & 3
                                        dnsState: (DhcpServerClientDnsStates)((native.AddressState >> 4) & 0x0F), // bits 4-7
                                        quarantineStatus: DhcpServerClientQuarantineStatuses.NoQuarantineInformation,
                                        probationEnds: DateTime.MaxValue,
                                        quarantineCapable: false);
        }

        private static DhcpServerClient FromNative(DhcpServer server, ref DHCP_CLIENT_INFO_VQ native)
        {
            return new DhcpServerClient(server: server,
                                        ipAddress: native.ClientIpAddress.AsNetworkToIpAddress(),
                                        subnetMask: native.SubnetMask.AsNetworkToIpMask(),
                                        hardwareAddress: native.ClientHardwareAddress.DataAsHardwareAddress,
                                        name: native.ClientName,
                                        comment: native.ClientComment,
                                        leaseExpires: native.ClientLeaseExpires.ToDateTime(),
                                        type: (DhcpServerClientTypes)native.bClientType,
                                        addressState: (DhcpServerClientAddressStates)(native.AddressState & 0x03), // bits 0 & 1
                                        nameProtectionState: (DhcpServerClientNameProtectionStates)((native.AddressState >> 2) & 0x03), // bits 2 & 3
                                        dnsState: (DhcpServerClientDnsStates)((native.AddressState >> 4) & 0x0F), // bits 4-7
                                        quarantineStatus: (DhcpServerClientQuarantineStatuses)native.Status,
                                        probationEnds: native.ProbationEnds.ToDateTime(),
                                        quarantineCapable: native.QuarantineCapable);
        }

        public override string ToString()
        {
            return $"[{HardwareAddress}] {AddressState} {IpAddress}/{SubnetMask.SignificantBits} ({(LeaseHasExpiry ? (LeaseExpired ? "expired" : LeaseExpiresLocal.ToString()) : "no expiry")}): {Name}{(string.IsNullOrEmpty(Comment) ? null : $" ({Comment})")}";
        }
    }
}
