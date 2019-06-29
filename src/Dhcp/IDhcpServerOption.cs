using System.Collections.Generic;

namespace Dhcp
{
    public interface IDhcpServerOption
    {
        IDhcpServerClass Class { get; }
        string ClassName { get; }
        string Comment { get; }
        IEnumerable<IDhcpServerOptionElement> DefaultValue { get; }
        bool IsUnaryOption { get; }
        bool IsUserClassOption { get; }
        bool IsVendorClassOption { get; }
        string Name { get; }
        int OptionId { get; }
        IDhcpServer Server { get; }
        DhcpServerOptionElementType ValueType { get; }
        string VendorName { get; }

        IDhcpServerOptionValue CreateOptionBinaryValue(byte[] value);
        IDhcpServerOptionValue CreateOptionBinaryValue(IEnumerable<byte[]> values);
        IDhcpServerOptionValue CreateOptionBinaryValue(params byte[][] values);
        IDhcpServerOptionValue CreateOptionByteValue(byte value);
        IDhcpServerOptionValue CreateOptionByteValue(IEnumerable<byte> values);
        IDhcpServerOptionValue CreateOptionByteValue(params byte[] values);
        IDhcpServerOptionValue CreateOptionEncapsulatedValue(byte[] value);
        IDhcpServerOptionValue CreateOptionEncapsulatedValue(IEnumerable<byte[]> values);
        IDhcpServerOptionValue CreateOptionEncapsulatedValue(params byte[][] values);
        IDhcpServerOptionValue CreateOptionInt16Value(IEnumerable<short> values);
        IDhcpServerOptionValue CreateOptionInt16Value(params short[] values);
        IDhcpServerOptionValue CreateOptionInt16Value(short value);
        IDhcpServerOptionValue CreateOptionInt32Value(IEnumerable<int> values);
        IDhcpServerOptionValue CreateOptionInt32Value(int value);
        IDhcpServerOptionValue CreateOptionInt32Value(params int[] values);
        IDhcpServerOptionValue CreateOptionInt64Value(IEnumerable<long> values);
        IDhcpServerOptionValue CreateOptionInt64Value(long value);
        IDhcpServerOptionValue CreateOptionInt64Value(params long[] values);
        IDhcpServerOptionValue CreateOptionIpAddressValue(DhcpServerIpAddress value);
        IDhcpServerOptionValue CreateOptionIpAddressValue(IEnumerable<DhcpServerIpAddress> values);
        IDhcpServerOptionValue CreateOptionIpAddressValue(params DhcpServerIpAddress[] values);
        IDhcpServerOptionValue CreateOptionIpv6AddressValue(IEnumerable<string> values);
        IDhcpServerOptionValue CreateOptionIpv6AddressValue(params string[] values);
        IDhcpServerOptionValue CreateOptionIpv6AddressValue(string value);
        IDhcpServerOptionValue CreateOptionStringValue(IEnumerable<string> values);
        IDhcpServerOptionValue CreateOptionStringValue(params string[] values);
        IDhcpServerOptionValue CreateOptionStringValue(string value);
    }
}
