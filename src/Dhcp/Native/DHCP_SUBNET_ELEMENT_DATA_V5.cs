using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_SUBNET_ELEMENT_DATA_V5 structure defines an element that describes a feature or restriction of a subnet. Together, a set of elements describes the set of IP addresses served for a subnet by DHCP or BOOTP. DHCP_SUBNET_ELEMENT_DATA_V5 specifically allows for the definition of BOOTP-served addresses.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_SUBNET_ELEMENT_DATA_V5
    {
        /// <summary>
        /// DHCP_SUBNET_ELEMENT_TYPE enumeration value describing the type of element in the subsequent field.
        /// </summary>
        public readonly DHCP_SUBNET_ELEMENT_TYPE_V5 ElementType;

        private readonly IntPtr Element;

        /// <summary>
        /// DHCP_BOOTP_IP_RANGE structure that contains the set of BOOTP-served IP addresses. This member is present if ElementType is set to DhcpIpRangesBootpOnly.
        /// </summary>
        /// <returns></returns>
        public DHCP_BOOTP_IP_RANGE ReadIpRange() => Element.MarshalToStructure<DHCP_BOOTP_IP_RANGE>();

        /// <summary>
        /// DHCP_HOST_INFO structure that contains the IP addresses of secondary DHCP servers available on the subnet. This member is present if ElementType is set to DhcpSecondaryHosts.
        /// </summary>
        /// <returns></returns>
        public DHCP_HOST_INFO ReadSecondaryHost() => Element.MarshalToStructure<DHCP_HOST_INFO>();

        /// <summary>
        /// DHCP_IP_RESERVATION_V4 structure that contains the set of reserved IP addresses for the subnet. This member is present if ElementType is set to DhcpReservedIps.
        /// </summary>
        /// <returns></returns>
        public DHCP_IP_RESERVATION_V4 ReadReservedIp() => Element.MarshalToStructure<DHCP_IP_RESERVATION_V4>();

        /// <summary>
        /// DHCP_IP_RANGE structure that contains a range of IP addresses. This member is present if ElementType is set to DhcpIpRanges or DhcpExcludedIpRanges.
        /// </summary>
        /// <returns></returns>
        public DHCP_IP_RANGE ReadExcludeIpRange() => Element.MarshalToStructure<DHCP_IP_RANGE>();
    }
}
