namespace Dhcp
{
    public interface IDhcpServerScopeFailoverStatistics
    {
        int AddressesFree { get; }
        int AddressesInUse { get; }
        int AddressesTotal { get; }
        int LocalAddressesFree { get; }
        int LocalAddressesInUse { get; }
        int PartnerAddressesFree { get; }
        int PartnerAddressesInUse { get; }
        IDhcpServerScope Scope { get; }
        IDhcpServer Server { get; }
    }
}
