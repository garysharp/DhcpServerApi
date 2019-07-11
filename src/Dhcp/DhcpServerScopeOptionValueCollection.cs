using System.Collections;
using System.Collections.Generic;

namespace Dhcp
{
    public class DhcpServerScopeOptionValueCollection : IDhcpServerScopeOptionValueCollection
    {
        public DhcpServer Server { get; }
        IDhcpServer IDhcpServerScopeOptionValueCollection.Server => Server;
        public DhcpServerScope Scope { get; }
        IDhcpServerScope IDhcpServerScopeOptionValueCollection.Scope => Scope;

        internal DhcpServerScopeOptionValueCollection(DhcpServerScope scope)
        {
            Server = scope.Server;
            Scope = scope;
        }

        /// <summary>
        /// Enumerates a list of All Option Values, including vendor/user class option values, associated with the DHCP Scope
        /// </summary>
        public IEnumerator<IDhcpServerOptionValue> GetEnumerator()
            => DhcpServerOptionValue.GetAllScopeOptionValues(Scope).GetEnumerator();

        /// <summary>
        /// Enumerates a list of All Option Values, including vendor/user class option values, associated with the DHCP Scope
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        /// <summary>
        /// Enumerates a list of Default Option Values associated with the DHCP Scope
        /// </summary>
        public IEnumerable<IDhcpServerOptionValue> GetDefaultOptionValues()
            => DhcpServerOptionValue.EnumScopeDefaultOptionValues(Scope);

        public IEnumerable<IDhcpServerOptionValue> GetUserOptionValues(string className)
            => DhcpServerOptionValue.EnumScopeUserOptionValues(Scope, className);

        public IEnumerable<IDhcpServerOptionValue> GetVendorOptionValues(string vendorName)
            => DhcpServerOptionValue.EnumScopeVendorOptionValues(Scope, vendorName);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope
        /// </summary>
        /// <param name="option">The associated option to retrieve the option value for</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public IDhcpServerOptionValue GetOptionValue(IDhcpServerOption option) => ((DhcpServerOption)option).GetScopeValue(Scope);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope from the Default options
        /// </summary>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public IDhcpServerOptionValue GetDefaultOptionValue(int optionId)
            => DhcpServerOptionValue.GetScopeDefaultOptionValue(Scope, optionId);
        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope from the Default options
        /// </summary>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public IDhcpServerOptionValue GetDefaultOptionValue(DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.GetScopeDefaultOptionValue(Scope, (int)optionId);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public IDhcpServerOptionValue GetUserOptionValue(string className, int optionId)
            => DhcpServerOptionValue.GetScopeUserOptionValue(Scope, optionId, className);
        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public IDhcpServerOptionValue GetUserOptionValue(string className, DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.GetScopeUserOptionValue(Scope, (int)optionId, className);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public IDhcpServerOptionValue GetVendorOptionValue(string vendorName, int optionId)
            => DhcpServerOptionValue.GetScopeVendorOptionValue(Scope, optionId, vendorName);
        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public IDhcpServerOptionValue GetVendorOptionValue(string vendorName, DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.GetScopeVendorOptionValue(Scope, (int)optionId, vendorName);

        public void SetOptionValue(IDhcpServerOptionValue value)
            => DhcpServerOptionValue.SetScopeOptionValue(Scope, (DhcpServerOptionValue)value);
        public void AddOrSetOptionValue(IDhcpServerOptionValue value)
            => DhcpServerOptionValue.SetScopeOptionValue(Scope, (DhcpServerOptionValue)value);

        /// <summary>
        /// Deletes the Option Value associated with the Option and Scope within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class</param>
        /// <param name="optionId">The identifier for the option value</param>
        public void RemoveUserOptionValue(string className, int optionId)
            => DhcpServerOptionValue.DeleteScopeUserOptionValue(Scope, optionId, className);
        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public void RemoveUserOptionValue(string className, DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.DeleteScopeUserOptionValue(Scope, (int)optionId, className);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public void RemoveVendorOptionValue(string vendorName, int optionId)
            => DhcpServerOptionValue.DeleteScopeVendorOptionValue(Scope, optionId, vendorName);
        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public void RemoveVendorOptionValue(string vendorName, DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.DeleteScopeVendorOptionValue(Scope, (int)optionId, vendorName);
        public void RemoveOptionValue(int optionId)
            => DhcpServerOptionValue.DeleteScopeOptionValue(Scope, optionId);
        public void RemoveOptionValue(DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.DeleteScopeOptionValue(Scope, (int)optionId);
        public void RemoveOptionValue(IDhcpServerOptionValue value)
            => DhcpServerOptionValue.DeleteScopeOptionValue(Scope, (DhcpServerOptionValue)value);

    }
}
