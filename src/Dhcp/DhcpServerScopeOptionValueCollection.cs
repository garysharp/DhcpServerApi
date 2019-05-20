using System.Collections;
using System.Collections.Generic;

namespace Dhcp
{
    public class DhcpServerScopeOptionValueCollection : IEnumerable<DhcpServerOptionValue>
    {
        public DhcpServer Server { get; }
        public DhcpServerScope Scope { get; }

        internal DhcpServerScopeOptionValueCollection(DhcpServerScope scope)
        {
            Server = scope.Server;
            Scope = scope;
        }

        /// <summary>
        /// Enumerates a list of All Option Values, including vendor/user class option values, associated with the DHCP Scope
        /// </summary>
        public IEnumerator<DhcpServerOptionValue> GetEnumerator()
            => DhcpServerOptionValue.GetAllScopeOptionValues(Scope).GetEnumerator();

        /// <summary>
        /// Enumerates a list of All Option Values, including vendor/user class option values, associated with the DHCP Scope
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        /// <summary>
        /// Enumerates a list of Default Option Values associated with the DHCP Scope
        /// </summary>
        public IEnumerable<DhcpServerOptionValue> GetDefaultOptionValues()
            => DhcpServerOptionValue.EnumScopeDefaultOptionValues(Scope);

        public IEnumerable<DhcpServerOptionValue> GetUserOptionValues(string className)
            => DhcpServerOptionValue.EnumScopeUserOptionValues(Scope, className);

        public IEnumerable<DhcpServerOptionValue> GetVendorOptionValues(string vendorName)
            => DhcpServerOptionValue.EnumScopeVendorOptionValues(Scope, vendorName);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope
        /// </summary>
        /// <param name="option">The associated option to retrieve the option value for</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetOptionValue(DhcpServerOption option) => option.GetScopeValue(Scope);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope from the Default options
        /// </summary>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetDefaultOptionValue(int optionId)
            => DhcpServerOptionValue.GetScopeDefaultOptionValue(Scope, optionId);
        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope from the Default options
        /// </summary>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetDefaultOptionValue(DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.GetScopeDefaultOptionValue(Scope, (int)optionId);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetUserOptionValue(string className, int optionId)
            => DhcpServerOptionValue.GetScopeUserOptionValue(Scope, optionId, className);
        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetUserOptionValue(string className, DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.GetScopeUserOptionValue(Scope, (int)optionId, className);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetVendorOptionValue(string vendorName, int optionId)
            => DhcpServerOptionValue.GetScopeVendorOptionValue(Scope, optionId, vendorName);
        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetVendorOptionValue(string vendorName, DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.GetScopeVendorOptionValue(Scope, (int)optionId, vendorName);

        public void SetOptionValue(DhcpServerOptionValue value)
            => DhcpServerOptionValue.SetScopeOptionValue(Scope, value);
        public void AddOrSetOptionValue(DhcpServerOptionValue value)
            => DhcpServerOptionValue.SetScopeOptionValue(Scope, value);

        /// <summary>
        /// Deletes the Option Value associated with the Option and Scope within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class</param>
        /// <param name="optionId">The identifier for the option value</param>
        public void DeleteUserOptionValue(string className, int optionId)
            => DhcpServerOptionValue.DeleteScopeUserOptionValue(Scope, optionId, className);
        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public void DeleteUserOptionValue(string className, DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.GetScopeUserOptionValue(Scope, (int)optionId, className);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public void DeleteVendorOptionValue(string vendorName, int optionId)
            => DhcpServerOptionValue.GetScopeVendorOptionValue(Scope, optionId, vendorName);
        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public void DeleteVendorOptionValue(string vendorName, DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.GetScopeVendorOptionValue(Scope, (int)optionId, vendorName);
        public void DeleteOptionValue(int optionId)
            => DhcpServerOptionValue.DeleteScopeOptionValue(Scope, optionId);
        public void DeleteOptionValue(DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.DeleteScopeOptionValue(Scope, (int)optionId);
        public void DeleteOptionValue(DhcpServerOptionValue value)
            => DhcpServerOptionValue.DeleteScopeOptionValue(Scope, value);

    }
}
