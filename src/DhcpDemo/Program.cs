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
                Console.WriteLine("      Is Vendor: {0}", c.IsVendor);
                Console.WriteLine("      Data: {0}", c.Data.ToString());
            }

            // Enum Options
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Options:");
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var option in dhcpServer.Options.ToList())
            {
                Console.WriteLine("   {0} [{1}: {2}]", option.OptionId, option.Name, option.Comment);
                foreach (var element in option.DefaultValue.ToList())
                {
                    Console.WriteLine("      Default: {0}", element);
                }
            }

            // Enum Global Option Values
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Global Option Values:");
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var value in dhcpServer.GlobalOptionValues.ToList())
            {
                if (value.Option == null)
                    Console.WriteLine("   {0} [UNKNOWN OPTION]:", value.OptionId);
                else
                    Console.WriteLine("   {0} [{1}]:", value.OptionId, value.Option.Name);
                foreach (var element in value.Values.ToList())
                {
                    Console.WriteLine("      {0}", element);
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
                foreach (var value in scope.OptionValues.ToList())
                {
                    if (value.Option == null)
                        Console.WriteLine("        {0} [UNKNOWN OPTION]:", value.OptionId);
                    else
                        Console.WriteLine("        {0} [{1}]:", value.OptionId, value.Option.Name);
                    foreach (var element in value.Values.ToList())
                    {
                        Console.WriteLine("           {0}", element);
                    }
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
                        if (value.Option == null)
                            Console.WriteLine("            {0} [UNKNOWN OPTION]:", value.OptionId);
                        else
                            Console.WriteLine("            {0} [{1}]:", value.OptionId, value.Option.Name);
                        foreach (var element in value.Values.ToList())
                        {
                            Console.WriteLine("               {0}", element);
                        }
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
