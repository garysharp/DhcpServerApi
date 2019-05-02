using System.Linq;
using Dhcp.Native;

namespace Dhcp
{
    /// <summary>
    /// Defines Dynamic DNS settings
    /// </summary>
    public class DhcpServerDnsSettings
    {
        private const uint flagDefaultSettings = Constants.DNS_FLAG_ENABLED | Constants.DNS_FLAG_CLEANUP_EXPIRED;
        private readonly uint flags;

        /// <summary>
        /// True when DNS dynamic updates are enabled
        /// </summary>
        public bool DynamicDnsUpdatesEnabled => (flags & Constants.DNS_FLAG_ENABLED) == Constants.DNS_FLAG_ENABLED;

        /// <summary>
        /// True when DNS dynamic updates are enabled only when requested by the DHCP clients
        /// </summary>
        public bool DynamicDnsUpdatedOnlyWhenRequested => (flags & Constants.DNS_FLAG_UPDATE_BOTH_ALWAYS) == 0;

        /// <summary>
        /// True when DNS dynamic updates always update DNS records 
        /// </summary>
        public bool DynamicDnsUpdatedAlways => (flags & Constants.DNS_FLAG_UPDATE_BOTH_ALWAYS) == Constants.DNS_FLAG_UPDATE_BOTH_ALWAYS;

        /// <summary>
        /// True when A and PTR records are discarded when the lease is deleted
        /// </summary>
        public bool DiscardRecordsWhenLeasesDeleted => (flags & Constants.DNS_FLAG_CLEANUP_EXPIRED) == Constants.DNS_FLAG_CLEANUP_EXPIRED;

        /// <summary>
        /// True when DNS records are dynamically updated for DHCP clients that do not request updates (for example, clients running Windows NT 4.0)
        /// </summary>
        public bool UpdateRecordsForDownLevelClients => (flags & Constants.DNS_FLAG_UPDATE_DOWNLEVEL) == Constants.DNS_FLAG_UPDATE_DOWNLEVEL;

        /// <summary>
        /// True when Dynamic updates for DNS PTR records are disabled
        /// </summary>
        public bool DisableDynamicPtrRecordUpdates => (flags & Constants.DNS_FLAG_DISABLE_PTR_UPDATE) == Constants.DNS_FLAG_DISABLE_PTR_UPDATE;

        private DhcpServerDnsSettings(uint flags)
        {
            this.flags = flags;
        }

        internal static DhcpServerDnsSettings GetGlobalDnsSettings(DhcpServer server)
        {
            // Flag is Global Option 81
            try
            {
                var option = DhcpServerOptionValue.GetGlobalDefaultOptionValue(server, 81);

                if (option.Values.FirstOrDefault() is DhcpServerOptionElementDWord value)
                    return new DhcpServerDnsSettings((uint)value.RawValue);
                else
                    return new DhcpServerDnsSettings(flagDefaultSettings);
            }
            catch (DhcpServerException e) when (e.ApiErrorId == (uint)DhcpErrors.ERROR_FILE_NOT_FOUND)
            {
                // Default Settings
                return new DhcpServerDnsSettings(flagDefaultSettings);
            }
        }

        internal static DhcpServerDnsSettings GetScopeDnsSettings(DhcpServerScope scope)
        {
            // Flag is Option 81
            try
            {
                var option = DhcpServerOptionValue.GetScopeDefaultOptionValue(scope, 81);
                if (option.Values.FirstOrDefault() is DhcpServerOptionElementDWord value)
                    return new DhcpServerDnsSettings((uint)value.RawValue);
                else
                    return null;
            }
            catch (DhcpServerException e) when (e.ApiErrorId == (uint)DhcpErrors.ERROR_FILE_NOT_FOUND)
            {
                return null;
            }
        }
    }
}
