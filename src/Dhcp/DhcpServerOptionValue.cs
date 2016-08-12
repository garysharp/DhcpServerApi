using Dhcp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Dhcp
{
    public class DhcpServerOptionValue
    {
        /// <summary>
        /// The associated DHCP Server
        /// </summary>
        public DhcpServer Server { get; private set; }

        public int OptionId { get; private set; }

        public string ClassName { get; private set; }

        public string VendorName { get; private set; }

        private Lazy<DhcpServerOption> option;
        /// <summary>
        /// The associated Option
        /// </summary>
        public DhcpServerOption Option { get { return option.Value; } }

        /// <summary>
        /// Contains the value associated with this option
        /// </summary>
        public List<DhcpServerOptionElement> Values { get; private set; }

        private DhcpServerOptionValue(DhcpServer Server)
        {
            this.Server = Server;
            this.option = new Lazy<DhcpServerOption>(GetOption);
        }

        private DhcpServerOption GetOption()
        {
            return DhcpServerOption.GetOption(this.Server, this.OptionId, this.ClassName, this.VendorName);
        }

        private static DhcpServerOptionValue FromNative(DhcpServer Server, DHCP_OPTION_VALUE Native, string ClassName, string VendorName)
        {
            return new DhcpServerOptionValue(Server)
            {
                OptionId = Native.OptionID,
                Values = DhcpServerOptionElement.ReadNativeElements(Native.Value).ToList(),
                ClassName = ClassName,
                VendorName = VendorName
            };
        }

        internal static IEnumerable<DhcpServerOptionValue> EnumScopeReservationDefaultOptionValues(DhcpServerScopeReservation Reservation)
        {
            return EnumScopeReservationOptionValues(Reservation, null, null);
        }

        internal static IEnumerable<DhcpServerOptionValue> EnumScopeReservationVendorOptionValues(DhcpServerScopeReservation Reservation, string VendorName)
        {
            return EnumScopeReservationOptionValues(Reservation, null, VendorName);
        }

        internal static IEnumerable<DhcpServerOptionValue> EnumScopeReservationUserOptionValues(DhcpServerScopeReservation Reservation, string ClassName)
        {
            return EnumScopeReservationOptionValues(Reservation, ClassName, null);
        }

        private static IEnumerable<DhcpServerOptionValue> EnumScopeReservationOptionValues(DhcpServerScopeReservation Reservation, string ClassName, string VendorName)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_RESERVED()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpReservedOptions,
                ReservedIpAddress = Reservation.ipAddress,
                ReservedIpSubnetAddress = Reservation.ipAddressMask
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);

            return EnumOptionValues(Reservation.Server, scopeInfoPtr, ClassName, VendorName);
        }

        internal static DhcpServerOptionValue GetScopeReservationDefaultOptionValue(DhcpServerScopeReservation Reservation, int OptionId)
        {
            return GetScopeReservationOptionValue(Reservation, OptionId, null, null);
        }

        internal static DhcpServerOptionValue GetScopeReservationVendorOptionValue(DhcpServerScopeReservation Reservation, int OptionId, string VendorName)
        {
            return GetScopeReservationOptionValue(Reservation, OptionId, null, VendorName);
        }

        internal static DhcpServerOptionValue GetScopeReservationUserOptionValue(DhcpServerScopeReservation Reservation, int OptionId, string ClassName)
        {
            return GetScopeReservationOptionValue(Reservation, OptionId, ClassName, null);
        }

        internal static DhcpServerOptionValue GetScopeReservationOptionValue(DhcpServerScopeReservation Reservation, int OptionId, string ClassName, string VendorName)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_RESERVED()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpReservedOptions,
                ReservedIpAddress = Reservation.ipAddress,
                ReservedIpSubnetAddress = Reservation.ipAddressMask
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);

            return GetOptionValue(Reservation.Server, scopeInfoPtr, OptionId, ClassName, VendorName);
        }

        internal static IEnumerable<DhcpServerOptionValue> EnumScopeDefaultOptionValues(DhcpServerScope Scope)
        {
            return EnumScopeOptionValues(Scope, null, null);
        }

        internal static IEnumerable<DhcpServerOptionValue> EnumScopeVendorOptionValues(DhcpServerScope Scope, string VendorName)
        {
            return EnumScopeOptionValues(Scope, null, VendorName);
        }

        internal static IEnumerable<DhcpServerOptionValue> EnumScopeUserOptionValues(DhcpServerScope Scope, string ClassName)
        {
            return EnumScopeOptionValues(Scope, ClassName, null);
        }

        private static IEnumerable<DhcpServerOptionValue> EnumScopeOptionValues(DhcpServerScope Scope, string ClassName, string VendorName)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_SUBNET()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpSubnetOptions,
                SubnetScopeInfo = Scope.address
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);

            return EnumOptionValues(Scope.Server, scopeInfoPtr, ClassName, VendorName);
        }

        internal static DhcpServerOptionValue GetScopeDefaultOptionValue(DhcpServerScope Scope, int OptionId)
        {
            return GetScopeOptionValue(Scope, OptionId, null, null);
        }

        internal static DhcpServerOptionValue GetScopeVendorOptionValue(DhcpServerScope Scope, int OptionId, string VendorName)
        {
            return GetScopeOptionValue(Scope, OptionId, null, VendorName);
        }

        internal static DhcpServerOptionValue GetScopeUserOptionValue(DhcpServerScope Scope, int OptionId, string ClassName)
        {
            return GetScopeOptionValue(Scope, OptionId, ClassName, null);
        }

        internal static DhcpServerOptionValue GetScopeOptionValue(DhcpServerScope Scope, int OptionId, string ClassName, string VendorName)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_SUBNET()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpSubnetOptions,
                SubnetScopeInfo = Scope.address
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);

            return GetOptionValue(Scope.Server, scopeInfoPtr, OptionId, ClassName, VendorName);
        }

        internal static IEnumerable<DhcpServerOptionValue> GetAllGlobalOptionValues(DhcpServer Server)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_GLOBAL()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpGlobalOptions,
                GlobalScopeInfo = IntPtr.Zero
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);

            return GetAllOptionValues(Server, scopeInfoPtr);
        }

        internal static IEnumerable<DhcpServerOptionValue> GetAllScopeOptionValues(DhcpServerScope Scope)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_SUBNET()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpSubnetOptions,
                SubnetScopeInfo = Scope.address
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);

            return GetAllOptionValues(Scope.Server, scopeInfoPtr);
        }

        internal static IEnumerable<DhcpServerOptionValue> GetAllScopeReservationOptionValues(DhcpServerScopeReservation Reservation)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_RESERVED()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpReservedOptions,
                ReservedIpAddress = Reservation.ipAddress,
                ReservedIpSubnetAddress = Reservation.ipAddressMask
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);

            return GetAllOptionValues(Reservation.Server, scopeInfoPtr);
        }

        private static IEnumerable<DhcpServerOptionValue> GetAllOptionValues(DhcpServer Server, IntPtr ScopeInfo)
        {
            try
            {
                IntPtr valuesPtr;

                var result = Api.DhcpGetAllOptionValues(Server.ipAddress.ToString(), 0, ScopeInfo, out valuesPtr);

                if (result != DhcpErrors.SUCCESS)
                    throw new DhcpServerException("DhcpGetAllOptionValues", result);

                var values = (DHCP_ALL_OPTION_VALUES)Marshal.PtrToStructure(valuesPtr, typeof(DHCP_ALL_OPTION_VALUES));

                try
                {
                    foreach (var @class in values.Options)
                    {
                        foreach (var value in @class.OptionsArray.Values)
                        {
                            // Ignore OptionID 81 (Used for DNS Settings - has no matching Option)
                            if (!(@class.ClassName == null && @class.VendorName == null && value.OptionID == 81))
                            {
                                yield return FromNative(Server, value, @class.ClassName, @class.VendorName);
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
                Marshal.FreeHGlobal(ScopeInfo);
            }
        }

        internal static IEnumerable<DhcpServerOptionValue> EnumGlobalDefaultOptionValues(DhcpServer Server)
        {
            return EnumGlobalOptionValues(Server, null, null);
        }

        internal static IEnumerable<DhcpServerOptionValue> EnumGlobalVendorOptionValues(DhcpServer Server, string VendorName)
        {
            return EnumGlobalOptionValues(Server, null, VendorName);
        }

        internal static IEnumerable<DhcpServerOptionValue> EnumGlobalUserOptionValues(DhcpServer Server, string ClassName)
        {
            return EnumGlobalOptionValues(Server, ClassName, null);
        }

        private static IEnumerable<DhcpServerOptionValue> EnumGlobalOptionValues(DhcpServer Server, string ClassName, string VendorName)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_GLOBAL()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpGlobalOptions,
                GlobalScopeInfo = IntPtr.Zero
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);

            return EnumOptionValues(Server, scopeInfoPtr, ClassName, VendorName);
        }

        internal static DhcpServerOptionValue GetGlobalDefaultOptionValue(DhcpServer Server, int OptionId)
        {
            return GetGlobalOptionValue(Server, OptionId, null, null);
        }

        internal static DhcpServerOptionValue GetGlobalVendorOptionValue(DhcpServer Server, int OptionId, string VendorName)
        {
            return GetGlobalOptionValue(Server, OptionId, null, VendorName);
        }

        internal static DhcpServerOptionValue GetGlobalUserOptionValue(DhcpServer Server, int OptionId, string ClassName)
        {
            return GetGlobalOptionValue(Server, OptionId, ClassName, null);
        }

        internal static DhcpServerOptionValue GetGlobalOptionValue(DhcpServer Server, int OptionId, string ClassName, string VendorName)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_GLOBAL()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpGlobalOptions,
                GlobalScopeInfo = IntPtr.Zero
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);

            return GetOptionValue(Server, scopeInfoPtr, OptionId, ClassName, VendorName);
        }

        private static IEnumerable<DhcpServerOptionValue> EnumOptionValues(DhcpServer Server, IntPtr ScopeInfo, string ClassName, string VendorName)
        {
            if (Server.IsCompatible(DhcpServerVersions.Windows2008R2))
            {
                return EnumOptionValuesV5(Server, ScopeInfo, ClassName, VendorName);
            }
            else
            {
                if (VendorName != null || ClassName != null)
                {
                    throw new PlatformNotSupportedException(string.Format("DHCP Server v{0}.{1} does not support this feature", Server.VersionMajor, Server.VersionMinor));
                }
                return EnumOptionValuesV0(Server, ScopeInfo);
            }
        }

        private static IEnumerable<DhcpServerOptionValue> EnumOptionValuesV0(DhcpServer Server, IntPtr ScopeInfo)
        {
            try
            {
                IntPtr valueArrayPtr;
                int elementsRead, elementsTotal;
                IntPtr resumeHandle = IntPtr.Zero;

                var result = Api.DhcpEnumOptionValues(Server.ipAddress.ToString(), ScopeInfo, ref resumeHandle, 0xFFFFFFFF, out valueArrayPtr, out elementsRead, out elementsTotal);

                if (result == DhcpErrors.ERROR_NO_MORE_ITEMS || result == DhcpErrors.EPT_S_NOT_REGISTERED)
                    yield break;

                if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                    throw new DhcpServerException("DhcpEnumOptionValues", result);

                if (elementsRead == 0)
                    yield break;

                var valueArray = (DHCP_OPTION_VALUE_ARRAY)Marshal.PtrToStructure(valueArrayPtr, typeof(DHCP_OPTION_VALUE_ARRAY));

                try
                {
                    foreach (var value in valueArray.Values)
                    {
                        if (value.OptionID != 81)
                        {
                            yield return FromNative(Server, value, null, null);
                        }
                    }
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(valueArrayPtr);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(ScopeInfo);
            }
        }

        private static IEnumerable<DhcpServerOptionValue> EnumOptionValuesV5(DhcpServer Server, IntPtr ScopeInfo, string ClassName, string VendorName)
        {
            try
            {
                IntPtr valueArrayPtr;
                int elementsRead, elementsTotal;
                IntPtr resumeHandle = IntPtr.Zero;
                uint flags = VendorName == null ? 0 : Constants.DHCP_FLAGS_OPTION_IS_VENDOR;

                var result = Api.DhcpEnumOptionValuesV5(Server.ipAddress.ToString(), flags, ClassName, VendorName, ScopeInfo, ref resumeHandle, 0xFFFFFFFF, out valueArrayPtr, out elementsRead, out elementsTotal);

                if (result == DhcpErrors.ERROR_NO_MORE_ITEMS || result == DhcpErrors.EPT_S_NOT_REGISTERED)
                    yield break;

                if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                    throw new DhcpServerException("DhcpEnumOptionValuesV5", result);

                if (elementsRead == 0)
                    yield break;

                var valueArray = (DHCP_OPTION_VALUE_ARRAY)Marshal.PtrToStructure(valueArrayPtr, typeof(DHCP_OPTION_VALUE_ARRAY));

                try
                {
                    foreach (var value in valueArray.Values)
                    {
                        if (value.OptionID != 81)
                        {
                            yield return FromNative(Server, value, ClassName, VendorName);
                        }
                    }
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(valueArrayPtr);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(ScopeInfo);
            }
        }

        private static DhcpServerOptionValue GetOptionValue(DhcpServer Server, IntPtr ScopeInfo, int OptionId, string ClassName, string VendorName)
        {
            if (Server.IsCompatible(DhcpServerVersions.Windows2008R2))
            {
                return GetOptionValueV5(Server, ScopeInfo, OptionId, ClassName, VendorName);
            }
            else
            {
                if (VendorName != null || ClassName != null)
                {
                    throw new PlatformNotSupportedException(string.Format("DHCP Server v{0}.{1} does not support this feature", Server.VersionMajor, Server.VersionMinor));
                }
                return GetOptionValueV0(Server, ScopeInfo, OptionId);
            }
        }

        private static DhcpServerOptionValue GetOptionValueV0(DhcpServer Server, IntPtr ScopeInfo, int OptionId)
        {
            try
            {
                IntPtr valuePtr;

                var result = Api.DhcpGetOptionValue(Server.ipAddress.ToString(), OptionId, ScopeInfo, out valuePtr);

                if (result != DhcpErrors.SUCCESS)
                    throw new DhcpServerException("DhcpGetOptionValue", result);

                try
                {
                    var value = (DHCP_OPTION_VALUE)Marshal.PtrToStructure(valuePtr, typeof(DHCP_OPTION_VALUE));

                    return FromNative(Server, value, null, null);
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(valuePtr);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(ScopeInfo);
            }
        }

        private static DhcpServerOptionValue GetOptionValueV5(DhcpServer Server, IntPtr ScopeInfo, int OptionId, string ClassName, string VendorName)
        {
            try
            {
                IntPtr valuePtr;
                uint flags = VendorName == null ? 0 : Constants.DHCP_FLAGS_OPTION_IS_VENDOR;

                var result = Api.DhcpGetOptionValueV5(Server.ipAddress.ToString(), flags, OptionId, ClassName, VendorName, ScopeInfo, out valuePtr);

                if (result != DhcpErrors.SUCCESS)
                    throw new DhcpServerException("DhcpGetOptionValueV5", result);

                try
                {
                    var value = (DHCP_OPTION_VALUE)Marshal.PtrToStructure(valuePtr, typeof(DHCP_OPTION_VALUE));

                    return FromNative(Server, value, ClassName, VendorName);
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(valuePtr);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(ScopeInfo);
            }
        }

        public override string ToString()
        {
            string className;
            if (VendorName != null)
                className = VendorName;
            else if (ClassName != null)
                className = ClassName;
            else
                className = Server.DefaultVendorClassName;

            return $"{className}: {OptionId} [{Option.Name}]: {string.Join("; ", Values)}";
        }
    }
}
