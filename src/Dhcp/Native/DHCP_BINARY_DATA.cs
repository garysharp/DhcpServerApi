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
        private readonly IntPtr DataPointer;

        /// <summary>
        /// Blob of byte (binary) data.
        /// </summary>
        public byte[] Data
        {
            get
            {
                if (DataPointer == IntPtr.Zero)
                    return null;
                else
                {
                    var blob = new byte[DataLength];

                    if (DataLength != 0)
                        Marshal.Copy(DataPointer, blob, 0, DataLength);

                    return blob;
                }
            }
        }

        public void Dispose()
        {
            if (DataPointer != IntPtr.Zero)
                Api.DhcpRpcFreeMemory(DataPointer);
        }
    }
}
