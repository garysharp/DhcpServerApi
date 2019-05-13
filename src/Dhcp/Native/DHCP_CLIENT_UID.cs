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

    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_CLIENT_UID_Managed : IDisposable
    {
        /// <summary>
        /// Specifies the size of Data, in bytes.
        /// </summary>
        public readonly int DataLength;

        /// <summary>
        /// Pointer to an opaque blob of byte (binary) data.
        /// </summary>
        private readonly IntPtr DataPointer;

        public DHCP_CLIENT_UID_Managed(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                DataLength = 0;
                DataPointer = IntPtr.Zero;
            }
            else
            {
                DataLength = data.Length;
                DataPointer = Marshal.AllocHGlobal(data.Length);
                Marshal.Copy(data, 0, DataPointer, data.Length);
            }
        }

        public DHCP_CLIENT_UID_Managed(DHCP_IP_ADDRESS data)
        {
            DataLength = Marshal.SizeOf(data);
            DataPointer = Marshal.AllocHGlobal(DataLength);
            Marshal.StructureToPtr(data, DataPointer, false);
        }

        public DHCP_CLIENT_UID_Managed(DhcpServerHardwareAddress data)
        {
            var bytes = data.Native;
            DataLength = bytes.Length;
            DataPointer = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, DataPointer, bytes.Length);
        }

        public void Dispose()
        {
            if (DataPointer != IntPtr.Zero)
                Marshal.FreeHGlobal(DataPointer);
        }
    }
}
