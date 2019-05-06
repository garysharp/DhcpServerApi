namespace Dhcp
{
    public interface IDhcpServerPacketRawWritable : IDhcpServerPacketWritable, IDhcpServerPacketRaw
    {

        /// <summary>
        /// Attempts to increase or decrease the packet buffer at the <paramref name="index"/> by the <paramref name="adjustmentAmount"/>.
        /// </summary>
        /// <param name="index">The buffer index where the adjustment should be made.</param>
        /// <param name="adjustmentAmount">The amount to adjust the buffer by. Positive numbers increase the buffer, negative numbers shrink the buffer.</param>
        /// <returns>true if the adjustment was successful</returns>
        bool TryAdjustBuffer(int index, int adjustmentAmount);

    }
}
