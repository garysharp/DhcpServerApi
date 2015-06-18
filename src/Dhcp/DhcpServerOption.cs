using Dhcp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Dhcp
{
    public class DhcpServerOption
    {
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

        private DhcpServerOption(DhcpServer Server)
        {
            this.Server = Server;
        }

        internal static DhcpServerOption GetOption(DhcpServer Server, int OptionId)
        {
            IntPtr optionPtr;

            var result = Api.DhcpGetOptionInfo(Server.IpAddress, OptionId, out optionPtr);

            if (result == DhcpErrors.OPTION_NOT_PRESENT)
                return null;

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException("DhcpGetOptionInfo", result);

            try
            {
                var option = (DHCP_OPTION)Marshal.PtrToStructure(optionPtr, typeof(DHCP_OPTION));

                return DhcpServerOption.FromNative(Server, option);
            }
            finally
            {
                Api.DhcpRpcFreeMemory(optionPtr);
            }
        }

        internal static IEnumerable<DhcpServerOption> GetOptions(DhcpServer Server)
        {
            IntPtr enumInfoPtr;
            int elementsRead, elementsTotal;
            IntPtr resumeHandle = IntPtr.Zero;

            var result = Api.DhcpEnumOptions(Server.IpAddress, ref resumeHandle, 0xFFFFFFFF, out enumInfoPtr, out elementsRead, out elementsTotal);

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
                    yield return DhcpServerOption.FromNative(Server, option);
                }
            }
            finally
            {
                Api.DhcpRpcFreeMemory(enumInfoPtr);
            }
        }

        private static DhcpServerOption FromNative(DhcpServer Server, DHCP_OPTION Native)
        {
            return new DhcpServerOption(Server)
            {
                OptionId = Native.OptionID,
                Name = Native.OptionName,
                Comment = Native.OptionComment,
                DefaultValue = DhcpServerOptionElement.ReadNativeElements(Native.DefaultValue).ToList(),
                IsUnaryOption = Native.OptionType == DHCP_OPTION_TYPE.DhcpUnaryElementTypeOption
            };
        }
    }
}
