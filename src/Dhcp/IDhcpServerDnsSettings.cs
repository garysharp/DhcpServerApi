namespace Dhcp
{
    public interface IDhcpServerDnsSettings
    {
        DhcpServerDnsSettingSource SettingSource { get; }

        bool DisableDynamicPtrRecordUpdates { get; set; }
        bool DiscardRecordsWhenLeasesDeleted { get; set; }
        bool DynamicDnsUpdatedAlways { get; set; }
        bool DynamicDnsUpdatedOnlyWhenRequested { get; set; }
        bool DynamicDnsUpdatesEnabled { get; set; }
        bool UpdateRecordsForDownLevelClients { get; set; }
    }
}
