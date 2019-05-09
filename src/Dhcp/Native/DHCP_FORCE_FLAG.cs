namespace Dhcp.Native
{
    internal enum DHCP_FORCE_FLAG
    {
        /// <summary>
        /// The operation deletes all client records affected by the element, and then deletes the element.
        /// </summary>
        DhcpFullForce,
        /// <summary>
        /// The operation only deletes the subnet element, leaving intact any client records impacted by the change.
        /// </summary>
        DhcpNoForce,
        /// <summary>
        /// The operation deletes all client records affected by the element, and then deletes the element from the DHCP server.
        /// But it does not delete any registered DNS records associated with the deleted client records from the DNS server.
        /// This flag is only valid when passed to DhcpDeleteSubnet.
        /// Note that the minimum server OS requirement for this value is Windows Server 2012 R2 with KB 3100473 installed.
        /// </summary>
        DhcpFailoverForce
    }
}
