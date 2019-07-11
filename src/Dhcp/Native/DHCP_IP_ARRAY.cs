using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_IP_ARRAY structure defines an array of IP addresses.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_IP_ARRAY : IDisposable
    {
        /// <summary>
        /// Specifies the number of IP addresses in Elements.
        /// </summary>
        public readonly uint NumElements;
        /// <summary>
        /// Pointer to a list of DHCP_IP_ADDRESS values.
        /// </summary>
        private readonly IntPtr ElementsPointer;

        /// <summary>
        /// Pointer to a list of DHCP_IP_ADDRESS values.
        /// </summary>
        public IEnumerable<DHCP_IP_ADDRESS> Elements
        {
            get
            {
                if (NumElements == 0 || ElementsPointer == IntPtr.Zero)
                    yield break;

                var iter = ElementsPointer;
                var size = Marshal.SizeOf(typeof(DHCP_IP_ADDRESS));
                for (var i = 0; i < NumElements; i++)
                {
                    yield return iter.MarshalToStructure<DHCP_IP_ADDRESS>();
                    iter += size;
                }
            }
        }

        public void Dispose()
        {
            Api.FreePointer(ElementsPointer);
        }
    }

    /// <summary>
    /// The DHCP_IP_ARRAY structure defines an array of IP addresses.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_IP_ARRAY_Managed : IDisposable
    {
        /// <summary>
        /// Specifies the number of IP addresses in Elements.
        /// </summary>
        public readonly uint NumElements;
        /// <summary>
        /// Pointer to a list of DHCP_IP_ADDRESS values.
        /// </summary>
        private readonly IntPtr ElementsPointer;

        public IEnumerable<DHCP_IP_ADDRESS> Elements
        {
            get
            {
                if (NumElements == 0 || ElementsPointer == IntPtr.Zero)
                    yield break;

                var iter = ElementsPointer;
                var size = Marshal.SizeOf(typeof(DHCP_IP_ADDRESS));
                for (var i = 0; i < NumElements; i++)
                {
                    yield return iter.MarshalToStructure<DHCP_IP_ADDRESS>();
                    iter += size;
                }
            }
        }

        public DHCP_IP_ARRAY_Managed(List<DHCP_IP_ADDRESS> elements)
        {
            var elementSize = Marshal.SizeOf(typeof(DHCP_IP_ADDRESS));
            var elementsPointer = Marshal.AllocHGlobal(elementSize * elements.Count);

            NumElements = (uint)elements.Count;
            ElementsPointer = elementsPointer;

            foreach (var element in elements)
            {
                Marshal.StructureToPtr(element, elementsPointer, true);
                elementsPointer += elementSize;
            }
        }

        public DHCP_IP_ARRAY_Managed(DHCP_IP_ADDRESS element)
        {
            var elementSize = Marshal.SizeOf(typeof(DHCP_IP_ADDRESS));
            var elementsPointer = Marshal.AllocHGlobal(elementSize);

            NumElements = 1;
            ElementsPointer = elementsPointer;

            Marshal.StructureToPtr(element, elementsPointer, true);
        }

        public void Dispose()
        {
            if (ElementsPointer != IntPtr.Zero)
                Marshal.FreeHGlobal(ElementsPointer);
        }
    }
}
