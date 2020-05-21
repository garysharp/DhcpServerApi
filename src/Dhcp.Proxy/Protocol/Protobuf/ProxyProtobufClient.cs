using Dhcp.Proxy.Models;
using Dhcp.Proxy.Protocol.Protobuf.Models;
using Dhcp.Proxy.Transport;
using Google.Protobuf;
using System;
using System.Collections.Generic;

namespace Dhcp.Proxy.Protocol.Protobuf
{
    public class ProxyProtobufClient : IProxy
    {
        private readonly IProxyClientTransport transport;

        public ProxyProtobufClient(IProxyClientTransport transport)
        {
            this.transport = transport ?? throw new ArgumentNullException(nameof(transport));
        }

        public int GetProxyVersion()
            => Invoke<GetProxyVersionResponse>(RequestType.GetProxyVersion).Version;

        public IEnumerable<string> GetRemoteServerNames()
            => Invoke<GetRemoteServerNamesResponse>(RequestType.GetRemoteServerNames).ServerName;

        public ConnectModel Connect(string hostNameOrAddress)
            => Invoke<ConnectResponse>(RequestType.Connect, (ConnectRequest)hostNameOrAddress);

        public GetAuditLogModel GetAuditLog()
            => Invoke<GetAuditLogResponse>(RequestType.GetAuditLog);

        private T Invoke<T>(RequestType requestType) where T : IMessage => Invoke<T>(requestType, null);
        private T Invoke<T>(RequestType requestType, IMessage request) where T : IMessage
        {
            byte[] requestBytes;

            if (request == null)
            {
                // no request body
                requestBytes = new byte[CodedOutputStream.ComputeUInt32Size((uint)requestType)];
                var messageWriter = new CodedOutputStream(requestBytes);
                messageWriter.WriteUInt32((uint)requestType);
                messageWriter.CheckNoSpaceLeft();
            }
            else
            {
                var requestSize = CodedOutputStream.ComputeUInt32Size((uint)requestType) + request.CalculateSize();
                requestBytes = new byte[requestSize];
                var messageWriter = new CodedOutputStream(requestBytes);
                messageWriter.WriteUInt32((uint)requestType);
                request.WriteTo(messageWriter);
                messageWriter.CheckNoSpaceLeft();
            }

            var response = transport.Invoke(new ArraySegment<byte>(requestBytes));
            var responseReader = new CodedInputStream(response.Array, response.Offset, response.Count);

            var result = responseReader.ReadMessage<T>();

            return result;
        }

        #region IDisposable Support
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    transport.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose() => Dispose(true);
        #endregion

    }
}
