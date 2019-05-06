using Dhcp;
using Dhcp.Callout;
using System;
using System.IO;
using System.Text;

namespace DhcpCalloutDemo
{
    // A simple DHCP Callout consumer which logs events to a text document
    // This sample is IO-bound and not tuned for performance - this will impact on the DHCP Server's performance.
    // Installation:
    //  - Build, then copy 'DhcpServerApi.dll' and 'DhcpCalloutDemo.dll' to your DHCP server
    //  - Add the Multi-String registry value 'CalloutDlls' to:
    //      HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\DHCPServer\Parameters
    //  - Set the value of CalloutDlls to the full path to the 'DhcpServerApi.dll' file.
    //  - Restart the DHCP Service
    // Startup:
    //  - On service startup DHCP will discover the Callout library (DhcpServerApi.dll) and load it.
    //  - The library will then discover other .NET consumers, by:
    //       1. Looking files in HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\DHCPServer\Parameters\CalloutDotNetDlls
    //          OR
    //       2. Looking for .NET assemblies it the same directory as itself (with implementations of the ICalloutConsumer interface)
    //  - The library loads these consumers in seperate AppDomains
    //  - The library will subscribe to all required Callout functions
    //      (based on ICalloutConsumer implementations also implementing relevant interfaces (eg. IAddressOffer, INewPacket, etc))
    //  - The library also respects and handles chained dlls in the CalloutDlls registry key.

