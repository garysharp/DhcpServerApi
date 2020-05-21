namespace Dhcp.Proxy.Protocol.Protobuf
{
    internal enum RequestType : uint
    {
        GetProxyVersion = 0,
        GetRemoteServerNames = 10,

        Connect = 20,

        GetAuditLog = 1000,
    }
}
