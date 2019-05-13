using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_BINARY_DATA structure defines an opaque blob of binary data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_BINARY_DATA : IDisposable
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
                if (DataPointer == IntPtr.Zero)
                {
                    return null;
                }
                else
                {
                    var blob = new byte[DataLength];

                    if (DataLength != 0)
                        Marshal.Copy(DataPointer, blob, 0, DataLength);

                    return blob;
                }
            }
        }

        public DhcpServerHardwareAddress DataAsHardwareAddress
        {
            get
            {
                if (DataPointer == IntPtr.Zero || DataLength > DhcpServerHardwareAddress.MaximumLength)
                {
                    return default;
                }
                else
                {
                    return DhcpServerHardwareAddress.FromNative(DhcpServerHardwareType.Ethernet, DataPointer, DataLength);
                }
            }
        }

        public void Dispose()
        {
            Api.FreePointer(ref DataPointer);
        }
    }

    /// <summary>
    /// The DHCP_BINARY_DATA structure defines an opaque blob of binary data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_BINARY_DATA_Managed : IDisposable
    {
        /// <summary>
        /// Specifies the size of Data, in bytes.
        /// </summary>
        public readonly int DataLength;

        /// <summary>
        /// Pointer to an opaque blob of byte (binary) data.
        /// </summary>
        private IntPtr DataPointer;

        public DHCP_BINARY_DATA_Managed(byte[] data)
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

        public void Dispose()
        {
            if (DataPointer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(DataPointer);
            }
        }
    }
}
