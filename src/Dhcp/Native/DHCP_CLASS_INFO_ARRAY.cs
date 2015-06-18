using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_CLASS_INFO_ARRAY structure defines an array of elements that contain DHCP class information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_CLASS_INFO_ARRAY
    {
        /// <summary>
        /// Specifies the number of elements in Classes.
        /// </summary>
        public int NumElements;
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
                var instanceIter = this.ClassesPointer;
                var instanceSize = Marshal.SizeOf(typeof(DHCP_CLASS_INFO));
                for (int i = 0; i < this.NumElements; i++)
                {
                    yield return (DHCP_CLASS_INFO)Marshal.PtrToStructure(instanceIter, typeof(DHCP_CLASS_INFO));

                    instanceIter += instanceSize;
                }
            }
        }
    }
}