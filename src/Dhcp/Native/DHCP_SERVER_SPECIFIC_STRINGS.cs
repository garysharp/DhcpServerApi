using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_SERVER_SPECIFIC_STRINGS structure contains the default string values for user and vendor class names.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_SERVER_SPECIFIC_STRINGS
    {
        /// <summary>
        /// Pointer to a Unicode string that specifies the default vendor class name for the DHCP server.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public readonly string DefaultVendorClassName;
        /// <summary>
        /// Pointer to a Unicode string that specifies the default user class name for the DHCP server.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public readonly string DefaultUserClassName;
    }
}
