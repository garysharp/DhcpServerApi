using System.Collections.Generic;

namespace Dhcp
{
    public interface IDhcpServerClass
    {
        string Comment { get; }
        byte[] Data { get; }
        string DataText { get; }
        IEnumerable<IDhcpServerOptionValue> GlobalOptionValues { get; }
        bool IsUserClass { get; }
        bool IsVendorClass { get; }
        string Name { get; }
        IEnumerable<IDhcpServerOption> Options { get; }
        IDhcpServer Server { get; }
    }
}
