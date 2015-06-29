using Dhcp.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp
{
    /// <summary>
    /// Defines a DHCP option class
    /// </summary>
    public class DhcpServerClass
    {
        /// <summary>
        /// The associated DHCP Server
        /// </summary>
        public DhcpServer Server { get; private set; }

        /// <summary>
        /// Name of the Class
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Comment associated with the Class
        /// </summary>
        public string Comment { get; private set; }

        /// <summary>
        /// Indicates whether or not the options are vendor-specific
        /// </summary>
        public bool IsVendor { get; private set; }

        /// <summary>
        /// A byte buffer that contains specific data for the class
        /// </summary>
        public byte[] Data { get; private set; }

        private DhcpServerClass(DhcpServer Server)
        {
            this.Server = Server;
        }

        internal static IEnumerable<DhcpServerClass> GetClasses(DhcpServer Server)
        {
            IntPtr enumInfoPtr;
            int elementsRead, elementsTotal;
            IntPtr resumeHandle = IntPtr.Zero;

            var result = Api.DhcpEnumClasses(Server.ipAddress.ToString(), 0, ref resumeHandle, 0xFFFFFFFF, out enumInfoPtr, out elementsRead, out elementsTotal);

            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS || result == DhcpErrors.EPT_S_NOT_REGISTERED)
                yield break;

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                throw new DhcpServerException("DhcpEnumClasses", result);

            if (elementsRead > 0)
            {
                var enumInfo = (DHCP_CLASS_INFO_ARRAY)Marshal.PtrToStructure(enumInfoPtr, typeof(DHCP_CLASS_INFO_ARRAY));
                try
                {
                    foreach (var element in enumInfo.Classes)
                    {
                        yield return FromNative(Server, element);
                    }
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(enumInfoPtr);
                }
            }
        }

        internal static DhcpServerClass FromNative(DhcpServer Server, DHCP_CLASS_INFO Native)
        {
            var data = new byte[Native.ClassDataLength];
            Marshal.Copy(Native.ClassData, data, 0, Native.ClassDataLength);

            return new DhcpServerClass(Server)
            {
                Name = Native.ClassName,
                Comment = Native.ClassComment,
                IsVendor = Native.IsVendor,
                Data = data
            };
        }
    }
}
