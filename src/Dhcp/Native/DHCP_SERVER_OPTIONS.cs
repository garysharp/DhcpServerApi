using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_SERVER_OPTIONS structure specifies requested DHCP Server options.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_SERVER_OPTIONS
    {
        /// <summary>
        /// Pointer to DHCP message type.
        /// </summary>
        public IntPtr MessageType;
        /// <summary>
        /// Pointer to Subnet mask.
        /// </summary>
        public IntPtr SubnetMask;
        /// <summary>
        /// Pointer to Requested IP address.
        /// </summary>
        public IntPtr RequestedAddress;
        /// <summary>
        /// Pointer to Requested duration of the IP address lease, in seconds.
        /// </summary>
        public IntPtr RequestLeaseTime;
        /// <summary>
        /// Pointer to Overlay fields to apply to the request.
        /// </summary>
        public IntPtr OverlayFields;
        /// <summary>
        /// Pointer to IP address of the default gateway.
        /// </summary>
        public IntPtr RouterAddress;
        /// <summary>
        /// Pointer to IP address of the DHCP Server.
        /// </summary>
        public IntPtr Server;
        /// <summary>
        /// Pointer to List of requested parameters.
        /// </summary>
        public IntPtr ParameterRequestList;
        /// <summary>
        /// Length of ParameterRequestList, in bytes.
        /// </summary>
        public uint ParameterRequestListLength;
        /// <summary>
        /// Pointer to Machine name (host name) of the computer making the request.
        /// </summary>
        public IntPtr MachineName;
        /// <summary>
        /// Length of MachineName, in bytes.
        /// </summary>
        public uint MachineNameLength;
        /// <summary>
        /// Type of hardware address expressed in ClientHardwareAddress.
        /// </summary>
        public DhcpServerHardwareType ClientHardwareAddressType;
        /// <summary>
        /// Length of ClientHardwareAddress, in bytes.
        /// </summary>
        public byte ClientHardwareAddressLength;
        /// <summary>
        /// Pointer to Client hardware address.
        /// </summary>
        public IntPtr ClientHardwareAddress;
        /// <summary>
        /// Pointer to Class identifier for the client.
        /// </summary>
        public IntPtr ClassIdentifier;
        /// <summary>
        /// Length of ClassIdentifier, in bytes.
        /// </summary>
        public uint ClassIdentifierLength;
        /// <summary>
        /// Pointer to Vendor class, if applicable.
        /// </summary>
        public IntPtr VendorClass;
        /// <summary>
        /// Length of VendorClass, in bytes.
        /// </summary>
        public uint VendorClassLength;
        /// <summary>
        /// Flags used for DNS.
        /// </summary>
        public uint DNSFlags;
        /// <summary>
        /// Length of DNSName, in bytes.
        /// </summary>
        public uint DNSNameLength;
        /// <summary>
        /// Pointer to the DNS name.
        /// </summary>
        public IntPtr DNSName;
        /// <summary>
        /// Specifies whether the domain name is requested.
        /// </summary>
        public bool DSDomainNameRequested;
        /// <summary>
        /// Pointer to the domain name.
        /// </summary>
        public IntPtr DSDomainName;
        /// <summary>
        /// Length of DSDomainName, in characters.
        /// </summary>
        public uint DSDomainNameLen;
        /// <summary>
        /// Scope identifier for the IP address.
        /// </summary>
        public IntPtr ScopeId;
    }
}
