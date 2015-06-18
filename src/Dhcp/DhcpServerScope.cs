using Dhcp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Dhcp
{
    public class DhcpServerScope
    {
        internal DHCP_IP_ADDRESS address;
        private Lazy<DHCP_SUBNET_INFO> info;
        private Lazy<TimeSpan> timeDelayOffer;
        private Lazy<DhcpServerIpRange> ipRange;
        private Lazy<List<DhcpServerIpRange>> excludedIpRanges;

        public DhcpServer Server { get; private set; }

        public string Address { get { return address.ToString(); } }
        public string Mask { get { return info.Value.SubnetMask.ToString(); } }

        public string Name { get { return info.Value.SubnetName; } }
        public string Comment { get { return info.Value.SubnetComment; } }

        public string PrimaryHostIpAddress { get { return info.Value.PrimaryHost.IpAddress.ToNetworkOrderString(); } }

        public DhcpServerScopeState State { get { return (DhcpServerScopeState)info.Value.SubnetState; } }

        public TimeSpan TimeDelayOffer { get { return timeDelayOffer.Value; } }

        internal DhcpServerScope(DhcpServer Server, DHCP_IP_ADDRESS SubnetAddress)
        {
            this.Server = Server;
            this.address = SubnetAddress;

            this.info = new Lazy<DHCP_SUBNET_INFO>(GetInfo);
            this.timeDelayOffer = new Lazy<TimeSpan>(GetTimeDelayOffer);
            this.ipRange = new Lazy<DhcpServerIpRange>(GetIpRange);
            this.excludedIpRanges = new Lazy<List<DhcpServerIpRange>>(GetExcludedIpRanges);
        }

        /// <summary>
        /// Enumerates a list of Global Option Values associated with the DHCP Server
        /// </summary>
        public IEnumerable<DhcpServerOptionValue> OptionValues
        {
            get
            {
                return DhcpServerOptionValue.EnumScopeOptionValues(this);
            }
        }

        public IEnumerable<DhcpServerClient> Clients
        {
            get
            {
                return DhcpServerClient.GetClients(this);
            }
        }

        public IEnumerable<DhcpServerScopeReservation> Reservations
        {
            get
            {
                return DhcpServerScopeReservation.GetReservations(this);
            }
        }

        public DhcpServerIpRange IpRange
        {
            get
            {
                return this.ipRange.Value;
            }
        }

        public IEnumerable<DhcpServerIpRange> ExcludedIpRanges
        {
            get
            {
                return this.excludedIpRanges.Value;
            }
        }

        internal static IEnumerable<DhcpServerScope> GetScopes(DhcpServer Server)
        {
            IntPtr enumInfoPtr;
            int elementsRead, elementsTotal;
            IntPtr resumeHandle = IntPtr.Zero;

            var result = Api.DhcpEnumSubnets(Server.IpAddress, ref resumeHandle, 0xFFFFFFFF, out enumInfoPtr, out elementsRead, out elementsTotal);

            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS || result == DhcpErrors.EPT_S_NOT_REGISTERED)
                yield break;

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException("DhcpEnumSubnets", result);

            if (elementsRead > 0)
            {
                var enumInfo = (DHCP_IP_ARRAY)Marshal.PtrToStructure(enumInfoPtr, typeof(DHCP_IP_ARRAY));

                try
                {
                    foreach (var scopeAddress in enumInfo.Elements)
                    {
                        yield return new DhcpServerScope(Server, (DHCP_IP_ADDRESS)scopeAddress);
                    }
                }
                finally
                {
                    Api.DhcpRpcFreeMemory(enumInfoPtr);
                }
            }
        }

        private DHCP_SUBNET_INFO GetInfo()
        {
            IntPtr subnetInfoPtr;

            var result = Api.DhcpGetSubnetInfo(Server.IpAddress, this.address, out subnetInfoPtr);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException("DhcpGetSubnetInfo", result);

            try
            {
                return (DHCP_SUBNET_INFO)Marshal.PtrToStructure(subnetInfoPtr, typeof(DHCP_SUBNET_INFO));
            }
            finally
            {
                Api.DhcpRpcFreeMemory(subnetInfoPtr);
            }
        }

        private List<DhcpServerIpRange> GetExcludedIpRanges()
        {
            return EnumSubnetElements(DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpExcludedIpRanges)
                .Select(e => DhcpServerIpRange.FromNative(e.ReadExcludeIpRange()))
                .ToList();
        }

        private DhcpServerIpRange GetIpRange()
        {
            return EnumSubnetElements(DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpIpRangesDhcpBootp)
                .Select(e => DhcpServerIpRange.FromNative(e.ReadIpRange()))
                .First();
        }

        private IEnumerable<DHCP_SUBNET_ELEMENT_DATA_V5> EnumSubnetElements(DHCP_SUBNET_ELEMENT_TYPE_V5 EnumElementType)
        {
            IntPtr elementsPtr;
            int elementsRead, elementsTotal;
            IntPtr resumeHandle = IntPtr.Zero;

            var result = Api.DhcpEnumSubnetElementsV5(Server.IpAddress, this.address, EnumElementType, ref resumeHandle, 0xFFFFFFFF, out elementsPtr, out elementsRead, out elementsTotal);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException("DhcpEnumSubnetElementsV5", result);

            if (result == DhcpErrors.ERROR_NO_MORE_ITEMS)
                yield break;

            if (result != DhcpErrors.SUCCESS && result != DhcpErrors.ERROR_MORE_DATA)
                throw new DhcpServerException("DhcpEnumSubnetElementsV5", result);

            if (elementsRead == 0)
                yield break;

            try
            {
                var elements = (DHCP_SUBNET_ELEMENT_INFO_ARRAY_V5)Marshal.PtrToStructure(elementsPtr, typeof(DHCP_SUBNET_ELEMENT_INFO_ARRAY_V5));

                foreach (var element in elements.Elements)
                    yield return element;
            }
            finally
            {
                Api.DhcpRpcFreeMemory(elementsPtr);
            }
        }

        private TimeSpan GetTimeDelayOffer()
        {
            UInt16 timeDelay;

            var result = Api.DhcpGetSubnetDelayOffer(Server.IpAddress, this.address, out timeDelay);

            if (result != DhcpErrors.SUCCESS)
                throw new DhcpServerException("DhcpGetSubnetDelayOffer", result);

            return TimeSpan.FromMilliseconds(timeDelay);
        }

        public override string ToString()
        {
            return string.Format("DHCP Scope: {0} ({1} ({2}))", this.Address, this.Server.Name, this.Server.IpAddress);
        }
    }
}
