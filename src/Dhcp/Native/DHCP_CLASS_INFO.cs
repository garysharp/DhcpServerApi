using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_CLASS_INFO structure defines a DHCP option class.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_CLASS_INFO : IDisposable
    {
        /// <summary>
        /// Unicode string that contains the name of the class.
        /// </summary>
        private IntPtr ClassNamePointer;
        /// <summary>
        /// Unicode string that contains a comment associated with the class.
        /// </summary>
        private IntPtr ClassCommentPointer;
        /// <summary>
        /// Specifies the size of ClassData, in bytes. When passing this structure into DhcpGetClassInfo, this value should be set to the size of the initialized buffer.
        /// </summary>
        public int ClassDataLength;
        /// <summary>
        /// Specifies whether or not this option class is a vendor-defined option class. If TRUE, it is a vendor class; if not, it is not a vendor class. Vendor-defined option classes can be used by DHCP clients that are configured to optionally identify themselves by their vendor type to the DHCP server when obtaining a lease.
        /// </summary>
        public bool IsVendor;
        /// <summary>
        /// Specifies a bit flag that indicates whether or not the options are vendor-specific. If it is not, this parameter should be 0. 
        /// </summary>
        public uint Flags;
        /// <summary>
        /// Pointer to a byte buffer that contains specific data for the class. When passing this structure into DhcpGetClassInfo, this buffer should be initialized to the anticipated size of the data to be returned.
        /// </summary>
        public IntPtr ClassData;

        /// <summary>
        /// Unicode string that contains the name of the class.
        /// </summary>
        public string ClassName => Marshal.PtrToStringUni(ClassNamePointer);
        /// <summary>
        /// Unicode string that contains a comment associated with the class.
        /// </summary>
        public string ClassComment => Marshal.PtrToStringUni(ClassCommentPointer);

        public void Dispose()
        {
            Api.FreePointer(ref ClassNamePointer);
            Api.FreePointer(ref ClassCommentPointer);
            Api.FreePointer(ref ClassData);
        }
    }

    /// <summary>
    /// The DHCP_CLASS_INFO structure defines a DHCP option class.
    /// Used when creating managed copies
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_CLASS_INFO_Managed
    {
        /// <summary>
        /// Unicode string that contains the name of the class.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string ClassName;
        /// <summary>
        /// Unicode string that contains a comment associated with the class.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        private string ClassComment;
        /// <summary>
        /// Specifies the size of ClassData, in bytes. When passing this structure into DhcpGetClassInfo, this value should be set to the size of the initialized buffer.
        /// </summary>
        public int ClassDataLength;
        /// <summary>
        /// Specifies whether or not this option class is a vendor-defined option class. If TRUE, it is a vendor class; if not, it is not a vendor class. Vendor-defined option classes can be used by DHCP clients that are configured to optionally identify themselves by their vendor type to the DHCP server when obtaining a lease.
        /// </summary>
        public bool IsVendor;
        /// <summary>
        /// Specifies a bit flag that indicates whether or not the options are vendor-specific. If it is not, this parameter should be 0. 
        /// </summary>
        public uint Flags;
        /// <summary>
        /// Pointer to a byte buffer that contains specific data for the class. When passing this structure into DhcpGetClassInfo, this buffer should be initialized to the anticipated size of the data to be returned.
        /// </summary>
        public IntPtr ClassData;
    }
}
