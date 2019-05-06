using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_CLASS_INFO_ARRAY structure defines an array of elements that contain DHCP class information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_CLASS_INFO_ARRAY : IDisposable
    {
        /// <summary>
        /// Specifies the number of elements in Classes.
        /// </summary>
        public readonly int NumElements;
        /// <summary>
        /// Pointer to an array of <see cref="DHCP_CLASS_INFO"/> structures that contain DHCP class information.
        /// </summary>
        private IntPtr ClassesPointer;

        /// <summary>
        /// Pointer to an array of <see cref="DHCP_CLASS_INFO"/> structures that contain DHCP class information.
        /// </summary>
        public IEnumerable<DHCP_CLASS_INFO> Classes
        {
            get
            {
                if (NumElements == 0 || ClassesPointer == IntPtr.Zero)
                    yield break;

                var iter = ClassesPointer;
                var size = Marshal.SizeOf(typeof(DHCP_CLASS_INFO));
                for (var i = 0; i < NumElements; i++)
                {
                    yield return iter.MarshalToStructure<DHCP_CLASS_INFO>();
                    iter += size;
                }
            }
        }

        public void Dispose()
        {
            foreach (var @class in Classes)
                @class.Dispose();
            
            Api.FreePointer(ref ClassesPointer);
        }
    }
}
