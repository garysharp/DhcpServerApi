using System.Collections.Generic;

namespace Dhcp
{
    public interface IDhcpServerScopeReservationOptionValueCollection : IEnumerable<IDhcpServerOptionValue>
    {
        IDhcpServerScopeReservation Reservation { get; }
        IDhcpServerScope Scope { get; }
        IDhcpServer Server { get; }

        void AddOrSetOptionValue(IDhcpServerOptionValue value);
        IDhcpServerOptionValue GetDefaultOptionValue(DhcpServerOptionIds optionId);
        IDhcpServerOptionValue GetDefaultOptionValue(int optionId);
        IEnumerable<IDhcpServerOptionValue> GetDefaultOptionValues();
        IDhcpServerOptionValue GetOptionValue(IDhcpServerOption option);
        IDhcpServerOptionValue GetUserOptionValue(string className, DhcpServerOptionIds optionId);
        IDhcpServerOptionValue GetUserOptionValue(string className, int optionId);
        IEnumerable<IDhcpServerOptionValue> GetUserOptionValues(string className);
        IDhcpServerOptionValue GetVendorOptionValue(string vendorName, DhcpServerOptionIds optionId);
        IDhcpServerOptionValue GetVendorOptionValue(string vendorName, int optionId);
        IEnumerable<IDhcpServerOptionValue> GetVendorOptionValues(string vendorName);
        void RemoveOptionValue(DhcpServerOptionIds optionId);
        void RemoveOptionValue(IDhcpServerOptionValue value);
        void RemoveOptionValue(int optionId);
        void RemoveUserOptionValue(string className, DhcpServerOptionIds optionId);
        void RemoveUserOptionValue(string className, int optionId);
        void RemoveVendorOptionValue(string vendorName, DhcpServerOptionIds optionId);
        void RemoveVendorOptionValue(string vendorName, int optionId);
        void SetOptionValue(IDhcpServerOptionValue value);
    }
}
