using System;
using System.IO.Pipes;
using System.Text;

namespace Dhcp.Proxy.Transport.NamedPipe
{
    public class NamedPipeClientTransport : IProxyClientTransport
    {
        private readonly NamedPipeClientStream connection;
        private int messageId;
        private int connectionTimeout;
        private byte[] requestBuffer = new byte[1024];
        private byte[] responseBuffer = new byte[1024];

        public NamedPipeClientTransport(string proxyServerName, string pipeName, TimeSpan connectionTimeout)
        {
            if (connectionTimeout.Ticks <= 0)
                throw new ArgumentOutOfRangeException(nameof(connectionTimeout));
            if (string.IsNullOrWhiteSpace(proxyServerName))
                proxyServerName = "."; // default local
            if (string.IsNullOrWhiteSpace(pipeName))
                pipeName = "dhcpserverproxy"; // default pipe name

            this.connectionTimeout = (int)connectionTimeout.TotalMilliseconds;

            connection = new NamedPipeClientStream(proxyServerName, pipeName, PipeDirection.InOut, PipeOptions.None);
            //connection.WriteTimeout = this.connectionTimeout;
            connection.Connect(this.connectionTimeout);
        }

        public NamedPipeClientTransport(string proxyServerName)
            : this(proxyServerName, null, TimeSpan.FromSeconds(30))
        {
        }

        public ArraySegment<byte> Invoke(ArraySegment<byte> request)
        {
            if (disposedValue)
                throw new ObjectDisposedException(nameof(NamedPipeClientTransport));

            lock (connection)
            {
                var messageId = this.messageId++;
                if (messageId > 0x7FFF_FFFF)
                    messageId = this.messageId = 0;

                BufferHelpers.InitializeMessage(ref requestBuffer, NamedPipeMessageInstruction.InvokeRequest, messageId, request.Count, out var requestOffset);
                Array.Copy(request.Array, request.Offset, requestBuffer, requestOffset, request.Count);
                requestOffset += request.Count;

                // reconnect if needed
                if (!connection.IsConnected)
                    connection.Connect(connectionTimeout);

                // send request
                connection.Write(requestBuffer, 0, requestOffset);
                connection.Flush();

                // read response
                var responseLength = connection.Read(responseBuffer, 0, responseBuffer.Length);
                var responseOffset = 0;
                var (instruction, responseMessageId, dataLength) = responseBuffer.ReadNamedPipeHeader(ref responseOffset, ref responseLength);

                if (dataLength < 0 || dataLength > 0x7FFFFFFF)
                    throw new ProxyTransportException("Invalid Request Size (>2GB)");

                // ensure we have the whole message
                if (responseLength < dataLength)
                {
                    // ensure buffer is large enough
                    BufferHelpers.EnsureBufferCapacityPreserve(ref responseBuffer, ref requestBuffer, responseOffset + dataLength);

                    // attempt to read the rest of the data
                    responseLength = connection.Read(responseBuffer, responseOffset + responseLength, dataLength - responseLength) + responseLength;
                    responseOffset = 8;

                    if (responseLength < dataLength)
                    {
                        connection.Close();
                        throw new ProxyTransportException("Incomplete message received, protocol corrupt.");
                    }
                }

                if (responseMessageId != messageId)
                {
                    connection.Close();
                    throw new ProxyTransportException("Messages sequence out of order, protocol corrupt.");
                }

                var responseSegment = new ArraySegment<byte>(responseBuffer, responseOffset, dataLength);

                switch (instruction)
                {
                    case NamedPipeMessageInstruction.InvokeResponse:
                        return responseSegment;
                    case NamedPipeMessageInstruction.DhcpServerException:
                        return HandleDhcpServerException(responseSegment);
                    case NamedPipeMessageInstruction.TransportException:
                        return HandleTransportException(responseSegment);
                    case NamedPipeMessageInstruction.Exception:
                        return HandleException(responseSegment);
                    case NamedPipeMessageInstruction.InvokeRequest:
                    default:
                        throw new ProxyTransportException($"Unknown transport version/instruction ({instruction})");
                }

            }
        }

        public string Invoke(string request)
        {
            throw new NotSupportedException();
        }

        public ArraySegment<byte> HandleDhcpServerException(ArraySegment<byte> response)
        {
            var offset = response.Offset;

            var apiErrorNative = response.Array.ReadBigEndian(ref offset);
            var apiFunctionBytes = response.Array.ReadNamedPipeEmbedded(ref offset);
            var descriptionBytes = response.Array.ReadNamedPipeEmbedded(ref offset);

            var apiFunction = apiFunctionBytes.HasValue
                ? Encoding.UTF8.GetString(apiFunctionBytes.Value.Array, apiFunctionBytes.Value.Offset, apiFunctionBytes.Value.Count)
                : null;
            var description = descriptionBytes.HasValue
                ? Encoding.UTF8.GetString(descriptionBytes.Value.Array, descriptionBytes.Value.Offset, descriptionBytes.Value.Count)
                : null;

            throw new DhcpServerException(apiFunction, (DhcpServerNativeErrors)apiErrorNative, description);
        }

        public ArraySegment<byte> HandleTransportException(ArraySegment<byte> response)
        {
            var offset = response.Offset;

            var messageBytes = response.Array.ReadNamedPipeEmbedded(ref offset);
            
            var message = messageBytes.HasValue
                ? Encoding.UTF8.GetString(messageBytes.Value.Array, messageBytes.Value.Offset, messageBytes.Value.Count)
                : null;

            throw new ProxyTransportException(message);
        }

        public ArraySegment<byte> HandleException(ArraySegment<byte> response)
        {
            var offset = response.Offset;

            var messageBytes = response.Array.ReadNamedPipeEmbedded(ref offset);

            var message = messageBytes.HasValue
                ? Encoding.UTF8.GetString(messageBytes.Value.Array, messageBytes.Value.Offset, messageBytes.Value.Count)
                : null;

            throw new Exception(message);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    connection.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose() => Dispose(true);
        #endregion
    }
}
