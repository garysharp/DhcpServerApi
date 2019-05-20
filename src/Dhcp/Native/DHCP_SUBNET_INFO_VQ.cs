using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_SUBNET_INFO_VQ : IDisposable
    {
        /// <summary>
        /// DHCP_IP_ADDRESS value that specifies the subnet ID.
        /// </summary>
        public readonly DHCP_IP_ADDRESS SubnetAddress;
        /// <summary>
        /// DHCP_IP_MASK value that specifies the subnet IP mask.
        /// </summary>
        public readonly DHCP_IP_MASK SubnetMask;
        /// <summary>
        /// Unicode string that specifies the network name of the subnet.
        /// </summary>
        private readonly IntPtr SubnetNamePointer;
        /// <summary>
        /// Unicode string that contains an optional comment particular to this subnet.
        /// </summary>
        private readonly IntPtr SubnetCommentPointer;
        /// <summary>
        /// DHCP_HOST_INFO structure that contains information about the DHCP server servicing this subnet.
        /// </summary>
        public readonly DHCP_HOST_INFO PrimaryHost;
        /// <summary>
        /// DHCP_SUBNET_STATE enumeration value indicating the current state of the subnet (enabled/disabled).
        /// </summary>
        public readonly uint SubnetState;
        /// <summary>
        /// Integer value used as a BOOL to represent whether or not Quarantine is enabled for the subnet.
        /// If TRUE (0x00000001), Quarantine is turned ON on the DHCP server; if FALSE (0x00000000), it is turned OFF.
        /// </summary>
        public readonly uint QuarantineOn;
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        public readonly uint Reserved1;
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        public readonly uint Reserved2;
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        public readonly ulong Reserved3;
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        public readonly ulong Reserved4;

        /// <summary>
        /// Unicode string that specifies the network name of the subnet.
        /// </summary>
        public string SubnetName => Marshal.PtrToStringUni(SubnetNamePointer);
        /// <summary>
        /// Unicode string that contains an optional comment particular to this subnet.
        /// </summary>
        public string SubnetComment => Marshal.PtrToStringUni(SubnetCommentPointer);

        public void Dispose()
        {
            Api.FreePointer(SubnetNamePointer);

            // Freeing SubnetComment causes heap corruption ?!?!?
            // Api.FreePointer(ref SubnetCommentPointer);

            PrimaryHost.Dispose();
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DHCP_SUBNET_INFO_VQ_Managed
    {
        /// <summary>
        /// DHCP_IP_ADDRESS value that specifies the subnet ID.
        /// </summary>
        public readonly DHCP_IP_ADDRESS SubnetAddress;
        /// <summary>
        /// DHCP_IP_MASK value that specifies the subnet IP mask.
        /// </summary>
        public readonly DHCP_IP_MASK SubnetMask;
        /// <summary>
        /// Unicode string that specifies the network name of the subnet.
        /// </summary>
        private readonly string SubnetName;
        /// <summary>
        /// Unicode string that contains an optional comment particular to this subnet.
        /// </summary>
        private readonly string SubnetComment;
        /// <summary>
        /// DHCP_HOST_INFO structure that contains information about the DHCP server servicing this subnet.
        /// </summary>
        public readonly DHCP_HOST_INFO_Managed PrimaryHost;
        /// <summary>
        /// DHCP_SUBNET_STATE enumeration value indicating the current state of the subnet (enabled/disabled).
        /// </summary>
        public readonly DHCP_SUBNET_STATE SubnetState;
        /// <summary>
        /// Integer value used as a BOOL to represent whether or not Quarantine is enabled for the subnet.
        /// If TRUE (0x00000001), Quarantine is turned ON on the DHCP server; if FALSE (0x00000000), it is turned OFF.
        /// </summary>
        public readonly uint QuarantineOn;
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        private readonly uint Reserved1;
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        private readonly uint Reserved2;
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        private readonly ulong Reserved3;
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        private readonly ulong Reserved4;

        public DHCP_SUBNET_INFO_VQ_Managed(DHCP_IP_ADDRESS subnetAddress, DHCP_IP_MASK subnetMask, string subnetName, string subnetComment, DHCP_HOST_INFO_Managed primaryHost, DHCP_SUBNET_STATE subnetState, uint quarantineOn, uint reserved1, uint reserved2, ulong reserved3, ulong reserved4)
        {
            SubnetAddress = subnetAddress;
            SubnetMask = subnetMask;
            SubnetName = subnetName;
            SubnetComment = subnetComment;
            PrimaryHost = primaryHost;
            SubnetState = subnetState;
            QuarantineOn = quarantineOn;
            Reserved1 = reserved1;
            Reserved2 = reserved2;
            Reserved3 = reserved3;
            Reserved4 = reserved4;
        }
    }
}
