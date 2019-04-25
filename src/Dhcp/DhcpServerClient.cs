using Dhcp.Native;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Dhcp
{
    public class DhcpServerClient
    {
        internal DHCP_IP_ADDRESS ipAddress;
        internal DHCP_IP_MASK subnetMask;
        internal byte[] hardwareAddress;

        public DhcpServer Server { get; private set; }

        public IPAddress IpAddress { get { return ipAddress.ToIPAddress(); } }
        public int IpAddressNative { get { return (int)ipAddress; } }
        public IPAddress SubnetMask { get { return subnetMask.ToIPAddress(); } }
        public int SubnetMaskNative { get { return (int)subnetMask; } }

        public string HardwareAddress
        {
            get
            {
                var builder = new StringBuilder((hardwareAddress.Length * 2) + hardwareAddress.Length);

                if (hardwareAddress.Length > 0)
                {
                    builder.Append(hardwareAddress[0].ToString("X2"));

                    for (int i = 1; i < hardwareAddress.Length; i++)
                    {
                        builder.Append(":").Append(hardwareAddress[i].ToString("X2"));
                    }
                }

                return builder.ToString();
            }
        }
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
                        (long)hardwareAddress[5];
                }
                else
                {
                    return -1;
                }
            }
        }
        public byte[] HardwareAddressBytes { get { return hardwareAddress; } }

        public string Name { get; private set; }
        public string Comment { get; private set; }

        public DateTime LeaseExpires { get; private set; }

        public bool LeaseExpired
        {
            get
            {
                return DateTime.Now > LeaseExpires;
            }
        }

        public DhcpServerClientTypes Type { get; private set; }

        public DhcpServerClientAddressStates AddressState { get; private set; }

        public DhcpServerClientQuarantineStatuses QuarantineStatus { get; private set; }

        public DateTime ProbationEnds { get; private set; }

        public bool QuarantineCapable { get; private set; }

        private DhcpServerClient(DhcpServer Server)
        {
            this.Server = Server;
        }

        internal static DhcpServerClient GetClient(DhcpServer Server, DHCP_IP_ADDRESS IpAddress)
        {
            var searchInfo = new DHCP_SEARCH_INFO_IPADDRESS
            {
                SearchType = DHCP_SEARCH_INFO_TYPE.DhcpClientIpAddress,
                ClientIpAddress = IpAddress
            };

            var searchInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(searchInfo));

            Marshal.StructureToPtr(searchInfo, searchInfoPtr, true);

            if (Server.IsCompatible(DhcpServerVersions.Windows2008R2))
                return GetClientVQ(Server, searchInfoPtr);
            else if (Server.IsCompatible(DhcpServerVersions.Windows2000))
                return GetClientV0(Server, searchInfoPtr);
            else
            {
                Marshal.FreeHGlobal(searchInfoPtr);

                throw new PlatformNotSupportedException(string.Format("DHCP Server v{0}.{1} does not support this feature", Server.VersionMajor, Server.VersionMinor));
            }
        }

        private static DhcpServerClient GetClientV0(DhcpServer Server, IntPtr SearchInfo)
        {
            try
            {
                IntPtr clientPtr;

                DhcpErrors result = Api.DhcpGetClientInfo(Server.ipAddress.ToString(), SearchInfo, out clientPtr);

                if (result == DhcpErrors.JET_ERROR)
                    return null;

                if (result != DhcpErrors.SUCCESS)
                    throw new DhcpServerException("DhcpGetClientInfo", result);

                try
                {
                    var client = (DHCP_CLIENT_INFO)Marshal.PtrToStructure(clientPtr, typeof(DHCP_CLIENT_INFO));

                    return DhcpServerClient.FromNative(Server, client);
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(clientPtr);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(SearchInfo);
            }
        }

        private static DhcpServerClient GetClientVQ(DhcpServer Server, IntPtr SearchInfo)
        {
            try
            {
                IntPtr clientPtr;

                DhcpErrors result = Api.DhcpGetClientInfoVQ(Server.ipAddress.ToString(), SearchInfo, out clientPtr);

                if (result == DhcpErrors.JET_ERROR)
                    return null;

                if (result != DhcpErrors.SUCCESS)
                    throw new DhcpServerException("DhcpGetClientInfoVQ", result);

                try
                {
                    var client = (DHCP_CLIENT_INFO_VQ)Marshal.PtrToStructure(clientPtr, typeof(DHCP_CLIENT_INFO_VQ));

                    return DhcpServerClient.FromNative(Server, client);
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(clientPtr);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(SearchInfo);
            }
        }

        internal static IEnumerable<DhcpServerClient> GetClients(DhcpServer Server)
        {
            if (Server.IsCompatible(DhcpServerVersions.Windows2008R2))
                return GetClientsVQ(Server, (DHCP_IP_ADDRESS)0);
            else if (Server.IsCompatible(DhcpServerVersions.Windows2000))
                return GetClientsV0(Server, (DHCP_IP_ADDRESS)0);
            else
                throw new PlatformNotSupportedException(string.Format("DHCP Server v{0}.{1} does not support this feature", Server.VersionMajor, Server.VersionMinor));
        }

        internal static IEnumerable<DhcpServerClient> GetClients(DhcpServerScope Scope)
        {
            if (Scope.Server.IsCompatible(DhcpServerVersions.Windows2008R2))
                return GetClientsVQ(Scope.Server, Scope.address);
            else if (Scope.Server.IsCompatible(DhcpServerVersions.Windows2000))
                return GetClientsV0(Scope.Server, Scope.address);
            else
                throw new PlatformNotSupportedException(string.Format("DHCP Server v{0}.{1} does not support this feature", Scope.Server.VersionMajor, Scope.Server.VersionMinor));
        }

        private static IEnumerable<DhcpServerClient> GetClientsV0(DhcpServer Server, DHCP_IP_ADDRESS SubnetAddress)
        {
            IntPtr clientsPtr;
            IntPtr resultHandle = IntPtr.Zero;
            int clientsRead, clientsTotal;

            DhcpErrors result = Api.DhcpEnumSubnetClients(Server.ipAddress.ToString(), SubnetAddress, ref resultHandle, 65536, out clientsPtr, out clientsRead, out clientsTotal);

            // shortcut if no subnet clients are returned
            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS)
                yield break;

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                throw new DhcpServerException("DhcpEnumSubnetClients", result);

            while (result == DhcpErrors.SUCCESS || result == DhcpErrors.ERROR_MORE_DATA)
            {
                try
                {
                    var clients = (DHCP_CLIENT_INFO_ARRAY)Marshal.PtrToStructure(clientsPtr, typeof(DHCP_CLIENT_INFO_ARRAY));

                    foreach (var client in clients.Clients)
                    {
                        yield return DhcpServerClient.FromNative(Server, client);
                    }
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(clientsPtr);
                }

                if (result == DhcpErrors.SUCCESS)
                    yield break; // Last results

                result = Api.DhcpEnumSubnetClients(Server.ipAddress.ToString(), SubnetAddress, ref resultHandle, 65536, out clientsPtr, out clientsRead, out clientsTotal);

                if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA && result != DhcpErrors.ERROR_NO_MORE_ITEMS)
                    throw new DhcpServerException("DhcpEnumSubnetClients", result);
            }
        }

        private static IEnumerable<DhcpServerClient> GetClientsV4(DhcpServer Server, DHCP_IP_ADDRESS SubnetAddress)
        {
            IntPtr clientsPtr;
            IntPtr resultHandle = IntPtr.Zero;
            int clientsRead, clientsTotal;

            DhcpErrors result = Api.DhcpEnumSubnetClientsV4(Server.ipAddress.ToString(), SubnetAddress, ref resultHandle, 65536, out clientsPtr, out clientsRead, out clientsTotal);

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA && result != DhcpErrors.ERROR_NO_MORE_ITEMS)
                throw new DhcpServerException("DhcpEnumSubnetClientsV4", result);

            while (result == DhcpErrors.SUCCESS || result == DhcpErrors.ERROR_MORE_DATA)
            {
                try
                {
                    var clients = (DHCP_CLIENT_INFO_ARRAY_V4)Marshal.PtrToStructure(clientsPtr, typeof(DHCP_CLIENT_INFO_ARRAY_V4));

                    foreach (var client in clients.Clients)
                    {
                        yield return DhcpServerClient.FromNative(Server, client);
                    }
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(clientsPtr);
                }

                if (result == DhcpErrors.SUCCESS)
                    yield break; // Last results

                result = Api.DhcpEnumSubnetClientsV4(Server.ipAddress.ToString(), SubnetAddress, ref resultHandle, 65536, out clientsPtr, out clientsRead, out clientsTotal);

                if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA && result != DhcpErrors.ERROR_NO_MORE_ITEMS)
                    throw new DhcpServerException("DhcpEnumSubnetClientsV4", result);
            }
        }

        private static IEnumerable<DhcpServerClient> GetClientsV5(DhcpServer Server, DHCP_IP_ADDRESS SubnetAddress)
        {
            IntPtr clientsPtr;
            IntPtr resultHandle = IntPtr.Zero;
            int clientsRead, clientsTotal;

            DhcpErrors result = Api.DhcpEnumSubnetClientsV5(Server.ipAddress.ToString(), SubnetAddress, ref resultHandle, 65536, out clientsPtr, out clientsRead, out clientsTotal);

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA && result != DhcpErrors.ERROR_NO_MORE_ITEMS)
                throw new DhcpServerException("DhcpEnumSubnetClientsV5", result);

            while (result == DhcpErrors.SUCCESS || result == DhcpErrors.ERROR_MORE_DATA)
            {
                try
                {
                    var clients = (DHCP_CLIENT_INFO_ARRAY_V5)Marshal.PtrToStructure(clientsPtr, typeof(DHCP_CLIENT_INFO_ARRAY_V5));

                    foreach (var client in clients.Clients)
                    {
                        yield return DhcpServerClient.FromNative(Server, client);
                    }
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(clientsPtr);
                }

                if (result == DhcpErrors.SUCCESS)
                    yield break; // Last results

                result = Api.DhcpEnumSubnetClientsV5(Server.ipAddress.ToString(), SubnetAddress, ref resultHandle, 65536, out clientsPtr, out clientsRead, out clientsTotal);

                if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA && result != DhcpErrors.ERROR_NO_MORE_ITEMS)
                    throw new DhcpServerException("DhcpEnumSubnetClientsV5", result);
            }
        }

        private static IEnumerable<DhcpServerClient> GetClientsVQ(DhcpServer Server, DHCP_IP_ADDRESS SubnetAddress)
        {
            IntPtr clientsPtr;
            IntPtr resultHandle = IntPtr.Zero;
            int clientsRead, clientsTotal;

            DhcpErrors result = Api.DhcpEnumSubnetClientsVQ(Server.ipAddress.ToString(), SubnetAddress, ref resultHandle, 65536, out clientsPtr, out clientsRead, out clientsTotal);

            // shortcut if no subnet clients are returned
            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS)
                yield break;

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                throw new DhcpServerException("DhcpEnumSubnetClientsVQ", result);

            while (result == DhcpErrors.SUCCESS || result == DhcpErrors.ERROR_MORE_DATA)
            {
                try
                {
                    using (var clients = DHCP_CLIENT_INFO_ARRAY_VQ.Read(clientsPtr))
                    {
                        foreach (var client in clients.Clients)
                        {
                            yield return DhcpServerClient.FromNative(Server, client.Item2);
                        }
                    }
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(clientsPtr);
                    clientsPtr = IntPtr.Zero;
                }

                if (result == DhcpErrors.SUCCESS)
                    yield break; // Last results

                clientsRead = 0;
                clientsTotal = 0;

                result = Api.DhcpEnumSubnetClientsVQ(Server.ipAddress.ToString(), SubnetAddress, ref resultHandle, 65536, out clientsPtr, out clientsRead, out clientsTotal);

                if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                    throw new DhcpServerException("DhcpEnumSubnetClientsVQ", result);
            }
        }

        private static DhcpServerClient FromNative(DhcpServer Server, DHCP_CLIENT_INFO Native)
        {
            return new DhcpServerClient(Server)
            {
                ipAddress = Native.ClientIpAddress,
                subnetMask = Native.SubnetMask,
                hardwareAddress = Native.ClientHardwareAddress.Data,
                Name = Native.ClientName,
                Comment = Native.ClientComment,
                LeaseExpires = Native.ClientLeaseExpires.ToDateTime(),
                Type = DhcpServerClientTypes.Unspecified,
                AddressState = DhcpServerClientAddressStates.Unknown,
                QuarantineStatus = DhcpServerClientQuarantineStatuses.NoQuarantineInformation,
                ProbationEnds = DateTime.MaxValue,
                QuarantineCapable = false
            };
        }

        private static DhcpServerClient FromNative(DhcpServer Server, DHCP_CLIENT_INFO_V4 Native)
        {
            return new DhcpServerClient(Server)
            {
                ipAddress = Native.ClientIpAddress,
                subnetMask = Native.SubnetMask,
                hardwareAddress = Native.ClientHardwareAddress.Data,
                Name = Native.ClientName,
                Comment = Native.ClientComment,
                LeaseExpires = Native.ClientLeaseExpires.ToDateTime(),
                Type = (DhcpServerClientTypes)Native.bClientType,
                AddressState = DhcpServerClientAddressStates.Unknown,
                QuarantineStatus = DhcpServerClientQuarantineStatuses.NoQuarantineInformation,
                ProbationEnds = DateTime.MaxValue,
                QuarantineCapable = false
            };
        }

        private static DhcpServerClient FromNative(DhcpServer Server, DHCP_CLIENT_INFO_V5 Native)
        {
            return new DhcpServerClient(Server)
            {
                ipAddress = Native.ClientIpAddress,
                subnetMask = Native.SubnetMask,
                hardwareAddress = Native.ClientHardwareAddress.Data,
                Name = Native.ClientName,
                Comment = Native.ClientComment,
                LeaseExpires = Native.ClientLeaseExpires.ToDateTime(),
                Type = (DhcpServerClientTypes)Native.bClientType,
                AddressState = (DhcpServerClientAddressStates)Native.AddressState,
                QuarantineStatus = DhcpServerClientQuarantineStatuses.NoQuarantineInformation,
                ProbationEnds = DateTime.MaxValue,
                QuarantineCapable = false
            };
        }

        private static DhcpServerClient FromNative(DhcpServer Server, DHCP_CLIENT_INFO_VQ Native)
        {
            return new DhcpServerClient(Server)
            {
                ipAddress = Native.ClientIpAddress,
                subnetMask = Native.SubnetMask,
                hardwareAddress = Native.ClientHardwareAddress.Data,
                Name = Native.ClientName,
                Comment = Native.ClientComment,
                LeaseExpires = Native.ClientLeaseExpires.ToDateTime(),
                Type = (DhcpServerClientTypes)Native.bClientType,
                AddressState = (DhcpServerClientAddressStates)Native.AddressState,
                QuarantineStatus = (DhcpServerClientQuarantineStatuses)Native.Status,
                ProbationEnds = Native.ProbationEnds.ToDateTime(),
                QuarantineCapable = Native.QuarantineCapable
            };
        }

        public override string ToString()
        {
            return string.Format("{0}/{1} [{2}]: {3} ({4})", ipAddress, subnetMask.SignificantBits, HardwareAddress, Name, Comment);
        }
    }
}
