using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Dhcp.Native;

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
        public DhcpServer Server { get; }

        /// <summary>
        /// Name of the Class
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Comment associated with the Class
        /// </summary>
        public string Comment { get; }

        /// <summary>
        /// Indicates whether or not the options are vendor-specific
        /// </summary>
        public bool IsVendorClass { get; }

        /// <summary>
        /// Indicates whether or not the options are user-specific
        /// </summary>
        public bool IsUserClass => !IsVendorClass;

        /// <summary>
        /// A byte buffer that contains specific data for the class
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// An ASCII representation of the <see cref="Data"/> buffer.
        /// </summary>
        public string DataText => (Data == null) ? null : Encoding.ASCII.GetString(Data);

        /// <summary>
        /// Enumerates a list of all Options associated with this class
        /// </summary>
        public IEnumerable<DhcpServerOption> Options
        {
            get
            {
                if (IsVendorClass)
                    return DhcpServerOption.EnumVendorOptions(Server, Name);
                else
                    return DhcpServerOption.EnumUserOptions(Server, Name);
            }
        }

        /// <summary>
        /// Enumerates a list of all Global Option Values associated with this class
        /// </summary>
        public IEnumerable<DhcpServerOptionValue> GlobalOptionValues
        {
            get
            {
                if (IsVendorClass)
                    return DhcpServerOptionValue.EnumGlobalVendorOptionValues(Server, Name);
                else
                    return DhcpServerOptionValue.EnumGlobalUserOptionValues(Server, Name);
            }
        }

        public DhcpServerClass Value { get; internal set; }

        private DhcpServerClass(DhcpServer server, string name, string comment, bool isVendorClass, byte[] data)
        {
            Server = server;
            Name = name;
            Comment = comment;
            IsVendorClass = isVendorClass;
            Data = data;
        }

        internal static DhcpServerClass GetClass(DhcpServer server, string name)
        {
            var query = new DHCP_CLASS_INFO_Managed()
            {
                ClassName = name,
                ClassDataLength = 0,
                ClassData = IntPtr.Zero
            };

            var result = Api.DhcpGetClassInfo(ServerIpAddress: server.IpAddress,
                                              ReservedMustBeZero: 0,
                                              PartialClassInfo: query,
                                              FilledClassInfo: out var classIntoPtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetClassInfo), result);

            try
            {
                using (var classInfo = classIntoPtr.MarshalToStructure<DHCP_CLASS_INFO>())
                {
                    var classInfoRef = classInfo;
                    return FromNative(server, ref classInfoRef);
                }
            }
            finally
            {
                Api.FreePointer(classIntoPtr);
            }
        }

        internal static IEnumerable<DhcpServerClass> GetClasses(DhcpServer server)
        {
            var resumeHandle = IntPtr.Zero;
            var result = Api.DhcpEnumClasses(ServerIpAddress: server.IpAddress,
                                             ReservedMustBeZero: 0,
                                             ResumeHandle: ref resumeHandle,
                                             PreferredMaximum: 0xFFFFFFFF,
                                             ClassInfoArray: out var enumInfoPtr,
                                             nRead: out var elementsRead,
                                             nTotal: out _);

            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS || result == DhcpErrors.EPT_S_NOT_REGISTERED)
                yield break;

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                throw new DhcpServerException(nameof(Api.DhcpEnumClasses), result);

            try
            {
                if (elementsRead == 0)
                    yield break;

                using (var enumInfo = enumInfoPtr.MarshalToStructure<DHCP_CLASS_INFO_ARRAY>())
                {
                    foreach (var element in enumInfo.Classes)
                    {
                        var elementRef = element;
                        yield return FromNative(server, ref elementRef);
                    }
                }
            }
            finally
            {
                Api.FreePointer(enumInfoPtr);
            }
        }

        internal static DhcpServerClass FromNative(DhcpServer server, ref DHCP_CLASS_INFO native)
        {
            var data = new byte[native.ClassDataLength];
            Marshal.Copy(native.ClassData, data, 0, native.ClassDataLength);

            return new DhcpServerClass(server: server,
                                       name: native.ClassName,
                                       comment: native.ClassComment,
                                       isVendorClass: native.IsVendor,
                                       data: data);
        }
    }
}
