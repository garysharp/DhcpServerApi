using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_CLIENT_UID
    {
        /// <summary>
        /// Specifies the size of Data, in bytes.
        /// </summary>
        public readonly int DataLength;

        /// <summary>
        /// Pointer to an opaque blob of byte (binary) data.
        /// </summary>
        private readonly IntPtr DataPointer;

        /// <summary>
        /// Blob of byte (binary) data.
        /// </summary>
        public byte[] Data
        {
            get
            {
                var blob = new byte[DataLength];

                if (DataLength != 0)
                    Marshal.Copy(DataPointer, blob, 0, DataLength);

                return blob;
            }
        }

        public DHCP_IP_ADDRESS ClientIpAddressMask => (DHCP_IP_ADDRESS)Marshal.ReadInt32(DataPointer);

        public byte[] ClientMacAddress
        {
            get
            {
                var macAddress = new byte[6];

                Marshal.Copy(DataPointer + 5, macAddress, 0, 6);

                return macAddress;
            }
        }
    }
}
