using System.Collections;
using System.Collections.Generic;

namespace Dhcp
{
    public class DhcpServerOptionCollection : IEnumerable<DhcpServerOption>
    {
        public DhcpServer Server { get; }

        internal DhcpServerOptionCollection(DhcpServer server)
        {
            Server = server;
        }

        public IEnumerator<DhcpServerOption> GetEnumerator()
            => DhcpServerOption.GetAllOptions(Server).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public IEnumerable<DhcpServerOption> GetDefaultOptions()
            => DhcpServerOption.EnumDefaultOptions(Server);

        public IEnumerable<DhcpServerOption> GetUserOptions(string className)
            => DhcpServerOption.EnumUserOptions(Server, className);

        public IEnumerable<DhcpServerOption> GetVendorOptions(string vendorName)
            => DhcpServerOption.EnumVendorOptions(Server, vendorName);

        /// <summary>
        /// Queries the DHCP Server for the specified OptionId from the Default options
        /// </summary>
        /// <param name="optionId">The identifier for the option to retrieve</param>
        /// <returns>A <see cref="DhcpServerOption"/>.</returns>
        public DhcpServerOption GetDefaultOption(int optionId)
            => DhcpServerOption.GetDefaultOption(Server, optionId);

        /// <summary>
        /// Queries the DHCP Server for the specified OptionId within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option to retrieve</param>
        /// <returns>A <see cref="DhcpServerOption"/>.</returns>
        public DhcpServerOption GetUserOption(string className, int optionId)
            => DhcpServerOption.GetUserOption(Server, className, optionId);

        /// <summary>
        /// Queries the DHCP Server for the specified OptionId within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option to retrieve</param>
        /// <returns>A <see cref="DhcpServerOption"/>.</returns>
        public DhcpServerOption GetVendorOption(string vendorName, int optionId)
            => DhcpServerOption.GetVendorOption(Server, vendorName, optionId);

        /// <summary>
        /// Enumerates a list of all Global Option Values, including vendor/user class options, associated with the DHCP Server
        /// </summary>
        public IEnumerable<DhcpServerOptionValue> GetOptionValues()
            => DhcpServerOptionValue.GetAllGlobalOptionValues(Server);

        /// <summary>
        /// Enumerates a list of Global Option Values associated with the DHCP Server
        /// </summary>
        public IEnumerable<DhcpServerOptionValue> GetDefaultOptionValues()
            => DhcpServerOptionValue.EnumGlobalDefaultOptionValues(Server);

        /// <summary>
        /// Queries the DHCP Server for the specified OptionId Value from the Default options
        /// </summary>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetDefaultOptionValue(int optionId)
            => DhcpServerOptionValue.GetGlobalDefaultOptionValue(Server, optionId);

        /// <summary>
        /// Queries the DHCP Server for the specified OptionId Value within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetDefaultOptionValue(string className, int optionId)
            => DhcpServerOptionValue.GetGlobalUserOptionValue(Server, className, optionId);
    }
}
