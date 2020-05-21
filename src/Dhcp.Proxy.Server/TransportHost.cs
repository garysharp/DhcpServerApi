using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dhcp.Proxy.Server
{
    public class TransportHost : IHostedService
    {
        private readonly IProxyTransportServer transportServer;

        public TransportHost(IProxyTransportServer transportServer)
        {
            this.transportServer = transportServer ?? throw new ArgumentNullException(nameof(transportServer));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            transportServer.Start();
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            transportServer.Dispose();

            return Task.CompletedTask;
        }
    }
}
