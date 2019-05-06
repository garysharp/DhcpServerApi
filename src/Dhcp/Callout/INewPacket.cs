namespace Dhcp.Callout
{
    /// <summary>
    /// If inherited by <see cref="ICalloutConsumer"/> classes the library registers
    /// the consumer for the <see cref="NewPacket"/> hook (DhcpNewPktHook)
    /// </summary>
    /// <remarks>
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363292.aspx
    /// </remarks>
    public interface INewPacket
    {

        /// <summary>
        /// The NewPacket (DhcpNewPktHook) function is called soon after the DHCP Server receives a
        /// packet that it attempts to process. This function is in the
        /// critical path of server execution and should return very fast, as
        /// otherwise server performance will be impacted.
        /// </summary>
        /// <remarks>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363292.aspx
        /// </remarks>
        /// <param name="Packet">The packet being processed.</param>
        /// <param name="ServerAddress">The IP address of the socket on which the packet was received.</param>
        /// <param name="ProcessIt">Flag identifying whether Microsoft DHCP Server should continue processing the packet. Set to <see cref="true"/> to indicate processing should proceed. Set to <see cref="false"/> to have Microsoft DHCP Server drop the packet.</param>
        /// <param name="StopPropagation">Set to <see cref="true"/> to prevent propagation of this event to subsequent consumers of this hook. If the ProcessIt argument is set to false then propagation stops regardless.</param>
        void NewPacket(IDhcpServerPacketWritable Packet, DhcpServerIpAddress ServerAddress, ref bool ProcessIt, ref bool StopPropagation);

    }
}
