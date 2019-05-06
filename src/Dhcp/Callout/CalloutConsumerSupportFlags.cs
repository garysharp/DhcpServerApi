namespace Dhcp.Callout
{
    internal enum CalloutConsumerSupportFlags
    {
        Control = 1,
        NewPacket = 2,
        PacketDrop = 4,
        PacketSend = 16,
        AddressDelete = 32,
        AddressOffer = 64,
        HandleOptions = 128,
        DeleteClient = 256,
        
        // https://msdn.microsoft.com/en-us/library/windows/desktop/aa363343.aspx
        // Reserved for future use:
        Extension = 512,
        Reserved = 1024,
    }
}
