using System;

namespace Dhcp
{
    [Flags]
    public enum DhcpServerPacketFlags : ushort
    {
        Broadcast = 0x8000
    }
}
