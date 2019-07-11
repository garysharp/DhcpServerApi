using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_OPTION_ARRAY structure defines an array of DHCP server options.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCP_OPTION_ARRAY : IDisposable
    {
        /// <summary>
        /// Specifies the number of option elements in Options.
        /// </summary>
        public readonly int NumElements;
        /// <summary>
        /// Pointer to a list of <see cref="DHCP_OPTION"/> structures containing DHCP server options and the associated data.
        /// </summary>
        private readonly IntPtr OptionsPointer;

        /// <summary>
        /// Pointer to a list of <see cref="DHCP_OPTION"/> structures containing DHCP server options and the associated data.
        /// </summary>
        public IEnumerable<DHCP_OPTION> Options
        {
            get
            {
                if (NumElements == 0 || OptionsPointer == IntPtr.Zero)
                    yield break;

                var iter = OptionsPointer;
                var size = Marshal.SizeOf(typeof(DHCP_OPTION));
                for (var i = 0; i < NumElements; i++)
                {
                    yield return iter.MarshalToStructure<DHCP_OPTION>();
                    iter += size;
                }
            }
        }

        public void Dispose()
        {
            foreach (var option in Options)
                option.Dispose();

            Api.FreePointer(OptionsPointer);
        }
    }
}
