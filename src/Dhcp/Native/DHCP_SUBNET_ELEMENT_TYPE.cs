using System;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_SUBNET_ELEMENT_TYPE enumeration defines the set of possible subnet element types.
    /// </summary>
    internal enum DHCP_SUBNET_ELEMENT_TYPE
    {
        /// <summary>
        /// The subnet element contains the range of DHCP-served IP addresses.
        /// </summary>
        DhcpIpRanges,
        /// <summary>
        /// The subnet element contains the IP addresses of secondary DHCP hosts available in the subnet.
        /// </summary>
        [Obsolete("Not Supported")]
        DhcpSecondaryHosts,
        /// <summary>
        /// The subnet element contains the individual reserved IP addresses for the subnet.
        /// </summary>
        DhcpReservedIps,
        /// <summary>
        /// The subnet element contains the IP addresses excluded from the range of DHCP-served addresses.
        /// </summary>
        DhcpExcludedIpRanges,
        /// <summary>
        /// Undocumented Option
        /// </summary>
        DhcpIpUsedClusters,
        /// <summary>
        /// The subnet element contains the IP addresses served by DHCP to the subnet (as opposed to those served by other dynamic address services, such as BOOTP).
        /// </summary>
        DhcpIpRangesDhcpOnly,
        /// <summary>
        /// The subnet element contains the IP addresses served by both DHCP and BOOTP to the subnet.
        /// </summary>
        DhcpIpRangesDhcpBootp,
        /// <summary>
        /// The subnet element contains the IP addresses served by BOOTP to the subnet (specifically excluding DHCP-served addresses).
        /// </summary>
        DhcpIpRangesBootpOnly,
    }
}
