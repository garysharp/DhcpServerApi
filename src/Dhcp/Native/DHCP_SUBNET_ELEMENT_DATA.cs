using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_SUBNET_ELEMENT_DATA structure defines an element that describes a feature or restriction of a subnet. Together, a set of elements describes the set of IP addresses served for a subnet by DHCP.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_SUBNET_ELEMENT_DATA : IDisposable
    {
        /// <summary>
        /// DHCP_SUBNET_ELEMENT_TYPE enumeration value describing the type of element in the subsequent field.
        /// </summary>
        public readonly DHCP_SUBNET_ELEMENT_TYPE_V5 ElementType;

        private IntPtr ElementPointer;

        /// <summary>
        /// DHCP_IP_RANGE structure that contains the set of DHCP-served IP addresses. This member is present if ElementType is set to DhcpIpRanges.
        /// </summary>
        /// <returns></returns>
        public DHCP_IP_RANGE ReadIpRange() => ElementPointer.MarshalToStructure<DHCP_IP_RANGE>();

        /// <summary>
        /// DHCP_HOST_INFO structure that contains the IP addresses of secondary DHCP servers available on the subnet. This member is present if ElementType is set to DhcpSecondaryHosts.
        /// </summary>
        /// <returns></returns>
        public DHCP_HOST_INFO ReadSecondaryHost() => ElementPointer.MarshalToStructure<DHCP_HOST_INFO>();

        /// <summary>
        /// DHCP_IP_RESERVATION structure that contains the set of reserved IP addresses for the subnet. This member is present if ElementType is set to DhcpExcludedIpRanges.
        /// </summary>
        /// <returns></returns>
        public DHCP_IP_RESERVATION ReadReservedIp() => ElementPointer.MarshalToStructure<DHCP_IP_RESERVATION>();

        /// <summary>
        /// DHCP_IP_RANGE structure that contains the set of excluded IP addresses. This member is present if ElementType is set to DhcpExcludedIpRanges.
        /// </summary>
        /// <returns></returns>
        public DHCP_IP_RANGE ReadExcludeIpRange() => ElementPointer.MarshalToStructure<DHCP_IP_RANGE>();

        /// <summary>
        /// DHCP_IP_CLUSTER structure that contains the set of clusters within the subnet. This member is present if ElementType is set to DhcpIpUsedClusters.
        /// </summary>
        /// <returns></returns>
        public DHCP_IP_CLUSTER ReadIpUsedCluster() => ElementPointer.MarshalToStructure<DHCP_IP_CLUSTER>();

        public void Dispose()
        {
            if (ElementPointer != IntPtr.Zero)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                switch (ElementType)
                {
                    case DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpSecondaryHosts:
                        ReadSecondaryHost().Dispose();
                        break;
                    case DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpReservedIps:
                        ReadReservedIp().Dispose();
                        break;
                }
#pragma warning restore CS0618 // Type or member is obsolete

                Api.FreePointer(ref ElementPointer);
            }
        }
    }
}
