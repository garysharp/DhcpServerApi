using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_CLIENT_UID : IDisposable
    {
        /// <summary>
        /// Specifies the size of Data, in bytes.
        /// </summary>
        public readonly int DataLength;

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
                var blob = new byte[DataLength];

                if (DataLength != 0)
                    Marshal.Copy(DataPointer, blob, 0, DataLength);

                return blob;
            }
        }

        public DHCP_IP_ADDRESS ClientIpAddress
        {
            get
            {
                if (DataLength < 4)
                    throw new ArgumentOutOfRangeException(nameof(DataLength));

                return (DHCP_IP_ADDRESS)Marshal.ReadInt32(DataPointer);
            }
        }

        public DhcpServerHardwareAddress ClientHardwareAddress
        {
            get
            {
                if (DataLength < 5)
                    throw new ArgumentOutOfRangeException(nameof(DataLength));

                return DhcpServerHardwareAddress.FromNative(DhcpServerHardwareType.Ethernet, DataPointer + 5, DataLength - 5);
            }
        }

        public void Dispose()
        {
            Api.FreePointer(ref DataPointer);
        }
    }
}
