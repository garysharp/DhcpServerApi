using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        private static DhcpServerOptionValue FromNative(DhcpServer server, DHCP_OPTION_VALUE native, string className, string vendorName)
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
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_RESERVED()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpReservedOptions,
                ReservedIpAddress = reservation.ipAddress,
                ReservedIpSubnetAddress = reservation.ipAddressMask
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);
            return EnumOptionValues(reservation.Server, scopeInfoPtr, className, vendorName);
        }

        internal static DhcpServerOptionValue GetScopeReservationDefaultOptionValue(DhcpServerScopeReservation reservation, int optionId)
            => GetScopeReservationOptionValue(reservation, optionId, null, null);

        internal static DhcpServerOptionValue GetScopeReservationVendorOptionValue(DhcpServerScopeReservation reservation, int optionId, string vendorName)
            => GetScopeReservationOptionValue(reservation, optionId, null, vendorName);

        internal static DhcpServerOptionValue GetScopeReservationUserOptionValue(DhcpServerScopeReservation reservation, int optionId, string className)
            => GetScopeReservationOptionValue(reservation, optionId, className, null);

        internal static DhcpServerOptionValue GetScopeReservationOptionValue(DhcpServerScopeReservation reservation, int optionId, string className, string vendorName)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_RESERVED()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpReservedOptions,
                ReservedIpAddress = reservation.ipAddress,
                ReservedIpSubnetAddress = reservation.ipAddressMask
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            try
            {
                Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);
                return GetOptionValue(reservation.Server, scopeInfoPtr, optionId, className, vendorName);
            }
            finally
            {
                Marshal.FreeHGlobal(scopeInfoPtr);
            }
        }

        internal static IEnumerable<DhcpServerOptionValue> EnumScopeDefaultOptionValues(DhcpServerScope scope)
            => EnumScopeOptionValues(scope, null, null);

        internal static IEnumerable<DhcpServerOptionValue> EnumScopeVendorOptionValues(DhcpServerScope scope, string vendorName)
            => EnumScopeOptionValues(scope, null, vendorName);

        internal static IEnumerable<DhcpServerOptionValue> EnumScopeUserOptionValues(DhcpServerScope scope, string className)
            => EnumScopeOptionValues(scope, className, null);

        private static IEnumerable<DhcpServerOptionValue> EnumScopeOptionValues(DhcpServerScope scope, string className, string vendorName)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_SUBNET()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpSubnetOptions,
                SubnetScopeInfo = scope.address
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);
            return EnumOptionValues(scope.Server, scopeInfoPtr, className, vendorName);
        }

        internal static DhcpServerOptionValue GetScopeDefaultOptionValue(DhcpServer server, DHCP_IP_ADDRESS address, int optionId)
            => GetScopeOptionValue(server, address, optionId, null, null);
        internal static DhcpServerOptionValue GetScopeDefaultOptionValue(DhcpServerScope scope, int optionId)
            => GetScopeOptionValue(scope, optionId, null, null);
        internal static DhcpServerOptionValue GetScopeVendorOptionValue(DhcpServer server, DHCP_IP_ADDRESS address, int optionId, string vendorName)
            => GetScopeOptionValue(server, address, optionId, null, vendorName);
        internal static DhcpServerOptionValue GetScopeVendorOptionValue(DhcpServerScope scope, int optionId, string vendorName)
            => GetScopeOptionValue(scope, optionId, null, vendorName);
        internal static DhcpServerOptionValue GetScopeUserOptionValue(DhcpServer server, DHCP_IP_ADDRESS address, int optionId, string className)
            => GetScopeOptionValue(server, address, optionId, className, null);
        internal static DhcpServerOptionValue GetScopeUserOptionValue(DhcpServerScope scope, int optionId, string className)
            => GetScopeOptionValue(scope, optionId, className, null);

        internal static DhcpServerOptionValue GetScopeOptionValue(DhcpServerScope scope, int optionId, string className, string vendorName)
            => GetScopeOptionValue(scope.Server, scope.address, optionId, className, vendorName);

        internal static DhcpServerOptionValue GetScopeOptionValue(DhcpServer server, DHCP_IP_ADDRESS address, int optionId, string className, string vendorName)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_SUBNET()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpSubnetOptions,
                SubnetScopeInfo = address
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            try
            {
                Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);
                return GetOptionValue(server, scopeInfoPtr, optionId, className, vendorName);
            }
            finally
            {
                Marshal.FreeHGlobal(scopeInfoPtr);
            }
        }

        internal static IEnumerable<DhcpServerOptionValue> GetAllGlobalOptionValues(DhcpServer server)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_GLOBAL()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpGlobalOptions,
                GlobalScopeInfo = IntPtr.Zero
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);
            return GetAllOptionValues(server, scopeInfoPtr);
        }

        internal static IEnumerable<DhcpServerOptionValue> GetAllScopeOptionValues(DhcpServerScope scope)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_SUBNET()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpSubnetOptions,
                SubnetScopeInfo = scope.address
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);
            return GetAllOptionValues(scope.Server, scopeInfoPtr);
        }

        internal static IEnumerable<DhcpServerOptionValue> GetAllScopeReservationOptionValues(DhcpServerScopeReservation reservation)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_RESERVED()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpReservedOptions,
                ReservedIpAddress = reservation.ipAddress,
                ReservedIpSubnetAddress = reservation.ipAddressMask
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);
            return GetAllOptionValues(reservation.Server, scopeInfoPtr);
        }

        private static IEnumerable<DhcpServerOptionValue> GetAllOptionValues(DhcpServer server, IntPtr scopeInfo)
        {
            try
            {
                var result = Api.DhcpGetAllOptionValues(ServerIpAddress: server.ipAddress,
                                                    Flags: 0,
                                                    ScopeInfo: scopeInfo,
                                                    Values: out var valuesPtr);

                if (result != DhcpErrors.SUCCESS)
                    throw new DhcpServerException(nameof(Api.DhcpGetAllOptionValues), result);

                var values = valuesPtr.MarshalToStructure<DHCP_ALL_OPTION_VALUES>();

                try
                {
                    foreach (var option in values.Options)
                    {
                        foreach (var value in option.OptionsArray.Values)
                        {
                            // Ignore OptionID 81 (Used for DNS Settings - has no matching Option)
                            if (!(option.ClassName == null && option.VendorName == null && value.OptionID == 81))
                            {
                                yield return FromNative(server, value, option.ClassName, option.VendorName);
                            }
                        }
                    }
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(valuesPtr);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(scopeInfo);
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
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_GLOBAL()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpGlobalOptions,
                GlobalScopeInfo = IntPtr.Zero
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);
            return EnumOptionValues(server, scopeInfoPtr, className, vendorName);
        }

        internal static DhcpServerOptionValue GetGlobalDefaultOptionValue(DhcpServer server, int optionId)
            => GetGlobalOptionValue(server, optionId, null, null);

        internal static DhcpServerOptionValue GetGlobalVendorOptionValue(DhcpServer server, int optionId, string vendorName)
            => GetGlobalOptionValue(server, optionId, null, vendorName);

        internal static DhcpServerOptionValue GetGlobalUserOptionValue(DhcpServer server, int optionId, string className)
            => GetGlobalOptionValue(server, optionId, className, null);

        internal static DhcpServerOptionValue GetGlobalOptionValue(DhcpServer server, int optionId, string className, string vendorName)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_GLOBAL()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpGlobalOptions,
                GlobalScopeInfo = IntPtr.Zero
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            try
            {
                Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);
                return GetOptionValue(server, scopeInfoPtr, optionId, className, vendorName);
            }
            finally
            {
                Marshal.FreeHGlobal(scopeInfoPtr);
            }
        }

        private static IEnumerable<DhcpServerOptionValue> EnumOptionValues(DhcpServer server, IntPtr scopeInfo, string className, string vendorName)
        {
            if (server.IsCompatible(DhcpServerVersions.Windows2008R2))
            {
                return EnumOptionValuesV5(server, scopeInfo, className, vendorName);
            }
            else
            {
                if (vendorName != null || className != null)
                {
                    throw new PlatformNotSupportedException($"DHCP Server v{server.VersionMajor}.{server.VersionMinor} does not support this feature");
                }
                return EnumOptionValuesV0(server, scopeInfo);
            }
        }

        private static IEnumerable<DhcpServerOptionValue> EnumOptionValuesV0(DhcpServer server, IntPtr scopeInfo)
        {
            try
            {
                var resumeHandle = IntPtr.Zero;

                var result = Api.DhcpEnumOptionValues(ServerIpAddress: server.ipAddress,
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

                var valueArray = valueArrayPtr.MarshalToStructure<DHCP_OPTION_VALUE_ARRAY>();

                try
                {
                    foreach (var value in valueArray.Values)
                        if (value.OptionID != 81)
                            yield return FromNative(server, value, null, null);
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(valueArrayPtr);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(scopeInfo);
            }
        }

        private static IEnumerable<DhcpServerOptionValue> EnumOptionValuesV5(DhcpServer server, IntPtr scopeInfo, string className, string vendorName)
        {
            try
            {
                var resumeHandle = IntPtr.Zero;

                var result = Api.DhcpEnumOptionValuesV5(ServerIpAddress: server.ipAddress,
                                                        Flags: (vendorName == null) ? 0U : Constants.DHCP_FLAGS_OPTION_IS_VENDOR,
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

                var valueArray = valueArrayPtr.MarshalToStructure<DHCP_OPTION_VALUE_ARRAY>();

                try
                {
                    foreach (var value in valueArray.Values)
                        if (value.OptionID != 81)
                            yield return FromNative(server, value, className, vendorName);
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(valueArrayPtr);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(scopeInfo);
            }
        }

        private static DhcpServerOptionValue GetOptionValue(DhcpServer server, IntPtr scopeInfo, int optionId, string className, string vendorName)
        {
            if (server.IsCompatible(DhcpServerVersions.Windows2008R2))
            {
                return GetOptionValueV5(server, scopeInfo, optionId, className, vendorName);
            }
            else
            {
                if (vendorName != null || className != null)
                {
                    throw new PlatformNotSupportedException($"DHCP Server v{server.VersionMajor}.{server.VersionMinor} does not support this feature");
                }
                return GetOptionValueV0(server, scopeInfo, optionId);
            }
        }

        private static DhcpServerOptionValue GetOptionValueV0(DhcpServer server, IntPtr scopeInfo, int optionId)
        {
            var result = Api.DhcpGetOptionValue(ServerIpAddress: server.ipAddress,
                                            OptionID: optionId,
                                            ScopeInfo: scopeInfo,
                                            OptionValue: out var valuePtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetOptionValue), result);

            try
            {
                var value = valuePtr.MarshalToStructure<DHCP_OPTION_VALUE>();
                return FromNative(server, value, null, null);
            }
            finally
            {
                Api.DhcpRpcFreeMemory(valuePtr);
            }
        }

        private static DhcpServerOptionValue GetOptionValueV5(DhcpServer server, IntPtr scopeInfo, int optionId, string className, string vendorName)
        {
            var result = Api.DhcpGetOptionValueV5(ServerIpAddress: server.ipAddress,
                                              Flags: (vendorName == null) ? 0U : Constants.DHCP_FLAGS_OPTION_IS_VENDOR,
                                              OptionID: optionId,
                                              ClassName: className,
                                              VendorName: vendorName,
                                              ScopeInfo: scopeInfo,
                                              OptionValue: out var valuePtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetOptionValueV5), result);

            try
            {
                var value = valuePtr.MarshalToStructure<DHCP_OPTION_VALUE>();
                return FromNative(server, value, className, vendorName);
            }
            finally
            {
                Api.DhcpRpcFreeMemory(valuePtr);
            }
        }

        public override string ToString()
            => $"{VendorName ?? ClassName ?? Server.DefaultVendorClassName}: {OptionId} [{Option.Name}]: {string.Join("; ", Values)}";
    }
}
