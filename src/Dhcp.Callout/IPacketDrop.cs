namespace Dhcp.Callout
{
    /// <summary>
    /// If inherited by <see cref="ICalloutConsumer"/> classes the library registers
    /// the consumer for the <see cref="PacketDrop"/> hook (DhcpPktDropHook)
    /// </summary>
    /// <remarks>
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363293.aspx
    /// </remarks>
    public interface IPacketDrop
    {

        /// <summary>
        /// The function is called by Microsoft DHCP Server when a DHCP packet is dropped, or a packet is completely processed.
        /// If a packet is dropped, the function is called twice as it is called once again to note that the packet has been completely processed.
        /// The implementor should be prepared to handle this hook multiple times for a packet.
        /// The function should not block.
        /// </summary>
        /// <remarks>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363293.aspx
        /// </remarks>
        /// <param name="Packet">The dropped packet.</param>
        /// <param name="ControlCode">Control code that specifies the reason for dropping.</param>
        /// <param name="ServerAddress">The IP address of the socket on which the packet was received.</param>
        /// <param name="StopPropagation">Set to <see cref="true"/> to prevent propagation of this event to subsequent consumers of this hook.</param>
        void PacketDrop(IDhcpServerPacket Packet, PacketDropControlCodes ControlCode, DhcpServerIpAddress ServerAddress, ref bool StopPropagation);

    }
}
