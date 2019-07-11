using System;
using System.Collections.Generic;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerBindingElement : IDhcpServerBindingElement
    {
        private readonly byte[] interfaceId;

        /// <summary>
        /// The associated DHCP Server
        /// </summary>
        public IDhcpServer Server { get; }

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
        public DhcpServerIpAddress AdapterPrimaryIpAddress { get; }

        /// <summary>
        /// DHCP_IP_ADDRESS value that specifies the subnet IP mask used by this ethernet adapter.
        /// </summary>
        public DhcpServerIpMask AdapterSubnetAddress { get; }

        /// <summary>
        /// Unicode string that specifies the name assigned to this network interface device.
        /// </summary>
        public string InterfaceDescription { get; }

        /// <summary>
        /// Specifies the network interface device ID.
        /// </summary>
        public byte[] InterfaceId
        {
            get
            {
                if (interfaceId == null)
                    return InterfaceGuidId.ToByteArray();
                else
                    return interfaceId;
            }
        }

        public Guid InterfaceGuidId { get; }

        private DhcpServerBindingElement(DhcpServer server, bool cantModify, bool isBound, DhcpServerIpAddress adapterPrimaryIpAddress, DhcpServerIpMask adapterSubnetAddress, string interfaceDescription, Guid interfaceGuidId, byte[] interfaceId)
        {
            Server = server;
            CantModify = cantModify;
            IsBound = isBound;
            AdapterPrimaryIpAddress = adapterPrimaryIpAddress;
            AdapterSubnetAddress = adapterSubnetAddress;
            InterfaceDescription = interfaceDescription;
            this.interfaceId = interfaceId;
            InterfaceGuidId = interfaceGuidId;
        }

        internal static IEnumerable<DhcpServerBindingElement> GetBindingInfo(DhcpServer server)
        {
            var result = Api.DhcpGetServerBindingInfo(ServerIpAddress: server.Address,
                                                      Flags: 0,
                                                      BindElementsInfo: out var elementsPtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException(nameof(Api.DhcpGetServerBindingInfo), result);

            try
            {
                using (var elements = elementsPtr.MarshalToStructure<DHCP_BIND_ELEMENT_ARRAY>())
                {
                    foreach (var element in elements.Elements)
                        yield return FromNative(server, in element);
                }
            }
            finally
            {
                Api.FreePointer(elementsPtr);
            }
        }

        private static DhcpServerBindingElement FromNative(DhcpServer server, in DHCP_BIND_ELEMENT native)
        {
            return new DhcpServerBindingElement(server: server,
                                                cantModify: (native.Flags & Constants.DHCP_ENDPOINT_FLAG_CANT_MODIFY) == Constants.DHCP_ENDPOINT_FLAG_CANT_MODIFY,
                                                isBound: native.fBoundToDHCPServer,
                                                adapterPrimaryIpAddress: native.AdapterPrimaryAddress.AsHostToIpAddress(),
                                                adapterSubnetAddress: native.AdapterSubnetAddress.AsHostToIpMask(),
                                                interfaceDescription: native.IfDescription,
                                                interfaceGuidId: native.IfIdGuid,
                                                interfaceId: native.IfIdIsGuid ? null : native.IfId);
        }
    }
}
