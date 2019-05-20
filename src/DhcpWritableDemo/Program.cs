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
            // gather name and range information
            var name = "Test Scope";
            var description = "Test Scope Description";

            var ipRange = DhcpServerIpRange.AsDhcpScope("192.168.128.0/24"); // use CIDR notation
            // "DhcpScope" automatically removes subnet and broadcast address: 192.168.128.1-192.168.128.254
            // or:
            // var ipRange = DhcpServerIpRange.AsDhcpScope("192.168.128.1", "192.168.128.254");
            // var ipRange = DhcpServerIpRange.AsDhcpScope("192.168.128.0", (DhcpServerIpMask)"255.255.255.0");

            // create scope (mask can be explicity set if required)
            var dhcpScope = dhcpServer.Scopes.CreateScope(
                name: name,
                description: description,
                ipRange: ipRange);

            // specify excluded ip ranges
            dhcpScope.ExcludedIpRanges.AddExcludedIpRange(startAddress: "192.168.128.1", endAddress: "192.168.128.10");
            dhcpScope.ExcludedIpRanges.AddExcludedIpRange(startAddress: "192.168.128.240", endAddress: "192.168.128.254");

            // fetch the default gateway/router option
            var option3 = dhcpServer.Options.GetDefaultOption(DhcpServerOptionIds.Router);
            // prepare an option value
            var option3Value = option3.CreateOptionIpAddressValue("192.168.128.0");
            // add the option value to the scope
            dhcpScope.Options.AddOrSetOptionValue(option3Value);

            // fetch option 15 (DNS Domain Name) from the server
            var option15 = dhcpServer.Options.GetDefaultOption(DhcpServerOptionIds.DomainName);
            var option15Value = option15.CreateOptionStringValue("mydomain.biz.local");
            dhcpScope.Options.AddOrSetOptionValue(option15Value);

            // activate the scope
            dhcpScope.Activate();

            // write out scope information
            DumpScope(dhcpScope);

            // modify the scope

            // remove one of the excluded ip ranges
            dhcpScope.ExcludedIpRanges.DeleteExcludedIpRange(DhcpServerIpRange.AsExcluded("192.168.128.240", "192.168.128.254"));
            
            // remove the DNS Domain Name value
            dhcpScope.Options.DeleteOptionValue(DhcpServerOptionIds.DomainName);
            
            // update the router option
            option3Value = option3.CreateOptionIpAddressValue("192.168.128.1");
            dhcpScope.Options.SetOptionValue(option3Value);
            
            // deactivate scope
            dhcpScope.Deactivate();

            // write out scope information
            DumpScope(dhcpScope);

            // delete the scope
            dhcpScope.Delete();
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
