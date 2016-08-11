using Dhcp;
using System;
using System.Linq;

namespace DhcpDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Discover DHCP Servers
            foreach (var dhcpServer in DhcpServer.Servers.ToList())
            {
                DumpDhcpInfo(dhcpServer);
                Console.WriteLine();
            }

            // Directly Connect to DHCP Server
            var server = DhcpServer.Connect("192.168.1.1");
            DumpDhcpInfo(server);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("<Press any key to continue>");
            Console.ReadKey(true);
        }

        static void DumpDhcpInfo(DhcpServer dhcpServer)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("{0} (v{1}.{2} - {3})", dhcpServer.Name, dhcpServer.VersionMajor, dhcpServer.VersionMinor, dhcpServer.IpAddress);

            // Configuration
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Configuration:");
            Console.ForegroundColor = ConsoleColor.Gray;
            var config = dhcpServer.Configuration;
            Console.WriteLine("{0,30}: {1}", "Api Protocol Support", config.ApiProtocolSupport.ToString());
            Console.WriteLine("{0,30}: {1}", "Database Name", config.DatabaseName);
            Console.WriteLine("{0,30}: {1}", "Database Path", config.DatabasePath);
            Console.WriteLine("{0,30}: {1}", "Backup Path", config.BackupPath);
            Console.WriteLine("{0,30}: {1}", "Backup Interval", config.BackupInterval);
            Console.WriteLine("{0,30}: {1}", "Database Logging Enabled", config.DatabaseLoggingEnabled);
            Console.WriteLine("{0,30}: {1}", "Cleanup Interval", config.DatabaseCleanupInterval);

            // Audit Logging
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Audit Log:");
            Console.ForegroundColor = ConsoleColor.Gray;
            var auditLog = dhcpServer.AuditLog;
            Console.WriteLine("{0,30}: {1}", "Log Directory", auditLog.AuditLogDirectory);
            Console.WriteLine("{0,30}: {1}", "Disk Check Interval", auditLog.DiskCheckInterval);
            Console.WriteLine("{0,30}: {1}", "Max Log Files Size", auditLog.MaxLogFilesSize);
            Console.WriteLine("{0,30}: {1}", "Min Space On Disk", auditLog.MinSpaceOnDisk);

            // DNS Settings
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" DNS Settings:");
            Console.ForegroundColor = ConsoleColor.Gray;
            var dnsSettings = dhcpServer.DnsSettings;
            Console.WriteLine("{0,44}: {1}", "Dynamic DNS Updates Enabled", dnsSettings.DynamicDnsUpdatesEnabled);
            Console.WriteLine("{0,44}: {1}", "Dynamic DNS Updates Only When Requested", dnsSettings.DynamicDnsUpdatedOnlyWhenRequested);
            Console.WriteLine("{0,44}: {1}", "Dynamic DNS Updates Always", dnsSettings.DynamicDnsUpdatedAlways);
            Console.WriteLine("{0,44}: {1}", "Discard Records When Leases Deleted", dnsSettings.DiscardRecordsWhenLeasesDeleted);
            Console.WriteLine("{0,44}: {1}", "Update Records for Down-Level Clients", dnsSettings.UpdateRecordsForDownLevelClients);
            Console.WriteLine("{0,44}: {1}", "Disable Dynamic PTR Record Updates", dnsSettings.DisableDynamicPtrRecordUpdates); 

            // Binding Elements
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Binding Elements:");
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var be in dhcpServer.BindingElements.ToList())
            {
                Console.WriteLine("   {0} {1}", be.InterfaceDescription, be.InterfaceGuidId.ToString());
                Console.WriteLine("{0,30}: {1}", "Unmodifiable Endpoint", be.CantModify);
                Console.WriteLine("{0,30}: {1}", "Is Bound", be.IsBound);
                Console.WriteLine("{0,30}: {1}", "Adapter Primary IP Address", be.AdapterPrimaryIpAddress);
                Console.WriteLine("{0,30}: {1}", "Adapter Subnet Address", be.AdapterSubnetAddress);
            }

            // Enum Default Options
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" All Options:");
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var option in dhcpServer.AllOptions.ToList())
            {
                Console.WriteLine("   {0}", option.ToString());
            }

            // Enum Default Options
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Default Options ({0}):", dhcpServer.DefaultVendorClassName);
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var option in dhcpServer.Options.ToList())
            {
                Console.WriteLine("   {0}", option.ToString());
            }

            // Enum Default Global Option Values
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Global Option Values:");
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var value in dhcpServer.AllGlobalOptionValues.ToList())
            {
                Console.WriteLine("   {0}", value.ToString());
            }

            // Enum Classes
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Classes:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("{0,30}: {1}", "Default Vendor Class Name", dhcpServer.DefaultVendorClassName);
            Console.WriteLine("{0,30}: {1}", "Default User Class Name", dhcpServer.DefaultUserClassName);

            foreach (var c in dhcpServer.Classes.ToList())
            {
                Console.WriteLine("   {0}", c.Name);
                Console.WriteLine("      Comment: {0}", c.Comment);
                Console.WriteLine("      Type: {0}", c.IsVendorClass ? "Vendor Class" : "User Class");
                Console.WriteLine("      Data: {0}", c.DataText);

                // Enum Class Options
                Console.WriteLine("      Options");
                foreach (var option in c.Options.ToList())
                {
                    Console.WriteLine("         {0}", option.ToString());
                }
            }

            // Server Clients
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("      Server Clients:");
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var client in dhcpServer.Clients.ToList())
            {
                Console.WriteLine("          {0}", client);
            }

            // Enum Scopes
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Scopes:");
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var scope in dhcpServer.Scopes.ToList())
            {
                Console.WriteLine("   {0}", scope.Address);
                Console.WriteLine("      IP Range: {0}", scope.IpRange);
                Console.WriteLine("      Mask: {0}", scope.Mask);
                Console.WriteLine("      State: {0}", scope.State);
                Console.WriteLine("      Name: {0}", scope.Name);
                Console.WriteLine("      Comment: {0}", scope.Comment);
                Console.WriteLine("      Primary Host: {0}", scope.PrimaryHostIpAddress);
                Console.WriteLine("      Lease Duration: {0}", scope.LeaseDuration);
                Console.WriteLine("      Delay Offer: {0} milliseconds", scope.TimeDelayOffer.TotalMilliseconds);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("      Excluded IP Ranges:");
                Console.ForegroundColor = ConsoleColor.Gray;
                foreach (var ipRange in scope.ExcludedIpRanges)
                {
                    Console.WriteLine("        {0}", ipRange);
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("      Options:");
                Console.ForegroundColor = ConsoleColor.Gray;
                foreach (var value in scope.AllOptionValues.ToList())
                {
                    Console.WriteLine("        {0}", value.ToString());
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("      Reservations:");
                Console.ForegroundColor = ConsoleColor.Gray;
                foreach (var reservation in scope.Reservations.ToList())
                {
                    Console.WriteLine("        {0}", reservation);
                    Console.WriteLine("        Client: {0}", reservation.Client);
                    Console.WriteLine("          Options:");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    foreach (var value in reservation.OptionValues.ToList())
                    {
                        Console.WriteLine("            {0}", value.ToString());
                    }
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("      Clients:");
                Console.ForegroundColor = ConsoleColor.Gray;
                foreach (var client in scope.Clients.ToList())
                {
                    Console.WriteLine("        {0}", client);
                }
            }
        }
    }
}
