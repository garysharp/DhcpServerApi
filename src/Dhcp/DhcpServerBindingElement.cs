using Dhcp.Native;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;

namespace Dhcp
{
    public class DhcpServerBindingElement
    {
        private DHCP_IP_ADDRESS adapterPrimaryIpAddress;
        private DHCP_IP_MASK adapterSubnetAddress;

        /// <summary>
        /// The associated DHCP Server
        /// </summary>
        public DhcpServer Server { get; private set; }

        /// <summary>
        /// The binding specified in this structure cannot be modified.
        /// </summary>
        public bool CantModify { get; private set; }

        /// <summary>
        /// Specifies whether or not this binding is set on the DHCP server. If TRUE, the binding is set; if FALSE, it is not.
        /// </summary>
        public bool IsBound { get; private set; }

        /// <summary>
        /// DHCP_IP_ADDRESS value that specifies the IP address assigned to the ethernet adapter of the DHCP server.
        /// </summary>
        public IPAddress AdapterPrimaryIpAddress { get { return adapterPrimaryIpAddress.ToIPAddress(); } }

        /// <summary>
        /// DHCP_IP_ADDRESS value that specifies the IP address assigned to the ethernet adapter of the DHCP server.
        /// </summary>
        public int AdapterPrimaryIpAddressNative { get { return (int)adapterPrimaryIpAddress; } }

        /// <summary>
        /// DHCP_IP_ADDRESS value that specifies the subnet IP mask used by this ethernet adapter.
        /// </summary>
        public IPAddress AdapterSubnetAddress { get { return adapterSubnetAddress.ToIPAddress(); } }

        /// <summary>
        /// DHCP_IP_ADDRESS value that specifies the subnet IP mask used by this ethernet adapter.
        /// </summary>
        public int AdapterSubnetAddressNative { get { return (int)adapterSubnetAddress; } }

        /// <summary>
        /// Unicode string that specifies the name assigned to this network interface device.
        /// </summary>
        public string InterfaceDescription { get; private set; }

        /// <summary>
        /// Specifies the network interface device ID.
        /// </summary>
        public byte[] InterfaceId { get; private set; }

        public Guid InterfaceGuidId
        {
            get
            {
                if (InterfaceId.Length == 16)
                    return new Guid(InterfaceId);
                else
                    return Guid.Empty;
            }
        }

        private DhcpServerBindingElement(DhcpServer Server)
        {
            this.Server = Server;
        }

        internal static IEnumerable<DhcpServerBindingElement> GetBindingInfo(DhcpServer Server)
        {
            IntPtr elementsPtr;

            var result = Api.DhcpGetServerBindingInfo(Server.ipAddress.ToString(), 0, out elementsPtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException("DhcpGetServerBindingInfo", result);

            try
            {
                var elements = (DHCP_BIND_ELEMENT_ARRAY)Marshal.PtrToStructure(elementsPtr, typeof(DHCP_BIND_ELEMENT_ARRAY));

                foreach (var element in elements.Elements)
                {
                    yield return FromNative(Server, element);
                }
            }
            finally
            {
                Api.DhcpRpcFreeMemory(elementsPtr);
            }
        }

        private static DhcpServerBindingElement FromNative(DhcpServer Server, DHCP_BIND_ELEMENT Native)
        {
            var interfaceId = new byte[Native.IfIdSize];
            Marshal.Copy(Native.IfId, interfaceId, 0, Native.IfIdSize);

            return new DhcpServerBindingElement(Server)
            {
                CantModify = (Native.Flags & Constants.DHCP_ENDPOINT_FLAG_CANT_MODIFY) == Constants.DHCP_ENDPOINT_FLAG_CANT_MODIFY,
                IsBound = Native.fBoundToDHCPServer,
                adapterPrimaryIpAddress = Native.AdapterPrimaryAddress.ToReverseOrder(),
                adapterSubnetAddress = Native.AdapterSubnetAddress.ToReverseOrder(),
                InterfaceDescription = Native.IfDescription,
                InterfaceId = interfaceId
            };
        }
    }
}
