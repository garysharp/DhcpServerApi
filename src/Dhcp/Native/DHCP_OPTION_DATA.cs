using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_OPTION_DATA structure defines a data container for one or more data elements associated with a DHCP option.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_OPTION_DATA
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
                var instanceIter = ElementsPointer;
                var instanceSize = IntPtr.Size == 8 ? 24 : 12;
                for (var i = 0; i < NumElements; i++)
                {
                    yield return new DHCP_OPTION_DATA_ELEMENT()
                    {
                        OptionType = (DHCP_OPTION_DATA_TYPE)Marshal.ReadIntPtr(instanceIter),
                        DataOffset = instanceIter + IntPtr.Size
                    };
                    instanceIter += instanceSize;
                }
            }
        }
    }
}
