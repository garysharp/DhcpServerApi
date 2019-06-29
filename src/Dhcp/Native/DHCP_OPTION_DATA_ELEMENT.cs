using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_OPTION_DATA_ELEMENT structure defines a data element present (either singly or as a member of an array) within a DHCP_OPTION_DATA structure. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_OPTION_DATA_ELEMENT : IDisposable
    {
        /// <summary>
        /// A DHCP_OPTION_DATA_TYPE enumeration value that indicates the type of data that is present in the subsequent field, Element.
        /// </summary>
        public readonly DHCP_OPTION_DATA_TYPE OptionType;

        /// <summary>
        /// Pointer to the element data
        /// </summary>
        public readonly IntPtr DataOffset;

        public DHCP_OPTION_DATA_ELEMENT(DHCP_OPTION_DATA_TYPE OptionType, IntPtr DataOffset)
        {
            this.OptionType = OptionType;
            this.DataOffset = DataOffset;
        }

        /// <summary>
        /// Specifies the data as a BYTE value. This field will be present if the OptionType is DhcpByteOption.
        /// </summary>
        public byte ByteOption => Marshal.ReadByte(DataOffset);

        /// <summary>
        /// Specifies the data as a WORD value. This field will be present if the OptionType is DhcpWordOption.
        /// </summary>
        public short WordOption => Marshal.ReadInt16(DataOffset);

        /// <summary>
        /// Specifies the data as a DWORD value. This field will be present if the OptionType is DhcpDWordOption.
        /// </summary>
        public int DWordOption => Marshal.ReadInt32(DataOffset);

        /// <summary>
        /// Specifies the data as a DWORD_DWORD value. This field will be present if the OptionType is DhcpDWordDWordOption.
        /// </summary>
        public long DWordDWordOption => Marshal.ReadInt64(DataOffset);

        /// <summary>
        /// Specifies the data as a DHCP_IP_ADDRESS (DWORD) value. This field will be present if the OptionType is IpAddressOption.
        /// </summary>
        public DHCP_IP_ADDRESS IpAddressOption => DataOffset.MarshalToStructure<DHCP_IP_ADDRESS>();

        /// <summary>
        /// Specifies the data as a Unicode string value. This field will be present if the OptionType is DhcpStringDataOption.
        /// </summary>
        public IntPtr StringDataOption => Marshal.ReadIntPtr(DataOffset);

        /// <summary>
        /// Specifies the data as a DHCP_BINARY_DATA structure. This field will be present if the OptionType is DhcpBinaryDataOption.
        /// </summary>
        public DHCP_BINARY_DATA BinaryDataOption => DataOffset.MarshalToStructure<DHCP_BINARY_DATA>();

        /// <summary>
        /// Specifies the data as encapsulated within a DHCP_BINARY_DATA structure. The application must know the format of the opaque data capsule in order to read it from the Data field of DHCP_BINARY_DATA. This field will be present if the OptionType is DhcpEncapsulatedDataOption.
        /// </summary>
        public DHCP_BINARY_DATA EncapsulatedDataOption => DataOffset.MarshalToStructure<DHCP_BINARY_DATA>();

        /// <summary>
        /// Specifies the data as a Unicode string value. This field will be present if the OptionType is DhcpIpv6AddressOption.
        /// </summary>
        public IntPtr Ipv6AddressDataOption => Marshal.ReadIntPtr(DataOffset);

        public void Dispose()
        {
            if (DataOffset != IntPtr.Zero)
            {
                if (OptionType == DHCP_OPTION_DATA_TYPE.DhcpStringDataOption ||
                    OptionType == DHCP_OPTION_DATA_TYPE.DhcpIpv6AddressOption)
                {
                    // Pointer at Offset
                    var ptr = Marshal.ReadIntPtr(DataOffset);
                    Api.FreePointer(ptr);
                }
                else if (OptionType == DHCP_OPTION_DATA_TYPE.DhcpBinaryDataOption ||
                   OptionType == DHCP_OPTION_DATA_TYPE.DhcpBinaryDataOption)
                {
                    // Pointer in Binary Data structure
                    var data = DataOffset.MarshalToStructure<DHCP_BINARY_DATA>();
                    data.Dispose();
                }
            }
        }
    }

    /// <summary>
    /// The DHCP_OPTION_DATA_ELEMENT structure defines a data element present (either singly or as a member of an array) within a DHCP_OPTION_DATA structure. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_OPTION_DATA_ELEMENT_Managed : IDisposable
    {
        /// <summary>
        /// A DHCP_OPTION_DATA_TYPE enumeration value that indicates the type of data that is present in the subsequent field, Element.
        /// </summary>
        public readonly int OptionType;

        private readonly DHCP_OPTION_DATA_ELEMENT_ManagedValue Data;

        public DHCP_OPTION_DATA_ELEMENT_Managed(byte dataByte)
        {
            OptionType = (int)DHCP_OPTION_DATA_TYPE.DhcpByteOption;
            Data = new DHCP_OPTION_DATA_ELEMENT_ManagedValue() { DataByte = dataByte };
        }

        public DHCP_OPTION_DATA_ELEMENT_Managed(short dataWord)
        {
            OptionType = (int)DHCP_OPTION_DATA_TYPE.DhcpWordOption;
            Data = new DHCP_OPTION_DATA_ELEMENT_ManagedValue() { DataWord = dataWord };
        }

        public DHCP_OPTION_DATA_ELEMENT_Managed(int dataDWord)
        {
            OptionType = (int)DHCP_OPTION_DATA_TYPE.DhcpDWordOption;
            Data = new DHCP_OPTION_DATA_ELEMENT_ManagedValue() { DataDWord = dataDWord };
        }

        public DHCP_OPTION_DATA_ELEMENT_Managed(long dataDWordDWord)
        {
            OptionType = (int)DHCP_OPTION_DATA_TYPE.DhcpDWordDWordOption;
            Data = new DHCP_OPTION_DATA_ELEMENT_ManagedValue() { DataDWordDWord = dataDWordDWord };
        }

        public DHCP_OPTION_DATA_ELEMENT_Managed(DHCP_IP_ADDRESS dataIpAddress)
        {
            OptionType = (int)DHCP_OPTION_DATA_TYPE.DhcpIpAddressOption;
            Data = new DHCP_OPTION_DATA_ELEMENT_ManagedValue() { DataIpAddress = dataIpAddress };
        }

        public DHCP_OPTION_DATA_ELEMENT_Managed(DHCP_OPTION_DATA_TYPE type, string dataString)
        {
            OptionType = (int)DHCP_OPTION_DATA_TYPE.DhcpStringDataOption;
            Data = new DHCP_OPTION_DATA_ELEMENT_ManagedValue() { DataString = Marshal.StringToHGlobalUni(dataString) };

            switch (type)
            {
                case DHCP_OPTION_DATA_TYPE.DhcpStringDataOption:
                    OptionType = (int)DHCP_OPTION_DATA_TYPE.DhcpStringDataOption;
                    Data = new DHCP_OPTION_DATA_ELEMENT_ManagedValue() { DataString = Marshal.StringToHGlobalUni(dataString) };
                    break;
                case DHCP_OPTION_DATA_TYPE.DhcpIpv6AddressOption:
                    OptionType = (int)DHCP_OPTION_DATA_TYPE.DhcpIpv6AddressOption;
                    Data = new DHCP_OPTION_DATA_ELEMENT_ManagedValue() { DataIpv6Address = Marshal.StringToHGlobalUni(dataString) };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        public DHCP_OPTION_DATA_ELEMENT_Managed(DHCP_OPTION_DATA_TYPE type, DHCP_BINARY_DATA_Managed dataBinary)
        {
            switch (type)
            {
                case DHCP_OPTION_DATA_TYPE.DhcpBinaryDataOption:
                    OptionType = (int)DHCP_OPTION_DATA_TYPE.DhcpBinaryDataOption;
                    Data = new DHCP_OPTION_DATA_ELEMENT_ManagedValue() { DataBinary = dataBinary };
                    break;
                case DHCP_OPTION_DATA_TYPE.DhcpEncapsulatedDataOption:
                    OptionType = (int)DHCP_OPTION_DATA_TYPE.DhcpEncapsulatedDataOption;
                    Data = new DHCP_OPTION_DATA_ELEMENT_ManagedValue() { DataBinary = dataBinary };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        public void Dispose()
        {
            switch ((DHCP_OPTION_DATA_TYPE)OptionType)
            {
                case DHCP_OPTION_DATA_TYPE.DhcpStringDataOption:
                    Marshal.FreeHGlobal(Data.DataString);
                    break;
                case DHCP_OPTION_DATA_TYPE.DhcpIpv6AddressOption:
                    Marshal.FreeHGlobal(Data.DataIpv6Address);
                    break;
                case DHCP_OPTION_DATA_TYPE.DhcpBinaryDataOption:
                    Data.DataBinary.Dispose();
                    break;
                case DHCP_OPTION_DATA_TYPE.DhcpEncapsulatedDataOption:
                    Data.DataEncapsulated.Dispose();
                    break;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct DHCP_OPTION_DATA_ELEMENT_ManagedValue
        {
            [FieldOffset(0)]
            public byte DataByte;
            [FieldOffset(0)]
            public short DataWord;
            [FieldOffset(0)]
            public int DataDWord;
            [FieldOffset(0)]
            public long DataDWordDWord;
            [FieldOffset(0)]
            public DHCP_IP_ADDRESS DataIpAddress;
            [FieldOffset(0)]
            public IntPtr DataString;
            [FieldOffset(0)]
            public DHCP_BINARY_DATA_Managed DataBinary;
            [FieldOffset(0)]
            public DHCP_BINARY_DATA_Managed DataEncapsulated;
            [FieldOffset(0)]
            public IntPtr DataIpv6Address;
        }
    }
}
