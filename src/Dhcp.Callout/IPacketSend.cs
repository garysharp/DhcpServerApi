namespace Dhcp.Callout
{
    /// <summary>
    /// If inherited by <see cref="ICalloutConsumer"/> classes the library registers
    /// the consumer for the <see cref="PacketSend"/> hook (DhcpPktSendHook)
    /// </summary>
    /// <remarks>
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363294.aspx
    /// </remarks>
    public interface IPacketSend
    {

        /// <summary>
        /// The function is called by Microsoft DHCP Server directly before Microsoft DHCP Server sends a response to a client.
        /// Registering for this notification enables third-party developers to alter the response of the Microsoft DHCP Server by manipulation of the packet pointers.
        /// Manipulation of packet pointers is not currently supported by the DhcpServerAPI wrapper.
        /// The function should not block.
        /// </summary>
        /// <remarks>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363294.aspx
        /// </remarks>
        /// <param name="Packet">The packet to be sent by Microsoft DHCP Server.</param>
        /// <param name="ServerAddress">The IP address of the socket on which the packet was received.</param>
        /// <param name="StopPropagation">Set to <see cref="true"/> to prevent propagation of this event to subsequent consumers of this hook.</param>
        void PacketSend(IDhcpServerPacketWritable Packet, DhcpServerIpAddress ServerAddress, ref bool StopPropagation);

    }
}
