using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerSpecificStrings
    {
        public string DefaultVendorClassName { get; }
        public string DefaultUserClassName { get; }

        private DhcpServerSpecificStrings(string defaultVendorClassName, string defaultUserClassName)
        {
            DefaultVendorClassName = defaultVendorClassName;
            DefaultUserClassName = defaultUserClassName;
        }

        private static DhcpServerSpecificStrings FromNative(DHCP_SERVER_SPECIFIC_STRINGS strings)
            => new DhcpServerSpecificStrings(strings.DefaultVendorClassName, strings.DefaultUserClassName);

        internal static DhcpServerSpecificStrings GetSpecificStrings(DhcpServer server)
        {
            var result = Api.DhcpGetServerSpecificStrings(ServerIpAddress: server.Address,
                                                          ServerSpecificStrings: out var stringsPtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetServerSpecificStrings), result);

            try
            {
                var strings = stringsPtr.MarshalToStructure<DHCP_SERVER_SPECIFIC_STRINGS>();
                return FromNative(strings);
            }
            finally
            {
                Api.DhcpRpcFreeMemory(stringsPtr);
            }
        }
    }
}
