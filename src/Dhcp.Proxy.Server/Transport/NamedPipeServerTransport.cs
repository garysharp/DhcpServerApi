using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace Dhcp.Proxy.Transport.NamedPipe
{
    public class NamedPipeServerTransport : IProxyTransportServer
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ConcurrentDictionary<Guid, NamedPipeServerStream> connections;
        private readonly NamedPipeServerTransportConfiguration config;
        private readonly ILogger<NamedPipeServerTransport> logger;
        private bool running = false;

        public NamedPipeServerTransport(IServiceProvider serviveProvider, IConfiguration config, ILogger<NamedPipeServerTransport> logger)
        {
            this.serviceProvider = serviveProvider ?? throw new ArgumentNullException(nameof(serviveProvider));
            this.config = config.GetSection("namedPipes").Get<NamedPipeServerTransportConfiguration>();

            this.logger = logger;
            this.connections = new ConcurrentDictionary<Guid, NamedPipeServerStream>();
        }

        public void Start()
        {
            running = true;
            if (connections.Count == 0)
            {
                lock (connections)
                {
                    if (connections.Count == 0)
                    {
                        Task.Run(RunThread);
                    }
                }
            }
        }

        public void Stop()
        {
            running = false;
            if (connections.Count > 0)
            {
                lock (connections)
                {
                    if (connections.Count > 0)
                    {
                        foreach (var connection in connections)
                        {
                            connection.Value.Dispose();
                        }
                        connections.Clear();
                    }
                }
            }
        }

        private async Task RunThread()
        {
            using (var connection = new NamedPipeServerStream(config.Name, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
            {
                var connectionId = Guid.NewGuid();

                logger.LogInformation($"{connectionId}: New Listener, waiting for connections");

                connections.TryAdd(connectionId, connection);

                // wait for a new connection

                await connection.WaitForConnectionAsync().ConfigureAwait(false);

                // new connection received (or stopping)

                if (running)
                {
                    // start a new listener
                    _ = Task.Run(RunThread);
                }

                if (connection.IsConnected && running)
                {
                    // create handler
                    using (var handler = serviceProvider.GetRequiredService<IProxyTransportMessageHandler>())
                    {
                        var requestOffset = 0;
                        var requestLength = 0;
                        var requestBuffer = new byte[1024];
                        var responseBuffer = new byte[1024];

                        while (connection.IsConnected)
                        {
                            requestLength = await connection.ReadAsync(requestBuffer, requestOffset, requestBuffer.Length - requestOffset).ConfigureAwait(false) + requestOffset;
                            if (requestLength == 0 || !connection.IsConnected)
                            {
                                logger.LogWarning($"{connectionId} Disconnected");
                                continue; // likely disconnection
                            }
                            
                            requestOffset = 0;

                            // check we got the whole message
                            if (requestLength < 8)
                            {
                                // invalid message - disconnect
                                logger.LogWarning($"{connectionId} Invalid message received, disconnecting");
                                goto disconnect;
                            }

                            var (instruction, messageId, dataLength) = requestBuffer.ReadNamedPipeHeader(ref requestOffset, ref requestLength);

                            if (instruction != NamedPipeMessageInstruction.InvokeRequest) // server only needs to handle InvokeRequest at this time
                            {
                                // unknown instruction - disconnect
                                logger.LogWarning($"{connectionId} Unknown transport version/instruction ({instruction}), disconnecting");
                                goto disconnect;
                            }
                            if (dataLength < 0 || dataLength > 0x7FFFFFFF)
                                throw new ProxyTransportException("Invalid Request Size (>2GB)");

                            // ensure we have the whole message
                            if (requestLength < dataLength)
                            {
                                // ensure buffer is large enough
                                BufferHelpers.EnsureBufferCapacityPreserve(ref requestBuffer, ref responseBuffer, requestOffset + dataLength);

                                // attempt to read the rest of the data
                                requestLength = (await connection.ReadAsync(requestBuffer, requestOffset + requestLength, dataLength - requestLength).ConfigureAwait(false)) + requestLength;

                                if (requestLength < dataLength)
                                {
                                    // unknown version - disconnect
                                    logger.LogWarning($"{connectionId} Incomplete message received, protocol corrupt, disconnecting");
                                    goto disconnect;
                                }
                            }

                            // handle request
                            var responseLength = HandleRequest(ref requestBuffer, requestOffset, requestLength, messageId, handler, ref responseBuffer);
                            requestOffset += dataLength;
                            requestLength -= dataLength;

                            // send response
                            await connection.WriteAsync(responseBuffer, 0, responseLength).ConfigureAwait(false);
                            await connection.FlushAsync().ConfigureAwait(false);

                            // append remainder (if we got too much data assume its part of the next message)
                            // precaution only - this shouldn't happen due to named pipes message mode.
                            BufferHelpers.ReindexRemainder(ref requestBuffer, ref requestOffset, requestLength, ref responseBuffer);

                            // shrink buffers if they are exceptionally large
                            BufferHelpers.ShrinkBuffer(ref responseBuffer, 0xFFFFFF); // ~16MB
                            BufferHelpers.ShrinkBufferPreserve(ref requestBuffer, requestLength, 0xFFFFFF, ref responseBuffer);
                        }
                    disconnect:;
                    }
                }

                connections.TryRemove(connectionId, out _);
            }
        }

        private int HandleRequest(ref byte[] requestBuffer, int requestOffset, int requestLength, int messageId, IProxyTransportMessageHandler handler, ref byte[] responseBuffer)
        {
            try
            {
                var responseData = handler.HandleMessage(new ArraySegment<byte>(requestBuffer, requestOffset, requestLength));
                var responseDataLength = responseData?.Length ?? 0;

                BufferHelpers.EnsureBufferCapacity(ref responseBuffer, responseDataLength + BufferHelpers.MessageHeaderLength);
                BufferHelpers.InitializeMessage(ref responseBuffer, NamedPipeMessageInstruction.InvokeResponse, messageId, responseDataLength, out var responseOffset);
                
                if (responseDataLength > 0)
                {
                    Array.Copy(responseData, 0, responseBuffer, responseOffset, responseData.Length);
                    responseOffset += responseData.Length;
                }
                
                return responseOffset;
            }
            catch (DhcpServerException ex)
            {
                return SerializeException(messageId, ref responseBuffer, ex);
            }
            catch (Exception ex)
            {
                return SerializeException(messageId, ref responseBuffer, ex);
            }
        }

        private int SerializeException(int messageId, ref byte[] responseBuffer, DhcpServerException exception)
        {
            var apiFunctionBytes = default(byte[]);
            var descriptionBytes = default(byte[]);

            if (exception.ApiFunction != null)
            {
                if (exception.ApiFunction.Length > 0x7FFF_FFFF)
                    throw new ProxyTransportException("Unable to serialize DhcpServerException; ApiFunction too long (>2GB)");

                apiFunctionBytes = Encoding.UTF8.GetBytes(exception.ApiFunction);
            }
            if (exception.Description != null)
            {
                if (exception.Description.Length > 0x7FFF_FFFF)
                    throw new ProxyTransportException("Unable to serialize DhcpServerException; Description too long (>2GB)");

                descriptionBytes = Encoding.UTF8.GetBytes(exception.Description);
            }

            var responseDataLength = 12 + (apiFunctionBytes?.Length ?? 0) + (descriptionBytes?.Length ?? 0);

            BufferHelpers.InitializeMessage(ref responseBuffer, NamedPipeMessageInstruction.DhcpServerException, messageId, responseDataLength, out var offset);

            // api error native
            responseBuffer.WriteBigEndian((int)exception.ApiErrorNative, ref offset);

            // api function
            responseBuffer.WriteNamedPipeEmbedded(apiFunctionBytes, ref offset);

            // description
            responseBuffer.WriteNamedPipeEmbedded(descriptionBytes, ref offset);

            return offset;
        }

        private int SerializeException(int messageId, ref byte[] responseBuffer, Exception exception)
        {
            var messageBytes = default(byte[]);

            if (exception.Message != null)
            {
                if (exception.Message.Length > 0x7FFF_FFFF)
                    throw new ProxyTransportException("Unable to serialize Exception; Message too long (>2GB)");

                messageBytes = Encoding.UTF8.GetBytes(exception.Message);
            }

            var responseDataLength = 4 + (messageBytes?.Length ?? 0);

            var instruction = NamedPipeMessageInstruction.Exception;
            if (exception is ProxyTransportException)
                instruction = NamedPipeMessageInstruction.TransportException;

            BufferHelpers.InitializeMessage(ref responseBuffer, instruction, messageId, responseDataLength, out var offset);
            responseBuffer.WriteNamedPipeEmbedded(messageBytes, ref offset);

            return offset;
        }

        #region IDisposable Support
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                }
                disposedValue = true;
            }
        }
        public void Dispose() => Dispose(true);
        #endregion
    }
}
