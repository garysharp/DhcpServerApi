using System;

namespace Dhcp.Native
{
    /// <summary>
    /// Flags used by DhcpV4FailoverSetRelationship
    /// </summary>
    [Flags]
    internal enum DHCP_FAILOVER_RELATIONSHIP_SET_FLAGS
    {
        /// <summary>
        /// The mclt member in pRelationship parameter structure is populated.
        /// </summary>
        MCLT = 0x00000001,
        /// <summary>
        /// The safePeriod member in pRelationship parameter structure is populated.
        /// </summary>
        SAFEPERIOD = 0x00000002,
        /// <summary>
        /// The state member in pRelationship parameter structure is populated.
        /// </summary>
        CHANGESTATE = 0x00000004,
        /// <summary>
        /// The percentage member in pRelationship parameter structure is populated.
        /// </summary>
        PERCENTAGE = 0x00000008,
        /// <summary>
        /// The mode member in pRelationship parameter structure is populated.
        /// </summary>
        MODE = 0x00000010,
        /// <summary>
        /// The prevState member in pRelationship parameter structure is populated.
        /// </summary>
        PREVSTATE = 0x00000020,
    }
}
