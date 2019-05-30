namespace Dhcp.Callout
{
    /// <summary>
    /// If inherited by <see cref="ICalloutConsumer"/> classes the library registers
    /// the consumer for the <see cref="AddressDelete"/> hook (DhcpAddressOfferHook)
    /// </summary>
    /// <remarks>
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363269.aspx
    /// </remarks>
    public interface IAddressDelete
    {
        /// <summary>
        /// The function is called by Microsoft DHCP Server when one of the events defined by <see cref="AddressDeleteControlCodes"/> Control Codes.
        /// The function should not block.
        /// </summary>
        /// <remarks>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363269.aspx
        /// </remarks>
        /// <param name="Packet">The packet being processed.</param>
        /// <param name="ControlCode">Specifies the reason for the event.</param>
        /// <param name="ServerAddress">The IP address of the socket on which the packet was received.</param>
        /// <param name="LeaseAddress">The IP address relevant to this event. The meaning varies based on the ControlCode.</param>
        /// <param name="StopPropagation">Set to <see cref="true"/> to prevent propagation of this event to subsequent consumers of this hook.</param>
        void AddressDelete(IDhcpServerPacket Packet, AddressDeleteControlCodes ControlCode, DhcpServerIpAddress ServerAddress, DhcpServerIpAddress LeaseAddress, ref bool StopPropagation);
    }
}
