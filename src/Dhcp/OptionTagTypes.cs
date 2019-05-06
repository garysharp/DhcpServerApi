namespace Dhcp
{
    public enum OptionTagTypes : byte
    {
        Hex = 0,
        Pad,
        End,
        IpAddress,
        IpAddressList,
        Byte,
        Int16,
        UInt16,
        Int32,
        UInt32,
        AsciiString,
        Utf8String,
        IpAddressAndSubnet,
        IpAddressAndIpAddress,
        UInt16List,
        DhcpMessageType,
        DhcpParameterRequestList,
        ZeroLengthFlag,
        ClientFQDN,
        DnsName,
        DnsNameList,
        ClientUUID,
        SipServers,
        StatusCode,
        DhcpState
    }
}
