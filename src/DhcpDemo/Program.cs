using Dhcp;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace DhcpDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Discover DHCP Servers
            try
            {
                foreach (var dhcpServer in DhcpServer.Servers.ToList())
                {
                    DumpDhcpInfo(dhcpServer);
                    Console.WriteLine();
                }
            }
            catch (DhcpServerException ex) when (ex.ApiError == "DDS_NO_DS_AVAILABLE")
            {
                Console.WriteLine("No DHCP Servers could be automatically discovered");
            }

            // Directly Connect to DHCP Server
            var server = DhcpServer.Connect("localhost");
            DumpDhcpInfo(server);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("<Press any key to continue>");
            Console.ReadKey(true);
        }

        static void DumpDhcpInfo(DhcpServer dhcpServer)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{dhcpServer.Name} (v{dhcpServer.VersionMajor}.{dhcpServer.VersionMinor} - {dhcpServer.IpAddress})");

            // Configuration
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Configuration:");
            Console.ForegroundColor = ConsoleColor.Gray;
            var config = dhcpServer.Configuration;
            Console.WriteLine($"      Api Protocol Support: {config.ApiProtocolSupport}");
            Console.WriteLine($"             Database Name: {config.DatabaseName}");
            Console.WriteLine($"             Database Path: {config.DatabasePath}");
            Console.WriteLine($"               Backup Path: {config.BackupPath}");
            Console.WriteLine($"           Backup Interval: {config.BackupInterval}");
            Console.WriteLine($"  Database Logging Enabled: {config.DatabaseLoggingEnabled}");
            Console.WriteLine($"          Cleanup Interval: {config.DatabaseCleanupInterval}");

            // Audit Logging
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Audit Log:");
            Console.ForegroundColor = ConsoleColor.Gray;
            var auditLog = dhcpServer.AuditLog;
            Console.WriteLine($"        Log Directory: {auditLog.AuditLogDirectory}");
            Console.WriteLine($"  Disk Check Interval: {auditLog.DiskCheckInterval}");
            Console.WriteLine($"   Max Log Files Size: {auditLog.MaxLogFilesSize}");
            Console.WriteLine($"    Min Space On Disk: {auditLog.MinSpaceOnDisk}");

            // DNS Settings
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" DNS Settings:");
            Console.ForegroundColor = ConsoleColor.Gray;
            var dnsSettings = dhcpServer.DnsSettings;
            Console.WriteLine($"              Dynamic DNS Updates Enabled: {dnsSettings.DynamicDnsUpdatesEnabled}");
            Console.WriteLine($"  Dynamic DNS Updates Only When Requested: {dnsSettings.DynamicDnsUpdatedOnlyWhenRequested}");
            Console.WriteLine($"               Dynamic DNS Updates Always: {dnsSettings.DynamicDnsUpdatedAlways}");
            Console.WriteLine($"      Discard Records When Leases Deleted: {dnsSettings.DiscardRecordsWhenLeasesDeleted}");
            Console.WriteLine($"    Update Records for Down-Level Clients: {dnsSettings.UpdateRecordsForDownLevelClients}");
            Console.WriteLine($"       Disable Dynamic PTR Record Updates: {dnsSettings.DisableDynamicPtrRecordUpdates}");

            // Binding Elements
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Binding Elements:");
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var be in dhcpServer.BindingElements.ToList())
            {
                Console.WriteLine($"  {be.InterfaceDescription} {be.InterfaceGuidId}");
                Console.WriteLine($"       Unmodifiable Endpoint: {be.CantModify}");
                Console.WriteLine($"                    Is Bound: {be.IsBound}");
                Console.WriteLine($"  Adapter Primary IP Address: {be.AdapterPrimaryIpAddress}");
                Console.WriteLine($"      Adapter Subnet Address: {be.AdapterSubnetAddress}");
            }

            // Classes
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Classes:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"  Default Vendor Class Name: {dhcpServer.SpecificStrings.DefaultVendorClassName}");
            Console.WriteLine($"    Default User Class Name: {dhcpServer.SpecificStrings.DefaultUserClassName}");

            foreach (var c in dhcpServer.Classes.ToList())
            {
                Console.WriteLine($"  {c.Name}");
                Console.WriteLine($"      Comment: {c.Comment}");
                Console.WriteLine($"         Type: {(c.IsVendorClass ? "Vendor Class" : "User Class")}");
                Console.WriteLine($"         Data: {c.DataText}");

                // Enum Class Options
                Console.WriteLine("      Options:");
                foreach (var option in c.Options.ToList())
                {
                    Console.WriteLine($"         {option}");
                }
            }

            // Global Options
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Global Options:");
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var option in dhcpServer.Options.ToList())
            {
                Console.WriteLine($"  {option}");
            }

            // Global Option Values
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("   Global Option Values:");
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var value in dhcpServer.Options.GetOptionValues().ToList())
            {
                Console.WriteLine($"     {value}");
            }

            // Server Clients
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("      Server Clients:");
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var client in dhcpServer.Clients.ToList())
            {
                Console.WriteLine($"          {client}");
            }

            // Scopes
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Scopes:");
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var scope in dhcpServer.Scopes.ToList())
            {
                Console.WriteLine($"   {scope.Address}");
                Console.WriteLine($"            IP Range: {scope.IpRange}");
                Console.WriteLine($"                Mask: {scope.Mask}");
                Console.WriteLine($"               State: {scope.State}");
                Console.WriteLine($"                Name: {scope.Name}");
                Console.WriteLine($"             Comment: {scope.Comment}");
                Console.WriteLine($"        Primary Host: {scope.PrimaryHost}");
                Console.WriteLine($"      Lease Duration: {scope.LeaseDuration?.ToString() ?? "Unlimited"}");
                Console.WriteLine($"         Delay Offer: {scope.TimeDelayOffer.TotalMilliseconds} milliseconds");
                Console.WriteLine($"       Quarantine On: {scope.QuarantineOn}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("      Excluded IP Ranges:");
                Console.ForegroundColor = ConsoleColor.Gray;
                foreach (var ipRange in scope.ExcludedIpRanges)
                {
                    Console.WriteLine($"        {ipRange}");
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("      Options:");
                Console.ForegroundColor = ConsoleColor.Gray;
                foreach (var value in scope.Options.ToList())
                {
                    Console.WriteLine($"        {value}");
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("      Reservations:");
                Console.ForegroundColor = ConsoleColor.Gray;
                foreach (var reservation in scope.Reservations.ToList())
                {
                    Console.WriteLine($"        {reservation}");
                    Console.WriteLine($"        Client: {reservation.Client}");
                    Console.WriteLine("          Options:");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    foreach (var value in reservation.OptionValues.ToList())
                    {
                        Console.WriteLine($"            {value}");
                    }
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("      Clients:");
                Console.ForegroundColor = ConsoleColor.Gray;
                foreach (var client in scope.Clients.ToList())
                {
                    Console.WriteLine($"        {client}");
                }
            }
        }
    }
}
