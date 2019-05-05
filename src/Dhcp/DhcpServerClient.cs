using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerClient
    {
        internal readonly DHCP_IP_ADDRESS ipAddress;
        internal readonly DHCP_IP_MASK subnetMask;
        internal readonly byte[] hardwareAddress;

        public DhcpServer Server { get; }

        public IPAddress IpAddress => ipAddress.ToIPAddress();
        public int IpAddressNative => (int)ipAddress;
        public IPAddress SubnetMask => subnetMask.ToIPAddress();
        public int SubnetMaskNative => (int)subnetMask;

        public string HardwareAddress => hardwareAddress.ToHexString(':');
        public long HardwareAddressNative
        {
            get
            {
                if (hardwareAddress.Length == 6)
                {
                    return (long)hardwareAddress[0] << 40 |
                        (long)hardwareAddress[1] << 32 |
                        (long)hardwareAddress[2] << 24 |
                        (long)hardwareAddress[3] << 16 |
                        (long)hardwareAddress[4] << 08 |
                        hardwareAddress[5];
                }
                else
                    return -1;
            }
        }
        public byte[] HardwareAddressBytes => hardwareAddress;

        public string Name { get; }
        public string Comment { get; }

        public DateTime LeaseExpires { get; }

        public bool LeaseExpired => DateTime.Now > LeaseExpires;

        public DhcpServerClientTypes Type { get; }

        public DhcpServerClientAddressStates AddressState { get; }
        public DhcpServerClientNameProtectionStates NameProtectionState { get; }
        public DhcpServerClientDnsStates DnsState { get; }

        public DhcpServerClientQuarantineStatuses QuarantineStatus { get; }

        public DateTime ProbationEnds { get; }

        public bool QuarantineCapable { get; }

        private DhcpServerClient(DhcpServer server, DHCP_IP_ADDRESS ipAddress, DHCP_IP_MASK subnetMask, byte[] hardwareAddress, string name, string comment, DateTime leaseExpires, DhcpServerClientTypes type, DhcpServerClientAddressStates addressState, DhcpServerClientNameProtectionStates nameProtectionState, DhcpServerClientDnsStates dnsState, DhcpServerClientQuarantineStatuses quarantineStatus, DateTime probationEnds, bool quarantineCapable)
        {
            Server = server;
            this.ipAddress = ipAddress;
            this.subnetMask = subnetMask;
            this.hardwareAddress = hardwareAddress;
            Name = name;
            Comment = comment;
            LeaseExpires = leaseExpires;
            Type = type;
            AddressState = addressState;
            NameProtectionState = nameProtectionState;
            DnsState = dnsState;
            QuarantineStatus = quarantineStatus;
            ProbationEnds = probationEnds;
            QuarantineCapable = quarantineCapable;
        }

        internal static DhcpServerClient GetClient(DhcpServer server, DHCP_IP_ADDRESS ipAddress)
        {
            var searchInfo = new DHCP_SEARCH_INFO_IPADDRESS
            {
                SearchType = DHCP_SEARCH_INFO_TYPE.DhcpClientIpAddress,
                ClientIpAddress = ipAddress
            };

            var searchInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(searchInfo));

            try
            {
                Marshal.StructureToPtr(searchInfo, searchInfoPtr, true);

                if (server.IsCompatible(DhcpServerVersions.Windows2008R2))
                    return GetClientVQ(server, searchInfoPtr);
                else if (server.IsCompatible(DhcpServerVersions.Windows2000))
                    return GetClientV0(server, searchInfoPtr);
                else
                {
                    throw new PlatformNotSupportedException($"DHCP Server v{server.VersionMajor}.{server.VersionMinor} does not support this feature");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(searchInfoPtr);
            }
        }

        private static DhcpServerClient GetClientV0(DhcpServer server, IntPtr searchInfo)
        {
            var result = Api.DhcpGetClientInfo(ServerIpAddress: server.address,
                                               SearchInfo: searchInfo,
                                               ClientInfo: out var clientPtr);

            if (result == DhcpErrors.JET_ERROR)
                return null;

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetClassInfo), result);

            try
            {
                var client = clientPtr.MarshalToStructure<DHCP_CLIENT_INFO>();
                return FromNative(server, client);
            }
            finally
            {
                Api.DhcpRpcFreeMemory(clientPtr);
            }
        }

        private static DhcpServerClient GetClientVQ(DhcpServer server, IntPtr searchInfo)
        {
            var result = Api.DhcpGetClientInfoVQ(ServerIpAddress: server.address,
                                                 SearchInfo: searchInfo,
                                                 ClientInfo: out var clientPtr);

            if (result == DhcpErrors.JET_ERROR)
                return null;

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetClientInfoVQ), result);

            try
            {
                var client = clientPtr.MarshalToStructure<DHCP_CLIENT_INFO_VQ>();
                return FromNative(server, client);
            }
            finally
            {
                Api.DhcpRpcFreeMemory(clientPtr);
            }
        }

        internal static IEnumerable<DhcpServerClient> GetClients(DhcpServer server)
        {
            if (server.IsCompatible(DhcpServerVersions.Windows2008R2))
                return GetClientsVQ(server, (DHCP_IP_ADDRESS)0);
            else if (server.IsCompatible(DhcpServerVersions.Windows2000))
                return GetClientsV0(server, (DHCP_IP_ADDRESS)0);
            else
                throw new PlatformNotSupportedException($"DHCP Server v{server.VersionMajor}.{server.VersionMinor} does not support this feature");
        }

        internal static IEnumerable<DhcpServerClient> GetClients(DhcpServerScope scope)
        {
            if (scope.Server.IsCompatible(DhcpServerVersions.Windows2008R2))
                return GetClientsVQ(scope.Server, scope.address);
            else if (scope.Server.IsCompatible(DhcpServerVersions.Windows2000))
                return GetClientsV0(scope.Server, scope.address);
            else
                throw new PlatformNotSupportedException($"DHCP Server v{scope.Server.VersionMajor}.{scope.Server.VersionMinor} does not support this feature");
        }

        private static IEnumerable<DhcpServerClient> GetClientsV0(DhcpServer server, DHCP_IP_ADDRESS subnetAddress)
        {
            var resultHandle = IntPtr.Zero;
            var result = Api.DhcpEnumSubnetClients(ServerIpAddress: server.address,
                                                   SubnetAddress: subnetAddress,
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

            while (result == DhcpErrors.SUCCESS || result == DhcpErrors.ERROR_MORE_DATA)
            {
                try
                {
                    var clients = clientsPtr.MarshalToStructure<DHCP_CLIENT_INFO_ARRAY>();

                    foreach (var client in clients.Clients)
                        yield return FromNative(server, client);
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(clientsPtr);
                }

                if (result == DhcpErrors.SUCCESS)
                    yield break; // Last results

                result = Api.DhcpEnumSubnetClients(ServerIpAddress: server.address,
                                                   SubnetAddress: subnetAddress,
                                                   ResumeHandle: ref resultHandle,
                                                   PreferredMaximum: 0x10000,
                                                   ClientInfo: out clientsPtr,
                                                   ClientsRead: out _,
                                                   ClientsTotal: out _);

                if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA && result != DhcpErrors.ERROR_NO_MORE_ITEMS)
                    throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClients), result);
            }
        }

        private static IEnumerable<DhcpServerClient> GetClientsV4(DhcpServer server, DHCP_IP_ADDRESS subnetAddress)
        {
            var resultHandle = IntPtr.Zero;
            var result = Api.DhcpEnumSubnetClientsV4(ServerIpAddress: server.address,
                                                     SubnetAddress: subnetAddress,
                                                     ResumeHandle: ref resultHandle,
                                                     PreferredMaximum: 0x10000,
                                                     ClientInfo: out var clientsPtr,
                                                     ClientsRead: out _,
                                                     ClientsTotal: out _);

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA && result != DhcpErrors.ERROR_NO_MORE_ITEMS)
                throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClientsV4), result);

            while (result == DhcpErrors.SUCCESS || result == DhcpErrors.ERROR_MORE_DATA)
            {
                try
                {
                    var clients = clientsPtr.MarshalToStructure<DHCP_CLIENT_INFO_ARRAY_V4>();

                    foreach (var client in clients.Clients)
                        yield return FromNative(server, client);
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(clientsPtr);
                }

                if (result == DhcpErrors.SUCCESS)
                    yield break; // Last results

                result = Api.DhcpEnumSubnetClientsV4(ServerIpAddress: server.address,
                                                     SubnetAddress: subnetAddress,
                                                     ResumeHandle: ref resultHandle,
                                                     PreferredMaximum: 0x10000,
                                                     ClientInfo: out clientsPtr,
                                                     ClientsRead: out _,
                                                     ClientsTotal: out _);

                if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA && result != DhcpErrors.ERROR_NO_MORE_ITEMS)
                    throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClientsV4), result);
            }
        }

        private static IEnumerable<DhcpServerClient> GetClientsV5(DhcpServer server, DHCP_IP_ADDRESS subnetAddress)
        {
            var resultHandle = IntPtr.Zero;
            var result = Api.DhcpEnumSubnetClientsV5(ServerIpAddress: server.address,
                                                     SubnetAddress: subnetAddress,
                                                     ResumeHandle: ref resultHandle,
                                                     PreferredMaximum: 0x10000,
                                                     ClientInfo: out var clientsPtr,
                                                     ClientsRead: out _,
                                                     ClientsTotal: out _);

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA && result != DhcpErrors.ERROR_NO_MORE_ITEMS)
                throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClientsV5), result);

            while (result == DhcpErrors.SUCCESS || result == DhcpErrors.ERROR_MORE_DATA)
            {
                try
                {
                    var clients = clientsPtr.MarshalToStructure<DHCP_CLIENT_INFO_ARRAY_V5>();

                    foreach (var client in clients.Clients)
                        yield return FromNative(server, client);
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(clientsPtr);
                }

                if (result == DhcpErrors.SUCCESS)
                    yield break; // Last results

                result = Api.DhcpEnumSubnetClientsV5(ServerIpAddress: server.address,
                                                     SubnetAddress: subnetAddress,
                                                     ResumeHandle: ref resultHandle,
                                                     PreferredMaximum: 0x10000,
                                                     ClientInfo: out clientsPtr,
                                                     ClientsRead: out _,
                                                     ClientsTotal: out _);

                if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA && result != DhcpErrors.ERROR_NO_MORE_ITEMS)
                    throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClientsV5), result);
            }
        }

        private static IEnumerable<DhcpServerClient> GetClientsVQ(DhcpServer server, DHCP_IP_ADDRESS subnetAddress)
        {
            var resultHandle = IntPtr.Zero;
            var result = Api.DhcpEnumSubnetClientsVQ(ServerIpAddress: server.address,
                                                     SubnetAddress: subnetAddress,
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

            while (result == DhcpErrors.SUCCESS || result == DhcpErrors.ERROR_MORE_DATA)
            {
                try
                {
                    using (var clients = DHCP_CLIENT_INFO_ARRAY_VQ.Read(clientsPtr))
                    {
                        foreach (var client in clients.Clients)
                            yield return FromNative(server, client.Item2);
                    }
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(clientsPtr);
                }

                if (result == DhcpErrors.SUCCESS)
                    yield break; // Last results

                result = Api.DhcpEnumSubnetClientsVQ(ServerIpAddress: server.address,
                                                     SubnetAddress: subnetAddress,
                                                     ResumeHandle: ref resultHandle,
                                                     PreferredMaximum: 0x10000,
                                                     ClientInfo: out clientsPtr,
                                                     ClientsRead: out _,
                                                     ClientsTotal: out _);

                if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                    throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClientsVQ), result);
            }
        }

        private static DhcpServerClient FromNative(DhcpServer server, DHCP_CLIENT_INFO native)
        {
            return new DhcpServerClient(server: server,
                                        ipAddress: native.ClientIpAddress,
                                        subnetMask: native.SubnetMask,
                                        hardwareAddress: native.ClientHardwareAddress.Data,
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

        private static DhcpServerClient FromNative(DhcpServer server, DHCP_CLIENT_INFO_V4 native)
        {
            return new DhcpServerClient(server: server,
                                        ipAddress: native.ClientIpAddress,
                                        subnetMask: native.SubnetMask,
                                        hardwareAddress: native.ClientHardwareAddress.Data,
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

        private static DhcpServerClient FromNative(DhcpServer server, DHCP_CLIENT_INFO_V5 native)
        {
            return new DhcpServerClient(server: server,
                                        ipAddress: native.ClientIpAddress,
                                        subnetMask: native.SubnetMask,
                                        hardwareAddress: native.ClientHardwareAddress.Data,
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

        private static DhcpServerClient FromNative(DhcpServer server, DHCP_CLIENT_INFO_VQ native)
        {
            return new DhcpServerClient(server: server,
                                        ipAddress: native.ClientIpAddress,
                                        subnetMask: native.SubnetMask,
                                        hardwareAddress: native.ClientHardwareAddress.Data,
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

        public override string ToString() => $"{ipAddress}/{subnetMask.SignificantBits} [{HardwareAddress}]: {Name} ({Comment})";
    }
}
