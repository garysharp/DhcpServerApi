using System;

namespace Dhcp
{
    [Flags]
    public enum PacketFlags : ushort
    {
        Broadcast = 0x8000
    }
}