    public class DemoDhcpCalloutConsumer : ICalloutConsumer,
        IAddressDelete,
        IAddressOffer,
        IDeleteClient,
        IHandleOptions,
        INewPacket,
        IPacketDrop,
        IPacketSend
    {
        private StreamWriter logStream;

        /// <summary>
        /// Called during DhcpServerCalloutEntry.
        /// </summary>
        public void Initialize(int ApiVersion)
        {
            var demoDirectory = Path.GetDirectoryName(typeof(DemoDhcpCalloutConsumer).Assembly.Location);
            var logFile = Path.Combine(demoDirectory, "DemoDhcpCalloutLog.txt");

            var stream = new FileStream(logFile, FileMode.Append, FileAccess.Write, FileShare.Read);
            logStream = new StreamWriter(stream);

            WriteLog($"Initialized; API Version: {ApiVersion}");
        }

        /// <summary>
        /// The Control function (DhcpControlHook) is called by Microsoft DHCP Server when the DHCP Server
        /// service is started, stopped, paused, or continued. The function should not block.
        /// </summary>
        /// <remarks>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363276(v=vs.85).aspx
        /// </remarks>
        /// <param name="ControlCode">Specifies the control event that triggered the notification.</param>
        public void Control(CalloutControlCodes ControlCode)
        {
            WriteLog($"Control: {ControlCode}");
        }

        private void WriteLog(string Message)
        {
            logStream.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {Message}");
            logStream.Flush();
        }

        /// <summary>
        /// Called before the consumers AppDomain is unloaded
        /// </summary>
        public void Dispose()
        {
            // If any unmanaged resources need to be disposed that
            //  should be done here.
            // For example, database connections, etc.

            if (logStream != null)
            {
                WriteLog("Disposing");

                logStream.Dispose();
                logStream = null;
            }
        }

        public void NewPacket(IDhcpServerPacketWritable Packet, DhcpServerIpAddress ServerAddress, ref bool ProcessIt, ref bool StopPropagation)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("NEW PACKET:");
                sb.AppendLine(Packet.ToString());
                sb.AppendLine("---------------------");
                WriteLog(sb.ToString());
            }
            catch (Exception ex)
            {

                WriteLog($"NEW PACKET ERROR: {ex.Message} [{ex.GetType().Name}]");
                WriteLog(ex.StackTrace);

                throw;
            }
        }

        public void PacketSend(IDhcpServerPacketWritable Packet, DhcpServerIpAddress ServerAddress, ref bool StopPropagation)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("PACKET SEND:");
                sb.AppendLine(Packet.ToString());
                sb.AppendLine("---------------------");
                WriteLog(sb.ToString());
            }
            catch (Exception ex)
            {

                WriteLog($"SEND PACKET ERROR: {ex.Message} [{ex.GetType().Name}]");
                WriteLog(ex.StackTrace);

                throw;
            }

        }

        public void AddressOffer(IDhcpServerPacket Packet, OfferAddressControlCodes ControlCode, DhcpServerIpAddress ServerAddress, DhcpServerIpAddress LeaseAddress, OfferAddressTypes AddressType, TimeSpan LeaseTime, ref bool StopPropagation)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("ADDRESS OFFER:");
                sb.AppendLine($" Transaction Id: {Packet.TransactionId}");
                sb.AppendLine($" Parameters:");
                sb.AppendLine($"  Dhcp Message Type: {Packet.DhcpMessageType}");
                sb.AppendLine($"  Control Code: {ControlCode}");
                sb.AppendLine($"  Offering: {LeaseAddress}");
                sb.AppendLine($"  Type: {AddressType}");
                sb.AppendLine($"  Time: {LeaseTime}");
                sb.AppendLine("---------------------");

                WriteLog(sb.ToString());
            }
            catch (Exception ex)
            {

                WriteLog($"ADDRESS OFFER ERROR: {ex.Message} [{ex.GetType().Name}]");
                WriteLog(ex.StackTrace);

                throw;
            }
        }

        public void PacketDrop(IDhcpServerPacket Packet, PacketDropControlCodes ControlCode, DhcpServerIpAddress ServerAddress, ref bool StopPropagation)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("PACKET DROP:");
                sb.AppendLine($" Transaction Id: {Packet.TransactionId}");
                sb.AppendLine($" Parameters:");
                sb.AppendLine($"  Control Code: {ControlCode}");
                sb.AppendLine("---------------------");

                WriteLog(sb.ToString());
            }
            catch (Exception ex)
            {

                WriteLog($"PACKET DROP ERROR: {ex.Message} [{ex.GetType().Name}]");
                WriteLog(ex.StackTrace);

                throw;
            }
        }

        public void AddressDelete(IDhcpServerPacket Packet, AddressDeleteControlCodes ControlCode, DhcpServerIpAddress ServerAddress, DhcpServerIpAddress LeaseAddress, ref bool StopPropagation)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("ADDRESS DELETE:");
                sb.AppendLine($" Transaction Id: {Packet.TransactionId}");
                sb.AppendLine($" Parameters:");
                sb.AppendLine($"  Control Code: {ControlCode}");
                sb.AppendLine($"  Lease Address: {LeaseAddress}");
                sb.AppendLine("---------------------");

                WriteLog(sb.ToString());
            }
            catch (Exception ex)
            {

                WriteLog($"ADDRESS DELETE ERROR: {ex.Message} [{ex.GetType().Name}]");
                WriteLog(ex.StackTrace);

                throw;
            }
        }

        public void DeleteClient(DhcpServerIpAddress LeaseAddress, DhcpServerHardwareAddress LeaseHardwareAddress, ref bool StopPropagation)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("DELETE CLIENT:");
                sb.AppendLine($"  Lease Address: {LeaseAddress}");
                sb.AppendLine($"  Hardware Address: {LeaseHardwareAddress}");
                sb.AppendLine("---------------------");

                WriteLog(sb.ToString());
            }
            catch (Exception ex)
            {

                WriteLog($"DELETE CLIENT ERROR: {ex.Message} [{ex.GetType().Name}]");
                WriteLog(ex.StackTrace);

                throw;
            }
        }

        public void HandleOptions(IDhcpServerPacket Packet, DhcpServerPacketOptions ServerOptions, ref bool StopPropagation)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("HANDLE OPTIONS:");
                sb.AppendLine($" Transaction Id: {Packet.TransactionId}");
                sb.AppendLine($" Server Options:");
                sb.AppendLine($"  Message Type: {ServerOptions.MessageType}");
                sb.AppendLine($"  Subnet Mask: {ServerOptions.SubnetMask}");
                sb.AppendLine($"  Requested Address: {ServerOptions.RequestedAddress}");
                sb.AppendLine($"  Requested Lease Time: {ServerOptions.RequestedLeaseTime}");
                sb.AppendLine($"  Router Address: {ServerOptions.RouterAddress}");
                sb.AppendLine($"  Server: {ServerOptions.Server}");
                sb.AppendLine($"  Parameter Request List: {string.Join(", ", ServerOptions.ParameterRequestList)}");
                sb.AppendLine($"  Machine Name: {ServerOptions.MachineName}");
                sb.AppendLine($"  Client Hardware Address Type: {ServerOptions.ClientHardwareAddressType}");
                sb.AppendLine($"  Client Hardware Address: {ServerOptions.ClientHardwareAddress}");
                sb.AppendLine($"  Class Identifier: {ServerOptions.ClassIdentifier}");

                sb.Append($"  Vendor Class: ");
                foreach (var b in ServerOptions.VendorClass)
                    sb.Append(b.ToString("X2"));

                sb.AppendLine();
                sb.AppendLine($"  Dns Name: {ServerOptions.DnsName}");
                sb.AppendLine($"  Ds Domain Name Requested: {ServerOptions.DsDomainNameRequested}");
                sb.AppendLine($"  Ds Domain Name: {ServerOptions.DsDomainName}");
                sb.AppendLine($"  Scope Id: {ServerOptions.ScopeId}");
                sb.AppendLine("---------------------");

                WriteLog(sb.ToString());
            }
            catch (Exception ex)
            {

                WriteLog($"HANDLE OPTIONS ERROR: {ex.Message} [{ex.GetType().Name}]");
                WriteLog(ex.StackTrace);

                throw;
            }
        }
    }
}
