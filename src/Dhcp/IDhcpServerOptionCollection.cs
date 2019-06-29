using System.Collections.Generic;

namespace Dhcp
{
    public interface IDhcpServerOptionCollection : IEnumerable<IDhcpServerOption>
    {
        IDhcpServer Server { get; }

        IDhcpServerOption GetDefaultOption(DhcpServerOptionIds optionId);
        IDhcpServerOption GetDefaultOption(int optionId);
        IEnumerable<IDhcpServerOption> GetDefaultOptions();
        IDhcpServerOptionValue GetDefaultOptionValue(DhcpServerOptionIds optionId);
        IDhcpServerOptionValue GetDefaultOptionValue(int optionId);
        IEnumerable<IDhcpServerOptionValue> GetDefaultOptionValues();
        IEnumerable<IDhcpServerOptionValue> GetOptionValues();
        IDhcpServerOption GetUserOption(string className, DhcpServerOptionIds optionId);
        IDhcpServerOption GetUserOption(string className, int optionId);
        IEnumerable<IDhcpServerOption> GetUserOptions(string className);
        IDhcpServerOptionValue GetUserOptionValue(string className, DhcpServerOptionIds optionId);
        IDhcpServerOptionValue GetUserOptionValue(string className, int optionId);
        IDhcpServerOption GetVendorOption(string vendorName, DhcpServerOptionIds optionId);
        IDhcpServerOption GetVendorOption(string vendorName, int optionId);
        IEnumerable<IDhcpServerOption> GetVendorOptions(string vendorName);
        IDhcpServerOptionValue GetVendorOptionValue(string vendorName, DhcpServerOptionIds optionId);
        IDhcpServerOptionValue GetVendorOptionValue(string vendorName, int optionId);
    }
}
