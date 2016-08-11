
namespace Dhcp.Native
{
    internal static class Constants
    {

        public const uint DHCP_FLAGS_OPTION_IS_VENDOR = 0x03;

        public const uint DHCP_ENDPOINT_FLAG_CANT_MODIFY = 0x01;

        public const uint DNS_FLAG_ENABLED = 0x01;
        public const uint DNS_FLAG_UPDATE_DOWNLEVEL = 0x02;
        public const uint DNS_FLAG_CLEANUP_EXPIRED = 0x04;
        public const uint DNS_FLAG_UPDATE_BOTH_ALWAYS = 0x10;
        public const uint DNS_FLAG_UPDATE_DHCID = 0x20;
        public const uint DNS_FLAG_DISABLE_PTR_UPDATE = 0x40;

    }
}
