using Dhcp;
using System;
using System.Linq;

namespace DhcpWritableDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Directly Connect to DHCP Server
            var server = DhcpServer.Connect("localhost");

            CreateScope(server);
        }

        static void CreateScope(DhcpServer dhcpServer)
        {
            var name = "Test Scope";
            var description = "Test Scope Description";

            var ipRange = DhcpServerIpRange.FromAddressesDhcpScope("192.168.128.1", "192.168.128.254");

            var mask = DhcpServerIpMask.FromSignificantBits(24); // 255.255.255.0
                                                                 // or:
                                                                 // var mask = new DhcpServerIpMask("255.255.255.0");
                                                                 // var mask = (DhcpServerIpMask)"255.255.255.0";

            var excludedRanges = new DhcpServerIpRange[]
            {
                DhcpServerIpRange.FromAddressesExcluded("192.168.128.1", "192.168.128.10"),
                DhcpServerIpRange.FromAddressesExcluded("192.168.128.240", "192.168.128.254"),
            };

            var dhcpScope = dhcpServer.Scopes.CreateScope(name: name,
                                description: description,
                                ipRange: ipRange,
                                mask: mask,
                                excludedRanges: excludedRanges,
                                timeDelayOffer: DhcpServerScope.DefaultTimeDelayOffer,
                                leaseDuration: DhcpServerScope.DefaultLeaseDuration,
                                enable: true);

            DumpScope(dhcpScope);
        }

        static void DumpScope(DhcpServerScope scope)
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
            foreach (var value in scope.AllOptionValues.ToList())
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
