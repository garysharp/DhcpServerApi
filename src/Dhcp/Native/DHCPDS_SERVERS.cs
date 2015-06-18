using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCPDS_SERVERS structure defines a list of DHCP servers in the context of directory services.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCPDS_SERVERS
    {
        /// <summary>
        /// Reserved. This value should be 0.
        /// </summary>
        public UInt32 Flags;
        /// <summary>
        /// Specifies the number of elements in Servers.
        /// </summary>
        public UInt32 NumElements;
        /// <summary>
        /// Pointer to an array of <see cref="DHCPDS_SERVER"/> structures that contain information on individual DHCP servers.
        /// </summary>
        private IntPtr ServersPointer;

        /// <summary>
        /// Pointer to an array of <see cref="DHCPDS_SERVER"/> structures that contain information on individual DHCP servers.
        /// </summary>
        public IEnumerable<DHCPDS_SERVER> Servers
        {
            get
            {
                var instanceIter = this.ServersPointer;
                var instanceSize = Marshal.SizeOf(typeof(DHCPDS_SERVER));
                for (int i = 0; i < this.NumElements; i++)
                {
                    yield return (DHCPDS_SERVER)Marshal.PtrToStructure(instanceIter, typeof(DHCPDS_SERVER));

                    instanceIter += instanceSize;
                }
            }
        }
    }
}