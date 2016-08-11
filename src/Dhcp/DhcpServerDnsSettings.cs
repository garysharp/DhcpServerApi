using Dhcp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dhcp
{
    /// <summary>
    /// Defines Dynamic DNS settings
    /// </summary>
    public class DhcpServerDnsSettings
    {
        private const uint FlagDefaultSettings = Constants.DNS_FLAG_ENABLED | Constants.DNS_FLAG_CLEANUP_EXPIRED;

        /// <summary>
        /// True when DNS dynamic updates are enabled
        /// </summary>
        public bool DynamicDnsUpdatesEnabled { get; private set; }

        /// <summary>
        /// True when DNS dynamic updates are enabled only when requested by the DHCP clients
        /// </summary>
        public bool DynamicDnsUpdatedOnlyWhenRequested { get; private set; }

        /// <summary>
        /// True when DNS dynamic updates always update DNS records 
        /// </summary>
        public bool DynamicDnsUpdatedAlways { get; private set; }

        /// <summary>
        /// True when A and PTR records are discarded when the lease is deleted
        /// </summary>
        public bool DiscardRecordsWhenLeasesDeleted { get; private set; }

        /// <summary>
        /// True when DNS records are dynamically updated for DHCP clients that do not request updates (for example, clients running Windows NT 4.0)
        /// </summary>
        public bool UpdateRecordsForDownLevelClients { get; private set; }

        /// <summary>
        /// True when Dynamic updates for DNS PTR records are disabled
        /// </summary>
        public bool DisableDynamicPtrRecordUpdates { get; private set; }

        private DhcpServerDnsSettings(uint Flags)
        {
            DynamicDnsUpdatesEnabled = (Flags & Constants.DNS_FLAG_ENABLED) == Constants.DNS_FLAG_ENABLED;
            DynamicDnsUpdatedOnlyWhenRequested = (Flags & Constants.DNS_FLAG_UPDATE_BOTH_ALWAYS) == 0;
            UpdateRecordsForDownLevelClients = (Flags & Constants.DNS_FLAG_UPDATE_DOWNLEVEL) == Constants.DNS_FLAG_UPDATE_DOWNLEVEL;
            DiscardRecordsWhenLeasesDeleted = (Flags & Constants.DNS_FLAG_CLEANUP_EXPIRED) == Constants.DNS_FLAG_CLEANUP_EXPIRED;
            DynamicDnsUpdatedAlways = (Flags & Constants.DNS_FLAG_UPDATE_BOTH_ALWAYS) == Constants.DNS_FLAG_UPDATE_BOTH_ALWAYS;
            DisableDynamicPtrRecordUpdates = (Flags & Constants.DNS_FLAG_DISABLE_PTR_UPDATE) == Constants.DNS_FLAG_DISABLE_PTR_UPDATE;
        }

        internal static DhcpServerDnsSettings GetGlobalDnsSettings(DhcpServer Server)
        {
            // Flag is Global Option 81
            try
            {
                var option = DhcpServerOptionValue.GetGlobalDefaultOptionValue(Server, 81);
                if (option.Values.Count == 1 && option.Values[0] is DhcpServerOptionElementDWord)
                {
                    var value = (DhcpServerOptionElementDWord)option.Values[0];
                    return new DhcpServerDnsSettings((uint)value.RawValue);
                }
                else
                {
                    return new DhcpServerDnsSettings(FlagDefaultSettings);
                }
            }
            catch (DhcpServerException e) when (e.ApiErrorId == (uint)DhcpErrors.ERROR_FILE_NOT_FOUND)
            {
                // Default Settings
                return new DhcpServerDnsSettings(FlagDefaultSettings);
            }
        }

        internal static DhcpServerDnsSettings GetScopeDnsSettings(DhcpServerScope Scope)
        {
            // Flag is Option 81
            try
            {
                var option = DhcpServerOptionValue.GetScopeDefaultOptionValue(Scope, 81);
                if (option.Values.Count != 1)
                {
                    return GetGlobalDnsSettings(Scope.Server);
                }
                else
                {
                    return null;
                }
            }
            catch (DhcpServerException e) when (e.ApiErrorId == (uint)DhcpErrors.ERROR_FILE_NOT_FOUND)
            {
                return null;
            }
        }
    }
}
