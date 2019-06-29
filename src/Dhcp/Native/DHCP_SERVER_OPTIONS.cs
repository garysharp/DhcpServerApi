using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_SERVER_OPTIONS structure specifies requested DHCP Server options.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_SERVER_OPTIONS
    {
        /// <summary>
        /// Pointer to DHCP message type.
        /// </summary>
        public readonly IntPtr MessageType;
        /// <summary>
        /// Pointer to Subnet mask.
        /// </summary>
        public readonly IntPtr SubnetMask;
        /// <summary>
        /// Pointer to Requested IP address.
        /// </summary>
        public readonly IntPtr RequestedAddress;
        /// <summary>
        /// Pointer to Requested duration of the IP address lease, in seconds.
        /// </summary>
        public readonly IntPtr RequestLeaseTime;
        /// <summary>
        /// Pointer to Overlay fields to apply to the request.
        /// </summary>
        public readonly IntPtr OverlayFields;
        /// <summary>
        /// Pointer to IP address of the default gateway.
        /// </summary>
        public readonly IntPtr RouterAddress;
        /// <summary>
        /// Pointer to IP address of the DHCP Server.
        /// </summary>
        public readonly IntPtr Server;
        /// <summary>
        /// Pointer to List of requested parameters.
        /// </summary>
        public readonly IntPtr ParameterRequestList;
        /// <summary>
        /// Length of ParameterRequestList, in bytes.
        /// </summary>
        public readonly uint ParameterRequestListLength;
        /// <summary>
        /// Pointer to Machine name (host name) of the computer making the request.
        /// </summary>
        public readonly IntPtr MachineName;
        /// <summary>
        /// Length of MachineName, in bytes.
        /// </summary>
        public readonly uint MachineNameLength;
        /// <summary>
        /// Type of hardware address expressed in ClientHardwareAddress.
        /// </summary>
        public readonly DhcpServerHardwareType ClientHardwareAddressType;
        /// <summary>
        /// Length of ClientHardwareAddress, in bytes.
        /// </summary>
        public readonly byte ClientHardwareAddressLength;
        /// <summary>
        /// Pointer to Client hardware address.
        /// </summary>
        public readonly IntPtr ClientHardwareAddress;
        /// <summary>
        /// Pointer to Class identifier for the client.
        /// </summary>
        public readonly IntPtr ClassIdentifier;
        /// <summary>
        /// Length of ClassIdentifier, in bytes.
        /// </summary>
        public readonly uint ClassIdentifierLength;
        /// <summary>
        /// Pointer to Vendor class, if applicable.
        /// </summary>
        public readonly IntPtr VendorClass;
        /// <summary>
        /// Length of VendorClass, in bytes.
        /// </summary>
        public readonly uint VendorClassLength;
        /// <summary>
        /// Flags used for DNS.
        /// </summary>
        public readonly uint DNSFlags;
        /// <summary>
        /// Length of DNSName, in bytes.
        /// </summary>
        public readonly uint DNSNameLength;
        /// <summary>
        /// Pointer to the DNS name.
        /// </summary>
        public readonly IntPtr DNSName;
        /// <summary>
        /// Specifies whether the domain name is requested.
        /// </summary>
        public readonly bool DSDomainNameRequested;
        /// <summary>
        /// Pointer to the domain name.
        /// </summary>
        public readonly IntPtr DSDomainName;
        /// <summary>
        /// Length of DSDomainName, in characters.
        /// </summary>
        public readonly uint DSDomainNameLen;
        /// <summary>
        /// Scope identifier for the IP address.
        /// </summary>
        public readonly IntPtr ScopeId;
    }
}
