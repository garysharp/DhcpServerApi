using Dhcp.Proxy.Protocol.Protobuf.Models;
using Dhcp.Proxy.Transport;
using Google.Protobuf;
using System;

namespace Dhcp.Proxy.Protocol.Protobuf
{
    public class ProxyProtobufHandler : IProxyTransportMessageHandler
    {
        private readonly IProxy handler;

        public ProxyProtobufHandler(IProxy handler)
        {
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public byte[] HandleMessage(ArraySegment<byte> request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (request.Count < 1)
                throw new ArgumentOutOfRangeException(nameof(request), "Unexpected request length");

            var stream = new CodedInputStream(request.Array, request.Offset, request.Count);

            // decode message id
            var messageId = (RequestType)stream.ReadInt32();

            switch (messageId)
            {
                case RequestType.GetProxyVersion:
                    return new GetProxyVersionResponse(handler.GetProxyVersion()).ToByteArray();
                case RequestType.GetRemoteServerNames:
                    return new GetRemoteServerNamesResponse(handler.GetRemoteServerNames()).ToByteArray();
                case RequestType.Connect:
                    return ((ConnectResponse)handler.Connect(stream.ReadMessage<ConnectRequest>().HostNameOrAddress)).ToByteArray();
                case RequestType.GetAuditLog:
                    return ((GetAuditLogResponse)handler.GetAuditLog()).ToByteArray();
                default:
                    throw new ArgumentOutOfRangeException(nameof(request), "Unexpected request version");
            }

        }

        public string HandleMessage(string request) => throw new NotSupportedException();

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose() => Dispose(true);
        #endregion
    }
}
