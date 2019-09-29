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

        static void CreateScope(IDhcpServer dhcpServer)
        {
            // gather name and range information
            var name = "Test Scope";

            var ipRange = DhcpServerIpRange.AsDhcpScope("192.168.128.0/24"); // use CIDR notation
            // "DhcpScope" automatically removes subnet and broadcast address: 192.168.128.1-192.168.128.254
            // or:
            // var ipRange = DhcpServerIpRange.AsDhcpScope("192.168.128.1", "192.168.128.254");
            // var ipRange = DhcpServerIpRange.AsDhcpScope("192.168.128.0", (DhcpServerIpMask)"255.255.255.0");

            // create scope (mask can be explicitly set if required)
            var dhcpScope = dhcpServer.Scopes.AddScope(name, ipRange);

            // specify excluded IP ranges
            dhcpScope.ExcludedIpRanges.AddExcludedIpRange(startAddress: "192.168.128.1", endAddress: "192.168.128.20");
            dhcpScope.ExcludedIpRanges.AddExcludedIpRange(startAddress: "192.168.128.240", endAddress: "192.168.128.254");

            // fetch the default gateway/router option
            var option3 = dhcpServer.Options.GetDefaultOption(DhcpServerOptionIds.Router);
            // prepare an option value
            var option3Value = option3.CreateOptionIpAddressValue("192.168.128.0");
            // add the option value to the scope
            dhcpScope.Options.AddOrSetOptionValue(option3Value);

            // fetch option 15 (DNS Domain Name) from the server and set a scope-wide value
            var option15 = dhcpServer.Options.GetDefaultOption(DhcpServerOptionIds.DomainName);
            var option15Value = option15.CreateOptionStringValue("mydomain.biz.local");
            dhcpScope.Options.AddOrSetOptionValue(option15Value);

            // fetch option 6 (DNS Name Server) from the server and set a scope-wide value
            var option6 = dhcpServer.Options.GetDefaultOption(DhcpServerOptionIds.DomainNameServer);
            var option6Value = option6.CreateOptionIpAddressValue("192.168.128.10", "192.168.128.11");
            dhcpScope.Options.AddOrSetOptionValue(option6Value);

            // fetch option 4 (Time Server) from the server and set a scope-wide value
            var option4 = dhcpServer.Options.GetDefaultOption(DhcpServerOptionIds.TimeServer);
            var option4Value = option4.CreateOptionIpAddressValue("192.168.128.10");
            dhcpScope.Options.AddOrSetOptionValue(option4Value);

            // activate the scope
            dhcpScope.Activate();

            // write out scope information
            DumpScope(dhcpScope);

            // modify the scope
            dhcpScope.Comment = "A test scope description";

            // remove one of the excluded IP ranges
            dhcpScope.ExcludedIpRanges.RemoveExcludedIpRange(DhcpServerIpRange.AsExcluded("192.168.128.240", "192.168.128.254"));

            // remove the Time Server value
            dhcpScope.Options.RemoveOptionValue(DhcpServerOptionIds.TimeServer);

            // update the router option
            option3Value = option3.CreateOptionIpAddressValue("192.168.128.1");
            dhcpScope.Options.SetOptionValue(option3Value);

            // add a client
            var client = dhcpScope.Clients.AddClient("192.168.128.5", "AABBCCDDEEFF", "MyWorkstation.mydomain.biz.local", "My Workstation Lease");
            Console.WriteLine($"Client: {client}");

            // convert client to reservation
            var clientReservation = client.ConvertToReservation();
            Console.WriteLine($"Client Reservation: {clientReservation}");

            // change reservation hardware address (this effectively changes the Client hardware address)
            clientReservation.HardwareAddress = "ABBCCDDEEFFA";

            // set different dns name server option for reservation
            var reservationOption6Value = option6.CreateOptionIpAddressValue("192.168.128.11", "192.168.128.12");
            clientReservation.Options.AddOrSetOptionValue(reservationOption6Value);

            // add a reservation
            var reservation = dhcpScope.Reservations.AddReservation("192.168.128.10", "AA:00:CC:dd:EE:FF");
            Console.WriteLine($"Place-holder Reservation: {reservation}");
            reservation.Client.Name = "YourWorkstation.mydomain.biz.local";
            reservation.Client.Comment = "Your Workstation Lease";

            // set different dns domain name option for reservation
            var reservationOption15Value = option15.CreateOptionStringValue("youdomain.biz.local");
            reservation.Options.AddOrSetOptionValue(reservationOption15Value);

            // write out scope information
            DumpScope(dhcpScope);

            // remove dns domain name reservation option
            reservation.Options.RemoveOptionValue(DhcpServerOptionIds.DomainName);

            // deactivate scope
            dhcpScope.Deactivate();

            // write out scope information
            DumpScope(dhcpScope);

            // delete the scope
            dhcpScope.Delete();
        }

        static void ConfigureFailover(DhcpServer dhcpServer, DhcpServerScope scope)
        {
            // Sample snippets only

            // creating a failover relationship
            //  - matching scopes are created on the partner server
            //      including all settings supported by this library

            // create with an existing relationship
            var existingRelationship = dhcpServer.FailoverRelationships.GetRelationship("DHCPServer1-DHCPServer2");
            scope.ConfigureFailover(existingRelationship);

            // create with a new relationship (name and settings are default)
            var partnerServer = DhcpServer.Connect("MyPartnerDhcpServer");
            var relationship = scope.ConfigureFailover(partnerServer, "A Shared Secret", DhcpServerFailoverMode.HotStandby);
            
            // create with a new relationship (define a name)
            var namedRelationship = scope.ConfigureFailover(partnerServer: partnerServer,
                                                            name: "Relationship Name",
                                                            sharedSecret: "A Shared Secret",
                                                            mode: DhcpServerFailoverMode.HotStandby);

            // create with a new relationship (define settings)
            var customizedRelationship = scope.ConfigureFailover(partnerServer: partnerServer,
                                                                 name: "Relationship Name",
                                                                 sharedSecret: "A Shared Secret",
                                                                 mode: DhcpServerFailoverMode.HotStandby,
                                                                 modePercentage: 10,
                                                                 maximumClientLeadTime: TimeSpan.FromHours(2),
                                                                 stateSwitchInterval: TimeSpan.FromMinutes(10));

            // create load-balance failover
            var loadBalancedRelationship = scope.ConfigureFailover(partnerServer, "A Shared Secret", DhcpServerFailoverMode.LoadBalance);

            // retrieve scope failover relationship
            var scopeRelationship = scope.FailoverRelationship;

            // replicate failover
            //  - all supported settings are pushed to the partner server
            scope.ReplicateFailoverPartner();

            // replicate all scopes in a failover relationship
            relationship.ReplicateRelationship();

            // deconfigure failover
            //  - scopes are removed from the partner server
            //      (whichever isn't being used to deconfigure the failover)
            scope.DeconfigureFailover();

            // enumerate scopes associated with a relationship
            foreach (var relationshipScope in relationship.Scopes)
                Console.WriteLine(relationshipScope);
            
            // delete failover relationship
            //  (all associated scopes must be deconfigured first)
            relationship.Delete();
        }

        static void DumpScope(IDhcpServerScope scope)
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
                foreach (var value in reservation.Options.ToList())
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
