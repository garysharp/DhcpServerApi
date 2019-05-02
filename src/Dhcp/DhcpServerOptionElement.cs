using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Dhcp.Native;

namespace Dhcp
{
    public abstract class DhcpServerOptionElement
    {
        public abstract DhcpServerOptionElementType Type { get; }

        public abstract object Value { get; }
        public abstract string ValueFormatted { get; }

        internal static IEnumerable<DhcpServerOptionElement> ReadNativeElements(DHCP_OPTION_DATA elementArray)
        {
            foreach (var element in elementArray.Elements)
                yield return ReadNative(element);
        }

        private static DhcpServerOptionElement ReadNative(DHCP_OPTION_DATA_ELEMENT element)
        {
            switch (element.OptionType)
            {
                case DHCP_OPTION_DATA_TYPE.DhcpByteOption:
                    return DhcpServerOptionElementByte.ReadNative(element);
                case DHCP_OPTION_DATA_TYPE.DhcpWordOption:
                    return DhcpServerOptionElementWord.ReadNative(element);
                case DHCP_OPTION_DATA_TYPE.DhcpDWordOption:
                    return DhcpServerOptionElementDWord.ReadNative(element);
                case DHCP_OPTION_DATA_TYPE.DhcpDWordDWordOption:
                    return DhcpServerOptionElementDWordDWord.ReadNative(element);
                case DHCP_OPTION_DATA_TYPE.DhcpIpAddressOption:
                    return DhcpServerOptionElementIpAddress.ReadNative(element);
                case DHCP_OPTION_DATA_TYPE.DhcpStringDataOption:
                    return DhcpServerOptionElementString.ReadNative(element);
                case DHCP_OPTION_DATA_TYPE.DhcpBinaryDataOption:
                case DHCP_OPTION_DATA_TYPE.DhcpEncapsulatedDataOption:
                    return DhcpServerOptionElementBinary.ReadNative(element);
                case DHCP_OPTION_DATA_TYPE.DhcpIpv6AddressOption:
                    return DhcpServerOptionElementIpv6Address.ReadNative(element);
                default:
                    throw new InvalidCastException($"Unknown Option Data Type: {element.OptionType}");
            }
        }

        public override string ToString() => $"{Type}: {ValueFormatted}";
    }

    public class DhcpServerOptionElementByte : DhcpServerOptionElement
    {
        public override DhcpServerOptionElementType Type => DhcpServerOptionElementType.Byte;
        public override object Value => RawValue;
        public override string ValueFormatted => RawValue.ToHexString();

        public byte RawValue { get; }

        private DhcpServerOptionElementByte(byte value)
        {
            RawValue = value;
        }

        internal static DhcpServerOptionElementByte ReadNative(DHCP_OPTION_DATA_ELEMENT native)
            => new DhcpServerOptionElementByte(native.ByteOption);
    }

    public class DhcpServerOptionElementWord : DhcpServerOptionElement
    {
        public override DhcpServerOptionElementType Type => DhcpServerOptionElementType.Word;
        public override object Value => RawValue;
        public override string ValueFormatted => RawValue.ToString("N");

        public short RawValue { get; }

        private DhcpServerOptionElementWord(short value)
        {
            RawValue = value;
        }

        internal static DhcpServerOptionElementWord ReadNative(DHCP_OPTION_DATA_ELEMENT native) 
            => new DhcpServerOptionElementWord(native.WordOption);
    }

    public class DhcpServerOptionElementDWord : DhcpServerOptionElement
    {
        public override DhcpServerOptionElementType Type => DhcpServerOptionElementType.DWord;
        public override object Value => RawValue;
        public override string ValueFormatted => RawValue.ToString("N");

        public int RawValue { get; }

        private DhcpServerOptionElementDWord(int value)
        {
            RawValue = value;
        }

        internal static DhcpServerOptionElementDWord ReadNative(DHCP_OPTION_DATA_ELEMENT native)
            => new DhcpServerOptionElementDWord(native.DWordOption);
    }

    public class DhcpServerOptionElementDWordDWord : DhcpServerOptionElement
    {
        public override DhcpServerOptionElementType Type => DhcpServerOptionElementType.DWordDWord;
        public override object Value => RawValue;
        public override string ValueFormatted => RawValue.ToString("N");

        public long RawValue { get; }

        private DhcpServerOptionElementDWordDWord(long value)
        {
            RawValue = value;
        }

        internal static DhcpServerOptionElementDWordDWord ReadNative(DHCP_OPTION_DATA_ELEMENT native)
            => new DhcpServerOptionElementDWordDWord(native.DWordDWordOption);
    }

    public class DhcpServerOptionElementIpAddress : DhcpServerOptionElement
    {
        public override DhcpServerOptionElementType Type => DhcpServerOptionElementType.IpAddress;
        public override object Value => RawValue;
        public override string ValueFormatted => ipAddress;

        private DHCP_IP_ADDRESS ipAddress;

        public uint RawValue => (uint)ipAddress;

        private DhcpServerOptionElementIpAddress(DHCP_IP_ADDRESS value)
        {
            ipAddress = value;
        }

        internal static DhcpServerOptionElementIpAddress ReadNative(DHCP_OPTION_DATA_ELEMENT native)
            => new DhcpServerOptionElementIpAddress(native.IpAddressOption);
    }

    public class DhcpServerOptionElementString : DhcpServerOptionElement
    {
        public override DhcpServerOptionElementType Type => DhcpServerOptionElementType.StringData;
        public override object Value => RawValue;
        public override string ValueFormatted => RawValue;

        public string RawValue { get; }

        private DhcpServerOptionElementString(string value)
        {
            RawValue = value;
        }

        internal static DhcpServerOptionElementString ReadNative(DHCP_OPTION_DATA_ELEMENT native)
            => new DhcpServerOptionElementString(Marshal.PtrToStringUni(native.StringDataOption));
    }

    public class DhcpServerOptionElementBinary : DhcpServerOptionElement
    {
        private readonly DhcpServerOptionElementType type;

        public override DhcpServerOptionElementType Type => type;
        public override object Value => RawValue;
        public override string ValueFormatted { get => RawValue.ToHexString(' '); }

        public byte[] RawValue { get; }

        private DhcpServerOptionElementBinary(DhcpServerOptionElementType type, byte[] value)
        {
            this.type = type;
            RawValue = value;
        }

        internal static DhcpServerOptionElementBinary ReadNative(DHCP_OPTION_DATA_ELEMENT native)
            => new DhcpServerOptionElementBinary((DhcpServerOptionElementType)native.OptionType, native.BinaryDataOption.Data);
    }

    public class DhcpServerOptionElementIpv6Address : DhcpServerOptionElement
    {
        public override DhcpServerOptionElementType Type => DhcpServerOptionElementType.Ipv6Address;
        public override object Value => RawValue;
        public override string ValueFormatted => RawValue;

        public string RawValue { get; }

        private DhcpServerOptionElementIpv6Address(string value)
        {
            RawValue = value;
        }

        internal static DhcpServerOptionElementIpv6Address ReadNative(DHCP_OPTION_DATA_ELEMENT native)
            => new DhcpServerOptionElementIpv6Address(Marshal.PtrToStringUni(native.Ipv6AddressDataOption));
    }
}
