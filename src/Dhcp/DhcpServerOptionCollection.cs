using System.Collections;
using System.Collections.Generic;

namespace Dhcp
{
    public class DhcpServerOptionCollection : IDhcpServerOptionCollection
    {
        public DhcpServer Server { get; }
        IDhcpServer IDhcpServerOptionCollection.Server => Server;

        internal DhcpServerOptionCollection(DhcpServer server)
        {
            Server = server;
        }

        public IEnumerator<IDhcpServerOption> GetEnumerator()
            => DhcpServerOption.GetAllOptions(Server).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public IEnumerable<IDhcpServerOption> GetDefaultOptions()
            => DhcpServerOption.EnumDefaultOptions(Server);

        public IEnumerable<IDhcpServerOption> GetUserOptions(string className)
            => DhcpServerOption.EnumUserOptions(Server, className);

        public IEnumerable<IDhcpServerOption> GetVendorOptions(string vendorName)
            => DhcpServerOption.EnumVendorOptions(Server, vendorName);

        /// <summary>
        /// Queries the DHCP Server for the specified OptionId from the Default options
        /// </summary>
        /// <param name="optionId">The identifier for the option to retrieve</param>
        /// <returns>A <see cref="IDhcpServerOption"/>.</returns>
        public IDhcpServerOption GetDefaultOption(int optionId)
            => DhcpServerOption.GetDefaultOption(Server, optionId);
        /// <summary>
        /// Queries the DHCP Server for the specified OptionId from the Default options
        /// </summary>
        /// <param name="optionId">The identifier for the option to retrieve</param>
        /// <returns>A <see cref="IDhcpServerOption"/>.</returns>
        public IDhcpServerOption GetDefaultOption(DhcpServerOptionIds optionId)
            => DhcpServerOption.GetDefaultOption(Server, (int)optionId);

        /// <summary>
        /// Queries the DHCP Server for the specified OptionId within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option to retrieve</param>
        /// <returns>A <see cref="IDhcpServerOption"/>.</returns>
        public IDhcpServerOption GetUserOption(string className, int optionId)
            => DhcpServerOption.GetUserOption(Server, className, optionId);
        /// <summary>
        /// Queries the DHCP Server for the specified OptionId within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option to retrieve</param>
        /// <returns>A <see cref="IDhcpServerOption"/>.</returns>
        public IDhcpServerOption GetUserOption(string className, DhcpServerOptionIds optionId)
            => DhcpServerOption.GetUserOption(Server, className, (int)optionId);

        /// <summary>
        /// Queries the DHCP Server for the specified OptionId within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option to retrieve</param>
        /// <returns>A <see cref="IDhcpServerOption"/>.</returns>
        public IDhcpServerOption GetVendorOption(string vendorName, int optionId)
            => DhcpServerOption.GetVendorOption(Server, vendorName, optionId);
        /// <summary>
        /// Queries the DHCP Server for the specified OptionId within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option to retrieve</param>
        /// <returns>A <see cref="IDhcpServerOption"/>.</returns>
        public IDhcpServerOption GetVendorOption(string vendorName, DhcpServerOptionIds optionId)
            => DhcpServerOption.GetVendorOption(Server, vendorName, (int)optionId);

        /// <summary>
        /// Enumerates a list of all Global Option Values, including vendor/user class options, associated with the DHCP Server
        /// </summary>
        public IEnumerable<IDhcpServerOptionValue> GetOptionValues()
            => DhcpServerOptionValue.GetAllGlobalOptionValues(Server, includeDnsSettingsOption: false);

        /// <summary>
        /// Enumerates a list of all Global Option Values, including vendor/user class options, associated with the DHCP Server
        /// </summary>
        internal IEnumerable<IDhcpServerOptionValue> GetOptionValues(bool includeDnsSettingsOption)
            => DhcpServerOptionValue.GetAllGlobalOptionValues(Server, includeDnsSettingsOption);

