using System;

namespace Dhcp
{
    [Flags]
    public enum DhcpServerApiProtocol
    {
        /// <summary>
        /// TCP/IP can be used for DHCP API RPC calls. (DHCP_SERVER_USE_RPC_OVER_TCPIP)
        /// </summary>
        RpcOverTcpIp = 0x1,
        /// <summary>
        /// Named pipes can be used for DHCP API RPC calls. (DHCP_SERVER_USE_RPC_OVER_NP)
        /// </summary>
        RpcOverNamedPipes = 0x2,
        /// <summary>
        /// Local Procedure Call (LPC) can be used for local DHCP API RPC calls. (DHCP_SERVER_USE_RPC_OVER_LPC)
        /// </summary>
        RpcOverLocalProcedureCall = 0x4,
        /// <summary>
        /// The DHCP server supports all of the preceding protocols. (DHCP_SERVER_USE_RPC_OVER_ALL)
        /// </summary>
        RpcOverAll = 0x7
    }
}
