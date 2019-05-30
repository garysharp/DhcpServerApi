using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_FAILOVER_RELATIONSHIP structure defines information about a DHCPv4 server failover relationship.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_FAILOVER_RELATIONSHIP : IDisposable
    {
        /// <summary>
        /// This member specifies the IPv4 address of the primary server in the failover relationship.
        /// </summary>
        public readonly DHCP_IP_ADDRESS PrimaryServer;
        /// <summary>
        /// This member specifies the IPv4 address of the secondary server in the failover relationship.
        /// </summary>
        public readonly DHCP_IP_ADDRESS SecondaryServer;
        /// <summary>
        /// This member specifies the mode of the failover relationship.
        /// </summary>
        public readonly DHCP_FAILOVER_MODE Mode;
        /// <summary>
        /// This member is specifies the type of failover server.
        /// </summary>
        public readonly DHCP_FAILOVER_SERVER ServerType;
        /// <summary>
        /// This member specifies the state of the failover relationship.
        /// </summary>
        public readonly FSM_STATE State;
        /// <summary>
        /// This member specifies the previous state of the failover relationship.
        /// </summary>
        public readonly FSM_STATE PrevState;
        /// <summary>
        /// This member defines the maximum client lead time (MCLT) of the failover relationship, in seconds.
        /// </summary>
        public readonly int Mclt;
        /// <summary>
        /// This member specifies a safe period time in seconds, that the DHCPv4 server will wait before transitioning the server from the COMMUNICATION-INT state to PARTNER-DOWN state, as described in [IETF-DHCPFOP-12], section 10.
        /// </summary>
        public readonly int SafePeriod;
        /// <summary>
        /// This member is a pointer to a null-terminated Unicode string containing the name of the failover relationship that uniquely identifies a failover relationship. There is no restriction on the length of this Unicode string.
        /// </summary>
        private readonly IntPtr RelationshipNamePointer;
        /// <summary>
        /// This member is a pointer to a null-terminated Unicode string containing the host name of the primary server in the failover relationship. There is no restriction on the length of this Unicode string.
        /// </summary>
        private readonly IntPtr PrimaryServerNamePointer;
        /// <summary>
        /// This member is a pointer to a null-terminated Unicode string containing the host name of the secondary server in the failover relationship. There is no restriction on the length of this Unicode string.
        /// </summary>
        private readonly IntPtr SecondaryServerNamePointer;
        /// <summary>
        /// This member is a pointer of type LPDHCP_IP_ARRAY, which contains the list of IPv4 subnet addresses that are part of the failover relationship.
        /// </summary>
        private readonly IntPtr ScopesPointer;
        /// <summary>
        /// This member indicates the ratio of the DHCPv4 client load shared between a primary and secondary server in the failover relationship.
        /// </summary>
        public readonly byte Percentage;
        /// <summary>
        /// This member is a pointer to a null-terminated Unicode string containing the shared secret key associated with this failover relationship. There is no restriction on the length of this string.
        /// </summary>
        private readonly IntPtr SharedSecretPointer;

        /// <summary>
        /// This member is a null-terminated Unicode string containing the name of the failover relationship that uniquely identifies a failover relationship. There is no restriction on the length of this Unicode string.
        /// </summary>
        public string RelationshipName => Marshal.PtrToStringUni(RelationshipNamePointer);
        /// <summary>
        /// This member is a null-terminated Unicode string containing the host name of the primary server in the failover relationship. There is no restriction on the length of this Unicode string.
        /// </summary>
        public string PrimaryServerName => Marshal.PtrToStringUni(PrimaryServerNamePointer);
        /// <summary>
        /// This member is a null-terminated Unicode string containing the host name of the secondary server in the failover relationship. There is no restriction on the length of this Unicode string.
        /// </summary>
        public string SecondaryServerName => Marshal.PtrToStringUni(SecondaryServerNamePointer);

        public DHCP_IP_ARRAY Scopes => BitHelper.MarshalToStructure<DHCP_IP_ARRAY>(ScopesPointer);

        /// <summary>
        /// This member is a null-terminated Unicode string containing the shared secret key associated with this failover relationship. There is no restriction on the length of this string.
        /// </summary>
        public string SharedSecret => Marshal.PtrToStringUni(SharedSecretPointer);

        public void Dispose()
        {
            Api.FreePointer(RelationshipNamePointer);
            Api.FreePointer(PrimaryServerNamePointer);
            Api.FreePointer(SecondaryServerNamePointer);
            if (ScopesPointer != IntPtr.Zero)
                BitHelper.MarshalToStructure<DHCP_IP_ARRAY>(ScopesPointer).Dispose();
            Api.FreePointer(SharedSecretPointer);
        }
    }

    /// <summary>
    /// The DHCP_FAILOVER_RELATIONSHIP structure defines information about a DHCPv4 server failover relationship.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DHCP_FAILOVER_RELATIONSHIP_Managed : IDisposable
    {
        /// <summary>
        /// This member specifies the IPv4 address of the primary server in the failover relationship.
        /// </summary>
        public readonly DHCP_IP_ADDRESS PrimaryServer;
        /// <summary>
        /// This member specifies the IPv4 address of the secondary server in the failover relationship.
        /// </summary>
        public readonly DHCP_IP_ADDRESS SecondaryServer;
        /// <summary>
        /// This member specifies the mode of the failover relationship.
        /// </summary>
        public readonly DHCP_FAILOVER_MODE Mode;
        /// <summary>
        /// This member is specifies the type of failover server.
        /// </summary>
        public readonly DHCP_FAILOVER_SERVER ServerType;
        /// <summary>
        /// This member specifies the state of the failover relationship.
        /// </summary>
        public readonly FSM_STATE State;
        /// <summary>
        /// This member specifies the previous state of the failover relationship.
        /// </summary>
        public readonly FSM_STATE PrevState;
        /// <summary>
        /// This member defines the maximum client lead time (MCLT) of the failover relationship, in seconds.
        /// </summary>
        public readonly int Mclt;
        /// <summary>
        /// This member specifies a safe period time in seconds, that the DHCPv4 server will wait before transitioning the server from the COMMUNICATION-INT state to PARTNER-DOWN state, as described in [IETF-DHCPFOP-12], section 10.
        /// </summary>
        public readonly int SafePeriod;
        /// <summary>
        /// This member is a pointer to a null-terminated Unicode string containing the name of the failover relationship that uniquely identifies a failover relationship. There is no restriction on the length of this Unicode string.
        /// </summary>
        private readonly string RelationshipName;
        /// <summary>
        /// This member is a pointer to a null-terminated Unicode string containing the host name of the primary server in the failover relationship. There is no restriction on the length of this Unicode string.
        /// </summary>
        private readonly string PrimaryServerName;
        /// <summary>
        /// This member is a pointer to a null-terminated Unicode string containing the host name of the secondary server in the failover relationship. There is no restriction on the length of this Unicode string.
        /// </summary>
        private readonly string SecondaryServerName;
        /// <summary>
        /// This member is a pointer of type LPDHCP_IP_ARRAY, which contains the list of IPv4 subnet addresses that are part of the failover relationship.
        /// </summary>
        private readonly DHCP_IP_ARRAY_Managed Scopes;
        /// <summary>
        /// This member indicates the ratio of the DHCPv4 client load shared between a primary and secondary server in the failover relationship.
        /// </summary>
        public readonly byte Percentage;
        /// <summary>
        /// This member is a pointer to a null-terminated Unicode string containing the shared secret key associated with this failover relationship. There is no restriction on the length of this string.
        /// </summary>
        private readonly string SharedSecretPointer;

        public void Dispose()
        {
            Scopes.Dispose();
        }
    }
}
