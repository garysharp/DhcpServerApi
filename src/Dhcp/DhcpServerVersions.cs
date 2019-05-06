namespace Dhcp
{
    public enum DhcpServerVersions : long
    {
        WindowsNT310 = 0x3000A,
        WindowsNT350 = 0x30032,
        WindowsNT351 = 0x30033,
        WindowsNT40 = 0x40000,
        Windows2000 = 0x50000,
        Windows2003 = 0x50002,
        Windows2003R2 = 0x50002,
        Windows2008 = 0x60000,
        Windows2008R2 = 0x60001,
        Windows2012 = 0x60002,
        Windows2012R2 = 0x60003,
        /// <summary>
        /// Represents Windows Server 2016 and greater
        /// </summary>
        WindowsServer = 0xA0000,
    }
}