        /// <summary>
        /// Enumerates a list of Global Option Values associated with the DHCP Server
        /// </summary>
        public IEnumerable<IDhcpServerOptionValue> GetDefaultOptionValues()
            => DhcpServerOptionValue.EnumGlobalDefaultOptionValues(Server);

        /// <summary>
        /// Queries the DHCP Server for the specified OptionId Value from the Default options
        /// </summary>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="IDhcpServerOptionValue"/>.</returns>
        public IDhcpServerOptionValue GetDefaultOptionValue(int optionId)
            => DhcpServerOptionValue.GetGlobalDefaultOptionValue(Server, optionId);
        /// <summary>
        /// Queries the DHCP Server for the specified OptionId Value from the Default options
        /// </summary>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="IDhcpServerOptionValue"/>.</returns>
        public IDhcpServerOptionValue GetDefaultOptionValue(DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.GetGlobalDefaultOptionValue(Server, (int)optionId);

        /// <summary>
        /// Queries the DHCP Server for the specified OptionId Value within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="IDhcpServerOptionValue"/>.</returns>
        public IDhcpServerOptionValue GetUserOptionValue(string className, int optionId)
            => DhcpServerOptionValue.GetGlobalUserOptionValue(Server, className, optionId);
        /// <summary>
        /// Queries the DHCP Server for the specified OptionId Value within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="IDhcpServerOptionValue"/>.</returns>
        public IDhcpServerOptionValue GetUserOptionValue(string className, DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.GetGlobalUserOptionValue(Server, className, (int)optionId);

        /// <summary>
        /// Queries the DHCP Server for the specified OptionId Value within a Vendor Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="IDhcpServerOptionValue"/>.</returns>
        public IDhcpServerOptionValue GetVendorOptionValue(string vendorName, int optionId)
            => DhcpServerOptionValue.GetGlobalVendorOptionValue(Server, vendorName, optionId);
        /// <summary>
        /// Queries the DHCP Server for the specified OptionId Value within a Vendor Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="IDhcpServerOptionValue"/>.</returns>
        public IDhcpServerOptionValue GetVendorOptionValue(string vendorName, DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.GetGlobalVendorOptionValue(Server, vendorName, (int)optionId);

        public void SetOptionValue(IDhcpServerOptionValue value)
            => DhcpServerOptionValue.SetGlobalOptionValue(Server, (DhcpServerOptionValue)value);
        public void AddOrSetOptionValue(IDhcpServerOptionValue value)
            => DhcpServerOptionValue.SetGlobalOptionValue(Server, (DhcpServerOptionValue)value);

        /// <summary>
        /// Deletes the Global Option Value associated with the Option within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class</param>
        /// <param name="optionId">The identifier for the option value</param>
        public void RemoveUserOptionValue(string className, int optionId)
            => DhcpServerOptionValue.DeleteGlobalUserOptionValue(Server, optionId, className);
        /// <summary>
        /// Retrieves the Global Option Value associated with the Option within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        public void RemoveUserOptionValue(string className, DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.DeleteGlobalUserOptionValue(Server, (int)optionId, className);

        /// <summary>
        /// Retrieves the Global Option Value associated with the Option within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public void RemoveVendorOptionValue(string vendorName, int optionId)
            => DhcpServerOptionValue.DeleteGlobalVendorOptionValue(Server, optionId, vendorName);
        /// <summary>
        /// Retrieves the Global Option Value associated with the Option within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public void RemoveVendorOptionValue(string vendorName, DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.DeleteGlobalVendorOptionValue(Server, (int)optionId, vendorName);
        public void RemoveOptionValue(int optionId)
            => DhcpServerOptionValue.DeleteGlobalOptionValue(Server, optionId);
        public void RemoveOptionValue(DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.DeleteGlobalOptionValue(Server, (int)optionId);
        public void RemoveOptionValue(IDhcpServerOptionValue value)
            => DhcpServerOptionValue.DeleteGlobalOptionValue(Server, (DhcpServerOptionValue)value);
    }
}
