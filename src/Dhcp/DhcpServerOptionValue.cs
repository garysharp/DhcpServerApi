using System;
using System.Collections.Generic;
using System.Linq;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerOptionValue : IDhcpServerOptionValue
    {
        /// <summary>
        /// The associated DHCP Server
        /// </summary>
        public DhcpServer Server { get; }
        IDhcpServer IDhcpServerOptionValue.Server => Server;
        public int OptionId { get; }
        public string ClassName { get; }
        public string VendorName { get; }

        private DhcpServerOption option;
        /// <summary>
        /// The associated Option
        /// </summary>
        public IDhcpServerOption Option => option ??= GetOption();

        /// <summary>
        /// Contains the value associated with this option
        /// </summary>
        public IEnumerable<IDhcpServerOptionElement> Values { get; }

        internal DhcpServerOptionValue(DhcpServer server, int optionId, string className, string vendorName, List<DhcpServerOptionElement> values)
        {
            Server = server;

            OptionId = optionId;

            ClassName = className;
            VendorName = vendorName;

            Values = values;
        }

        internal DhcpServerOptionValue(DhcpServer server, int optionId, List<DhcpServerOptionElement> values)
            : this(server, optionId, className: null, vendorName: null, values)
        {
        }

        private DhcpServerOption GetOption() => DhcpServerOption.GetOption(Server, OptionId, ClassName, VendorName);

        private static DhcpServerOptionValue FromNative(DhcpServer server, in DHCP_OPTION_VALUE native, string className, string vendorName)
        {
            return new DhcpServerOptionValue(server: server,
                                             optionId: native.OptionID,
                                             className: className,
                                             vendorName: vendorName,
                                             values: DhcpServerOptionElement.ReadNativeElements(native.Value).ToList());
        }

        #region Global Option Values

        internal static IEnumerable<DhcpServerOptionValue> GetAllGlobalOptionValues(DhcpServer server)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_Managed_Global(IntPtr.Zero);

            using (var scopeInfoPtr = BitHelper.StructureToPtr(scopeInfo))
            {
                foreach (var value in GetAllOptionValues(server, scopeInfoPtr))
                    yield return value;
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
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_Managed_Global(IntPtr.Zero);

            using (var scopeInfoPtr = BitHelper.StructureToPtr(scopeInfo))
            {
                foreach (var value in EnumOptionValues(server, scopeInfoPtr, className, vendorName))
                    yield return value;
            }
        }


        internal static DhcpServerOptionValue GetGlobalDefaultOptionValue(DhcpServer server, int optionId)
            => GetGlobalOptionValue(server, optionId, null, null);

        internal static DhcpServerOptionValue GetGlobalVendorOptionValue(DhcpServer server, string vendorName, int optionId)
            => GetGlobalOptionValue(server, optionId, null, vendorName);

        internal static DhcpServerOptionValue GetGlobalUserOptionValue(DhcpServer server, string className, int optionId)
            => GetGlobalOptionValue(server, optionId, className, null);

        internal static DhcpServerOptionValue GetGlobalOptionValue(DhcpServer server, int optionId, string className, string vendorName)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_Managed_Global(IntPtr.Zero);

            using (var scopeInfoPtr = BitHelper.StructureToPtr(scopeInfo))
            {
                return GetOptionValue(server, scopeInfoPtr, optionId, className, vendorName);
            }
        }


        internal static void SetGlobalDefaultOptionValue(DhcpServer server, int optionId, IEnumerable<DhcpServerOptionElement> values)
            => SetGlobalOptionValue(server, optionId, null, null, values);
        internal static void SetGlobalVendorOptionValue(DhcpServer server, int optionId, string vendorName, IEnumerable<DhcpServerOptionElement> values)
            => SetGlobalOptionValue(server, optionId, null, vendorName, values);
        internal static void SetGlobalUserOptionValue(DhcpServer server, int optionId, string className, IEnumerable<DhcpServerOptionElement> values)
            => SetGlobalOptionValue(server, optionId, className, null, values);

        internal static void SetGlobalOptionValue(DhcpServer server, DhcpServerOptionValue optionValue)
            => SetGlobalOptionValue(server, optionValue.OptionId, optionValue.ClassName, optionValue.VendorName, optionValue.Values);

        internal static void SetGlobalOptionValue(DhcpServer server, int optionId, string className, string vendorName, IEnumerable<IDhcpServerOptionElement> values)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_Managed_Global(IntPtr.Zero);

            using (var scopeInfoPtr = BitHelper.StructureToPtr(scopeInfo))
            {
                SetOptionValue(server, scopeInfoPtr, optionId, className, vendorName, values);
            }
        }


        internal static void DeleteGlobalOptionValue(DhcpServer server, int optionId)
            => DeleteGlobalOptionValue(server, optionId, null, null);
        internal static void DeleteGlobalVendorOptionValue(DhcpServer server, int optionId, string vendorName)
            => DeleteGlobalOptionValue(server, optionId, null, vendorName);
        internal static void DeleteGlobalUserOptionValue(DhcpServer server, int optionId, string className)
            => DeleteGlobalOptionValue(server, optionId, className, null);

        internal static void DeleteGlobalOptionValue(DhcpServer server, DhcpServerOptionValue optionValue)
            => DeleteGlobalOptionValue(server, optionValue.OptionId, optionValue.ClassName, optionValue.VendorName);

        internal static void DeleteGlobalOptionValue(DhcpServer server, int optionId, string className, string vendorName)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_Managed_Global(IntPtr.Zero);

            using (var scopeInfoPtr = BitHelper.StructureToPtr(scopeInfo))
            {
                RemoveOptionValue(server, scopeInfoPtr, optionId, className, vendorName);
            }
        }

        #endregion

        #region Scope Option Values

        internal static IEnumerable<DhcpServerOptionValue> GetAllScopeOptionValues(DhcpServerScope scope)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_Managed_Subnet(scope.Address.ToNativeAsNetwork());

            using (var scopeInfoPtr = BitHelper.StructureToPtr(scopeInfo))
            {
                foreach (var value in GetAllOptionValues(scope.Server, scopeInfoPtr))
                    yield return value;
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
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_Managed_Subnet(address.ToNativeAsNetwork());

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
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_Managed_Subnet(scopeAddress.ToNativeAsNetwork());

            using (var scopeInfoPtr = BitHelper.StructureToPtr(scopeInfo))
            {
                return GetOptionValue(server, scopeInfoPtr, optionId, className, vendorName);
            }
        }


        internal static void SetScopeDefaultOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, int optionId, IEnumerable<DhcpServerOptionElement> values)
            => SetScopeOptionValue(server, scopeAddress, optionId, null, null, values);
        internal static void SetScopeDefaultOptionValue(DhcpServerScope scope, int optionId, IEnumerable<DhcpServerOptionElement> values)
            => SetScopeOptionValue(scope, optionId, null, null, values);
        internal static void SetScopeVendorOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, int optionId, string vendorName, IEnumerable<DhcpServerOptionElement> values)
            => SetScopeOptionValue(server, scopeAddress, optionId, null, vendorName, values);
        internal static void SetScopeVendorOptionValue(DhcpServerScope scope, int optionId, string vendorName, IEnumerable<DhcpServerOptionElement> values)
            => SetScopeOptionValue(scope, optionId, null, vendorName, values);
        internal static void SetScopeUserOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, int optionId, string className, IEnumerable<DhcpServerOptionElement> values)
            => SetScopeOptionValue(server, scopeAddress, optionId, className, null, values);
        internal static void SetScopeUserOptionValue(DhcpServerScope scope, int optionId, string className, IEnumerable<DhcpServerOptionElement> values)
            => SetScopeOptionValue(scope, optionId, className, null, values);

        internal static void SetScopeOptionValue(DhcpServerScope scope, DhcpServerOptionValue optionValue)
            => SetScopeOptionValue(scope, optionValue.OptionId, optionValue.ClassName, optionValue.VendorName, optionValue.Values);
        internal static void SetScopeOptionValue(DhcpServerScope scope, int optionId, string className, string vendorName, IEnumerable<IDhcpServerOptionElement> values)
            => SetScopeOptionValue(scope.Server, scope.Address, optionId, className, vendorName, values);

        internal static void SetScopeOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, int optionId, string className, string vendorName, IEnumerable<IDhcpServerOptionElement> values)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_Managed_Subnet(scopeAddress.ToNativeAsNetwork());

            using (var scopeInfoPtr = BitHelper.StructureToPtr(scopeInfo))
            {
                SetOptionValue(server, scopeInfoPtr, optionId, className, vendorName, values);
            }
        }


        internal static void DeleteScopeOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, int optionId)
            => DeleteScopeOptionValue(server, scopeAddress, optionId, null, null);
        internal static void DeleteScopeOptionValue(DhcpServerScope scope, int optionId)
            => DeleteScopeOptionValue(scope, optionId, null, null);
        internal static void DeleteScopeVendorOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, int optionId, string vendorName)
            => DeleteScopeOptionValue(server, scopeAddress, optionId, null, vendorName);
        internal static void DeleteScopeUserOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, int optionId, string className)
            => DeleteScopeOptionValue(server, scopeAddress, optionId, className, null);
        internal static void DeleteScopeVendorOptionValue(DhcpServerScope scope, int optionId, string vendorName)
            => DeleteScopeOptionValue(scope, optionId, null, vendorName);
        internal static void DeleteScopeUserOptionValue(DhcpServerScope scope, int optionId, string className)
            => DeleteScopeOptionValue(scope, optionId, className, null);

        internal static void DeleteScopeOptionValue(DhcpServerScope scope, DhcpServerOptionValue optionValue)
            => DeleteScopeOptionValue(scope.Server, scope.Address, optionValue.OptionId, optionValue.ClassName, optionValue.VendorName);
        internal static void DeleteScopeOptionValue(DhcpServerScope scope, int optionId, string className, string vendorName)
            => DeleteScopeOptionValue(scope.Server, scope.Address, optionId, className, vendorName);

        internal static void DeleteScopeOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, int optionId, string className, string vendorName)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_Managed_Subnet(scopeAddress.ToNativeAsNetwork());

            using (var scopeInfoPtr = BitHelper.StructureToPtr(scopeInfo))
            {
                RemoveOptionValue(server, scopeInfoPtr, optionId, className, vendorName);
            }
        }

        #endregion

        #region Reservation Option Values

        internal static IEnumerable<DhcpServerOptionValue> GetAllScopeReservationOptionValues(DhcpServerScopeReservation reservation)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_Managed_Reserved(reservation.Scope.Address.ToNativeAsNetwork(), reservation.Address.ToNativeAsNetwork());

            using (var scopeInfoPtr = BitHelper.StructureToPtr(scopeInfo))
            {
                foreach (var value in GetAllOptionValues(reservation.Server, scopeInfoPtr))
                    yield return value;
            }
        }


        internal static IEnumerable<DhcpServerOptionValue> EnumScopeReservationDefaultOptionValues(DhcpServerScopeReservation reservation)
            => EnumScopeReservationOptionValues(reservation, null, null);

        internal static IEnumerable<DhcpServerOptionValue> EnumScopeReservationVendorOptionValues(DhcpServerScopeReservation reservation, string vendorName)
            => EnumScopeReservationOptionValues(reservation, null, vendorName);

        internal static IEnumerable<DhcpServerOptionValue> EnumScopeReservationUserOptionValues(DhcpServerScopeReservation reservation, string className)
            => EnumScopeReservationOptionValues(reservation, className, null);

        private static IEnumerable<DhcpServerOptionValue> EnumScopeReservationOptionValues(DhcpServerScopeReservation reservation, string className, string vendorName)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_Managed_Reserved(reservation.Scope.Address.ToNativeAsNetwork(), reservation.Address.ToNativeAsNetwork());

            using (var scopeInfoPtr = BitHelper.StructureToPtr(scopeInfo))
            {
                foreach (var value in EnumOptionValues(reservation.Server, scopeInfoPtr, className, vendorName))
                    yield return value;
            }
        }


        internal static DhcpServerOptionValue GetScopeReservationDefaultOptionValue(DhcpServerScopeReservation reservation, int optionId)
            => GetScopeReservationDefaultOptionValue(reservation.Server, reservation.Scope.Address, reservation.Address, optionId);
        internal static DhcpServerOptionValue GetScopeReservationDefaultOptionValue(DhcpServerScope scope, DhcpServerIpAddress reservationAddress, int optionId)
            => GetScopeReservationDefaultOptionValue(scope.Server, scope.Address, reservationAddress, optionId);
        internal static DhcpServerOptionValue GetScopeReservationDefaultOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, DhcpServerIpAddress reservationAddress, int optionId)
            => GetScopeReservationOptionValue(server, scopeAddress, reservationAddress, optionId, null, null);

        internal static DhcpServerOptionValue GetScopeReservationVendorOptionValue(DhcpServerScopeReservation reservation, int optionId, string vendorName)
            => GetScopeReservationVendorOptionValue(reservation.Server, reservation.Scope.Address, reservation.Address, optionId, vendorName);
        internal static DhcpServerOptionValue GetScopeReservationVendorOptionValue(DhcpServerScope scope, DhcpServerIpAddress reservationAddress, int optionId, string vendorName)
            => GetScopeReservationVendorOptionValue(scope.Server, scope.Address, reservationAddress, optionId, vendorName);
        internal static DhcpServerOptionValue GetScopeReservationVendorOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, DhcpServerIpAddress reservationAddress, int optionId, string vendorName)
            => GetScopeReservationOptionValue(server, scopeAddress, reservationAddress, optionId, null, vendorName);

        internal static DhcpServerOptionValue GetScopeReservationUserOptionValue(DhcpServerScopeReservation reservation, int optionId, string className)
            => GetScopeReservationUserOptionValue(reservation.Server, reservation.Scope.Address, reservation.Address, optionId, className);
        internal static DhcpServerOptionValue GetScopeReservationUserOptionValue(DhcpServerScope scope, DhcpServerIpAddress reservationAddress, int optionId, string className)
            => GetScopeReservationUserOptionValue(scope.Server, scope.Address, reservationAddress, optionId, className);
        internal static DhcpServerOptionValue GetScopeReservationUserOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, DhcpServerIpAddress reservationAddress, int optionId, string className)
            => GetScopeReservationOptionValue(server, scopeAddress, reservationAddress, optionId, className, null);

        internal static DhcpServerOptionValue GetScopeReservationOptionValue(DhcpServerScopeReservation reservation, int optionId, string className, string vendorName)
            => GetScopeReservationOptionValue(reservation.Server, reservation.Scope.Address, reservation.Address, optionId, className, vendorName);
        internal static DhcpServerOptionValue GetScopeReservationOptionValue(DhcpServerScope scope, DhcpServerIpAddress reservationAddress, int optionId, string className, string vendorName)
            => GetScopeReservationOptionValue(scope.Server, scope.Address, reservationAddress, optionId, className, vendorName);

        internal static DhcpServerOptionValue GetScopeReservationOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, DhcpServerIpAddress reservationAddress, int optionId, string className, string vendorName)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_Managed_Reserved(scopeAddress.ToNativeAsNetwork(), reservationAddress.ToNativeAsNetwork());

            using (var scopeInfoPtr = BitHelper.StructureToPtr(scopeInfo))
            {
                return GetOptionValue(server, scopeInfoPtr, optionId, className, vendorName);
            }
        }


        internal static void SetScopeReservationDefaultOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, DhcpServerIpAddress reservationAddress, int optionId, IEnumerable<DhcpServerOptionElement> values)
            => SetScopeReservationOptionValue(server, scopeAddress, reservationAddress, optionId, null, null, values);
        internal static void SetScopeReservationDefaultOptionValue(DhcpServerScopeReservation reservation, int optionId, IEnumerable<DhcpServerOptionElement> values)
            => SetScopeReservationOptionValue(reservation, optionId, null, null, values);
        internal static void SetScopeReservationVendorOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, DhcpServerIpAddress reservationAddress, int optionId, string vendorName, IEnumerable<DhcpServerOptionElement> values)
            => SetScopeReservationOptionValue(server, scopeAddress, reservationAddress, optionId, null, vendorName, values);
        internal static void SetScopeReservationVendorOptionValue(DhcpServerScopeReservation reservation, int optionId, string vendorName, IEnumerable<DhcpServerOptionElement> values)
            => SetScopeReservationOptionValue(reservation, optionId, null, vendorName, values);
        internal static void SetScopeReservationUserOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, DhcpServerIpAddress reservationAddress, int optionId, string className, IEnumerable<DhcpServerOptionElement> values)
            => SetScopeReservationOptionValue(server, scopeAddress, reservationAddress, optionId, className, null, values);
        internal static void SetScopeReservationUserOptionValue(DhcpServerScopeReservation reservation, int optionId, string className, IEnumerable<DhcpServerOptionElement> values)
            => SetScopeReservationOptionValue(reservation, optionId, className, null, values);

        internal static void SetScopeReservationOptionValue(DhcpServerScopeReservation reservation, DhcpServerOptionValue optionValue)
            => SetScopeReservationOptionValue(reservation, optionValue.OptionId, optionValue.ClassName, optionValue.VendorName, optionValue.Values);
        internal static void SetScopeReservationOptionValue(DhcpServerScopeReservation reservation, int optionId, string className, string vendorName, IEnumerable<IDhcpServerOptionElement> values)
            => SetScopeReservationOptionValue(reservation.Server, reservation.Scope.Address, reservation.Address, optionId, className, vendorName, values);

        internal static void SetScopeReservationOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, DhcpServerIpAddress reservationAddress, int optionId, string className, string vendorName, IEnumerable<IDhcpServerOptionElement> values)
        {
            var reservationInfo = new DHCP_OPTION_SCOPE_INFO_Managed_Reserved(scopeAddress.ToNativeAsNetwork(), reservationAddress.ToNativeAsNetwork());

            using (var reservationInfoPtr = BitHelper.StructureToPtr(reservationInfo))
            {
                SetOptionValue(server, reservationInfoPtr, optionId, className, vendorName, values);
            }
        }


        internal static void DeleteScopeReservationOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, DhcpServerIpAddress reservationAddress, int optionId)
            => DeleteScopeReservationOptionValue(server, scopeAddress, reservationAddress, optionId, null, null);
        internal static void DeleteScopeReservationOptionValue(DhcpServerScopeReservation reservation, int optionId)
            => DeleteScopeReservationOptionValue(reservation, optionId, null, null);
        internal static void DeleteScopeReservationVendorOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, DhcpServerIpAddress reservationAddress, int optionId, string vendorName)
            => DeleteScopeReservationOptionValue(server, scopeAddress, reservationAddress, optionId, null, vendorName);
        internal static void DeleteScopeReservationUserOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, DhcpServerIpAddress reservationAddress, int optionId, string className)
            => DeleteScopeReservationOptionValue(server, scopeAddress, reservationAddress, optionId, className, null);
        internal static void DeleteScopeReservationVendorOptionValue(DhcpServerScopeReservation reservation, int optionId, string vendorName)
            => DeleteScopeReservationOptionValue(reservation, optionId, null, vendorName);
        internal static void DeleteScopeReservationUserOptionValue(DhcpServerScopeReservation reservation, int optionId, string className)
            => DeleteScopeReservationOptionValue(reservation, optionId, className, null);

        internal static void DeleteScopeReservationOptionValue(DhcpServerScopeReservation reservation, DhcpServerOptionValue optionValue)
            => DeleteScopeReservationOptionValue(reservation.Server, reservation.Scope.Address, reservation.Address, optionValue.OptionId, optionValue.ClassName, optionValue.VendorName);
        internal static void DeleteScopeReservationOptionValue(DhcpServerScopeReservation reservation, int optionId, string className, string vendorName)
            => DeleteScopeReservationOptionValue(reservation.Server, reservation.Scope.Address, reservation.Address, optionId, className, vendorName);

        internal static void DeleteScopeReservationOptionValue(DhcpServer server, DhcpServerIpAddress scopeAddress, DhcpServerIpAddress reservationAddress, int optionId, string className, string vendorName)
        {
            var reservationInfo = new DHCP_OPTION_SCOPE_INFO_Managed_Reserved(scopeAddress.ToNativeAsNetwork(), reservationAddress.ToNativeAsNetwork());

            using (var reservationInfoPtr = BitHelper.StructureToPtr(reservationInfo))
            {
                RemoveOptionValue(server, reservationInfoPtr, optionId, className, vendorName);
            }
        }

        #endregion


        #region Get All Option Values

        private static IEnumerable<DhcpServerOptionValue> GetAllOptionValues(DhcpServer server, IntPtr scopeInfo)
        {
            var result = Api.DhcpGetAllOptionValues(ServerIpAddress: server.Address,
                                                    Flags: 0,
                                                    ScopeInfo: scopeInfo,
                                                    Values: out var valuesPtr);

            if (result != DhcpServerNativeErrors.SUCCESS)
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
                            if (!(value.OptionID == DhcpServerDnsSettings.optionId))
                                yield return FromNative(server, in value, optionClass.ClassName, optionClass.VendorName);
                        }
                    }
                }
            }
            finally
            {
                Api.FreePointer(valuesPtr);
            }
        }

        #endregion

        #region Enumerate Option Values

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
            var result = Api.DhcpEnumOptionValues(ServerIpAddress: server.Address,
                                                  ScopeInfo: scopeInfo,
                                                  ResumeHandle: ref resumeHandle,
                                                  PreferredMaximum: 0xFFFFFFFF,
                                                  OptionValues: out var valueArrayPtr,
                                                  OptionsRead: out var elementsRead,
                                                  OptionsTotal: out _);

            if (result == DhcpServerNativeErrors.ERROR_NO_MORE_ITEMS || result == DhcpServerNativeErrors.EPT_S_NOT_REGISTERED)
                yield break;

            if (result != DhcpServerNativeErrors.SUCCESS && result != DhcpServerNativeErrors.ERROR_MORE_DATA)
                throw new DhcpServerException(nameof(Api.DhcpEnumOptionValues), result);

            if (elementsRead == 0)
                yield break;

            try
            {
                using (var valueArray = valueArrayPtr.MarshalToStructure<DHCP_OPTION_VALUE_ARRAY>())
                {
                    foreach (var value in valueArray.Values)
                    {
                        if (value.OptionID != DhcpServerDnsSettings.optionId)
                            yield return FromNative(server, in value, null, null);
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
            var result = Api.DhcpEnumOptionValuesV5(ServerIpAddress: server.Address,
                                                    Flags: (vendorName == null) ? 0 : Constants.DHCP_FLAGS_OPTION_IS_VENDOR,
                                                    ClassName: className,
                                                    VendorName: vendorName,
                                                    ScopeInfo: scopeInfo,
                                                    ResumeHandle: ref resumeHandle,
                                                    PreferredMaximum: 0xFFFFFFFF,
                                                    OptionValues: out var valueArrayPtr,
                                                    OptionsRead: out var elementsRead,
                                                    OptionsTotal: out _);

            if (result == DhcpServerNativeErrors.ERROR_NO_MORE_ITEMS || result == DhcpServerNativeErrors.EPT_S_NOT_REGISTERED)
                yield break;

            if (result != DhcpServerNativeErrors.SUCCESS && result != DhcpServerNativeErrors.ERROR_MORE_DATA)
                throw new DhcpServerException(nameof(Api.DhcpEnumOptionValuesV5), result);

            if (elementsRead == 0)
                yield break;

            try
            {
                using (var valueArray = valueArrayPtr.MarshalToStructure<DHCP_OPTION_VALUE_ARRAY>())
                {
                    foreach (var value in valueArray.Values)
                    {
                        if (value.OptionID != DhcpServerDnsSettings.optionId)
                            yield return FromNative(server, in value, className, vendorName);
                    }
                }
            }
            finally
            {
                Api.FreePointer(valueArrayPtr);
            }
        }

        #endregion

        #region Get Option Value

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
            var result = Api.DhcpGetOptionValue(ServerIpAddress: server.Address,
                                                OptionID: optionId,
                                                ScopeInfo: scopeInfo,
                                                OptionValue: out var valuePtr);

            if (result != DhcpServerNativeErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetOptionValue), result);

            try
            {
                using (var value = valuePtr.MarshalToStructure<DHCP_OPTION_VALUE>())
                {
                    return FromNative(server, in value, null, null);
                }
            }
            finally
            {
                Api.FreePointer(valuePtr);
            }
        }

        private static DhcpServerOptionValue GetOptionValueV5(DhcpServer server, IntPtr scopeInfo, int optionId, string className, string vendorName)
        {
            var result = Api.DhcpGetOptionValueV5(ServerIpAddress: server.Address,
                                                  Flags: (vendorName == null) ? 0 : Constants.DHCP_FLAGS_OPTION_IS_VENDOR,
                                                  OptionID: optionId,
                                                  ClassName: className,
                                                  VendorName: vendorName,
                                                  ScopeInfo: scopeInfo,
                                                  OptionValue: out var valuePtr);

            if (result != DhcpServerNativeErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetOptionValueV5), result);

            try
            {
                using (var value = valuePtr.MarshalToStructure<DHCP_OPTION_VALUE>())
                {
                    return FromNative(server, in value, className, vendorName);
                }
            }
            finally
            {
                Api.FreePointer(valuePtr);
            }
        }

        #endregion

        #region Set Option Value

        private static void SetOptionValue(DhcpServer server, IntPtr scopeInfo, int optionId, string className, string vendorName, IEnumerable<IDhcpServerOptionElement> values)
        {
            if (server.IsCompatible(DhcpServerVersions.Windows2008R2))
                SetOptionValueV5(server, scopeInfo, optionId, className, vendorName, values);
            else
            {
                if (vendorName != null || className != null)
                    throw new PlatformNotSupportedException($"DHCP Server v{server.VersionMajor}.{server.VersionMinor} does not support this feature");

                SetOptionValueV0(server, scopeInfo, optionId, values);
            }
        }

        private static void SetOptionValueV0(DhcpServer server, IntPtr scopeInfo, int optionId, IEnumerable<IDhcpServerOptionElement> values)
        {
            using (var valueNative = DhcpServerOptionElement.WriteNative(values))
            {
                using (var valueNativePtr = BitHelper.StructureToPtr(valueNative))
                {
                    var result = Api.DhcpSetOptionValue(ServerIpAddress: server.Address,
                                                    OptionID: optionId,
                                                    ScopeInfo: scopeInfo,
                                                    OptionValue: valueNativePtr);

                    if (result != DhcpServerNativeErrors.SUCCESS)
                        throw new DhcpServerException(nameof(Api.DhcpSetOptionValue), result);
                }
            }
        }

        private static void SetOptionValueV5(DhcpServer server, IntPtr scopeInfo, int optionId, string className, string vendorName, IEnumerable<IDhcpServerOptionElement> values)
        {
            using (var valueNative = DhcpServerOptionElement.WriteNative(values))
            {
                using (var valueNativePtr = BitHelper.StructureToPtr(valueNative))
                {
                    var result = Api.DhcpSetOptionValueV5(ServerIpAddress: server.Address,
                                                          Flags: (vendorName == null) ? 0 : Constants.DHCP_FLAGS_OPTION_IS_VENDOR,
                                                          OptionID: optionId,
                                                          ClassName: className,
                                                          VendorName: vendorName,
                                                          ScopeInfo: scopeInfo,
                                                          OptionValue: valueNativePtr);

                    if (result != DhcpServerNativeErrors.SUCCESS)
                        throw new DhcpServerException(nameof(Api.DhcpSetOptionValueV5), result);
                }
            }
        }

        #endregion

        #region Remove Option Value

        private static void RemoveOptionValue(DhcpServer server, IntPtr scopeInfo, int optionId, string className, string vendorName)
        {
            if (server.IsCompatible(DhcpServerVersions.Windows2008R2))
                RemoveOptionValueV5(server, scopeInfo, optionId, className, vendorName);
            else
            {
                if (vendorName != null || className != null)
                    throw new PlatformNotSupportedException($"DHCP Server v{server.VersionMajor}.{server.VersionMinor} does not support this feature");

                RemoveOptionValueV0(server, scopeInfo, optionId);
            }
        }

        private static void RemoveOptionValueV0(DhcpServer server, IntPtr scopeInfo, int optionId)
        {
            var result = Api.DhcpRemoveOptionValue(ServerIpAddress: server.Address,
                                                   OptionID: optionId,
                                                   ScopeInfo: scopeInfo);

            if (result != DhcpServerNativeErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpRemoveOptionValue), result);
        }

        private static void RemoveOptionValueV5(DhcpServer server, IntPtr scopeInfo, int optionId, string className, string vendorName)
        {
            var result = Api.DhcpRemoveOptionValueV5(ServerIpAddress: server.Address,
                                                     Flags: (vendorName == null) ? 0 : Constants.DHCP_FLAGS_OPTION_IS_VENDOR,
                                                     OptionID: optionId,
                                                     ClassName: className,
                                                     VendorName: vendorName,
                                                     ScopeInfo: scopeInfo);

            if (result != DhcpServerNativeErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpRemoveOptionValueV5), result);
        }

        #endregion

        public override string ToString()
            => $"{VendorName ?? ClassName ?? Server.SpecificStrings.DefaultVendorClassName}: {OptionId} [{Option.Name}]: {string.Join("; ", Values)}";
    }
}
