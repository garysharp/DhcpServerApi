using System;
using System.Collections.Generic;
using System.Linq;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerOption : IDhcpServerOption
    {
        private DhcpServerClass optionClass;

        /// <summary>
        /// The associated DHCP Server
        /// </summary>
        public DhcpServer Server { get; }
        IDhcpServer IDhcpServerOption.Server => Server;

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
        public IEnumerable<IDhcpServerOptionElement> DefaultValue { get; }

        public DhcpServerOptionElementType ValueType { get; }

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

        public IDhcpServerClass Class => optionClass ??= GetClass();

        private DhcpServerOption(DhcpServer server, int optionId, string name, string comment, IEnumerable<DhcpServerOptionElement> defaultValue, bool isUnaryOption, string vendorName, string className)
        {
            Server = server;
            OptionId = optionId;
            Name = name;
            Comment = comment;
            DefaultValue = defaultValue;
            ValueType = defaultValue.First().Type;
            IsUnaryOption = isUnaryOption;
            VendorName = vendorName;
            ClassName = className;
        }

        public IDhcpServerOptionValue CreateOptionByteValue(byte value)
        {
            ValidateCreateOptionArguments(DhcpServerOptionElementType.Byte);
            return CreateOptionValue(DhcpServerOptionElement.CreateElement(value));
        }
        public IDhcpServerOptionValue CreateOptionByteValue(IEnumerable<byte> values)
        {
            var valueList = values?.ToList() ?? throw new ArgumentNullException(nameof(values));
            ValidateCreateOptionArguments(DhcpServerOptionElementType.Byte, valueList.Count);
            return CreateOptionValue(DhcpServerOptionElement.CreateElement(valueList));
        }
        public IDhcpServerOptionValue CreateOptionByteValue(params byte[] values)
            => CreateOptionByteValue((IEnumerable<byte>)values);
        public IDhcpServerOptionValue CreateOptionInt16Value(short value)
        {
            ValidateCreateOptionArguments(DhcpServerOptionElementType.Word);
            return CreateOptionValue(DhcpServerOptionElement.CreateElement(value));
        }
        public IDhcpServerOptionValue CreateOptionInt16Value(IEnumerable<short> values)
        {
            var valueList = values?.ToList() ?? throw new ArgumentNullException(nameof(values));
            ValidateCreateOptionArguments(DhcpServerOptionElementType.Word, valueList.Count);
            return CreateOptionValue(DhcpServerOptionElement.CreateElement(valueList));
        }
        public IDhcpServerOptionValue CreateOptionInt16Value(params short[] values)
            => CreateOptionInt16Value((IEnumerable<short>)values);
        public IDhcpServerOptionValue CreateOptionInt32Value(int value)
        {
            ValidateCreateOptionArguments(DhcpServerOptionElementType.DWord);
            return CreateOptionValue(DhcpServerOptionElement.CreateElement(value));
        }
        public IDhcpServerOptionValue CreateOptionInt32Value(IEnumerable<int> values)
        {
            var valueList = values?.ToList() ?? throw new ArgumentNullException(nameof(values));
            ValidateCreateOptionArguments(DhcpServerOptionElementType.DWord, valueList.Count);
            return CreateOptionValue(DhcpServerOptionElement.CreateElement(valueList));
        }
        public IDhcpServerOptionValue CreateOptionInt32Value(params int[] values)
            => CreateOptionInt32Value((IEnumerable<int>)values);
        public IDhcpServerOptionValue CreateOptionInt64Value(long value)
        {
            ValidateCreateOptionArguments(DhcpServerOptionElementType.DWordDWord);
            return CreateOptionValue(DhcpServerOptionElement.CreateElement(value));
        }
        public IDhcpServerOptionValue CreateOptionInt64Value(IEnumerable<long> values)
        {
            var valueList = values?.ToList() ?? throw new ArgumentNullException(nameof(values));
            ValidateCreateOptionArguments(DhcpServerOptionElementType.DWordDWord, valueList.Count);
            return CreateOptionValue(DhcpServerOptionElement.CreateElement(valueList));
        }
        public IDhcpServerOptionValue CreateOptionInt64Value(params long[] values)
            => CreateOptionInt64Value((IEnumerable<long>)values);
        public IDhcpServerOptionValue CreateOptionIpAddressValue(DhcpServerIpAddress value)
        {
            ValidateCreateOptionArguments(DhcpServerOptionElementType.IpAddress);
            return CreateOptionValue(DhcpServerOptionElement.CreateElement(value));
        }
        public IDhcpServerOptionValue CreateOptionIpAddressValue(IEnumerable<DhcpServerIpAddress> values)
        {
            var valueList = values?.ToList() ?? throw new ArgumentNullException(nameof(values));
            ValidateCreateOptionArguments(DhcpServerOptionElementType.IpAddress, valueList.Count);
            return CreateOptionValue(DhcpServerOptionElement.CreateElement(valueList));
        }
        public IDhcpServerOptionValue CreateOptionIpAddressValue(params DhcpServerIpAddress[] values)
            => CreateOptionIpAddressValue((IEnumerable<DhcpServerIpAddress>)values);
        public IDhcpServerOptionValue CreateOptionStringValue(string value)
        {
            ValidateCreateOptionArguments(DhcpServerOptionElementType.StringData);
            return CreateOptionValue(DhcpServerOptionElement.CreateStringElement(value));
        }
        public IDhcpServerOptionValue CreateOptionStringValue(IEnumerable<string> values)
        {
            var valueList = values?.ToList() ?? throw new ArgumentNullException(nameof(values));
            ValidateCreateOptionArguments(DhcpServerOptionElementType.StringData, valueList.Count);
            return CreateOptionValue(DhcpServerOptionElement.CreateStringElement(valueList));
        }
        public IDhcpServerOptionValue CreateOptionStringValue(params string[] values)
            => CreateOptionStringValue((IEnumerable<string>)values);
        public IDhcpServerOptionValue CreateOptionBinaryValue(byte[] value)
        {
            ValidateCreateOptionArguments(DhcpServerOptionElementType.BinaryData);
            return CreateOptionValue(DhcpServerOptionElement.CreateBinaryElement(value));
        }
        public IDhcpServerOptionValue CreateOptionBinaryValue(IEnumerable<byte[]> values)
        {
            var valueList = values?.ToList() ?? throw new ArgumentNullException(nameof(values));
            ValidateCreateOptionArguments(DhcpServerOptionElementType.BinaryData, valueList.Count);
            return CreateOptionValue(DhcpServerOptionElement.CreateBinaryElement(valueList));
        }
        public IDhcpServerOptionValue CreateOptionBinaryValue(params byte[][] values)
            => CreateOptionBinaryValue((IEnumerable<byte[]>)values);
        public IDhcpServerOptionValue CreateOptionEncapsulatedValue(byte[] value)
        {
            ValidateCreateOptionArguments(DhcpServerOptionElementType.EncapsulatedData);
            return CreateOptionValue(DhcpServerOptionElement.CreateEncapsulatedElement(value));
        }
        public IDhcpServerOptionValue CreateOptionEncapsulatedValue(IEnumerable<byte[]> values)
        {
            var valueList = values?.ToList() ?? throw new ArgumentNullException(nameof(values));
            ValidateCreateOptionArguments(DhcpServerOptionElementType.EncapsulatedData, valueList.Count);
            return CreateOptionValue(DhcpServerOptionElement.CreateEncapsulatedElement(valueList));
        }
        public IDhcpServerOptionValue CreateOptionEncapsulatedValue(params byte[][] values)
            => CreateOptionEncapsulatedValue((IEnumerable<byte[]>)values);
        public IDhcpServerOptionValue CreateOptionIpv6AddressValue(string value)
        {
            ValidateCreateOptionArguments(DhcpServerOptionElementType.Ipv6Address);
            return CreateOptionValue(DhcpServerOptionElement.CreateIpv6AddressElement(value));
        }
        public IDhcpServerOptionValue CreateOptionIpv6AddressValue(IEnumerable<string> values)
        {
            var valueList = values?.ToList() ?? throw new ArgumentNullException(nameof(values));
            ValidateCreateOptionArguments(DhcpServerOptionElementType.Ipv6Address, valueList.Count);
            return CreateOptionValue(DhcpServerOptionElement.CreateIpv6AddressElement(valueList));
        }
        public IDhcpServerOptionValue CreateOptionIpv6AddressValue(params string[] values)
            => CreateOptionIpv6AddressValue((IEnumerable<string>)values);

        private IDhcpServerOptionValue CreateOptionValue(List<DhcpServerOptionElement> values)
            => new DhcpServerOptionValue(server: Server, optionId: OptionId, className: ClassName, vendorName: VendorName, values: values);

        private void ValidateCreateOptionArguments(DhcpServerOptionElementType providedType, int valueCount = 1)
        {
            if (ValueType != providedType)
                throw new ArgumentException($"Option {OptionId} ({Name}) requires value type {ValueType} not {providedType}");
            if (valueCount == 0)
                throw new ArgumentOutOfRangeException("At least one value is required");
            if (valueCount > 1 && IsUnaryOption)
                throw new ArgumentOutOfRangeException($"Option {OptionId} ({Name}) is unary and cannot accept multiple values");
        }

        /// <summary>
        /// The Option Value associated with the Scope Configuration
        /// </summary>
        internal DhcpServerOptionValue GetScopeValue(DhcpServerScope scope)
            => DhcpServerOptionValue.GetScopeOptionValue(scope, OptionId, ClassName, VendorName);

        /// <summary>
        /// The Option Value associated with the Reservation Configuration
        /// </summary>
        internal DhcpServerOptionValue GetScopeReservationValue(DhcpServerScopeReservation reservation)
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

        internal static DhcpServerOption GetVendorOption(DhcpServer server, string vendorName, int optionId)
            => GetOption(server, optionId, null, vendorName);

        internal static DhcpServerOption GetUserOption(DhcpServer server, string className, int optionId)
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
                    throw new PlatformNotSupportedException($"DHCP Server v{server.VersionMajor}.{server.VersionMinor} does not support this feature");

                return GetOptionV0(server, optionId);
            }
        }

        private static DhcpServerOption GetOptionV0(DhcpServer server, int optionId)
        {
            var result = Api.DhcpGetOptionInfo(ServerIpAddress: server.Address,
                                               OptionID: optionId,
                                               OptionInfo: out var optionPtr);

            if (result != DhcpServerNativeErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetOptionInfo), result);

            try
            {
                using (var option = optionPtr.MarshalToStructure<DHCP_OPTION>())
                    return FromNative(server, in option, null, null);
            }
            finally
            {
                Api.FreePointer(optionPtr);
            }
        }

        private static DhcpServerOption GetOptionV5(DhcpServer server, int optionId, string className, string vendorName)
        {
            var result = Api.DhcpGetOptionInfoV5(ServerIpAddress: server.Address,
                                                 Flags: (vendorName == null) ? 0 : Constants.DHCP_FLAGS_OPTION_IS_VENDOR,
                                                 OptionID: optionId,
                                                 ClassName: className,
                                                 VendorName: vendorName,
                                                 OptionInfo: out var optionPtr);

            if (result != DhcpServerNativeErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetOptionInfoV5), result);

            try
            {
                using (var option = optionPtr.MarshalToStructure<DHCP_OPTION>())
                {
                    return FromNative(server, in option, className, vendorName);
                }
            }
            finally
            {
                Api.FreePointer(optionPtr);
            }
        }

        internal static IEnumerable<DhcpServerOption> GetAllOptions(DhcpServer server)
        {
            var result = Api.DhcpGetAllOptions(ServerIpAddress: server.Address,
                                               Flags: 0,
                                               OptionStruct: out var optionsPtr);

            if (result != DhcpServerNativeErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetAllOptions), result);

            try
            {
                using (var options = optionsPtr.MarshalToStructure<DHCP_ALL_OPTIONS>())
                {
                    foreach (var option in options.NonVendorOptions.Options)
                        yield return FromNative(server, in option, null, null);

                    foreach (var option in options.VendorOptions)
                        yield return FromNative(server, in option);
                }
            }
            finally
            {
                Api.FreePointer(optionsPtr);
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
            var result = Api.DhcpEnumOptions(ServerIpAddress: server.Address,
                                             ResumeHandle: ref resumeHandle,
                                             PreferredMaximum: 0xFFFFFFFF,
                                             Options: out var enumInfoPtr,
                                             OptionsRead: out var elementsRead,
                                             OptionsTotal: out _);

            if (result == DhcpServerNativeErrors.ERROR_NO_MORE_ITEMS || result == DhcpServerNativeErrors.EPT_S_NOT_REGISTERED)
                yield break;

            if (result != DhcpServerNativeErrors.SUCCESS && result != DhcpServerNativeErrors.ERROR_MORE_DATA)
                throw new DhcpServerException(nameof(Api.DhcpEnumOptions), result);

            try
            {
                if (elementsRead == 0)
                    yield break;

                using (var enumInfo = enumInfoPtr.MarshalToStructure<DHCP_OPTION_ARRAY>())
                {
                    foreach (var option in enumInfo.Options)
                        yield return FromNative(server, in option, null, null);
                }
            }
            finally
            {
                Api.FreePointer(enumInfoPtr);
            }
        }

        private static IEnumerable<DhcpServerOption> EnumOptionsV5(DhcpServer server, string className, string vendorName)
        {
            var resumeHandle = IntPtr.Zero;
            var result = Api.DhcpEnumOptionsV5(ServerIpAddress: server.Address,
                                               Flags: (vendorName == null) ? 0 : Constants.DHCP_FLAGS_OPTION_IS_VENDOR,
                                               ClassName: className,
                                               VendorName: vendorName,
                                               ResumeHandle: ref resumeHandle,
                                               PreferredMaximum: 0xFFFFFFFF,
                                               Options: out var enumInfoPtr,
                                               OptionsRead: out var elementsRead,
                                               OptionsTotal: out _);

            if (result == DhcpServerNativeErrors.ERROR_NO_MORE_ITEMS || result == DhcpServerNativeErrors.EPT_S_NOT_REGISTERED)
                yield break;

            if (result != DhcpServerNativeErrors.SUCCESS && result != DhcpServerNativeErrors.ERROR_MORE_DATA)
                throw new DhcpServerException(nameof(Api.DhcpEnumOptionsV5), result);

            try
            {
                if (elementsRead == 0)
                    yield break;

                using (var enumInfo = enumInfoPtr.MarshalToStructure<DHCP_OPTION_ARRAY>())
                {
                    foreach (var option in enumInfo.Options)
                        yield return FromNative(server, in option, vendorName, className);
                }
            }
            finally
            {
                Api.FreePointer(enumInfoPtr);
            }
        }

        private static DhcpServerOption FromNative(DhcpServer server, in DHCP_OPTION native, string vendorName, string className)
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

        private static DhcpServerOption FromNative(DhcpServer server, in DHCP_VENDOR_OPTION native)
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
