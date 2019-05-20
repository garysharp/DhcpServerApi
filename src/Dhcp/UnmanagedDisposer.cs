using System;
using System.Runtime.InteropServices;

namespace Dhcp
{
    internal struct UnmanagedDisposer<T> : IDisposable
    {
#pragma warning disable IDE0032 // Use auto property
        private readonly IntPtr pointer;
#pragma warning restore IDE0032 // Use auto property

        public IntPtr Pointer => pointer;

        public UnmanagedDisposer(T structure)
        {
            var size = Marshal.SizeOf(structure);
            pointer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structure, pointer, false);
            }
            catch (Exception)
            {
                Marshal.FreeHGlobal(pointer);
                throw;
            }
        }

        public void Dispose()
        {
            if (pointer != IntPtr.Zero)
            {
                Marshal.DestroyStructure(pointer, typeof(T));
                Marshal.FreeHGlobal(pointer);
            }
        }

        public static implicit operator IntPtr(UnmanagedDisposer<T> disposer)
        {
            return disposer.pointer;
        }
    }
}
