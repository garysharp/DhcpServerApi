using System;
using System.Collections.Generic;
using System.Linq;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerOption
    {
        private DhcpServerClass openClass;

        /// <summary>
        /// The associated DHCP Server
        /// </summary>
        public DhcpServer Server { get; }

        /// <summary>
        /// Unique ID number (also called a "code") for this Option
        /// </summary>
        public int OptionId { get; }

        /// <summary>
        /// Name for this Option
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Comment about this Option
        /// </summary>
        public string Comment { get; }

        /// <summary>
        /// Contains the data associated with this option
        /// </summary>
        public IEnumerable<DhcpServerOptionElement> DefaultValue { get; }

        /// <summary>
        /// True if this Option has a single data item associated with it, False when an array of data items are associated with it.
        /// </summary>
        public bool IsUnaryOption { get; }

        /// <summary>
        /// Name of the Vendor Class associated with this Option
        /// </summary>
        public string VendorName { get; }

        /// <summary>
        /// Name of the User Class associated with this Option
        /// </summary>
        public string ClassName { get; }

        public bool IsVendorClassOption => VendorName != null;

        public bool IsUserClassOption => ClassName != null;

        public DhcpServerClass Class => openClass ??= GetClass();

        private DhcpServerOption(DhcpServer server, int optionId, string name, string comment, IEnumerable<DhcpServerOptionElement> defaultValue, bool isUnaryOption, string vendorName, string className)
        {
            Server = server;
            OptionId = optionId;
            Name = name;
            Comment = comment;
            DefaultValue = defaultValue;
            IsUnaryOption = isUnaryOption;
            VendorName = vendorName;
            ClassName = className;
        }

        /// <summary>
        /// The Option Value associated with the Global Server Configuration
        /// </summary>
        public DhcpServerOptionValue GetGlobalValue() => DhcpServerOptionValue.GetGlobalOptionValue(Server, OptionId, ClassName, VendorName);

        /// <summary>
        /// The Option Value associated with the Scope Configuration
        /// </summary>
        public DhcpServerOptionValue GetScopeValue(DhcpServerScope scope)
            => DhcpServerOptionValue.GetScopeOptionValue(scope, OptionId, ClassName, VendorName);

        /// <summary>
        /// The Option Value associated with the Reservation Configuration
        /// </summary>
        public DhcpServerOptionValue GetScopeReservationValue(DhcpServerScopeReservation reservation)
            => DhcpServerOptionValue.GetScopeReservationOptionValue(reservation, OptionId, ClassName, VendorName);

        private DhcpServerClass GetClass()
        {
            if (VendorName != null)
                return DhcpServerClass.GetClass(Server, VendorName);
            else if (ClassName != null)
                return DhcpServerClass.GetClass(Server, ClassName);
            else
                return null;
        }

        internal static DhcpServerOption GetDefaultOption(DhcpServer server, int optionId)
            => GetOption(server, optionId, null, null);

        internal static DhcpServerOption GetVendorOption(DhcpServer server, int optionId, string vendorName)
            => GetOption(server, optionId, null, vendorName);

        internal static DhcpServerOption GetUserOption(DhcpServer server, int optionId, string className)
            => GetOption(server, optionId, className, null);

        internal static DhcpServerOption GetOption(DhcpServer server, int optionId, string className, string vendorName)
        {
            if (server.IsCompatible(DhcpServerVersions.Windows2008R2))
            {
                return GetOptionV5(server, optionId, className, vendorName);
            }
            else
            {
                if (vendorName != null || className != null)
                {
                    throw new PlatformNotSupportedException($"DHCP Server v{server.VersionMajor}.{server.VersionMinor} does not support this feature");
                }
                return GetOptionV0(server, optionId);
            }
        }

        private static DhcpServerOption GetOptionV0(DhcpServer server, int optionId)
        {
            var result = Api.DhcpGetOptionInfo(ServerIpAddress: server.address,
                                               OptionID: optionId,
                                               OptionInfo: out var optionPtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetOptionInfo), result);

            try
            {
                var option = optionPtr.MarshalToStructure<DHCP_OPTION>();
                return FromNative(server, option, null, null);
            }
            finally
            {
                Api.DhcpRpcFreeMemory(optionPtr);
            }
        }

        private static DhcpServerOption GetOptionV5(DhcpServer server, int optionId, string className, string vendorName)
        {
            var result = Api.DhcpGetOptionInfoV5(ServerIpAddress: server.address,
                                                 Flags: (vendorName == null) ? 0 : Constants.DHCP_FLAGS_OPTION_IS_VENDOR,
                                                 OptionID: optionId,
                                                 ClassName: className,
                                                 VendorName: vendorName,
                                                 OptionInfo: out var optionPtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetOptionInfoV5), result);

            try
            {
                var option = optionPtr.MarshalToStructure<DHCP_OPTION>();
                return FromNative(server, option, className, vendorName);
            }
            finally
            {
                Api.DhcpRpcFreeMemory(optionPtr);
            }
        }

        internal static IEnumerable<DhcpServerOption> GetAllOptions(DhcpServer server)
        {
            var result = Api.DhcpGetAllOptions(ServerIpAddress: server.address,
                                               Flags: 0,
                                               OptionStruct: out var optionsPtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetAllOptions), result);

            try
            {
                var options = optionsPtr.MarshalToStructure<DHCP_ALL_OPTIONS>();

                foreach (var option in options.NonVendorOptions.Options)
                    yield return FromNative(server, option, null, null);

                foreach (var option in options.VendorOptions)
                    yield return FromNative(server, option);
            }
            finally
            {
                Api.DhcpRpcFreeMemory(optionsPtr);
            }
        }

        internal static IEnumerable<DhcpServerOption> EnumDefaultOptions(DhcpServer server)
        {
            if (server.IsCompatible(DhcpServerVersions.Windows2008R2))
                return EnumOptionsV5(server, null, null);
            else
                return EnumOptionsV0(server);
        }

        internal static IEnumerable<DhcpServerOption> EnumUserOptions(DhcpServer server, string className)
        {
            if (!server.IsCompatible(DhcpServerVersions.Windows2008R2))
                throw new PlatformNotSupportedException($"DHCP Server v{server.VersionMajor}.{server.VersionMinor} does not support this feature");

            return EnumOptionsV5(server, className, null);
        }

        internal static IEnumerable<DhcpServerOption> EnumVendorOptions(DhcpServer server, string vendorName)
        {
            if (!server.IsCompatible(DhcpServerVersions.Windows2008R2))
                throw new PlatformNotSupportedException($"DHCP Server v{server.VersionMajor}.{server.VersionMinor} does not support this feature");

            return EnumOptionsV5(server, null, vendorName);
        }

        /// <summary>
        /// Only returns Default Options
        /// </summary>
        private static IEnumerable<DhcpServerOption> EnumOptionsV0(DhcpServer server)
        {
            var resumeHandle = IntPtr.Zero;

            var result = Api.DhcpEnumOptions(ServerIpAddress: server.address,
                                             ResumeHandle: ref resumeHandle,
                                             PreferredMaximum: 0xFFFFFFFF,
                                             Options: out var enumInfoPtr,
                                             OptionsRead: out var elementsRead,
                                             OptionsTotal: out _);

            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS || result == DhcpErrors.EPT_S_NOT_REGISTERED)
                yield break;

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                throw new DhcpServerException(nameof(Api.DhcpEnumOptions), result);

            if (elementsRead == 0)
                yield break;

            try
            {
                var enumInfo = enumInfoPtr.MarshalToStructure<DHCP_OPTION_ARRAY>();

                foreach (var option in enumInfo.Options)
                    yield return FromNative(server, option, null, null);
            }
            finally
            {
                Api.DhcpRpcFreeMemory(enumInfoPtr);
            }
        }

        private static IEnumerable<DhcpServerOption> EnumOptionsV5(DhcpServer server, string className, string vendorName)
        {
            var resumeHandle = IntPtr.Zero;

            var result = Api.DhcpEnumOptionsV5(ServerIpAddress: server.address,
                                               Flags: (vendorName == null) ? 0 : Constants.DHCP_FLAGS_OPTION_IS_VENDOR,
                                               ClassName: className,
                                               VendorName: vendorName,
                                               ResumeHandle: ref resumeHandle,
                                               PreferredMaximum: 0xFFFFFFFF,
                                               Options: out var enumInfoPtr,
                                               OptionsRead: out var elementsRead,
                                               OptionsTotal: out _);

            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS || result == DhcpErrors.EPT_S_NOT_REGISTERED)
                yield break;

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                throw new DhcpServerException(nameof(Api.DhcpEnumOptionsV5), result);

            if (elementsRead == 0)
                yield break;

            try
            {
                var enumInfo = enumInfoPtr.MarshalToStructure<DHCP_OPTION_ARRAY>();

                foreach (var option in enumInfo.Options)
                    yield return FromNative(server, option, vendorName, className);
            }
            finally
            {
                Api.DhcpRpcFreeMemory(enumInfoPtr);
            }
        }

        private static DhcpServerOption FromNative(DhcpServer server, DHCP_OPTION native, string vendorName, string className)
        {
            return new DhcpServerOption(server: server,
                                        optionId: native.OptionID,
                                        name: native.OptionName,
                                        comment: native.OptionComment,
                                        defaultValue: DhcpServerOptionElement.ReadNativeElements(native.DefaultValue).ToList(),
                                        isUnaryOption: native.OptionType == DHCP_OPTION_TYPE.DhcpUnaryElementTypeOption,
                                        vendorName: vendorName,
                                        className: className);
        }

        private static DhcpServerOption FromNative(DhcpServer server, DHCP_VENDOR_OPTION native)
        {
            return new DhcpServerOption(server: server,
                                        optionId: native.OptionID,
                                        name: native.OptionName,
                                        comment: native.OptionComment,
                                        defaultValue: DhcpServerOptionElement.ReadNativeElements(native.DefaultValue).ToList(),
                                        isUnaryOption: native.OptionType == DHCP_OPTION_TYPE.DhcpUnaryElementTypeOption,
                                        vendorName: native.VendorName,
                                        className: native.ClassName);
        }

        public override string ToString() 
            => $"{VendorName ?? ClassName ?? Server.SpecificStrings.DefaultVendorClassName}: {OptionId} [{Name}: {Comment}]; Default: {string.Join("; ", DefaultValue)}";
    }
}
