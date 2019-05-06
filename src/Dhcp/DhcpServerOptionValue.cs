using System;
using System.Collections.Generic;
using System.Linq;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerOptionValue
    {
        /// <summary>
        /// The associated DHCP Server
        /// </summary>
        public DhcpServer Server { get; }
        public int OptionId { get; }
        public string ClassName { get; }
        public string VendorName { get; }

        private DhcpServerOption option;
        /// <summary>
        /// The associated Option
        /// </summary>
        public DhcpServerOption Option => option ??= GetOption();

        /// <summary>
        /// Contains the value associated with this option
        /// </summary>
        public IEnumerable<DhcpServerOptionElement> Values { get; }

        private DhcpServerOptionValue(DhcpServer server, int optionId, string className, string vendorName, List<DhcpServerOptionElement> values)
        {
            Server = server;

            OptionId = optionId;

            ClassName = className;
            VendorName = vendorName;

            Values = values;
        }

        private DhcpServerOption GetOption()
            => DhcpServerOption.GetOption(Server, OptionId, ClassName, VendorName);

        private static DhcpServerOptionValue FromNative(DhcpServer server, ref DHCP_OPTION_VALUE native, string className, string vendorName)
        {
            return new DhcpServerOptionValue(server: server,
                                             optionId: native.OptionID,
                                             className: className,
                                             vendorName: vendorName,
                                             values: DhcpServerOptionElement.ReadNativeElements(native.Value).ToList());
        }

        internal static IEnumerable<DhcpServerOptionValue> EnumScopeReservationDefaultOptionValues(DhcpServerScopeReservation reservation)
            => EnumScopeReservationOptionValues(reservation, null, null);

        internal static IEnumerable<DhcpServerOptionValue> EnumScopeReservationVendorOptionValues(DhcpServerScopeReservation reservation, string vendorName)
            => EnumScopeReservationOptionValues(reservation, null, vendorName);

        internal static IEnumerable<DhcpServerOptionValue> EnumScopeReservationUserOptionValues(DhcpServerScopeReservation reservation, string className)
            => EnumScopeReservationOptionValues(reservation, className, null);

        private static IEnumerable<DhcpServerOptionValue> EnumScopeReservationOptionValues(DhcpServerScopeReservation reservation, string className, string vendorName)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_LocalReserved()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpReservedOptions,
                ReservedIpAddress = reservation.IpAddress.ToNativeAsNetwork(),
                ReservedIpSubnetAddress = reservation.Scope.Address.ToNativeAsNetwork()
            };

            using (var scopeInfoPtr = BitHelper.StructureToPtr(scopeInfo))
            {
                foreach (var value in EnumOptionValues(reservation.Server, scopeInfoPtr, className, vendorName))
                    yield return value;
            }
        }

        internal static DhcpServerOptionValue GetScopeReservationDefaultOptionValue(DhcpServerScopeReservation reservation, int optionId)
            => GetScopeReservationOptionValue(reservation, optionId, null, null);

        internal static DhcpServerOptionValue GetScopeReservationVendorOptionValue(DhcpServerScopeReservation reservation, int optionId, string vendorName)
            => GetScopeReservationOptionValue(reservation, optionId, null, vendorName);

        internal static DhcpServerOptionValue GetScopeReservationUserOptionValue(DhcpServerScopeReservation reservation, int optionId, string className)
            => GetScopeReservationOptionValue(reservation, optionId, className, null);

        internal static DhcpServerOptionValue GetScopeReservationOptionValue(DhcpServerScopeReservation reservation, int optionId, string className, string vendorName)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_LocalReserved()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpReservedOptions,
                ReservedIpAddress = reservation.IpAddress.ToNativeAsNetwork(),
                ReservedIpSubnetAddress = reservation.Scope.Address.ToNativeAsNetwork()
            };

            using (var scopeInfoPtr = BitHelper.StructureToPtr(scopeInfo))
            {
                return GetOptionValue(reservation.Server, scopeInfoPtr, optionId, className, vendorName);
            }
        }

        internal static IEnumerable<DhcpServerOptionValue> EnumScopeDefaultOptionValues(DhcpServer server, DhcpServerIpAddress address)
            => EnumScopeOptionValues(server, address, null, null);
        internal static IEnumerable<DhcpServerOptionValue> EnumScopeDefaultOptionValues(DhcpServerScope scope)
            => EnumScopeOptionValues(scope, null, null);

        internal static IEnumerable<DhcpServerOptionValue> EnumScopeVendorOptionValues(DhcpServer server, DhcpServerIpAddress address, string vendorName)
            => EnumScopeOptionValues(server, address, null, vendorName);
        internal static IEnumerable<DhcpServerOptionValue> EnumScopeVendorOptionValues(DhcpServerScope scope, string vendorName)
            => EnumScopeOptionValues(scope, null, vendorName);

        internal static IEnumerable<DhcpServerOptionValue> EnumScopeUserOptionValues(DhcpServer server, DhcpServerIpAddress address, string className)
            => EnumScopeOptionValues(server, address, className, null);
        internal static IEnumerable<DhcpServerOptionValue> EnumScopeUserOptionValues(DhcpServerScope scope, string className)
            => EnumScopeOptionValues(scope, className, null);

        private static IEnumerable<DhcpServerOptionValue> EnumScopeOptionValues(DhcpServerScope scope, string className, string vendorName)
            => EnumScopeOptionValues(scope.Server, scope.Address, className, vendorName);

        private static IEnumerable<DhcpServerOptionValue> EnumScopeOptionValues(DhcpServer server, DhcpServerIpAddress address, string className, string vendorName)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_LocalSubnet()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpSubnetOptions,
                SubnetScopeInfo = address.ToNativeAsNetwork()
            };

            using (var scopeInfoPtr = BitHelper.StructureToPtr(scopeInfo))
            {
                foreach (var value in EnumOptionValues(server, scopeInfoPtr, className, vendorName))
                    yield return value;
            }
        }

        internal static DhcpServerOptionValue GetScopeDefaultOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, int optionId)
            => GetScopeOptionValue(server, scopeAddress, optionId, null, null);
        internal static DhcpServerOptionValue GetScopeDefaultOptionValue(DhcpServerScope scope, int optionId)
            => GetScopeOptionValue(scope, optionId, null, null);
        internal static DhcpServerOptionValue GetScopeVendorOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, int optionId, string vendorName)
            => GetScopeOptionValue(server, scopeAddress, optionId, null, vendorName);
        internal static DhcpServerOptionValue GetScopeVendorOptionValue(DhcpServerScope scope, int optionId, string vendorName)
            => GetScopeOptionValue(scope, optionId, null, vendorName);
        internal static DhcpServerOptionValue GetScopeUserOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, int optionId, string className)
            => GetScopeOptionValue(server, scopeAddress, optionId, className, null);
        internal static DhcpServerOptionValue GetScopeUserOptionValue(DhcpServerScope scope, int optionId, string className)
            => GetScopeOptionValue(scope, optionId, className, null);

        internal static DhcpServerOptionValue GetScopeOptionValue(DhcpServerScope scope, int optionId, string className, string vendorName)
            => GetScopeOptionValue(scope.Server, scope.Address, optionId, className, vendorName);

        internal static DhcpServerOptionValue GetScopeOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, int optionId, string className, string vendorName)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_LocalSubnet()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpSubnetOptions,
                SubnetScopeInfo = scopeAddress.ToNativeAsNetwork()
            };

            using (var scopeInfoPtr = BitHelper.StructureToPtr(scopeInfo))
            {
                return GetOptionValue(server, scopeInfoPtr, optionId, className, vendorName);
            }
        }

        internal static IEnumerable<DhcpServerOptionValue> GetAllGlobalOptionValues(DhcpServer server)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_LocalGlobal()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpGlobalOptions,
                GlobalScopeInfo = IntPtr.Zero
            };

            using (var scopeInfoPtr = BitHelper.StructureToPtr(scopeInfo))
            {
                foreach (var value in GetAllOptionValues(server, scopeInfoPtr))
                    yield return value;
            }
        }

        internal static IEnumerable<DhcpServerOptionValue> GetAllScopeOptionValues(DhcpServerScope scope)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_LocalSubnet()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpSubnetOptions,
                SubnetScopeInfo = scope.Address.ToNativeAsNetwork()
            };

            using (var scopeInfoPtr = BitHelper.StructureToPtr(scopeInfo))
            {
                foreach (var value in GetAllOptionValues(scope.Server, scopeInfoPtr))
                    yield return value;
            }
        }

        internal static IEnumerable<DhcpServerOptionValue> GetAllScopeReservationOptionValues(DhcpServerScopeReservation reservation)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_LocalReserved()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpReservedOptions,
                ReservedIpAddress = reservation.IpAddress.ToNativeAsNetwork(),
                ReservedIpSubnetAddress = reservation.Scope.Address.ToNativeAsNetwork()
            };

            using (var scopeInfoPtr = BitHelper.StructureToPtr(scopeInfo))
            {
                foreach (var value in GetAllOptionValues(reservation.Server, scopeInfoPtr))
                    yield return value;
            }
        }

        private static IEnumerable<DhcpServerOptionValue> GetAllOptionValues(DhcpServer server, IntPtr scopeInfo)
        {
            var result = Api.DhcpGetAllOptionValues(ServerIpAddress: server.IpAddress,
                                                    Flags: 0,
                                                    ScopeInfo: scopeInfo,
                                                    Values: out var valuesPtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetAllOptionValues), result);

            try
            {
                using (var values = valuesPtr.MarshalToStructure<DHCP_ALL_OPTION_VALUES>())
                {
                    foreach (var optionClass in values.Options)
                    {
                        foreach (var value in optionClass.OptionsArray.Values)
                        {
                            // Ignore OptionID 81 (Used for DNS Settings - has no matching Option)
                            if (!(value.OptionID == 81 && optionClass.ClassName == null && optionClass.VendorName == null))
                            {
                                var valueRef = value;
                                yield return FromNative(server, ref valueRef, optionClass.ClassName, optionClass.VendorName);
                            }
                        }
                    }
                }
            }
            finally
            {
                Api.FreePointer(valuesPtr);
            }
        }

        internal static IEnumerable<DhcpServerOptionValue> EnumGlobalDefaultOptionValues(DhcpServer server)
            => EnumGlobalOptionValues(server, null, null);

        internal static IEnumerable<DhcpServerOptionValue> EnumGlobalVendorOptionValues(DhcpServer server, string vendorName)
            => EnumGlobalOptionValues(server, null, vendorName);

        internal static IEnumerable<DhcpServerOptionValue> EnumGlobalUserOptionValues(DhcpServer server, string className)
            => EnumGlobalOptionValues(server, className, null);

        private static IEnumerable<DhcpServerOptionValue> EnumGlobalOptionValues(DhcpServer server, string className, string vendorName)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_LocalGlobal()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpGlobalOptions,
                GlobalScopeInfo = IntPtr.Zero
            };

            using (var scopeInfoPtr = BitHelper.StructureToPtr(scopeInfo))
            {
                foreach (var value in EnumOptionValues(server, scopeInfoPtr, className, vendorName))
                    yield return value;
            }
        }

        internal static DhcpServerOptionValue GetGlobalDefaultOptionValue(DhcpServer server, int optionId)
            => GetGlobalOptionValue(server, optionId, null, null);

        internal static DhcpServerOptionValue GetGlobalVendorOptionValue(DhcpServer server, int optionId, string vendorName)
            => GetGlobalOptionValue(server, optionId, null, vendorName);

        internal static DhcpServerOptionValue GetGlobalUserOptionValue(DhcpServer server, int optionId, string className)
            => GetGlobalOptionValue(server, optionId, className, null);

        internal static DhcpServerOptionValue GetGlobalOptionValue(DhcpServer server, int optionId, string className, string vendorName)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_LocalGlobal()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpGlobalOptions,
                GlobalScopeInfo = IntPtr.Zero
            };

            using (var scopeInfoPtr = BitHelper.StructureToPtr(scopeInfo))
            {
                return GetOptionValue(server, scopeInfoPtr, optionId, className, vendorName);
            }
        }

        private static IEnumerable<DhcpServerOptionValue> EnumOptionValues(DhcpServer server, IntPtr scopeInfo, string className, string vendorName)
        {
            if (server.IsCompatible(DhcpServerVersions.Windows2008R2))
                return EnumOptionValuesV5(server, scopeInfo, className, vendorName);
            else
            {
                if (vendorName != null || className != null)
                    throw new PlatformNotSupportedException($"DHCP Server v{server.VersionMajor}.{server.VersionMinor} does not support this feature");

                return EnumOptionValuesV0(server, scopeInfo);
            }
        }

        private static IEnumerable<DhcpServerOptionValue> EnumOptionValuesV0(DhcpServer server, IntPtr scopeInfo)
        {
            var resumeHandle = IntPtr.Zero;
            var result = Api.DhcpEnumOptionValues(ServerIpAddress: server.IpAddress,
                                                  ScopeInfo: scopeInfo,
                                                  ResumeHandle: ref resumeHandle,
                                                  PreferredMaximum: 0xFFFFFFFF,
                                                  OptionValues: out var valueArrayPtr,
                                                  OptionsRead: out var elementsRead,
                                                  OptionsTotal: out _);

            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS || result == DhcpErrors.EPT_S_NOT_REGISTERED)
                yield break;

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                throw new DhcpServerException(nameof(Api.DhcpEnumOptionValues), result);

            if (elementsRead == 0)
                yield break;

            try
            {
                using (var valueArray = valueArrayPtr.MarshalToStructure<DHCP_OPTION_VALUE_ARRAY>())
                {
                    foreach (var value in valueArray.Values)
                    {
                        if (value.OptionID != 81)
                        {
                            var valueRef = value;
                            yield return FromNative(server, ref valueRef, null, null);
                        }
                    }
                }
            }
            finally
            {
                Api.FreePointer(valueArrayPtr);
            }
        }

        private static IEnumerable<DhcpServerOptionValue> EnumOptionValuesV5(DhcpServer server, IntPtr scopeInfo, string className, string vendorName)
        {
            var resumeHandle = IntPtr.Zero;
            var result = Api.DhcpEnumOptionValuesV5(ServerIpAddress: server.IpAddress,
                                                    Flags: (vendorName == null) ? 0 : Constants.DHCP_FLAGS_OPTION_IS_VENDOR,
                                                    ClassName: className,
                                                    VendorName: vendorName,
                                                    ScopeInfo: scopeInfo,
                                                    ResumeHandle: ref resumeHandle,
                                                    PreferredMaximum: 0xFFFFFFFF,
                                                    OptionValues: out var valueArrayPtr,
                                                    OptionsRead: out var elementsRead,
                                                    OptionsTotal: out _);

            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS || result == DhcpErrors.EPT_S_NOT_REGISTERED)
                yield break;

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                throw new DhcpServerException(nameof(Api.DhcpEnumOptionValuesV5), result);

            if (elementsRead == 0)
                yield break;

            try
            {
                using (var valueArray = valueArrayPtr.MarshalToStructure<DHCP_OPTION_VALUE_ARRAY>())
                {
                    foreach (var value in valueArray.Values)
                    {
                        if (value.OptionID != 81)
                        {
                            var valueRef = value;
                            yield return FromNative(server, ref valueRef, className, vendorName);
                        }
                    }
                }
            }
            finally
            {
                Api.FreePointer(valueArrayPtr);
            }
        }

        private static DhcpServerOptionValue GetOptionValue(DhcpServer server, IntPtr scopeInfo, int optionId, string className, string vendorName)
        {
            if (server.IsCompatible(DhcpServerVersions.Windows2008R2))
                return GetOptionValueV5(server, scopeInfo, optionId, className, vendorName);
            else
            {
                if (vendorName != null || className != null)
                    throw new PlatformNotSupportedException($"DHCP Server v{server.VersionMajor}.{server.VersionMinor} does not support this feature");

                return GetOptionValueV0(server, scopeInfo, optionId);
            }
        }

        private static DhcpServerOptionValue GetOptionValueV0(DhcpServer server, IntPtr scopeInfo, int optionId)
        {
            var result = Api.DhcpGetOptionValue(ServerIpAddress: server.IpAddress,
                                                OptionID: optionId,
                                                ScopeInfo: scopeInfo,
                                                OptionValue: out var valuePtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetOptionValue), result);

            try
            {
                using (var value = valuePtr.MarshalToStructure<DHCP_OPTION_VALUE>())
                {
                    var valueRef = value;
                    return FromNative(server, ref valueRef, null, null);
                }
            }
            finally
            {
                Api.FreePointer(valuePtr);
            }
        }

        private static DhcpServerOptionValue GetOptionValueV5(DhcpServer server, IntPtr scopeInfo, int optionId, string className, string vendorName)
        {
            var result = Api.DhcpGetOptionValueV5(ServerIpAddress: server.IpAddress,
                                                  Flags: (vendorName == null) ? 0 : Constants.DHCP_FLAGS_OPTION_IS_VENDOR,
                                                  OptionID: optionId,
                                                  ClassName: className,
                                                  VendorName: vendorName,
                                                  ScopeInfo: scopeInfo,
                                                  OptionValue: out var valuePtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetOptionValueV5), result);

            try
            {
                using (var value = valuePtr.MarshalToStructure<DHCP_OPTION_VALUE>())
                {
                    var valueRef = value;
                    return FromNative(server, ref valueRef, className, vendorName);
                }
            }
            finally
            {
                Api.FreePointer(valuePtr);
            }
        }

        public override string ToString()
            => $"{VendorName ?? ClassName ?? Server.SpecificStrings.DefaultVendorClassName}: {OptionId} [{Option.Name}]: {string.Join("; ", Values)}";
    }
}
