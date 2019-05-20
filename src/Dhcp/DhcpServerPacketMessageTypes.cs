namespace Dhcp
{
    public enum DhcpServerPacketMessageTypes : byte
    {
        Unknown = 0,
        Discover = 1,
        Offer = 2,
        Request = 3,
        Decline = 4,
        Acknowledgment = 5,
        NegativeAcknowledgment = 6,
        Release = 7,
        Inform = 8,
        ForceRenew = 9,
        LeaseQuery = 10,
        LeaseUnassigned = 11,
        LeaseUknown = 12,
        LeaseActive = 13,
        BulkLeaseQuery = 14,
        LeaseQueryDone = 15,
        ActiveLeaseQuery = 16,
        LeaseQueryStatus = 17,
        Tls = 18
    }
}
