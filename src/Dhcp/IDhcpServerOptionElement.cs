using System;

namespace Dhcp
{
    public interface IDhcpServerOptionElement : IEquatable<IDhcpServerOptionElement>
    {
        DhcpServerOptionElementType Type { get; }
        object Value { get; }
        string ValueFormatted { get; }
    }
}
