DHCP Server API for .NET
====================


A .NET (C#, Visual Basic.NET, etc) wrapper for the native DHCP management APIs exposed by Windows.

> #### [on NuGet](https://nuget.org/packages/DhcpServerApi/)
> ```PowerShell
> Install-Package DhcpServerApi
> ```

An initial focus has been made for read-only access to the data via the APIs, but the project can be easily extended to support read-write access.

----------


USAGE
-----

#### Connect to a DHCP Server

Method 1: Discovering via directory services

```C#
// Discover DHCP Servers
foreach (var dhcpServer in DhcpServer.Servers)
{
    Console.WriteLine(dhcpServer.Name);
}
```

Method 2: Connecting directly

```C#
// Connect to DHCP Server
var dhcpServer = DhcpServer.Connect("SRV-DHCP-01"); // or 192.168.1.1

Console.WriteLine(dhcpServer.Name);
```

#### Read Basic DHCP Configuration

```C#
// Connect to DHCP Server
var dhcpServer = DhcpServer.Servers.First();

// Display some configuration
Console.WriteLine($"Protocol Support: {dhcpServer.Configuration.ApiProtocolSupport}");
Console.WriteLine($"Database Name: {dhcpServer.Configuration.DatabaseName}");
Console.WriteLine($"Database Path: {dhcpServer.Configuration.DatabasePath}");

// Show all bound interfaces
foreach (var binding in dhcpServer.BindingElements)
{
    Console.WriteLine($"Binding Interface Id: {binding.InterfaceGuidId}");
    Console.WriteLine($"  Description: {binding.InterfaceDescription}");
    Console.WriteLine($"  Adapter Address: {binding.AdapterPrimaryIpAddress}");
    Console.WriteLine($"  Adapter Subnet: {binding.AdapterSubnetAddress}");
}
```

#### Show DHCP Scopes

```C#
// Connect to DHCP Server
var dhcpServer = DhcpServer.Servers.First();

// Display scope information
foreach (var scope in dhcpServer.Scopes)
{
    Console.WriteLine($"Scope: {scope.Name}");
    Console.WriteLine($"  Address: {scope.Address}");
    Console.WriteLine($"  Mask: {scope.Mask}");
    Console.WriteLine($"  Range: {scope.IpRange}");
    Console.WriteLine($"  State: {scope.State}");
}
```

#### Show Client Leases

Client leases can be retrieved globally (all leases on the server) or individually for each scope.

```C#
// Connect to DHCP Server
var dhcpServer = DhcpServer.Servers.Skip(1).First();

// Get a scope
var scope = dhcpServer.Scopes.First();

Console.WriteLine($"Scope '{scope.Name}' Clients");
Console.WriteLine();

// Get active client leases
var activeClients = scope.Clients
    .Where(c => c.AddressState == DhcpServerClientAddressStates.Active);

// Display client information
foreach (var client in activeClients)
{
    Console.WriteLine($"{client.IpAddress} [{client.HardwareAddress}] {client.Name}, Expires: {client.LeaseExpires}");
}
```

#### Show Reservations

```C#
// Connect to DHCP Server
var dhcpServer = DhcpServer.Servers.First();

// Get a scope
var scope = dhcpServer.Scopes.First();

Console.WriteLine($"Scope '{scope.Name}' Reservations");
Console.WriteLine();

// Display reservation information
foreach (var reservation in scope.Reservations)
{
    Console.WriteLine($"{reservation.IpAddress} [{reservation.HardwareAddress}] {reservation.Client.Name}");
}
```

#### Show Scope Options

Options can be retrieved globally (all options on the server), for scopes or individual reservations.

```C#
// Connect to DHCP Server
var dhcpServer = DhcpServer.Servers.Skip(1).First();

// Get a scope
var scope = dhcpServer.Scopes.First();

Console.WriteLine($"Scope '{scope.Name}' Options");
Console.WriteLine();

// Get option values
foreach (var optionValue in scope.OptionValues)
{
    Console.WriteLine($"{optionValue.Option.Name} [{optionValue.OptionId}]:");

    foreach (var value in optionValue.Values)
    {
        Console.WriteLine($"  {value.Value} [{value.Type}]");
    }
}
```
