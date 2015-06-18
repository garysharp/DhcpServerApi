using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_OPTION_DATA_ELEMENT structure defines a data element present (either singly or as a member of an array) within a DHCP_OPTION_DATA structure. 
    /// </summary>
    internal struct DHCP_OPTION_DATA_ELEMENT
    {
        /// <summary>
        /// A DHCP_OPTION_DATA_TYPE enumeration value that indicates the type of data that is present in the subsequent field, Element.
        /// </summary>
        public DHCP_OPTION_DATA_TYPE OptionType;

        /// <summary>
        /// Pointer to the element data
        /// </summary>
        public IntPtr DataOffset;

        /// <summary>
        /// Specifies the data as a BYTE value. This field will be present if the OptionType is DhcpByteOption.
        /// </summary>
        public Byte ByteOption
        {
            get
            {
                return Marshal.ReadByte(DataOffset);
            }
        }

        /// <summary>
        /// Specifies the data as a WORD value. This field will be present if the OptionType is DhcpWordOption.
        /// </summary>
        public Int16 WordOption
        {
            get
            {
                return Marshal.ReadInt16(DataOffset);
            }
        }

        /// <summary>
        /// Specifies the data as a DWORD value. This field will be present if the OptionType is DhcpDWordOption.
        /// </summary>
        public Int32 DWordOption
        {
            get
            {
                return Marshal.ReadInt32(DataOffset);
            }
        }

        /// <summary>
        /// Specifies the data as a DWORD_DWORD value. This field will be present if the OptionType is DhcpDWordDWordOption.
        /// </summary>
        public Int64 DWordDWordOption
        {
            get
            {
                return Marshal.ReadInt64(DataOffset);
            }
        }
        
        /// <summary>
        /// Specifies the data as a DHCP_IP_ADDRESS (DWORD) value. This field will be present if the OptionType is IpAddressOption.
        /// </summary>
        public DHCP_IP_ADDRESS IpAddressOption
        {
            get
            {
                return (DHCP_IP_ADDRESS)Marshal.PtrToStructure(DataOffset, typeof(DHCP_IP_ADDRESS));
            }
        }

        /// <summary>
        /// Specifies the data as a Unicode string value. This field will be present if the OptionType is DhcpStringDataOption.
        /// </summary>
        public IntPtr StringDataOption
        {
            get
            {
                return Marshal.ReadIntPtr(DataOffset);
            }
        }
        
        /// <summary>
        /// Specifies the data as a DHCP_BINARY_DATA structure. This field will be present if the OptionType is DhcpBinaryDataOption.
        /// </summary>
        public DHCP_BINARY_DATA BinaryDataOption
        {
            get
            {
                return (DHCP_BINARY_DATA)Marshal.PtrToStructure(DataOffset, typeof(DHCP_BINARY_DATA));
            }
        }

        /// <summary>
        /// Specifies the data as encapsulated within a DHCP_BINARY_DATA structure. The application must know the format of the opaque data capsule in order to read it from the Data field of DHCP_BINARY_DATA. This field will be present if the OptionType is DhcpEncapsulatedDataOption.
        /// </summary>
        public DHCP_BINARY_DATA EncapsulatedDataOption
        {
            get
            {
                return (DHCP_BINARY_DATA)Marshal.PtrToStructure(DataOffset, typeof(DHCP_BINARY_DATA));
            }
        }

        /// <summary>
        /// Specifies the data as a Unicode string value. This field will be present if the OptionType is DhcpIpv6AddressOption.
        /// </summary>
        public IntPtr Ipv6AddressDataOption
        {
            get
            {
                return Marshal.ReadIntPtr(DataOffset);
            }
        }
    }
}
