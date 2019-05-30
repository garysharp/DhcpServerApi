namespace Dhcp.Callout
{
    public enum PacketDropControlCodes : int
    {
        /// <summary>
        /// The packet is a duplicate of another received by the DHCP server.
        /// </summary>
        Duplicate = 0x00000001,
        /// <summary>
        /// There is not enough memory available to process the packet.
        /// </summary>
        NoMemory = 0x00000002,
        /// <summary>
        /// An unexpected internal error has occurred.
        /// </summary>
        InternalError = 0x00000003,
        /// <summary>
        /// The packet is too old to process.
        /// </summary>
        Timeout = 0x00000004,
        /// <summary>
        /// The server is not authorized to process this packet.
        /// </summary>
        Unauthorized = 0x00000005,
        /// <summary>
        /// The server is paused.
        /// </summary>
        Paused = 0x00000006,
        /// <summary>
        /// There are no subnets configured, therefore there is no point in processing the packet.
        /// </summary>
        NoSubnets = 0x00000007,
        /// <summary>
        /// The packet is invalid, or it was received on an invalid socket.
        /// </summary>
        Invalid = 0x00000008,
        /// <summary>
        /// The packet was sent to the wrong DHCP Server.
        /// </summary>
        WrongServer = 0x00000009,
        /// <summary>
        /// There is no address to offer.
        /// </summary>
        NoAddress = 0x0000000A,
        /// <summary>
        /// The packet has been processed.
        /// </summary>
        Processed = 0x0000000B,
        /// <summary>
        /// An unknown error has occurred.
        /// </summary>
        GenericError = 0x00000100,
    }
}