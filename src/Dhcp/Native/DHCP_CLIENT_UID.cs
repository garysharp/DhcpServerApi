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
        public int DataLength;

        /// <summary>
        /// Pointer to an opaque blob of byte (binary) data.
        /// </summary>
        private IntPtr DataPointer;

        /// <summary>
        /// Blob of byte (binary) data.
        /// </summary>
        public byte[] Data
        {
            get
            {
                byte[] blob = new byte[this.DataLength];

                if (this.DataLength != 0)
                {
                    Marshal.Copy(this.DataPointer, blob, 0, this.DataLength);
                }

                return blob;
            }
        }

        public DHCP_IP_ADDRESS ClientIpAddressMask
        {
            get
            {
                return (DHCP_IP_ADDRESS)Marshal.ReadInt32(DataPointer);
            }
        }

        public byte[] ClientMacAddress
        {
            get
            {
                byte[] macAddress = new byte[6];

                Marshal.Copy(DataPointer + 5, macAddress, 0, 6);

                return macAddress;
            }
        }
    }
}
