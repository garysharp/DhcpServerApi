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
            return this.Server.GetOption(this.OptionId);
        }

        private static DhcpServerOptionValue FromNative(DhcpServer Server, DHCP_OPTION_VALUE Native)
        {
            return new DhcpServerOptionValue(Server)
            {
                OptionId = Native.OptionID,
                Values = DhcpServerOptionElement.ReadNativeElements(Native.Value).ToList()
            };
        }

        internal static IEnumerable<DhcpServerOptionValue> EnumScopeReservationOptionValues(DhcpServerScopeReservation Reservation)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_RESERVED()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpReservedOptions,
                ReservedIpAddress = Reservation.ipAddress,
                ReservedIpSubnetAddress = Reservation.ipAddressMask
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);

            return EnumOptionValues(Reservation.Server, scopeInfoPtr);
        }

        internal static DhcpServerOptionValue GetScopeReservationOptionValue(DhcpServerScopeReservation Reservation, int OptionId)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_RESERVED()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpReservedOptions,
                ReservedIpAddress = Reservation.ipAddress,
                ReservedIpSubnetAddress = Reservation.ipAddressMask
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);

            return GetOptionValue(Reservation.Server, scopeInfoPtr, OptionId);
        }

        internal static IEnumerable<DhcpServerOptionValue> EnumScopeOptionValues(DhcpServerScope Scope)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_SUBNET()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpSubnetOptions,
                SubnetScopeInfo = Scope.address
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);

            return EnumOptionValues(Scope.Server, scopeInfoPtr);
        }

        internal static DhcpServerOptionValue GetScopeOptionValue(DhcpServerScope Scope, int OptionId)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_SUBNET()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpSubnetOptions,
                SubnetScopeInfo = Scope.address
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);

            return GetOptionValue(Scope.Server, scopeInfoPtr, OptionId);
        }

        internal static IEnumerable<DhcpServerOptionValue> EnumGlobalOptionValues(DhcpServer Server)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_GLOBAL()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpGlobalOptions,
                GlobalScopeInfo = IntPtr.Zero
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);

            return EnumOptionValues(Server, scopeInfoPtr);
        }

        internal static DhcpServerOptionValue GetGlobalOptionValue(DhcpServer Server, int OptionId)
        {
            var scopeInfo = new DHCP_OPTION_SCOPE_INFO_GLOBAL()
            {
                ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpGlobalOptions,
                GlobalScopeInfo = IntPtr.Zero
            };

            var scopeInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(scopeInfo));

            Marshal.StructureToPtr(scopeInfo, scopeInfoPtr, true);

            return GetOptionValue(Server, scopeInfoPtr, OptionId);
        }

        private static IEnumerable<DhcpServerOptionValue> EnumOptionValues(DhcpServer Server, IntPtr ScopeInfo)
        {
            try
            {
                IntPtr valueArrayPtr;
                int elementsRead, elementsTotal;
                IntPtr resumeHandle = IntPtr.Zero;
                var elementSize = Marshal.SizeOf(typeof(DHCP_OPTION_VALUE));

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
                        yield return FromNative(Server, value);
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

        private static DhcpServerOptionValue GetOptionValue(DhcpServer Server, IntPtr ScopeInfo, int OptionId)
        {
            try
            {
                IntPtr valuePtr;
                var elementSize = Marshal.SizeOf(typeof(DHCP_OPTION_VALUE));

                var result = Api.DhcpGetOptionValue(Server.ipAddress.ToString(),OptionId, ScopeInfo, out valuePtr);

                if (result == DhcpErrors.ERROR_FILE_NOT_FOUND)
                    return null;

                if (result != DhcpErrors.SUCCESS)
                    throw new DhcpServerException("DhcpGetOptionValue", result);

                try
                {
                    var value = (DHCP_OPTION_VALUE)Marshal.PtrToStructure(valuePtr, typeof(DHCP_OPTION_VALUE));

                    return FromNative(Server, value);
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
    }
}
