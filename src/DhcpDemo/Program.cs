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
            try
            {
                foreach (var dhcpServer in DhcpServer.Servers.ToList())
                {
                    DumpDhcpInfo(dhcpServer);
                    WriteLine();
                }
            }
            catch (DhcpServerException ex) when (ex.ApiError == "DDS_NO_DS_AVAILABLE")
            {
                WriteLine("No DHCP Servers could be automatically discovered", ConsoleColor.Magenta);
            }

            // Directly Connect to DHCP Server
            var server = DhcpServer.Connect("localhost");
            DumpDhcpInfo(server);

            WriteLine();
            WriteLine("<Press any key to continue>", ConsoleColor.Yellow);
            Console.ReadKey(true);
        }

        static void DumpDhcpInfo(IDhcpServer dhcpServer)
        {
            WriteLine($"{dhcpServer.Name} (v{dhcpServer.VersionMajor}.{dhcpServer.VersionMinor} - {dhcpServer.Address})", ConsoleColor.Yellow);

            // Configuration
            WriteLine(" Configuration:", ConsoleColor.White);
            var config = dhcpServer.Configuration;
            WriteLine($"      API Protocol Support: {config.ApiProtocolSupport}");
            WriteLine($"             Database Name: {config.DatabaseName}");
            WriteLine($"             Database Path: {config.DatabasePath}");
            WriteLine($"               Backup Path: {config.BackupPath}");
            WriteLine($"           Backup Interval: {config.BackupInterval}");
            WriteLine($"  Database Logging Enabled: {config.DatabaseLoggingEnabled}");
            WriteLine($"          Cleanup Interval: {config.DatabaseCleanupInterval}");

            // Audit Logging
            WriteLine(" Audit Log:", ConsoleColor.White);
            var auditLog = dhcpServer.AuditLog;
            WriteLine($"        Log Directory: {auditLog.AuditLogDirectory}");
            WriteLine($"  Disk Check Interval: {auditLog.DiskCheckInterval}");
            WriteLine($"   Max Log Files Size: {auditLog.MaxLogFilesSize}");
            WriteLine($"    Min Space On Disk: {auditLog.MinSpaceOnDisk}");

            // DNS Settings
            WriteLine(" DNS Settings:", ConsoleColor.White);
            var dnsSettings = dhcpServer.DnsSettings;
            WriteLine($"              Dynamic DNS Updates Enabled: {dnsSettings.DynamicDnsUpdatesEnabled}");
            WriteLine($"  Dynamic DNS Updates Only When Requested: {dnsSettings.DynamicDnsUpdatedOnlyWhenRequested}");
            WriteLine($"               Dynamic DNS Updates Always: {dnsSettings.DynamicDnsUpdatedAlways}");
            WriteLine($"      Discard Records When Leases Deleted: {dnsSettings.DiscardRecordsWhenLeasesDeleted}");
            WriteLine($"    Update Records for Down-Level Clients: {dnsSettings.UpdateRecordsForDownLevelClients}");
            WriteLine($"       Disable Dynamic PTR Record Updates: {dnsSettings.DisableDynamicPtrRecordUpdates}");

            // Binding Elements
            WriteLine(" Binding Elements:", ConsoleColor.White);
            foreach (var be in dhcpServer.BindingElements.ToList())
            {
                WriteLine($"  {be.InterfaceDescription} {be.InterfaceGuidId}");
                WriteLine($"       Unmodifiable Endpoint: {be.CantModify}");
                WriteLine($"                    Is Bound: {be.IsBound}");
                WriteLine($"  Adapter Primary IP Address: {be.AdapterPrimaryIpAddress}");
                WriteLine($"      Adapter Subnet Address: {be.AdapterSubnetAddress}");
            }

            // Failover Relationships
            WriteLine(" Failover Relationships:", ConsoleColor.White);
            foreach (var failoverRelationship in dhcpServer.FailoverRelationships.ToList())
            {
                WriteLine($"   Name: {failoverRelationship.Name}");
                WriteLine($"                            Mode: {failoverRelationship.Mode}");
                WriteLine($"                           State: {failoverRelationship.State}");
                WriteLine($"                     Server Type: {failoverRelationship.ServerType}");
                WriteLine($"                  Primary Server: {failoverRelationship.PrimaryServerName} [{failoverRelationship.PrimaryServerAddress}]");
                WriteLine($"                Secondary Server: {failoverRelationship.SecondaryServerName} [{failoverRelationship.SecondaryServerAddress}]");
                WriteLine($"                   Shared Secret: {failoverRelationship.SharedSecret}");
                WriteLine($"        Maximum Client Lead Time: {failoverRelationship.MaximumClientLeadTime}");
                WriteLine($"       State Switchover Interval: {failoverRelationship.StateSwitchoverInterval}");
                WriteLine($"                  Load Balance %: {failoverRelationship.LoadBalancePercentage}");
                WriteLine($"    Standby Addresses Reserved %: {failoverRelationship.HotStandbyAddressesReservedPercentage}");
                WriteLine($"               Associated Scopes:");
                foreach (var failoverScope in failoverRelationship.Scopes.ToList())
                {
                    WriteLine($"                    {failoverScope}");
                }
            }

            // Classes
            WriteLine(" Classes:", ConsoleColor.White);
            WriteLine($"  Default Vendor Class Name: {dhcpServer.SpecificStrings.DefaultVendorClassName}");
            WriteLine($"    Default User Class Name: {dhcpServer.SpecificStrings.DefaultUserClassName}");

            foreach (var c in dhcpServer.Classes.ToList())
            {
                WriteLine($"  {c.Name}");
                WriteLine($"      Comment: {c.Comment}");
                WriteLine($"         Type: {(c.IsVendorClass ? "Vendor Class" : "User Class")}");
                WriteLine($"         Data: {c.DataText}");

                // Enumerate Class Options
                WriteLine("      Options:");
                foreach (var option in c.Options.ToList())
                {
                    WriteLine($"         {option}");
                }
            }

            // Global Options
            WriteLine(" Global Options:", ConsoleColor.White);
            foreach (var option in dhcpServer.Options.ToList())
            {
                WriteLine($"  {option}");
            }

            // Global Option Values
            WriteLine("   Global Option Values:", ConsoleColor.White);
            foreach (var value in dhcpServer.Options.GetOptionValues().ToList())
            {
                WriteLine($"     {value}");
            }

            // Server Clients
            WriteLine("      Server Clients:", ConsoleColor.White);
            foreach (var client in dhcpServer.Clients.ToList())
            {
                WriteLine($"          {client}");
            }

            // Scopes
            WriteLine(" Scopes:", ConsoleColor.White);
            foreach (var scope in dhcpServer.Scopes.ToList())
            {
                WriteLine($"   {scope.Address}");
                WriteLine($"            IP Range: {scope.IpRange}");
                WriteLine($"                Mask: {scope.Mask}");
                WriteLine($"               State: {scope.State}");
                WriteLine($"                Name: {scope.Name}");
                WriteLine($"             Comment: {scope.Comment}");
                WriteLine($"        Primary Host: {scope.PrimaryHost}");
                WriteLine($"      Lease Duration: {scope.LeaseDuration?.ToString() ?? "Unlimited"}");
                WriteLine($"         Delay Offer: {scope.TimeDelayOffer.TotalMilliseconds} milliseconds");
                WriteLine($"       Quarantine On: {scope.QuarantineOn}");

                // Scope Relationship
                var failoverRelationship = scope.GetFailoverRelationship();
                Write("    Failover Relationship:", ConsoleColor.White);
                if (failoverRelationship == null)
                {
                    WriteLine($" Not in a Failover Relationship");
                }
                else
                {
                    WriteLine($" {failoverRelationship}");
                    WriteLine("      Failover Statistics:", ConsoleColor.White);
                    var failoverStatistics = scope.GetFailoverStatistics();
                    WriteLine($"            Addresses Total: {failoverStatistics.AddressesTotal}");
                    WriteLine($"             Addresses Free: {failoverStatistics.AddressesFree}");
                    WriteLine($"           Addresses In Use: {failoverStatistics.AddressesInUse}");
                    WriteLine($"     Partner Addresses Free: {failoverStatistics.PartnerAddressesFree}");
                    WriteLine($"   Partner Addresses In Use: {failoverStatistics.PartnerAddressesInUse}");
                    WriteLine($"       Local Addresses Free: {failoverStatistics.LocalAddressesFree}");
                    WriteLine($"     Local Addresses In Use: {failoverStatistics.LocalAddressesInUse}");
                }

                // Scope IP Ranges
                WriteLine("      Excluded IP Ranges:", ConsoleColor.White);
                foreach (var ipRange in scope.ExcludedIpRanges)
                {
                    WriteLine($"        {ipRange}");
                }

                // Scope Options
                WriteLine("      Options:", ConsoleColor.White);
                foreach (var value in scope.Options.ToList())
                {
                    WriteLine($"        {value}");
                }

                // Scope Reservations
                WriteLine("      Reservations:", ConsoleColor.White);
                foreach (var reservation in scope.Reservations.ToList())
                {
                    WriteLine($"        {reservation}");
                    if (!reservation.HardwareAddress.IsValid)
                    {
                        WriteLine($"        Invalid Hardware Address ({reservation.HardwareAddress.Length} bytes)", ConsoleColor.Magenta);
                    }
                    WriteLine($"        Client: {reservation.Client}");
                    WriteLine("          Options:");
                    foreach (var value in reservation.Options.ToList())
                    {
                        WriteLine($"            {value}");
                    }
                }

                // Scope Clients
                WriteLine("      Clients:", ConsoleColor.White);
                foreach (var client in scope.Clients.ToList())
                {
                    WriteLine($"        {client}");
                }
            }
        }

        #region Console Helpers
        static ConsoleColor consoleColour = Console.ForegroundColor;
        static void SetColour(ConsoleColor colour)
        {
            if (consoleColour != colour)
            {
                Console.ForegroundColor = colour;
                consoleColour = colour;
            }
        }
        static void Write(string value, ConsoleColor colour = ConsoleColor.Gray)
        {
            SetColour(colour);
            Console.Write(value);
        }
        static void WriteLine() => Console.WriteLine();
        static void WriteLine(string value, ConsoleColor colour = ConsoleColor.Gray)
        {
            SetColour(colour);
            Console.WriteLine(value);
        }
        #endregion
    }
}
