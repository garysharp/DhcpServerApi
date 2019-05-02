using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerBindingElement
    {
        private readonly DHCP_IP_ADDRESS adapterPrimaryIpAddress;
        private readonly DHCP_IP_MASK adapterSubnetAddress;

        /// <summary>
        /// The associated DHCP Server
        /// </summary>
        public DhcpServer Server { get; }

        /// <summary>
        /// The binding specified in this structure cannot be modified.
        /// </summary>
        public bool CantModify { get; }

        /// <summary>
        /// Specifies whether or not this binding is set on the DHCP server. If TRUE, the binding is set; if FALSE, it is not.
        /// </summary>
        public bool IsBound { get; }

        /// <summary>
        /// DHCP_IP_ADDRESS value that specifies the IP address assigned to the ethernet adapter of the DHCP server.
        /// </summary>
        public IPAddress AdapterPrimaryIpAddress => adapterPrimaryIpAddress.ToIPAddress();

        /// <summary>
        /// DHCP_IP_ADDRESS value that specifies the IP address assigned to the ethernet adapter of the DHCP server.
        /// </summary>
        public int AdapterPrimaryIpAddressNative => (int)adapterPrimaryIpAddress;

        /// <summary>
        /// DHCP_IP_ADDRESS value that specifies the subnet IP mask used by this ethernet adapter.
        /// </summary>
        public IPAddress AdapterSubnetAddress => adapterSubnetAddress.ToIPAddress();

        /// <summary>
        /// DHCP_IP_ADDRESS value that specifies the subnet IP mask used by this ethernet adapter.
        /// </summary>
        public int AdapterSubnetAddressNative => (int)adapterSubnetAddress;

        /// <summary>
        /// Unicode string that specifies the name assigned to this network interface device.
        /// </summary>
        public string InterfaceDescription { get; }

        /// <summary>
        /// Specifies the network interface device ID.
        /// </summary>
        public byte[] InterfaceId { get; }

        public Guid InterfaceGuidId => (InterfaceId.Length == 16) ? new Guid(InterfaceId) : Guid.Empty;

        private DhcpServerBindingElement(DhcpServer server, bool cantModify, bool isBound, DHCP_IP_ADDRESS adapterPrimaryIpAddress, DHCP_IP_MASK adapterSubnetAddress, string interfaceDescription, byte[] interfaceId)
        {
            Server = server;
            CantModify = cantModify;
            IsBound = isBound;
            this.adapterPrimaryIpAddress = adapterPrimaryIpAddress;
            this.adapterSubnetAddress = adapterSubnetAddress;
            InterfaceDescription = interfaceDescription;
            InterfaceId = interfaceId;
        }

        internal static IEnumerable<DhcpServerBindingElement> GetBindingInfo(DhcpServer server)
        {
            var result = Api.DhcpGetServerBindingInfo(ServerIpAddress: server.ipAddress,
                                                      Flags: 0,
                                                      BindElementsInfo: out var elementsPtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetServerBindingInfo), result);

            try
            {
                var elements = elementsPtr.MarshalToStructure<DHCP_BIND_ELEMENT_ARRAY>();

                foreach (var element in elements.Elements)
                    yield return FromNative(server, element);
            }
            finally
            {
                Api.DhcpRpcFreeMemory(elementsPtr);
            }
        }

        private static DhcpServerBindingElement FromNative(DhcpServer server, DHCP_BIND_ELEMENT native)
        {
            var interfaceId = new byte[native.IfIdSize];
            Marshal.Copy(native.IfId, interfaceId, 0, native.IfIdSize);

            return new DhcpServerBindingElement(server: server,
                                                cantModify: (native.Flags & Constants.DHCP_ENDPOINT_FLAG_CANT_MODIFY) > 0,
                                                isBound: native.fBoundToDHCPServer,
                                                adapterPrimaryIpAddress: native.AdapterPrimaryAddress.ToReverseOrder(),
                                                adapterSubnetAddress: native.AdapterSubnetAddress.ToReverseOrder(),
                                                interfaceDescription: native.IfDescription,
                                                interfaceId: interfaceId);
        }
    }
}
