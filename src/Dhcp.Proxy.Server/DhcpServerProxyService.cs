using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dhcp.Proxy.Server
{
    public class DhcpServerProxyService : IHostedService
    {
        private readonly IProxyTransportServer transportServer;

        public DhcpServerProxyService(IProxyTransportServer transportServer)
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
            transportServer.Stop();

            return Task.CompletedTask;
        }
    }
}
