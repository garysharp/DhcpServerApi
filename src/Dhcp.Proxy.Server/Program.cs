using Dhcp.Proxy.Protocol.Protobuf;
using Dhcp.Proxy.Transport;
using Dhcp.Proxy.Transport.NamedPipe;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Dhcp.Proxy.Server
{
    public class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(config =>
                {
                    config.AddJsonFile("hostsettings.json", optional: true);
                    config.AddEnvironmentVariables("DHCPPROXY_");
                    config.AddCommandLine(args);
                })
                .ConfigureAppConfiguration(config =>
                {
                    config.AddEnvironmentVariables("DHCPPROXY_");
                    config.AddCommandLine(args);
                })
                .ConfigureServices(ConfigureServices)
                .UseConsoleLifetime();

        public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            var env = context.HostingEnvironment;

            // proxy handler
            services.AddTransient<DhcpServerProxyServer>();
            services.AddTransient<IProxy>(s => s.GetRequiredService<DhcpServerProxyServer>());

            // configure transport
            var transportName = context.Configuration.GetValue<string>("transport");
            if ("namedPipes".Equals(transportName, StringComparison.OrdinalIgnoreCase))
            {
                // Named Pipes Transport
                services.AddSingleton<NamedPipeServerTransport>();
                services.AddSingleton<IProxyTransportServer>(s => s.GetRequiredService<NamedPipeServerTransport>());
            }
            else
            {
                throw new Exception($"Invalid Transport Specified: '{transportName}'");
            }

            // configure protocol
            var protocolName = context.Configuration.GetValue<string>("protocol");
            if ("protocolBuffers".Equals(protocolName, StringComparison.OrdinalIgnoreCase))
            {
                // Protocol Buffers Protocol
                services.AddSingleton<ProxyProtobufHandler>();
                services.AddSingleton<IProxyTransportMessageHandler>(s => s.GetRequiredService<ProxyProtobufHandler>());
            }
            else
            {
                throw new Exception($"Invalid Protocol Specified: '{protocolName}'");
            }

            services.AddHostedService<TransportHost>();
        }

    }
}
