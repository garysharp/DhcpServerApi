using System;

namespace Dhcp.Callout
{
    /// <summary>
    /// If inherited by <see cref="ICalloutConsumer"/> classes the library registers
    /// the consumer for the <see cref="AddressOffer"/> hook (DhcpAddressOfferHook)
    /// </summary>
    /// <remarks>
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363269.aspx
    /// </remarks>
    public interface IAddressOffer
    {
        /// <summary>
        /// The function is called by Microsoft DHCP Server directly before Microsoft DHCP Server
        /// sends an acknowledgment (ACK) to a DHCP REQUEST message. The function should not block.
        /// </summary>
        /// <remarks>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363269.aspx
        /// </remarks>
        /// <param name="Packet">The packet being processed.</param>
        /// <param name="ControlCode">The type of lease being approved</param>
        /// <param name="ServerAddress">The IP address of the socket on which the packet was received.</param>
        /// <param name="LeaseAddress">The IP address being handed out in the lease.</param>
        /// <param name="AddressType">Specifies whether the address is DHCP or BOOTP.</param>
        /// <param name="LeaseTime">Lease duration being passed.</param>
        /// <param name="StopPropagation">Set to <see cref="true"/> to prevent propagation of this event to subsequent consumers of this hook.</param>
        void AddressOffer(IDhcpServerPacket Packet,
            OfferAddressControlCodes ControlCode,
            DhcpServerIpAddress ServerAddress,
            DhcpServerIpAddress LeaseAddress,
            OfferAddressTypes AddressType,
            TimeSpan LeaseTime,
            ref bool StopPropagation);
    }
}
