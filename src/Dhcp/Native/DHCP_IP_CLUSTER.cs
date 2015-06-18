using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_IP_CLUSTER structure defines the address and mast for a network cluster.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_IP_CLUSTER
    {
        /// <summary>
        /// DHCP_IP_ADDRESS value that contains the IP address of the cluster.
        /// </summary>
        public DHCP_IP_ADDRESS ClusterAddress;
        /// <summary>
        /// Specifies the mask value for a cluster. This value should be set to 0xFFFFFFFF if the cluster is full.
        /// </summary>
        public uint ClusterMask;
    }
}
