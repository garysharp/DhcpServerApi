using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_OPTION_DATA structure defines a data container for one or more data elements associated with a DHCP option.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_OPTION_DATA : IDisposable
    {
        /// <summary>
        /// Specifies the number of option data elements listed in Elements.
        /// </summary>
        public readonly int NumElements;
        /// <summary>
        /// Pointer to a list of <see cref="DHCP_OPTION_DATA_ELEMENT"/> structures that contain the data elements associated with this particular option element.
        /// </summary>
        private readonly IntPtr ElementsPointer;

        /// <summary>
        /// Pointer to a list of <see cref="DHCP_OPTION_DATA_ELEMENT"/> structures that contain the data elements associated with this particular option element.
        /// </summary>
        public IEnumerable<DHCP_OPTION_DATA_ELEMENT> Elements
        {
            get
            {
                if (NumElements == 0 || ElementsPointer == IntPtr.Zero)
                    yield break;

                var elementSize = IntPtr.Size * 3;
                var iter = ElementsPointer;
                for (var i = 0; i < NumElements; i++)
                {
                    yield return new DHCP_OPTION_DATA_ELEMENT(OptionType: (DHCP_OPTION_DATA_TYPE)Marshal.ReadInt32(iter),
                                                              DataOffset: iter + IntPtr.Size);

                    iter += elementSize;
                }
            }
        }

        public void Dispose()
        {
            foreach (var element in Elements)
                element.Dispose();

            Api.FreePointer(ElementsPointer);
        }
    }

    /// <summary>
    /// The DHCP_OPTION_DATA structure defines a data container for one or more data elements associated with a DHCP option.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_OPTION_DATA_Managed : IDisposable
    {
        /// <summary>
        /// Specifies the number of option data elements listed in Elements.
        /// </summary>
        public readonly int NumElements;
        /// <summary>
        /// Pointer to a list of <see cref="DHCP_OPTION_DATA_ELEMENT"/> structures that contain the data elements associated with this particular option element.
        /// </summary>
        private readonly IntPtr ElementsPointer;

        public DHCP_OPTION_DATA_Managed(DHCP_OPTION_DATA_ELEMENT_Managed[] elements)
        {
            NumElements = elements?.Length ?? 0;
            if (NumElements == 0)
            {
                ElementsPointer = IntPtr.Zero;
            }
            else
            {
                var elementsSize = Marshal.SizeOf(typeof(DHCP_OPTION_DATA_ELEMENT_Managed));
                var elementsPointer = Marshal.AllocHGlobal(elementsSize * elements.Length);
                ElementsPointer = elementsPointer;
                for (var i = 0; i < elements.Length; i++)
                {
                    Marshal.StructureToPtr(elements[i], elementsPointer, false);
                    elementsPointer += elementsSize;
                }
            }
        }

        public IEnumerable<DHCP_OPTION_DATA_ELEMENT_Managed> Elements
        {
            get
            {
                if (ElementsPointer != IntPtr.Zero && NumElements > 0)
                {
                    var elementSize = Marshal.SizeOf(typeof(DHCP_OPTION_DATA_ELEMENT_Managed));
                    var iter = ElementsPointer;
                    for (var i = 0; i < NumElements; i++)
                    {
                        yield return iter.MarshalToStructure<DHCP_OPTION_DATA_ELEMENT_Managed>();
                        iter += elementSize;
                    }
                }
            }
        }

        public void Dispose()
        {
            if (ElementsPointer != IntPtr.Zero)
            {
                foreach (var item in Elements)
                    item.Dispose();

                Marshal.FreeHGlobal(ElementsPointer);
            }
        }
    }
}
