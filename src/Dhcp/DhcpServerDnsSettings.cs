using System.Linq;
using Dhcp.Native;

namespace Dhcp
{
    /// <summary>
    /// Defines Dynamic DNS settings
    /// </summary>
    public class DhcpServerDnsSettings : IDhcpServerDnsSettings
    {
        internal const int optionId = 81;
        private const uint flagDefaultSettings = Constants.DNS_FLAG_ENABLED | Constants.DNS_FLAG_CLEANUP_EXPIRED;
        private uint flags;

        public DhcpServerDnsSettingSource SettingSource { get; }

        /// <summary>
        /// True when DNS dynamic updates are enabled
        /// </summary>
        public bool DynamicDnsUpdatesEnabled
        {
            get => (flags & Constants.DNS_FLAG_ENABLED) == Constants.DNS_FLAG_ENABLED;
            set
            {
                if (value)
                    flags |= Constants.DNS_FLAG_ENABLED;
                else
                    flags ^= Constants.DNS_FLAG_ENABLED;
            }
        }

        /// <summary>
        /// True when DNS dynamic updates are enabled only when requested by the DHCP clients
        /// </summary>
        public bool DynamicDnsUpdatedOnlyWhenRequested
        {
            get => (flags & Constants.DNS_FLAG_UPDATE_BOTH_ALWAYS) == 0;
            set
            {
                if (value)
                    flags ^= Constants.DNS_FLAG_UPDATE_BOTH_ALWAYS;
                else
                    flags |= Constants.DNS_FLAG_UPDATE_BOTH_ALWAYS;
            }
        }

        /// <summary>
        /// True when DNS dynamic updates always update DNS records 
        /// </summary>
        public bool DynamicDnsUpdatedAlways
        {
            get => (flags & Constants.DNS_FLAG_UPDATE_BOTH_ALWAYS) == Constants.DNS_FLAG_UPDATE_BOTH_ALWAYS;
            set
            {
                if (value)
                    flags |= Constants.DNS_FLAG_UPDATE_BOTH_ALWAYS;
                else
                    flags ^= Constants.DNS_FLAG_UPDATE_BOTH_ALWAYS;
            }
        }

        /// <summary>
        /// True when A and PTR records are discarded when the lease is deleted
        /// </summary>
        public bool DiscardRecordsWhenLeasesDeleted
        {
            get => (flags & Constants.DNS_FLAG_CLEANUP_EXPIRED) == Constants.DNS_FLAG_CLEANUP_EXPIRED;
            set
            {
                if (value)
                    flags |= Constants.DNS_FLAG_CLEANUP_EXPIRED;
                else
                    flags ^= Constants.DNS_FLAG_CLEANUP_EXPIRED;
            }
        }

        /// <summary>
        /// True when DNS records are dynamically updated for DHCP clients that do not request updates (for example, clients running Windows NT 4.0)
        /// </summary>
        public bool UpdateRecordsForDownLevelClients
        {
            get => (flags & Constants.DNS_FLAG_UPDATE_DOWNLEVEL) == Constants.DNS_FLAG_UPDATE_DOWNLEVEL;
            set
            {
                if (value)
                    flags |= Constants.DNS_FLAG_UPDATE_DOWNLEVEL;
                else
                    flags ^= Constants.DNS_FLAG_UPDATE_DOWNLEVEL;
            }
        }

        /// <summary>
        /// True when Dynamic updates for DNS PTR records are disabled
        /// </summary>
        public bool DisableDynamicPtrRecordUpdates
        {
            get => (flags & Constants.DNS_FLAG_DISABLE_PTR_UPDATE) == Constants.DNS_FLAG_DISABLE_PTR_UPDATE;
            set
            {
                if (value)
                    flags |= Constants.DNS_FLAG_DISABLE_PTR_UPDATE;
                else
                    flags ^= Constants.DNS_FLAG_DISABLE_PTR_UPDATE;
            }
        }

        private DhcpServerDnsSettings(uint flags, DhcpServerDnsSettingSource source)
        {
            this.flags = flags;
            SettingSource = source;
        }

        public DhcpServerDnsSettings()
        {
            flags = flagDefaultSettings;
            SettingSource = DhcpServerDnsSettingSource.Unknown;
        }

        internal static DhcpServerDnsSettings GetGlobalDnsSettings(DhcpServer server)
        {
            // Flag is Global Option 81
            try
            {
                var option = DhcpServerOptionValue.GetGlobalDefaultOptionValue(server, optionId);

                if (option.Values.FirstOrDefault() is DhcpServerOptionElementDWord value)
                    return new DhcpServerDnsSettings((uint)value.RawValue, DhcpServerDnsSettingSource.GlobalSetting);
                else
                    return new DhcpServerDnsSettings(flagDefaultSettings, DhcpServerDnsSettingSource.GlobalSetting);
            }
            catch (DhcpServerException e) when (e.ApiErrorNative == DhcpServerNativeErrors.ERROR_FILE_NOT_FOUND)
            {
                // Default Settings
                return new DhcpServerDnsSettings(flagDefaultSettings, DhcpServerDnsSettingSource.GlobalSetting);
            }
        }

        internal static DhcpServerDnsSettings SetGlobalDnsSettings(DhcpServer server, DhcpServerDnsSettings dnsSettings)
        {
            var optionValue = DhcpServerOptionElement.CreateElement((int)dnsSettings.flags);

            DhcpServerOptionValue.SetGlobalDefaultOptionValue(server, optionId, optionValue);

            return dnsSettings.Clone(DhcpServerDnsSettingSource.GlobalSetting);
        }

