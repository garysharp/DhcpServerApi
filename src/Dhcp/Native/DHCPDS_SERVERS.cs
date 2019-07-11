using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCPDS_SERVERS structure defines a list of DHCP servers in the context of directory services.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DHCPDS_SERVERS
    {
        /// <summary>
        /// Reserved. This value should be 0.
        /// </summary>
        public readonly uint Flags;
        /// <summary>
        /// Specifies the number of elements in Servers.
        /// </summary>
        public readonly uint NumElements;
        /// <summary>
        /// Pointer to an array of <see cref="DHCPDS_SERVER"/> structures that contain information on individual DHCP servers.
        /// </summary>
        private readonly IntPtr ServersPointer;

        /// <summary>
        /// Pointer to an array of <see cref="DHCPDS_SERVER"/> structures that contain information on individual DHCP servers.
        /// </summary>
        public IEnumerable<DHCPDS_SERVER> Servers
        {
            get
            {
                var instanceIter = ServersPointer;
                var instanceSize = Marshal.SizeOf(typeof(DHCPDS_SERVER));
                for (var i = 0; i < NumElements; i++)
                {
                    yield return instanceIter.MarshalToStructure<DHCPDS_SERVER>();
                    instanceIter += instanceSize;
                }
            }
        }
    }
}
