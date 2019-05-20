using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_SUBNET_ELEMENT_INFO_ARRAY structure defines an array of subnet element data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_SUBNET_ELEMENT_INFO_ARRAY : IDisposable
    {
        /// <summary>
        /// Specifies the number of elements in Elements.
        /// </summary>
        public readonly int NumElements;

        /// <summary>
        /// Pointer to a list of DHCP_SUBNET_ELEMENT_DATA structures that contain the data for the corresponding subnet elements.
        /// </summary>
        private IntPtr ElementsPointer;

        /// <summary>
        /// Pointer to a list of DHCP_SUBNET_ELEMENT_DATA structures that contain the data for the corresponding subnet elements.
        /// </summary>
        public IEnumerable<DHCP_SUBNET_ELEMENT_DATA> Elements
        {
            get
            {
                if (NumElements == 0 || ElementsPointer == IntPtr.Zero)
                    yield break;

                var iter = ElementsPointer;
                var size = Marshal.SizeOf(typeof(DHCP_SUBNET_ELEMENT_DATA));
                for (var i = 0; i < NumElements; i++)
                {
                    yield return iter.MarshalToStructure<DHCP_SUBNET_ELEMENT_DATA>();
                    iter += size;
                }
            }
        }

        public void Dispose()
        {
            foreach (var element in Elements)
                element.Dispose();

            Api.FreePointer(ref ElementsPointer);
        }
    }
}
