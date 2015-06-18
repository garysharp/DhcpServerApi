using Dhcp.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Dhcp
{
    public abstract class DhcpServerOptionElement
    {
        public abstract DhcpServerOptionElementType Type { get; }

        public abstract object Value { get; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Type, Value);
        }

        internal static IEnumerable<DhcpServerOptionElement> ReadNativeElements(DHCP_OPTION_DATA ElementArray)
        {
            foreach (var element in ElementArray.Elements)
            {
                yield return ReadNative(element);
            }
        }

        private static DhcpServerOptionElement ReadNative(DHCP_OPTION_DATA_ELEMENT Element)
        {
            switch (Element.OptionType)
            {
                case DHCP_OPTION_DATA_TYPE.DhcpByteOption:
                    return DhcpServerOptionElementByte.ReadNative(Element);
                case DHCP_OPTION_DATA_TYPE.DhcpWordOption:
                    return DhcpServerOptionElementWord.ReadNative(Element);
                case DHCP_OPTION_DATA_TYPE.DhcpDWordOption:
                    return DhcpServerOptionElementDWord.ReadNative(Element);
                case DHCP_OPTION_DATA_TYPE.DhcpDWordDWordOption:
                    return DhcpServerOptionElementDWordDWord.ReadNative(Element);
                case DHCP_OPTION_DATA_TYPE.DhcpIpAddressOption:
                    return DhcpServerOptionElementIpAddress.ReadNative(Element);
                case DHCP_OPTION_DATA_TYPE.DhcpStringDataOption:
                    return DhcpServerOptionElementString.ReadNative(Element);
                case DHCP_OPTION_DATA_TYPE.DhcpBinaryDataOption:
                case DHCP_OPTION_DATA_TYPE.DhcpEncapsulatedDataOption:
                    return DhcpServerOptionElementBinary.ReadNative(Element);
                case DHCP_OPTION_DATA_TYPE.DhcpIpv6AddressOption:
                    return DhcpServerOptionElementIpv6Address.ReadNative(Element);
                default:
                    throw new InvalidCastException(string.Format("Unknown Option Data Type: {0}", Element.OptionType));
            }
        }
    }

    public class DhcpServerOptionElementByte : DhcpServerOptionElement
    {
        public override DhcpServerOptionElementType Type { get { return DhcpServerOptionElementType.Byte; } }
        public override object Value { get { return RawValue; } }

        public Byte RawValue { get; private set; }

        private DhcpServerOptionElementByte(Byte Value)
        {
            this.RawValue = Value;
        }

        internal static DhcpServerOptionElementByte ReadNative(DHCP_OPTION_DATA_ELEMENT Native)
        {
            return new DhcpServerOptionElementByte(Native.ByteOption);
        }
    }

    public class DhcpServerOptionElementWord : DhcpServerOptionElement
    {
        public override DhcpServerOptionElementType Type { get { return DhcpServerOptionElementType.Word; } }
        public override object Value { get { return RawValue; } }

        public Int16 RawValue { get; private set; }

        private DhcpServerOptionElementWord(Int16 Value)
        {
            this.RawValue = Value;
        }

        internal static DhcpServerOptionElementWord ReadNative(DHCP_OPTION_DATA_ELEMENT Native)
        {
            return new DhcpServerOptionElementWord(Native.WordOption);
        }
    }

    public class DhcpServerOptionElementDWord : DhcpServerOptionElement
    {
        public override DhcpServerOptionElementType Type { get { return DhcpServerOptionElementType.DWord; } }
        public override object Value { get { return RawValue; } }

        public Int32 RawValue { get; private set; }

        private DhcpServerOptionElementDWord(Int32 Value)
        {
            this.RawValue = Value;
        }

        internal static DhcpServerOptionElementDWord ReadNative(DHCP_OPTION_DATA_ELEMENT Native)
        {
            return new DhcpServerOptionElementDWord(Native.DWordOption);
        }
    }

    public class DhcpServerOptionElementDWordDWord : DhcpServerOptionElement
    {
        public override DhcpServerOptionElementType Type { get { return DhcpServerOptionElementType.DWordDWord; } }
        public override object Value { get { return RawValue; } }

        public Int64 RawValue { get; private set; }

        private DhcpServerOptionElementDWordDWord(Int64 Value)
        {
            this.RawValue = Value;
        }

        internal static DhcpServerOptionElementDWordDWord ReadNative(DHCP_OPTION_DATA_ELEMENT Native)
        {
            return new DhcpServerOptionElementDWordDWord(Native.DWordDWordOption);
        }
    }

    public class DhcpServerOptionElementIpAddress : DhcpServerOptionElement
    {
        public override DhcpServerOptionElementType Type { get { return DhcpServerOptionElementType.IpAddress; } }
        public override object Value { get { return RawValue; } }

        private DHCP_IP_ADDRESS ipAddress;

        public UInt32 RawValue { get { return ipAddress.IpAddress; } }
        public string FormattedValue { get { return ipAddress.ToString(); } }

        private DhcpServerOptionElementIpAddress(DHCP_IP_ADDRESS Value)
        {
            this.ipAddress = Value;
        }

        internal static DhcpServerOptionElementIpAddress ReadNative(DHCP_OPTION_DATA_ELEMENT Native)
        {
            return new DhcpServerOptionElementIpAddress(Native.IpAddressOption);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Type, FormattedValue);
        }
    }

    public class DhcpServerOptionElementString : DhcpServerOptionElement
    {
        public override DhcpServerOptionElementType Type { get { return DhcpServerOptionElementType.StringData; } }
        public override object Value { get { return RawValue; } }

        public string RawValue { get; private set; }

        private DhcpServerOptionElementString(string Value)
        {
            this.RawValue = Value;
        }

        internal static DhcpServerOptionElementString ReadNative(DHCP_OPTION_DATA_ELEMENT Native)
        {
            return new DhcpServerOptionElementString(Marshal.PtrToStringUni(Native.StringDataOption));
        }
    }

    public class DhcpServerOptionElementBinary : DhcpServerOptionElement
    {
        private DhcpServerOptionElementType type;

        public override DhcpServerOptionElementType Type { get { return type; } }
        public override object Value { get { return RawValue; } }

        public byte[] RawValue { get; private set; }

        private DhcpServerOptionElementBinary(DhcpServerOptionElementType Type, byte[] Value)
        {
            this.type = Type;
            this.RawValue = Value;
        }

        internal static DhcpServerOptionElementBinary ReadNative(DHCP_OPTION_DATA_ELEMENT Native)
        {
            return new DhcpServerOptionElementBinary((DhcpServerOptionElementType)Native.OptionType, Native.BinaryDataOption.Data);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(Type).Append(": ");

            foreach (var b in RawValue)
            {
                builder.Append(b.ToString("X2")).Append(' ');
            }

            return builder.ToString();
        }
    }

    public class DhcpServerOptionElementIpv6Address : DhcpServerOptionElement
    {
        public override DhcpServerOptionElementType Type { get { return DhcpServerOptionElementType.Ipv6Address; } }
        public override object Value { get { return RawValue; } }

        public string RawValue { get; private set; }

        private DhcpServerOptionElementIpv6Address(string Value)
        {
            this.RawValue = Value;
        }

        internal static DhcpServerOptionElementIpv6Address ReadNative(DHCP_OPTION_DATA_ELEMENT Native)
        {
            return new DhcpServerOptionElementIpv6Address(Marshal.PtrToStringUni(Native.Ipv6AddressDataOption));
        }
    }
}
