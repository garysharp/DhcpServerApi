using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_CLASS_INFO structure defines a DHCP option class.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_CLASS_INFO
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
        public string ClassComment;
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
