using Dhcp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Dhcp
{
    public class DhcpServerOption
    {
        private Lazy<DhcpServerClass> @class;

        /// <summary>
        /// The associated DHCP Server
        /// </summary>
        public DhcpServer Server { get; private set; }

        /// <summary>
        /// Unique ID number (also called a "code") for this Option
        /// </summary>
        public int OptionId { get; private set; }

        /// <summary>
        /// Name for this Option
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Comment about this Option
        /// </summary>
        public string Comment { get; private set; }

        /// <summary>
        /// Contains the data associated with this option
        /// </summary>
        public List<DhcpServerOptionElement> DefaultValue { get; private set; }

        /// <summary>
        /// True if this Option has a single data item associated with it, False when an array of data items are associated with it.
        /// </summary>
        public bool IsUnaryOption { get; private set; }

        /// <summary>
        /// Name of the Vendor Class associated with this Option
        /// </summary>
        public string VendorName { get; private set; }

        /// <summary>
        /// Name of the User Class associated with this Option
        /// </summary>
        public string ClassName { get; private set; }

        public bool IsVendorClassOption { get { return VendorName != null; } }

        public bool IsUserClassOption { get { return ClassName != null; } }

        public DhcpServerClass Class { get { return this.@class.Value; } }

        private DhcpServerOption(DhcpServer Server)
        {
            this.Server = Server;
            this.@class = new Lazy<DhcpServerClass>(GetClass);
        }

        /// <summary>
        /// The Option Value associated with the Global Server Configuration
        /// </summary>
        public DhcpServerOptionValue GetGlobalValue()
        {
            return DhcpServerOptionValue.GetGlobalOptionValue(Server, OptionId, ClassName, VendorName);
        }

        /// <summary>
        /// The Option Value associated with the Scope Configuration
        /// </summary>
        public DhcpServerOptionValue GetScopeValue(DhcpServerScope Scope)
        {
            return DhcpServerOptionValue.GetScopeOptionValue(Scope, OptionId, ClassName, VendorName);
        }

        /// <summary>
        /// The Option Value associated with the Reservation Configuration
        /// </summary>
        public DhcpServerOptionValue GetScopeReservationValue(DhcpServerScopeReservation Reservation)
        {
            return DhcpServerOptionValue.GetScopeReservationOptionValue(Reservation, OptionId, ClassName, VendorName);
        }

        private DhcpServerClass GetClass()
        {
            if (VendorName != null)
                return DhcpServerClass.GetClass(Server, VendorName);
            else if (ClassName != null)
                return DhcpServerClass.GetClass(Server, ClassName);
            else
                return null;
        }

        internal static DhcpServerOption GetDefaultOption(DhcpServer Server, int OptionId)
        {
            return GetOption(Server, OptionId, null, null);
        }

        internal static DhcpServerOption GetVendorOption(DhcpServer Server, int OptionId, string VendorName)
        {
            return GetOption(Server, OptionId, null, VendorName);
        }

        internal static DhcpServerOption GetUserOption(DhcpServer Server, int OptionId, string ClassName)
        {
            return GetOption(Server, OptionId, ClassName, null);
        }

        private static DhcpServerOption GetOption(DhcpServer Server, int OptionId, string ClassName, string VendorName)
        {
            if (Server.IsCompatible(DhcpServerVersions.Windows2008R2))
            {
                return GetOptionV5(Server, OptionId, ClassName, VendorName);
            }
            else
            {
                if (VendorName != null || ClassName != null)
                {
                    throw new PlatformNotSupportedException(string.Format("DHCP Server v{0}.{1} does not support this feature", Server.VersionMajor, Server.VersionMinor));
                }
                return GetOptionV0(Server, OptionId);
            }
        }

        private static DhcpServerOption GetOptionV0(DhcpServer Server, int OptionId)
        {
            IntPtr optionPtr;

            var result = Api.DhcpGetOptionInfo(Server.ipAddress.ToString(), OptionId, out optionPtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException("DhcpGetOptionInfo", result);

            try
            {
                var option = (DHCP_OPTION)Marshal.PtrToStructure(optionPtr, typeof(DHCP_OPTION));

                return DhcpServerOption.FromNative(Server, option, null, null);
            }
            finally
            {
                Api.DhcpRpcFreeMemory(optionPtr);
            }
        }

        private static DhcpServerOption GetOptionV5(DhcpServer Server, int OptionId, string ClassName, string VendorName)
        {
            IntPtr optionPtr;
            uint flags = VendorName == null ? 0 : Constants.DHCP_FLAGS_OPTION_IS_VENDOR;
            var result = Api.DhcpGetOptionInfoV5(Server.ipAddress.ToString(), flags, OptionId, ClassName, VendorName, out optionPtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException("DhcpGetOptionInfoV5", result);

            try
            {
                var option = (DHCP_OPTION)Marshal.PtrToStructure(optionPtr, typeof(DHCP_OPTION));

                return DhcpServerOption.FromNative(Server, option, ClassName, VendorName);
            }
            finally
            {
                Api.DhcpRpcFreeMemory(optionPtr);
            }
        }

        internal static IEnumerable<DhcpServerOption> GetAllOptions(DhcpServer Server)
        {
            IntPtr optionsPtr;

            var result = Api.DhcpGetAllOptions(Server.ipAddress.ToString(), 0, out optionsPtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException("DhcpGetAllOptions", result);

            try
            {
                var bytes = new byte[64];
                Marshal.Copy(optionsPtr, bytes, 0, 64);

                var options = (DHCP_ALL_OPTIONS)Marshal.PtrToStructure(optionsPtr, typeof(DHCP_ALL_OPTIONS));

                foreach (var option in options.NonVendorOptions.Options)
                {
                    yield return DhcpServerOption.FromNative(Server, option, null, null);
                }

                foreach (var option in options.VendorOptions)
                {
                    yield return DhcpServerOption.FromNative(Server, option);
                }
            }
            finally
            {
                Api.DhcpRpcFreeMemory(optionsPtr);
            }
        }

        internal static IEnumerable<DhcpServerOption> EnumDefaultOptions(DhcpServer Server)
        {
            if (Server.IsCompatible(DhcpServerVersions.Windows2008R2))
            {
                return EnumOptionsV5(Server, null, null);
            }
            else
            {
                return EnumOptionsV0(Server);
            }
        }

        internal static IEnumerable<DhcpServerOption> EnumUserOptions(DhcpServer Server, string ClassName)
        {
            if (!Server.IsCompatible(DhcpServerVersions.Windows2008R2))
                throw new PlatformNotSupportedException(string.Format("DHCP Server v{0}.{1} does not support this feature", Server.VersionMajor, Server.VersionMinor));

            return EnumOptionsV5(Server, ClassName, null);
        }

        internal static IEnumerable<DhcpServerOption> EnumVendorOptions(DhcpServer Server, string VendorName)
        {
            if (!Server.IsCompatible(DhcpServerVersions.Windows2008R2))
                throw new PlatformNotSupportedException(string.Format("DHCP Server v{0}.{1} does not support this feature", Server.VersionMajor, Server.VersionMinor));

            return EnumOptionsV5(Server, null, VendorName);
        }

        /// <summary>
        /// Only returns Default Options
        /// </summary>
        private static IEnumerable<DhcpServerOption> EnumOptionsV0(DhcpServer Server)
        {
            IntPtr enumInfoPtr;
            int elementsRead, elementsTotal;
            IntPtr resumeHandle = IntPtr.Zero;

            var result = Api.DhcpEnumOptions(Server.ipAddress.ToString(), ref resumeHandle, 0xFFFFFFFF, out enumInfoPtr, out elementsRead, out elementsTotal);

            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS || result == DhcpErrors.EPT_S_NOT_REGISTERED)
                yield break;

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                throw new DhcpServerException("DhcpEnumOptions", result);

            if (elementsRead == 0)
                yield break;

            try
            {
                var enumInfo = (DHCP_OPTION_ARRAY)Marshal.PtrToStructure(enumInfoPtr, typeof(DHCP_OPTION_ARRAY));

                foreach (var option in enumInfo.Options)
                {
                    yield return DhcpServerOption.FromNative(Server, option, null, null);
                }
            }
            finally
            {
                Api.DhcpRpcFreeMemory(enumInfoPtr);
            }
        }

        private static IEnumerable<DhcpServerOption> EnumOptionsV5(DhcpServer Server, string ClassName, string VendorName)
        {
            IntPtr enumInfoPtr;
            int elementsRead, elementsTotal;
            IntPtr resumeHandle = IntPtr.Zero;

            uint flags = VendorName == null ? 0 : Constants.DHCP_FLAGS_OPTION_IS_VENDOR;

            var result = Api.DhcpEnumOptionsV5(Server.ipAddress.ToString(), flags, ClassName, VendorName, ref resumeHandle, 0xFFFFFFFF, out enumInfoPtr, out elementsRead, out elementsTotal);

            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS || result == DhcpErrors.EPT_S_NOT_REGISTERED)
                yield break;

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                throw new DhcpServerException("DhcpEnumOptions", result);

            if (elementsRead == 0)
                yield break;

            try
            {
                var enumInfo = (DHCP_OPTION_ARRAY)Marshal.PtrToStructure(enumInfoPtr, typeof(DHCP_OPTION_ARRAY));

                foreach (var option in enumInfo.Options)
                {
                    yield return DhcpServerOption.FromNative(Server, option, VendorName, ClassName);
                }
            }
            finally
            {
                Api.DhcpRpcFreeMemory(enumInfoPtr);
            }
        }

        private static DhcpServerOption FromNative(DhcpServer Server, DHCP_OPTION Native, string VendorName, string ClassName)
        {
            return new DhcpServerOption(Server)
            {
                OptionId = Native.OptionID,
                Name = Native.OptionName,
                Comment = Native.OptionComment,
                DefaultValue = DhcpServerOptionElement.ReadNativeElements(Native.DefaultValue).ToList(),
                IsUnaryOption = Native.OptionType == DHCP_OPTION_TYPE.DhcpUnaryElementTypeOption,
                VendorName = VendorName,
                ClassName = ClassName
            };
        }

        private static DhcpServerOption FromNative(DhcpServer Server, DHCP_VENDOR_OPTION Native)
        {
            return new DhcpServerOption(Server)
            {
                OptionId = Native.OptionID,
                Name = Native.OptionName,
                Comment = Native.OptionComment,
                DefaultValue = DhcpServerOptionElement.ReadNativeElements(Native.DefaultValue).ToList(),
                IsUnaryOption = Native.OptionType == DHCP_OPTION_TYPE.DhcpUnaryElementTypeOption,
                VendorName = Native.VendorName,
                ClassName = Native.ClassName
            };
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

            return $"{className}: {OptionId} [{Name}: {Comment}]; Default: {string.Join("; ", DefaultValue)}";
        }
    }
}
