using System;
using System.Runtime.InteropServices;

namespace Dhcp
{
    public class DhcpServerPacketOptions : MarshalByRefObject
    {
        private readonly Native.DHCP_SERVER_OPTIONS options;

        public DhcpServerPacketOptions(IntPtr pointer)
        {
            options = pointer.MarshalToStructure<Native.DHCP_SERVER_OPTIONS>();
        }

        /// <summary>
        /// DHCP message type.
        /// </summary>
        public DhcpServerPacketMessageTypes? MessageType => options.MessageType == IntPtr.Zero ? null : (DhcpServerPacketMessageTypes?)Marshal.ReadByte(options.MessageType);

        /// <summary>
        /// Subnet mask.
        /// </summary>
        public DhcpServerIpMask? SubnetMask => options.SubnetMask == IntPtr.Zero ? (DhcpServerIpMask?)null : DhcpServerIpMask.FromNative(options.SubnetMask);

        /// <summary>
        /// Requested IP address.
        /// </summary>
        public DhcpServerIpAddress? RequestedAddress => options.RequestedAddress == IntPtr.Zero ? (DhcpServerIpAddress?)null : DhcpServerIpAddress.FromNative(options.RequestedAddress);

        /// <summary>
        /// Requested duration of the IP address lease.
        /// </summary>
        public TimeSpan? RequestedLeaseTime => options.RequestLeaseTime == IntPtr.Zero ? (TimeSpan?)null : new TimeSpan(Marshal.ReadInt32(options.RequestLeaseTime) * 10_000_000L);

        // OverlayFields ?

        /// <summary>
        /// IP address of the default gateway.
        /// </summary>
        public DhcpServerIpAddress? RouterAddress => options.RouterAddress == IntPtr.Zero ? (DhcpServerIpAddress?)null : DhcpServerIpAddress.FromNative(options.RouterAddress);

        /// <summary>
        /// IP address of the DHCP Server.
        /// </summary>
        public DhcpServerIpAddress? Server => options.Server == IntPtr.Zero ? (DhcpServerIpAddress?)null : DhcpServerIpAddress.FromNative(options.Server);

        /// <summary>
        /// List of requested parameters.
        /// </summary>
        public DhcpServerOptionIds[] ParameterRequestList
        {
            get
            {
                if (options.ParameterRequestListLength == 0 || options.ParameterRequestList == IntPtr.Zero)
                    return new DhcpServerOptionIds[0];

                var list = new DhcpServerOptionIds[(int)options.ParameterRequestListLength];

                for (var i = 0; i < list.Length; i++)
                    list[i] = (DhcpServerOptionIds)Marshal.ReadByte(options.ParameterRequestList, i);

                return list;
            }
        }

        /// <summary>
        /// Machine name (host name) of the computer making the request.
        /// </summary>
        public string MachineName => options.MachineNameLength == 0 ? string.Empty : Marshal.PtrToStringAnsi(options.MachineName, (int)options.MachineNameLength);

        /// <summary>
        /// Type of hardware address expressed in <see cref="ClientHardwareAddress"/>.
        /// </summary>
        public DhcpServerHardwareType ClientHardwareAddressType => options.ClientHardwareAddressType;

        /// <summary>
        /// Client hardware address.
        /// </summary>
        public DhcpServerHardwareAddress ClientHardwareAddress
        {
            get
            {
                if (options.ClientHardwareAddressLength == 0 ||
                    options.ClientHardwareAddressLength > DhcpServerHardwareAddress.MaximumLength)
                {
                    return DhcpServerHardwareAddress.FromNative(ClientHardwareAddressType, 0, 0, 0);
                }
                else
                {
                    return DhcpServerHardwareAddress.FromNative(ClientHardwareAddressType, options.ClientHardwareAddress, options.ClientHardwareAddressLength);
                }
            }
        }

        /// <summary>
        /// Class identifier for the client.
        /// </summary>
        public string ClassIdentifier => options.ClassIdentifierLength == 0 ? string.Empty : Marshal.PtrToStringAnsi(options.ClassIdentifier, (int)options.ClassIdentifierLength);

        /// <summary>
        /// Vendor class, if applicable.
        /// </summary>
        public byte[] VendorClass
        {
            get
            {
                if (options.VendorClassLength == 0)
                {
                    return BitHelper.EmptyByteArray;
                }
                else
                {
                    var buffer = new byte[options.VendorClassLength];
                    Marshal.Copy(options.VendorClass, buffer, 0, buffer.Length);
                    return buffer;
                }
            }
        }

        /// <summary>
        /// Flags used for DNS.
        /// </summary>
        public int DnsFlags
        {
            get
            {
                return (int)options.DNSFlags;
            }
        }

        /// <summary>
        /// The DNS name.
        /// </summary>
        public string DnsName => options.DNSNameLength == 0 ? string.Empty : Marshal.PtrToStringAnsi(options.DNSName, (int)options.DNSNameLength);

        /// <summary>
        /// Specifies whether the domain name is requested.
        /// </summary>
        public bool DsDomainNameRequested
        {
            get
            {
                return options.DSDomainNameRequested;
            }
        }

        /// <summary>
        /// The domain name.
        /// </summary>
        public string DsDomainName => options.DSDomainNameLen == 0 ? string.Empty : Marshal.PtrToStringAnsi(options.DSDomainName, (int)options.DSDomainNameLen);

        /// <summary>
        /// Scope identifier for the IP address.
        /// </summary>
        public int? ScopeId => options.ScopeId == IntPtr.Zero ? (int?)null : Marshal.ReadInt32(options.ScopeId);

    }
}
