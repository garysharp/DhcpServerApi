namespace Dhcp.Callout
{
    /// <summary>
    /// If inherited by <see cref="ICalloutConsumer"/> classes the library registers
    /// the consumer for the <see cref="HandleOptions"/> hook (DhcpHandleOptionsHook)
    /// </summary>
    /// <remarks>
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363291.aspx
    /// </remarks>
    public interface IHandleOptions
    {

        /// <summary>
        /// The function enables third-party DLLs to obtain commonly used options from a DHCP packet, avoiding the need to process the entire DHCP packet.
        /// The DhcpHandleOptionsHook function should not block. Will only be called with the API Version is '0' (as the ServerOptions could change if the version is incremented).
        /// </summary>
        /// <remarks>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363291.aspx
        /// </remarks>
        /// <param name="Packet">The packet being processed.</param>
        /// <param name="ServerOptions">The information parsed from the packet by Microsoft DHCP Server, and provided as the collection of commonly used server options.</param>
        /// <param name="StopPropagation">Set to <see cref="true"/> to prevent propagation of this event to subsequent consumers of this hook.</param>
        void HandleOptions(IDhcpServerPacket Packet, DhcpServerPacketOptions ServerOptions, ref bool StopPropagation);

    }
}
