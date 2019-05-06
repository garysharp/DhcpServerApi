namespace Dhcp.Callout
{
    /// <summary>
    /// If inherited by <see cref="ICalloutConsumer"/> classes the library registers
    /// the consumer for the <see cref="DeleteClient"/> hook (DhcpDeleteClientHook)
    /// </summary>
    /// <remarks>
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363279.aspx
    /// </remarks>
    public interface IDeleteClient
    {

        /// <summary>
        /// The function is called by Microsoft DHCP Server directly before a client lease is deleted from the active leases database.
        /// The function should not block.
        /// </summary>
        /// <remarks>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363279.aspx
        /// </remarks>
        /// <param name="LeaseAddress">The IP address of the client lease being deleted.</param>
        /// <param name="LeaseHardwareAddress">The Hardware Address of the client, often referred to as the MAC Address.</param>
        /// <param name="StopPropagation">Set to <see cref="true"/> to prevent propagation of this event to subsequent consumers of this hook.</param>
        void DeleteClient(DhcpServerIpAddress LeaseAddress, DhcpServerHardwareAddress LeaseHardwareAddress, ref bool StopPropagation);

    }
}
