using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Dhcp.Native;

namespace Dhcp
{
    /// <summary>
    /// Represents a DHCP Server
    /// </summary>
    public class DhcpServer
    {
        internal readonly DHCP_IP_ADDRESS address;
        private DhcpServerConfiguration configuration;
        private DhcpServerAuditLog auditLog;
        private DhcpServerDnsSettings dnsSettings;
        private DhcpServerSpecificStrings specificStrings;

        public IPAddress IpAddress => address.ToIPAddress();
        public int IpAddressNative => (int)address;
        public string Name { get; }
        public int VersionMajor { get; }
        public int VersionMinor { get; }
        public DhcpServerVersions Version => (DhcpServerVersions)(((ulong)VersionMajor << 16) | (uint)VersionMinor);

        public DhcpServerConfiguration Configuration => configuration ??= DhcpServerConfiguration.GetConfiguration(this);
        /// <summary>
        /// The audit log configuration settings from the DHCP server.
        /// </summary>
        public DhcpServerAuditLog AuditLog => auditLog ??= DhcpServerAuditLog.GetAuditLog(this);
        public DhcpServerDnsSettings DnsSettings => dnsSettings ??= DhcpServerDnsSettings.GetGlobalDnsSettings(this);
        public DhcpServerSpecificStrings SpecificStrings => specificStrings ??= DhcpServerSpecificStrings.GetSpecificStrings(this);

        internal DhcpServer(DHCP_IP_ADDRESS address, string name)
        {
            this.address = address;
            Name = name;

            GetVersion(address, out var versionMajor, out var versionMinor);
            VersionMajor = versionMajor;
            VersionMinor = versionMinor;
        }

        /// <summary>
        /// Enumerates a list of DHCP servers found in the directory service. 
        /// </summary>
        public static IEnumerable<DhcpServer> Servers
        {
            get
            {
                var result = Api.DhcpEnumServers(Flags: 0,
                                                 IdInfo: IntPtr.Zero,
                                                 Servers: out var serversPtr,
                                                 CallbackFn: IntPtr.Zero,
                                                 CallbackData: IntPtr.Zero);

                if (result != DhcpErrors.SUCCESS)
                    throw new DhcpServerException(nameof(Api.DhcpEnumServers), result);

                try
                {
                    var servers = serversPtr.MarshalToStructure<DHCPDS_SERVERS>();

                    foreach (var server in servers.Servers)
                        yield return FromNative(server);
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(serversPtr);
                }
            }
        }

        public bool IsCompatible(DhcpServerVersions version) => ((long)version <= (long)Version);

        /// <summary>
        /// Enumerates a list of Classes associated with the DHCP Server
        /// </summary>
        public IEnumerable<DhcpServerClass> Classes => DhcpServerClass.GetClasses(this);

        /// <summary>
        /// Queries the DHCP Server for the specified User or Vendor Class
        /// </summary>
        /// <param name="name">The name of the User or Vendor Class to retrieve</param>
        /// <returns>A <see cref="DhcpServerClass"/>.</returns>
        public DhcpServerClass GetClass(string name) => DhcpServerClass.GetClass(this, name);

        /// <summary>
        /// Enumerates a list of Global Option Values associated with the DHCP Server
        /// </summary>
        public IEnumerable<DhcpServerOptionValue> GlobalOptionValues => DhcpServerOptionValue.EnumGlobalDefaultOptionValues(this);

        /// <summary>
        /// Enumerates a list of all Global Option Values, including vendor/user class options, associated with the DHCP Server
        /// </summary>
        public IEnumerable<DhcpServerOptionValue> AllGlobalOptionValues => DhcpServerOptionValue.GetAllGlobalOptionValues(this);

        /// <summary>
        /// Enumerates a list of all default Options associated with the DHCP Server
        /// </summary>
        public IEnumerable<DhcpServerOption> Options => DhcpServerOption.EnumDefaultOptions(this);

        /// <summary>
        /// Enumerates a list of all Options, including vendor/user class options, associated with the DHCP Server
        /// </summary>
        public IEnumerable<DhcpServerOption> AllOptions => DhcpServerOption.GetAllOptions(this);

