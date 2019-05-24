using System.Collections;
using System.Collections.Generic;

namespace Dhcp
{
    public class DhcpServerScopeReservationOptionValueCollection : IEnumerable<DhcpServerOptionValue>
    {
        public DhcpServer Server { get; }
        public DhcpServerScope Scope { get; }
        public DhcpServerScopeReservation Reservation { get; }

        internal DhcpServerScopeReservationOptionValueCollection(DhcpServerScopeReservation reservation)
        {
            Server = reservation.Scope.Server;
            Scope = reservation.Scope;
            Reservation = reservation;
        }

        /// <summary>
        /// Enumerates a list of All Option Values, including vendor/user class option values, associated with the DHCP Scope Reservation
        /// </summary>
        public IEnumerator<DhcpServerOptionValue> GetEnumerator()
            => DhcpServerOptionValue.GetAllScopeReservationOptionValues(Reservation).GetEnumerator();

        /// <summary>
        /// Enumerates a list of All Option Values, including vendor/user class option values, associated with the DHCP Scope Reservation
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        /// <summary>
        /// Enumerates a list of Default Option Values associated with the DHCP Scope Reservation
        /// </summary>
        public IEnumerable<DhcpServerOptionValue> GetDefaultOptionValues()
            => DhcpServerOptionValue.EnumScopeReservationDefaultOptionValues(Reservation);

        public IEnumerable<DhcpServerOptionValue> GetUserOptionValues(string className)
            => DhcpServerOptionValue.EnumScopeReservationUserOptionValues(Reservation, className);

        public IEnumerable<DhcpServerOptionValue> GetVendorOptionValues(string vendorName)
            => DhcpServerOptionValue.EnumScopeReservationVendorOptionValues(Reservation, vendorName);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Reservation Scope
        /// </summary>
        /// <param name="option">The associated option to retrieve the option value for</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetOptionValue(DhcpServerOption option) => option.GetScopeReservationValue(Reservation);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope Reservation from the Default options
        /// </summary>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetDefaultOptionValue(int optionId)
            => DhcpServerOptionValue.GetScopeReservationDefaultOptionValue(Reservation, optionId);
        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope Reservation from the Default options
        /// </summary>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetDefaultOptionValue(DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.GetScopeReservationDefaultOptionValue(Reservation, (int)optionId);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope Reservation within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetUserOptionValue(string className, int optionId)
            => DhcpServerOptionValue.GetScopeReservationUserOptionValue(Reservation, optionId, className);
        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope Reservation within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetUserOptionValue(string className, DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.GetScopeReservationUserOptionValue(Reservation, (int)optionId, className);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope Reservation within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetVendorOptionValue(string vendorName, int optionId)
            => DhcpServerOptionValue.GetScopeReservationVendorOptionValue(Reservation, optionId, vendorName);
        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope Reservation within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public DhcpServerOptionValue GetVendorOptionValue(string vendorName, DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.GetScopeReservationVendorOptionValue(Reservation, (int)optionId, vendorName);

        public void SetOptionValue(DhcpServerOptionValue value)
            => DhcpServerOptionValue.SetScopeReservationOptionValue(Reservation, value);
        public void AddOrSetOptionValue(DhcpServerOptionValue value)
            => DhcpServerOptionValue.SetScopeReservationOptionValue(Reservation, value);

        /// <summary>
        /// Deletes the Option Value associated with the Option and Scope Reservation within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class</param>
        /// <param name="optionId">The identifier for the option value</param>
        public void RemoveUserOptionValue(string className, int optionId)
            => DhcpServerOptionValue.DeleteScopeReservationUserOptionValue(Reservation, optionId, className);
        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope Reservation within a User Class
        /// </summary>
        /// <param name="className">The name of the User Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public void RemoveUserOptionValue(string className, DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.DeleteScopeReservationUserOptionValue(Reservation, (int)optionId, className);

        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope Reservation within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public void RemoveVendorOptionValue(string vendorName, int optionId)
            => DhcpServerOptionValue.DeleteScopeReservationVendorOptionValue(Reservation, optionId, vendorName);
        /// <summary>
        /// Retrieves the Option Value associated with the Option and Scope Reservation within a Vendor Class
        /// </summary>
        /// <param name="vendorName">The name of the Vendor Class to retrieve the Option from</param>
        /// <param name="optionId">The identifier for the option value to retrieve</param>
        /// <returns>A <see cref="DhcpServerOptionValue"/>.</returns>
        public void RemoveVendorOptionValue(string vendorName, DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.DeleteScopeReservationVendorOptionValue(Reservation, (int)optionId, vendorName);
        public void RemoveOptionValue(int optionId)
            => DhcpServerOptionValue.DeleteScopeReservationOptionValue(Reservation, optionId);
        public void RemoveOptionValue(DhcpServerOptionIds optionId)
            => DhcpServerOptionValue.DeleteScopeReservationOptionValue(Reservation, (int)optionId);
        public void RemoveOptionValue(DhcpServerOptionValue value)
            => DhcpServerOptionValue.DeleteScopeReservationOptionValue(Reservation, value);

    }
}