        internal static DhcpServerDnsSettings RemoveGlobalDnsSettings(DhcpServer server)
        {
            DhcpServerOptionValue.DeleteGlobalOptionValue(server, optionId);
            return new DhcpServerDnsSettings(flagDefaultSettings, DhcpServerDnsSettingSource.GlobalSetting);
        }

        internal static DhcpServerDnsSettings GetScopeDnsSettings(DhcpServerScope scope)
            => GetScopeDnsSettings(scope.Server, scope.Address);

        internal static DhcpServerDnsSettings GetScopeDnsSettings(DhcpServer server, DhcpServerIpAddress address)
        {
            // Flag is Option 81
            try
            {
                var option = DhcpServerOptionValue.GetScopeDefaultOptionValue(server, address, optionId);
                if (option.Values.FirstOrDefault() is DhcpServerOptionElementDWord value)
                    return new DhcpServerDnsSettings((uint)value.RawValue, DhcpServerDnsSettingSource.ScopeSetting);
                else
                    return GetGlobalDnsSettings(server);
            }
            catch (DhcpServerException e) when (e.ApiErrorNative == DhcpServerNativeErrors.ERROR_FILE_NOT_FOUND)
            {
                return GetGlobalDnsSettings(server);
            }
        }

        internal static DhcpServerDnsSettings SetScopeDnsSettings(DhcpServerScope scope, DhcpServerDnsSettings dnsSettings)
            => SetScopeDnsSettings(scope.Server, scope.Address, dnsSettings);
        internal static DhcpServerDnsSettings SetScopeDnsSettings(DhcpServer server, DhcpServerIpAddress address, DhcpServerDnsSettings dnsSettings)
        {
            var optionValue = DhcpServerOptionElement.CreateElement((int)dnsSettings.flags);

            DhcpServerOptionValue.SetScopeDefaultOptionValue(server, address, optionId, optionValue);

            return dnsSettings.Clone(DhcpServerDnsSettingSource.ScopeSetting);
        }

        internal static void RemoveScopeDnsSettings(DhcpServerScope scope)
            => RemoveScopeDnsSettings(scope.Server, scope.Address);
        internal static void RemoveScopeDnsSettings(DhcpServer server, DhcpServerIpAddress address)
            => DhcpServerOptionValue.DeleteScopeOptionValue(server, address, optionId);

        internal static DhcpServerDnsSettings GetScopeReservationDnsSettings(DhcpServerScopeReservation reservation)
            => GetScopeReservationDnsSettings(reservation.Server, reservation.Scope.Address, reservation.Address);
        internal static DhcpServerDnsSettings GetScopeReservationDnsSettings(DhcpServerScope scope, DhcpServerIpAddress reservationAddress)
            => GetScopeReservationDnsSettings(scope.Server, scope.Address, reservationAddress);
        internal static DhcpServerDnsSettings GetScopeReservationDnsSettings(DhcpServer server, DhcpServerIpAddress scopeAddress, DhcpServerIpAddress reservationAddress)
        {
            // Flag is Option 81
            try
            {
                var option = DhcpServerOptionValue.GetScopeReservationDefaultOptionValue(server, scopeAddress, reservationAddress, optionId);
                if (option.Values.FirstOrDefault() is DhcpServerOptionElementDWord value)
                    return new DhcpServerDnsSettings((uint)value.RawValue, DhcpServerDnsSettingSource.ReservationSetting);
                else
                    return GetScopeDnsSettings(server, scopeAddress);
            }
            catch (DhcpServerException e) when (e.ApiErrorNative == DhcpServerNativeErrors.ERROR_FILE_NOT_FOUND)
            {
                return GetScopeDnsSettings(server, scopeAddress);
            }
        }

        internal static DhcpServerDnsSettings SetScopeReservationDnsSettings(DhcpServerScopeReservation reservation, DhcpServerDnsSettings dnsSettings)
            => SetScopeReservationDnsSettings(reservation.Server, reservation.Scope.Address, reservation.Address, dnsSettings);
        internal static DhcpServerDnsSettings SetScopeReservationDnsSettings(DhcpServerScope scope, DhcpServerIpAddress reservationAddress, DhcpServerDnsSettings dnsSettings)
            => SetScopeReservationDnsSettings(scope.Server, scope.Address, reservationAddress, dnsSettings);
        internal static DhcpServerDnsSettings SetScopeReservationDnsSettings(DhcpServer server, DhcpServerIpAddress scopeAddress, DhcpServerIpAddress reservationAddress, DhcpServerDnsSettings dnsSettings)
        {
            var optionValue = DhcpServerOptionElement.CreateElement((int)dnsSettings.flags);

            DhcpServerOptionValue.SetScopeReservationDefaultOptionValue(server, scopeAddress, reservationAddress, optionId, optionValue);

            return dnsSettings.Clone(DhcpServerDnsSettingSource.ReservationSetting);
        }

        internal static void RemoveScopeReservationDnsSettings(DhcpServerScopeReservation reservation)
            => RemoveScopeReservationDnsSettings(reservation.Server, reservation.Scope.Address, reservation.Address);
        internal static void RemoveScopeReservationDnsSettings(DhcpServerScope scope, DhcpServerIpAddress reservationAddress)
            => RemoveScopeReservationDnsSettings(scope.Server, scope.Address, reservationAddress);
        internal static void RemoveScopeReservationDnsSettings(DhcpServer server, DhcpServerIpAddress scopeAddress, DhcpServerIpAddress reservationAddress)
            => DhcpServerOptionValue.DeleteScopeReservationOptionValue(server, scopeAddress, reservationAddress, optionId);

        internal DhcpServerDnsSettings Clone(DhcpServerDnsSettingSource source)
            => new DhcpServerDnsSettings(flags, source);

        internal DhcpServerDnsSettings Clone()
            => Clone(SettingSource);

    }
}
