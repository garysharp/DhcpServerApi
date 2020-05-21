namespace Dhcp.Proxy.Transport.NamedPipe
{
    public enum NamedPipeMessageInstruction : byte
    {
        InvokeRequest = 0,
        InvokeResponse = 1,
        DhcpServerException = 12,
        TransportException = 13,
        Exception = 14,
    }
}