        /// <summary>
        /// Enumerates a list of Scopes associated with the DHCP Server
        /// </summary>
        public IEnumerable<DhcpServerScope> Scopes => DhcpServerScope.GetScopes(this);

        /// <summary>
        /// Enumerates a list of all Clients (in all Scopes) associated with the DHCP Server
        /// </summary>
        public IEnumerable<DhcpServerClient> Clients => DhcpServerClient.GetClients(this);

        /// <summary>
        /// Enumerates a list of Binding Elements associated with the DHCP Server
        /// </summary>
        public IEnumerable<DhcpServerBindingElement> BindingElements => DhcpServerBindingElement.GetBindingInfo(this);

        /// <summary>
        /// Queries the DHCP Server for the specified OptionId from the Default options
        /// </summary>
        /// <param name="optionId">The identifier for the option to retrieve</param>
        /// <returns>A <see cref="DhcpServerOption"/>.</returns>
        public DhcpServerOption GetOption(int optionId) => DhcpServerOption.GetDefaultOption(this, optionId);

        /// <summary>
        /// Queries the DHCP Server for the specified OptionId within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option to retrieve</param>
        /// <returns>A <see cref="DhcpServerOption"/>.</returns>
        public DhcpServerOption GetVendorOption(string vendorName, int optionId) => DhcpServerOption.GetVendorOption(this, optionId, vendorName);

        /// <summary>
        /// Queries the DHCP Server for the specified OptionId within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option to retrieve</param>
        /// <returns>A <see cref="DhcpServerOption"/>.</returns>
        public DhcpServerOption GetUserOption(string className, int optionId) => DhcpServerOption.GetUserOption(this, optionId, className);

        /// <summary>
        /// Queries the DHCP Server for the specified OptionId Value from the Default options
        /// </summary>
        /// <param name="option">The associated option to retrieve the option value for</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetOptionValue(DhcpServerOption option) => option.GetGlobalValue();

        /// <summary>
        /// Queries the DHCP Server for the specified OptionId Value from the Default options
        /// </summary>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetOptionValue(int optionId) => DhcpServerOptionValue.GetGlobalDefaultOptionValue(this, optionId);

        /// <summary>
        /// Queries the DHCP Server for the specified OptionId Value within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetVendorOptionValue(string vendorName, int optionId)
            => DhcpServerOptionValue.GetGlobalVendorOptionValue(this, optionId, vendorName);

        /// <summary>
        /// Queries the DHCP Server for the specified OptionId Value within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetUserOptionValue(string className, int optionId)
            => DhcpServerOptionValue.GetGlobalUserOptionValue(this, optionId, className);


        /// <summary>
        /// Connects to a DHCP server
        /// </summary>
        /// <param name="hostNameOrAddress"></param>
        public static DhcpServer Connect(string hostNameOrAddress)
        {
            if (string.IsNullOrWhiteSpace(hostNameOrAddress))
                throw new ArgumentNullException(nameof(hostNameOrAddress));

            var dnsEntry = Dns.GetHostEntry(hostNameOrAddress);

            var dnsAddress = dnsEntry.AddressList.Where(a => a.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault();

            if (dnsAddress == null)
                throw new NotSupportedException("Unable to resolve an IPv4 address for the DHCP Server");

            var address = DHCP_IP_ADDRESS.FromIPAddress(dnsAddress);

            return new DhcpServer(address, dnsEntry.HostName);
        }

        private static DhcpServer FromNative(DHCPDS_SERVER native)
            => new DhcpServer(native.ServerAddress, native.ServerName);

        private static void GetVersion(DHCP_IP_ADDRESS address, out int versionMajor, out int versionMinor)
        {
            var result = Api.DhcpGetVersion(ServerIpAddress: address,
                                            MajorVersion: out versionMajor,
                                            MinorVersion: out versionMinor);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetVersion), result);
        }

        public override string ToString() => $"DHCP Server: {Name} ({IpAddress})";

    }
}
