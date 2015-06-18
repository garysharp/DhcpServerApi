using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_BIND_ELEMENT structure defines an individual network binding for the DHCP server. A single DHCP server can contain multiple bindings and serve multiple networks.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_BIND_ELEMENT
    {
        /// <summary>
        /// Specifies a set of bit flags indicating properties of the network binding.
        /// DHCP_ENDPOINT_FLAG_CANT_MODIFY (0x01) = The binding specified in this structure cannot be modified.
        /// </summary>
        public uint Flags;
        /// <summary>
        /// Specifies whether or not this binding is set on the DHCP server. If TRUE, the binding is set; if FALSE, it is not.
        /// </summary>
        public bool fBoundToDHCPServer;
        /// <summary>
        /// DHCP_IP_ADDRESS value that specifies the IP address assigned to the ethernet adapter of the DHCP server.
        /// </summary>
        public DHCP_IP_ADDRESS AdapterPrimaryAddress;
        /// <summary>
        /// DHCP_IP_ADDRESS value that specifies the subnet IP mask used by this ethernet adapter.
        /// </summary>
        public DHCP_IP_MASK AdapterSubnetAddress;
        /// <summary>
        /// Unicode string that specifies the name assigned to this network interface device.
        /// </summary>
        [MarshalAs( UnmanagedType.LPWStr)]
        public string IfDescription;
        /// <summary>
        /// Specifies the size of the network interface device ID, in bytes.
        /// </summary>
        public int IfIdSize;
        /// <summary>
        /// Specifies the network interface device ID.
        /// </summary>
        public IntPtr IfId;
    }
}
