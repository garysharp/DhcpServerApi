using System;

namespace Dhcp
{
    public enum OptionTags : byte
    {
        /// <summary>
        /// The pad option can be used to cause subsequent fields to align on word boundaries.
        /// The code for the pad option is 0, and its length is 1 octet. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.Pad)]
        Pad = 0,
        /// <summary>
        /// The end option marks the end of valid information in the vendor field.
        /// Subsequent octets should be filled with pad options.
        /// The code for the end option is 255, and its length is 1 octet.
        /// </summary>
        [OptionTagType(OptionTagTypes.End)]
        End = 255,
        /// <summary>
        /// The subnet mask option specifies the client's subnet mask as per RFC 950.
        /// If both the subnet mask and the router option are specified in a DHCP
        ///  reply, the subnet mask option MUST be first.
        ///  The code for the subnet mask option is 1, and its length is 4 octets. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddress)]
        SubnetMask = 1,
        /// <summary>
        /// The time offset field specifies the offset of the client's subnet in
        /// seconds from Coordinated Universal Time (UTC).  The offset is
        /// expressed as a signed 32-bit integer.
        /// The code for the time offset option is 2, and its length is 4 octets. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.Int32)]
        TimeOffset = 2,
        /// <summary>
        /// The router option specifies a list of IP addresses for routers on the
        ///  client's subnet.  Routers SHOULD be listed in order of preference.
        /// The code for the router option is 3.  The minimum length for the
        ///  router option is 4 octets, and the length MUST always be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        Router = 3,
        /// <summary>
        /// The time server option specifies a list of RFC 868 time servers
        /// available to the client.  Servers SHOULD be listed in order of
        /// preference.
        /// The code for the time server option is 4.  The minimum length for
        /// this option is 4 octets, and the length MUST always be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        TimeServer = 4,
        /// <summary>
        /// The name server option specifies a list of IEN 116 name servers
        /// available to the client.  Servers SHOULD be listed in order of preference.
        /// The code for the name server option is 5.  The minimum length for
        /// this option is 4 octets, and the length MUST always be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        NameServer = 5,
        /// <summary>
        /// The domain name server option specifies a list of Domain Name System
        /// (STD 13, RFC 1035) name servers available to the client.  Servers
        /// SHOULD be listed in order of preference.
        /// The code for the domain name server option is 6.  The minimum length
        /// for this option is 4 octets, and the length MUST always be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        DomainNameServer = 6,
        /// <summary>
        /// The log server option specifies a list of MIT-LCS UDP log servers
        /// available to the client.  Servers SHOULD be listed in order of preference.
        /// The code for the log server option is 7.  The minimum length for this
        /// option is 4 octets, and the length MUST always be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        LogServer = 7,
        /// <summary>
        /// The cookie server option specifies a list of RFC 865 cookie
        /// servers available to the client.  Servers SHOULD be listed in order of preference.
        /// The code for the log server option is 8.  The minimum length for this
        /// option is 4 octets, and the length MUST always be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        CookieServer = 8,
        /// <summary>
        /// The LPR server option specifies a list of RFC 1179 line printer
        /// servers available to the client.  Servers SHOULD be listed in order of preference.
        /// The code for the LPR server option is 9.  The minimum length for this
        /// option is 4 octets, and the length MUST always be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        LprServer = 9,
        /// <summary>
        /// The Impress server option specifies a list of Imagen Impress servers
        /// available to the client.  Servers SHOULD be listed in order of
        /// preference.
        /// The code for the Impress server option is 10.  The minimum length for
        /// this option is 4 octets, and the length MUST always be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        ImpressServer = 10,
        /// <summary>
        /// This option specifies a list of RFC 887 Resource Location
        /// servers available to the client.  Servers SHOULD be listed in order of preference.
        /// The code for this option is 11.  The minimum length for this option
        /// is 4 octets, and the length MUST always be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        ResourceLocationServer = 11,
        /// <summary>
        /// This option specifies the name of the client.  The name may or may
        /// not be qualified with the local domain name (see <see cref="DomainName"/> for the
        /// preferred way to retrieve the domain name).  See RFC 1035 for
        /// character set restrictions.
        /// The code for this option is 12, and its minimum length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.AsciiString)]
        HostName = 12,
        /// <summary>
        /// This option specifies the length in 512-octet blocks of the default
        /// boot image for the client.  The file length is specified as an
        /// unsigned 16-bit integer.
        /// The code for this option is 13, and its length is 2. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.UInt16)]
        BootFileSize = 13,
        /// <summary>
        /// This option specifies the path-name of a file to which the client's
        /// core image should be dumped in the event the client crashes.  The
        /// path is formatted as a character string consisting of characters from
        /// the NVT ASCII character set.
        /// The code for this option is 14.  Its minimum length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.AsciiString)]
        MeritDumpFile = 14,
        /// <summary>
        /// This option specifies the domain name that client should use when
        /// resolving hostnames via the Domain Name System.
        /// The code for this option is 15.  Its minimum length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.AsciiString)]
        DomainName = 15,
        /// <summary>
        /// This specifies the IP address of the client's swap server.
        /// The code for this option is 16 and its length is 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddress)]
        SwapServer = 16,
        /// <summary>
        /// This option specifies the path-name that contains the client's root
        /// disk.  The path is formatted as a character string consisting of
        /// characters from the NVT ASCII character set.
        /// The code for this option is 17.  Its minimum length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.AsciiString)]
        RootPath = 17,
        /// <summary>
        /// A string to specify a file, retrievable via TFTP, which contains
        /// information which can be interpreted in the same way as the 64-octet
        /// vendor-extension field within the BOOTP response, with the following
        /// exceptions:
        /// - the length of the file is unconstrained;
        /// - all references to Tag 18 (i.e., instances of the BOOTP Extensions
        ///   Path field) within the file are ignored.
        /// The code for this option is 18.  Its minimum length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.AsciiString)]
        ExtensionsPath = 18,
        /// <summary>
        /// This option specifies whether the client should configure its IP
        /// layer for packet forwarding.  A value of 0 means disable IP
        /// forwarding, and a value of 1 means enable IP forwarding.
        /// The code for this option is 19, and its length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.Byte)]
        IpForwarding = 19,
        /// <summary>
        /// This option specifies whether the client should configure its IP
        /// layer to allow forwarding of datagrams with non-local source routes.
        /// A value of 0 means disallow forwarding of such datagrams,
        /// and a value of 1 means allow forwarding.
        /// The code for this option is 20, and its length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.Byte)]
        NonLocalSourceRouting = 20,
        /// <summary>
        /// This option specifies policy filters for non-local source routing.
        /// The filters consist of a list of IP addresses and masks which specify
        /// destination/mask pairs with which to filter incoming source routes.
        /// Any source routed datagram whose next-hop address does not match one
        /// of the filters should be discarded by the client.
        /// The code for this option is 21.  The minimum length of this option is
        /// 8, and the length MUST be a multiple of 8. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressAndSubnet)]
        PolicyFilter = 21,
        /// <summary>
        /// This option specifies the maximum size datagram that the client
        /// should be prepared to reassemble.  The size is specified as a 16-bit
        /// unsigned integer.  The minimum value legal value is 576.
        /// The code for this option is 22, and its length is 2. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.UInt16)]
        MaximumDatagramReassemblySize = 22,
        /// <summary>
        /// This option specifies the default time-to-live that the client should
        /// use on outgoing datagrams.  The TTL is specified as an octet with a
        /// value between 1 and 255.
        /// The code for this option is 23, and its length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.Byte)]
        DefaultIpTtl = 23,
        /// <summary>
        /// This option specifies the timeout (in seconds) to use when aging Path
        /// MTU values discovered by the mechanism defined in RFC 1191.  The
        /// timeout is specified as a 32-bit unsigned integer.
        /// The code for this option is 24, and its length is 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.UInt32)]
        PathMtuAgingTimeout = 24,
        /// <summary>
        /// This option specifies a table of MTU sizes to use when performing
        /// Path MTU Discovery as defined in RFC 1191.  The table is formatted as
        /// a list of 16-bit unsigned integers, ordered from smallest to largest.
        /// The minimum MTU value cannot be smaller than 68.
        /// The code for this option is 25.  Its minimum length is 2, and the
        /// length MUST be a multiple of 2. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.UInt16List)]
        PathMtuPlateauTable = 25,
        /// <summary>
        /// This option specifies the MTU to use on this interface.  The MTU is
        /// specified as a 16-bit unsigned integer.  The minimum legal value for
        /// the MTU is 68.
        /// The code for this option is 26, and its length is 2. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.UInt16)]
        InterfaceMtu = 26,
        /// <summary>
        /// This option specifies whether or not the client may assume that all
        /// subnets of the IP network to which the client is connected use the
        /// same MTU as the subnet of that network to which the client is
        /// directly connected. A value of 1 indicates that all subnets share
        /// the same MTU. A value of 0 means that the client should assume that
        /// some subnets of the directly connected network may have smaller MTUs.
        /// The code for this option is 27, and its length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.Byte)]
        AllSubnetsAreLocal = 27,
        /// <summary>
        /// This option specifies the broadcast address in use on the client's
        /// subnet.
        /// The code for this option is 28, and its length is 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddress)]
        BroadcastAddress = 28,
        /// <summary>
        /// This option specifies whether or not the client should perform subnet
        /// mask discovery using ICMP. A value of 0 indicates that the client
        /// should not perform mask discovery. A value of 1 means that the
        /// client should perform mask discovery.
        /// The code for this option is 29, and its length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.Byte)]
        PerformMaskDiscovery = 29,
        /// <summary>
        /// This option specifies whether or not the client should respond to
        /// subnet mask requests using ICMP.  A value of 0 indicates that the
        /// client should not respond. A value of 1 means that the client should
        /// respond.
        /// The code for this option is 30, and its length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.Byte)]
        MaskSupplier = 30,
        /// <summary>
        /// This option specifies whether or not the client should solicit
        /// routers using the Router Discovery mechanism defined in RFC 1256.
        /// A value of 0 indicates that the client should not perform
        /// router discovery. A value of 1 means that the client should perform
        /// router discovery.
        /// The code for this option is 31, and its length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.Byte)]
        PerformRouterDiscovery = 31,
        /// <summary>
        /// This option specifies the address to which the client should transmit
        /// router solicitation requests.
        /// The code for this option is 32, and its length is 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddress)]
        RouterSolicitationAddress = 32,
        /// <summary>
        /// This option specifies a list of static routes that the client should
        /// install in its routing cache.  If multiple routes to the same
        /// destination are specified, they are listed in descending order of
        /// priority.
        /// The routes consist of a list of IP address pairs.  The first address
        /// is the destination address, and the second address is the router for
        /// the destination.
        /// The default route (0.0.0.0) is an illegal destination for a static
        /// route. See <see cref="Router"/> for information about the router option.
        /// The code for this option is 33.  The minimum length of this option is
        /// 8, and the length MUST be a multiple of 8. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressAndIpAddress)]
        [Obsolete("See RFC 3442")]
        StaticRouter = 33,
        /// <summary>
        /// This option specifies whether or not the client should negotiate the
        /// use of trailers (RFC 893) when using the ARP protocol. A value
        /// of 0 indicates that the client should not attempt to use trailers.  A
        /// value of 1 means that the client should attempt to use trailers.
        /// The code for this option is 34, and its length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.Byte)]
        TrailerEncapsulation = 34,
        /// <summary>
        /// This option specifies the timeout in seconds for ARP cache entries.
        /// The time is specified as a 32-bit unsigned integer.
        /// The code for this option is 35, and its length is 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.UInt32)]
        ArpCacheTimeout = 35,
        /// <summary>
        /// This option specifies whether or not the client should use Ethernet
        /// Version 2 (RFC 894) or IEEE 802.3 (RFC 1042) encapsulation
        /// if the interface is an Ethernet.A value of 0 indicates that the
        /// client should use RFC 894 encapsulation.A value of 1 means that the
        /// client should use RFC 1042 encapsulation.
        /// The code for this option is 36, and its length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.Byte)]
        EthernetEncapsulation = 36,
        /// <summary>
        /// This option specifies the default TTL that the client should use when
        /// sending TCP segments.  The value is represented as an 8-bit unsigned
        /// integer.  The minimum value is 1.
        /// The code for this option is 37, and its length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.Byte)]
        TcpDefaultTtl = 37,
        /// <summary>
        /// This option specifies the interval (in seconds) that the client TCP
        /// should wait before sending a keepalive message on a TCP connection.
        /// The time is specified as a 32-bit unsigned integer.  A value of zero
        /// indicates that the client should not generate keepalive messages on
        /// connections unless specifically requested by an application.
        /// The code for this option is 38, and its length is 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.UInt32)]
        TcpKeepaliveInterval = 38,
        /// <summary>
        /// This option specifies the whether or not the client should send TCP
        /// keepalive messages with a octet of garbage for compatibility with
        /// older implementations.  A value of 0 indicates that a garbage octet
        /// should not be sent. A value of 1 indicates that a garbage octet
        /// should be sent.
        /// The code for this option is 39, and its length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.Byte)]
        TcpKeepaliveGarbage = 39,
        /// <summary>
        /// This option specifies the name of the client's NIS domain.  The
        /// domain is formatted as a character string consisting of characters
        /// from the NVT ASCII character set.
        /// The code for this option is 40.  Its minimum length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.AsciiString)]
        NisDomain = 40,
        /// <summary>
        /// This option specifies a list of IP addresses indicating NIS servers
        /// available to the client.  Servers SHOULD be listed in order of
        /// preference.
        /// The code for this option is 41.  Its minimum length is 4, and the
        /// length MUST be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        NetworkInformationServers = 41,
        /// <summary>
        /// This option specifies a list of IP addresses indicating NTP
        /// servers available to the client.  Servers SHOULD be listed in order
        /// of preference.
        /// The code for this option is 42.  Its minimum length is 4, and the
        /// length MUST be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        NtpServers = 42,
        /// <summary>
        /// This option is used by clients and servers to exchange vendor-
        /// specific information.  The information is an opaque object of n
        /// octets, presumably interpreted by vendor-specific code on the clients
        /// and servers.  The definition of this information is vendor specific.
        /// The vendor is indicated in the class-identifier option.Servers not
        /// equipped to interpret the vendor-specific information sent by a
        /// client MUST ignore it (although it may be reported).  Clients which
        /// do not receive desired vendor-specific information SHOULD make an
        /// attempt to operate without it, although they may do so(and announce
        /// they are doing so) in a degraded mode.
        /// If a vendor potentially encodes more than one item of information in
        /// this option, then the vendor SHOULD encode the option using
        /// "Encapsulated vendor-specific options" as described below:
        /// The Encapsulated vendor-specific options field SHOULD be encoded as a
        /// sequence of code/length/value fields of identical syntax to the DHCP
        /// options field with the following exceptions:
        /// 1) There SHOULD NOT be a "magic cookie" field in the encapsulated
        /// vendor-specific extensions field.
        /// 2) Codes other than 0 or 255 MAY be redefined by the vendor within
        /// the encapsulated vendor-specific extensions field, but SHOULD
        /// conform to the tag-length-value syntax defined in section 2.
        /// 3) Code 255 (END), if present, signifies the end of the
        /// encapsulated vendor extensions, not the end of the vendor
        /// extensions field. If no code 255 is present, then the end of
        /// the enclosing vendor-specific information field is taken as the
        /// end of the encapsulated vendor-specific extensions field.
        /// The code for this option is 43 and its minimum length is 1. See [RFC2132]
        /// </summary>
        VendorSpecificInformation = 43,
        /// <summary>
        /// The NetBIOS name server (NBNS) option specifies a list of RFC 1001/1002
        /// NBNS name servers listed in order of preference.
        /// The code for this option is 44.  The minimum length of the option is
        /// 4 octets, and the length must always be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        NetBiosOverTcpIpNameServer = 44,
        /// <summary>
        /// The NetBIOS datagram distribution server (NBDD) option specifies a
        /// list of RFC 1001/1002 NBDD servers listed in order of preference.
        /// The code for this option is 45.  The minimum length of the option is 4
        /// octets, and the length must always be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        NetBiosOverTcpIpDatagramDistributionServer = 45,
        /// <summary>
        /// The NetBIOS node type option allows NetBIOS over TCP/IP clients which
        /// are configurable to be configured as described in RFC 1001/1002.  The
        /// value is specified as a single octet which identifies the client type
        /// as follows:
        /// Value         Node Type
        /// -----         ---------
        /// 0x1           B-node
        /// 0x2           P-node
        /// 0x4           M-node
        /// 0x8           H-node
        /// In the above chart, the notation '0x' indicates a number in base-16
        /// (hexadecimal).
        /// The code for this option is 46.  The length of this option is always 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.Byte)]
        NetBiosOverTcpIpNodeType = 46,
        /// <summary>
        /// The NetBIOS scope option specifies the NetBIOS over TCP/IP scope
        /// parameter for the client as specified in RFC 1001/1002.
        /// The code for this option is 47.  The minimum length of this option is 1. See [RFC2132]
        /// </summary>
        NetBiosOverTcpIpScope = 47,
        /// <summary>
        /// This option specifies a list of X Window System [21] Font servers
        /// available to the client. Servers SHOULD be listed in order of preference.
        /// The code for this option is 48.  The minimum length of this option is
        /// 4 octets, and the length MUST be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        XWindowSystemFontServer = 48,
        /// <summary>
        /// This option specifies a list of IP addresses of systems that are
        /// running the X Window System Display Manager and are available to the client.
        /// Addresses SHOULD be listed in order of preference.
        /// The code for the this option is 49. The minimum length of this option
        /// is 4, and the length MUST be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        XWindowSystemDisplayManager = 49,
        /// <summary>
        /// This option is used in a client request (DHCPDISCOVER) to allow the
        /// client to request that a particular IP address be assigned.
        /// The code for this option is 50, and its length is 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddress)]
        DhcpRequestedIpAddress = 50,
        /// <summary>
        /// This option is used in a client request (DHCPDISCOVER or DHCPREQUEST)
        /// to allow the client to request a lease time for the IP address.  In a
        /// server reply (DHCPOFFER), a DHCP server uses this option to specify
        /// the lease time it is willing to offer.
        /// The time is in units of seconds, and is specified as a 32-bit unsigned integer.
        /// The code for this option is 51, and its length is 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.UInt32)]
        DhcpIpAddressLeaseTime = 51,
        /// <summary>
        /// This option is used to indicate that the DHCP 'sname' or 'file'
        /// fields are being overloaded by using them to carry DHCP options.
        /// A DHCP server inserts this option if the returned parameters will
        /// exceed the usual space allotted for options.
        /// If this option is present, the client interprets the specified
        /// additional fields after it concludes interpretation of the standard
        /// option fields.
        /// The code for this option is 52, and its length is 1. See [RFC2132]
        /// Legal values for this option are:
        /// Value   Meaning
        /// -----   --------
        /// 1     the "file" field is used to hold options
        /// 2     the "sname" field is used to hold options
        /// 3     both fields are used to hold options
        /// </summary>
        [OptionTagType(OptionTagTypes.Byte)]
        DhcpOptionOverload = 52,
        /// <summary>
        /// This option is used to convey the type of the DHCP message.
        /// <see cref="PacketMessageTypes"/>
        /// The code for this option is 53, and its length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.DhcpMessageType)]
        DhcpMessageType = 53,
        /// <summary>
        /// This option is used in DHCPOFFER and DHCPREQUEST messages, and may
        /// optionally be included in the DHCPACK and DHCPNAK messages.  DHCP
        /// servers include this option in the DHCPOFFER in order to allow the
        /// client to distinguish between lease offers.  DHCP clients use the
        /// contents of the 'server identifier' field as the destination address
        /// for any DHCP messages unicast to the DHCP server.  DHCP clients also
        /// indicate which of several lease offers is being accepted by including
        /// this option in a DHCPREQUEST message.
        /// The identifier is the IP address of the selected server.
        /// The code for this option is 54, and its length is 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddress)]
        DhcpServerIdentifier = 54,
        /// <summary>
        /// This option is used by a DHCP client to request values for specified
        /// configuration parameters.  The list of requested parameters is
        /// specified as n octets, where each octet is a valid DHCP option code
        /// as defined in this document.
        /// The client MAY list the options in order of preference.  The DHCP
        /// server is not required to return the options in the requested order,
        /// but MUST try to insert the requested options in the order requested
        /// by the client.
        /// The code for this option is 55.  Its minimum length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.DhcpParameterRequestList)]
        DhcpParameterRequestList = 55,
        /// <summary>
        /// This option is used by a DHCP server to provide an error message to a
        /// DHCP client in a DHCPNAK message in the event of a failure. A client
        /// may use this option in a DHCPDECLINE message to indicate the why the
        /// client declined the offered parameters.  The message consists of n
        /// octets of NVT ASCII text, which the client may display on an
        /// available output device.
        /// The code for this option is 56 and its minimum length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.AsciiString)]
        DhcpMessage = 56,
        /// <summary>
        /// This option specifies the maximum length DHCP message that it is
        /// willing to accept.  The length is specified as an unsigned 16-bit
        /// integer.  A client may use the maximum DHCP message size option in
        /// DHCPDISCOVER or DHCPREQUEST messages, but should not use the option
        /// in DHCPDECLINE messages.
        /// The code for this option is 57, and its length is 2.  The minimum
        /// legal value is 576 octets. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.UInt16)]
        DhcpMaximumMessageSize = 57,
        /// <summary>
        /// This option specifies the time interval from address assignment until
        /// the client transitions to the RENEWING state.
        /// The value is in units of seconds, and is specified as a 32-bit
        /// unsigned integer.
        /// The code for this option is 58, and its length is 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.UInt32)]
        DhcpRenewalTime = 58,
        /// <summary>
        /// This option specifies the time interval from address assignment until
        /// the client transitions to the REBINDING state.
        /// The value is in units of seconds, and is specified as a 32-bit
        /// unsigned integer.
        /// The code for this option is 59, and its length is 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.UInt32)]
        DhcpRebindingTime = 59,
        /// <summary>
        /// This option is used by DHCP clients to optionally identify the vendor
        /// type and configuration of a DHCP client.  The information is a string
        /// of n octets, interpreted by servers.  Vendors may choose to define
        /// specific vendor class identifiers to convey particular configuration
        /// or other identification information about a client.For example, the
        /// identifier may encode the client's hardware configuration.  Servers
        /// not equipped to interpret the class-specific information sent by a
        /// client MUST ignore it(although it may be reported). Servers that
        /// respond SHOULD only use option 43 to return the vendor-specific
        /// information to the client.
        /// The code for this option is 60, and its minimum length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.AsciiString)]
        DhcpClassIdentifier = 60,
        /// <summary>
        /// This option is used by DHCP clients to specify their unique
        /// identifier.  DHCP servers use this value to index their database of
        /// address bindings.  This value is expected to be unique for all
        /// clients in an administrative domain.
        /// Identifiers SHOULD be treated as opaque objects by DHCP servers.
        /// The client identifier MAY consist of type-value pairs similar to the
        /// 'htype'/'chaddr' fields defined in [3]. For instance, it MAY consist
        /// of a hardware type and hardware address. In this case the type field
        /// SHOULD be one of the ARP hardware types defined in STD2 [22].  A
        /// hardware type of 0 (zero) should be used when the value field
        /// contains an identifier other than a hardware address (e.g. a fully
        /// qualified domain name).
        /// For correct identification of clients, each client's client-
        /// identifier MUST be unique among the client-identifiers used on the
        /// subnet to which the client is attached.  Vendors and system
        /// administrators are responsible for choosing client-identifiers that
        /// meet this requirement for uniqueness.
        /// The code for this option is 61, and its minimum length is 2. See [RFC2132]
        /// </summary>
        DhcpClientIdentifier = 61,

        /// <summary>
        /// NetWare/IP Domain Name
        /// This option code is used to convey the NetWare/IP domain name used by
        /// the NetWare/IP product. The NetWare/IP Domain in the option is an NVT
        /// ASCII [RFC 854] string whose length is inferred from the option 'len'
        /// field.
        /// The code for this option is 62, and its maximum length is 255. See [RFC2242]
        /// </summary>
        [OptionTagType(OptionTagTypes.AsciiString)]
        NetWareIpDomain = 62,
        /// <summary>
        /// NetWare/IP sub Options
        /// See [RFC2242]
        /// </summary>
        NetWareIpOption = 63,

        /// <summary>
        /// This option specifies the name of the client's NIS+ domain.  The
        /// domain is formatted as a character string consisting of characters
        /// from the NVT ASCII character set.
        /// The code for this option is 64.  Its minimum length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.AsciiString)]
        NisPlusDomain = 64,
        /// <summary>
        /// This option specifies a list of IP addresses indicating NIS+ servers
        /// available to the client.  Servers SHOULD be listed in order of
        /// preference.
        /// The code for this option is 65.  Its minimum length is 4, and the
        /// length MUST be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        NisPlusServerAddresses = 65,
        /// <summary>
        /// This option is used to identify a TFTP server when the 'sname' field
        /// in the DHCP header has been used for DHCP options.
        /// The code for this option is 66, and its minimum length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.AsciiString)]
        TftpServerName = 66,
        /// <summary>
        /// This option is used to identify a bootfile when the 'file' field in
        /// the DHCP header has been used for DHCP options.
        /// The code for this option is 67, and its minimum length is 1. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.AsciiString)]
        BootFileName = 67,
        /// <summary>
        /// This option specifies a list of IP addresses indicating mobile IP
        /// home agents available to the client.  Agents SHOULD be listed in
        /// order of preference.
        /// The code for this option is 68.  Its minimum length is 0 (indicating
        /// no home agents are available) and the length MUST be a multiple of 4.
        /// It is expected that the usual length will be four octets, containing
        /// a single home agent's address. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        MobileIpHomeAgents = 68,
        /// <summary>
        /// The SMTP server option specifies a list of SMTP servers available to
        /// the client.  Servers SHOULD be listed in order of preference.
        /// The code for the SMTP server option is 69.  The minimum length for
        /// this option is 4 octets, and the length MUST always be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        SmtpServer = 69,
        /// <summary>
        /// The POP3 server option specifies a list of POP3 available to the
        /// client.  Servers SHOULD be listed in order of preference.
        /// The code for the POP3 server option is 70.  The minimum length for
        /// this option is 4 octets, and the length MUST always be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        Pop3Server = 70,
        /// <summary>
        /// The NNTP server option specifies a list of NNTP available to the
        /// client.  Servers SHOULD be listed in order of preference.
        /// The code for the NNTP server option is 71. The minimum length for
        /// this option is 4 octets, and the length MUST always be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        NntpServer = 71,
        /// <summary>
        /// The WWW server option specifies a list of WWW available to the
        /// client.  Servers SHOULD be listed in order of preference.
        /// The code for the WWW server option is 72.  The minimum length for
        /// this option is 4 octets, and the length MUST always be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        WwwServer = 72,
        /// <summary>
        /// The Finger server option specifies a list of Finger available to the
        /// client.  Servers SHOULD be listed in order of preference.
        /// The code for the Finger server option is 73.  The minimum length for
        /// this option is 4 octets, and the length MUST always be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        DefaultFingerServer = 73,
        /// <summary>
        /// The IRC server option specifies a list of IRC available to the
        /// client.  Servers SHOULD be listed in order of preference.
        /// The code for the IRC server option is 74.  The minimum length for
        /// this option is 4 octets, and the length MUST always be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        DefaultIrcServer = 74,
        /// <summary>
        /// The StreetTalk server option specifies a list of StreetTalk servers
        /// available to the client.  Servers SHOULD be listed in order of preference.
        /// The code for the StreetTalk server option is 75.  The minimum length
        /// for this option is 4 octets, and the length MUST always be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        StreetTalkServer = 75,
        /// <summary>
        /// The StreetTalk Directory Assistance (STDA) server option specifies a
        /// list of STDA servers available to the client.  Servers SHOULD be
        /// listed in order of preference.
        /// The code for the StreetTalk Directory Assistance server option is 76.
        /// The minimum length for this option is 4 octets, and the length MUST
        /// always be a multiple of 4. See [RFC2132]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        StreetTalkDirectoryAssistanceServer = 76,

        /// <summary>
        /// User Class Information
        /// See [RFC3004]
        /// </summary>
        UserClass = 77,
        /// <summary>
        /// directory agent information
        /// See [RFC2610]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        DirectoryAgent = 78,
        /// <summary>
        /// service location agent scope
        /// See [RFC2610]
        /// </summary>
        [OptionTagType(OptionTagTypes.Utf8String)]
        ServiceScope = 79,
        /// <summary>
        /// Rapid Commit
        /// See [RFC4039]
        /// </summary>
        [OptionTagType(OptionTagTypes.ZeroLengthFlag)]
        RapidCommit = 80,
        /// <summary>
        /// Fully Qualified Domain Name
        /// See [RFC4702]
        /// </summary>
        [OptionTagType(OptionTagTypes.ClientFQDN)]
        ClientFQDN = 81,
        /// <summary>
        /// Relay Agent Information
        /// See [RFC3046]
        /// </summary>
        RelayAgentInformation = 82,
        /// <summary>
        /// Internet Storage Name Service
        /// See [RFC4174]
        /// </summary>
        InternetStorageNameService = 83,
        /// <summary>
        /// Novell Directory Services
        /// See [RFC2241]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        NdsServers = 85,
        /// <summary>
        /// Novell Directory Services
        /// See [RFC2241]
        /// </summary>
        [OptionTagType(OptionTagTypes.Utf8String)]
        NdsTreeName = 86,
        /// <summary>
        /// Novell Directory Services
        /// See [RFC2241]
        /// </summary>
        [OptionTagType(OptionTagTypes.Utf8String)]
        NdsContext = 87,
        /// <summary>
        /// The option MAY contain multiple domain names, but these domain names
        /// SHOULD be used to construct Service Record (SRV) lookups as specified
        /// in [BCMCS], rather than querying for different A records.  The client
        /// can try any or ALL of the domain names to construct the SRV lookups.
        /// The list of domain names MAY contain the domain name of the access
        /// provider and its partner networks that also offer Broadcast and
        /// Multicast Service.
        /// As an example, the access provider may have one or more partners or
        /// resellers often termed as MVNOs (Mobile Virtual Network Operators)
        /// for Broadcast and Multicast Service.  In this case, the access
        /// provider should be able to use the same DHCP option to send multiple
        /// of those domain names (MVNOs).  To illustrate this further, let's
        /// assume that the access provider (operator) has a reseller agreement
        /// with two MVNOs: mvno1 and mvno2.  Therefore, the Broadcast and
        /// Multicast Service Controller Domain Name list for the DHCPv4 option
        /// will contain three domain names: operator.com, mvno1.com, and
        /// mvno2.com.  Upon receiving this option, the BCMCS client may choose
        /// to use one of the domain names to fetch the appropriate BCMCS
        /// controller address (based on user's preference or configuration).  If
        /// no preferred domain name is found in the received list, the client
        /// should use a default setting, e.g., use the first one in the list.
        /// If the length of the domain list exceeds the maximum permissible
        /// length within a single option (254 octets), then the domain list MUST
        /// be represented in the DHCPv4 message as specified in [RFC3396].
        /// See [RFC4280]
        /// </summary>
        [OptionTagType(OptionTagTypes.DnsNameList)]
        BroadcastAndMulticastServiceControllerDomainNameList = 88,
        /// <summary>
        /// The Length byte (Len) is followed by a list of IPv4 addresses
        /// indicating BCMCS controller IPv4 addresses.  The BCMCS controllers
        /// MUST be listed in order of preference.  Its minimum length is 4, and
        /// the length MUST be a multiple of 4.
        /// See [RFC4280]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        BroadcastAndMulticastServiceControllerIpv4Addresses = 89,
        /// <summary>
        /// Authentication
        /// See [RFC3118]
        /// </summary>
        Authentication = 90,
        /// <summary>
        /// Client Last Transaction Time
        /// See [RFC4388]
        /// </summary>
        [OptionTagType(OptionTagTypes.UInt32)]
        ClientLastTransactionTime = 91,
        /// <summary>
        /// 
        /// See [RFC4388]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        AssociatedIp = 92,
        /// <summary>
        /// Client System Architecture
        /// See [RFC4578]
        /// </summary>
        [OptionTagType(OptionTagTypes.UInt16)]
        ClientSystem = 93,
        /// <summary>
        /// Client Network Device Interface
        /// See [RFC4578]
        /// </summary>
        ClientNDI = 94,
        /// <summary>
        /// Lightweight Directory Access Protocol
        /// See [RFC3679]
        /// </summary>
        LDAP = 95,
        /// <summary>
        /// UUID/GUID-based Client Identifier
        /// See [RFC4578]
        /// </summary>
        [OptionTagType(OptionTagTypes.ClientUUID)]
        ClientUUID = 97,
        /// <summary>
        /// Open Group's User Authentication
        /// See [RFC2485]
        /// </summary>
        [OptionTagType(OptionTagTypes.AsciiString)]
        UserAuth = 98,
        /// <summary>
        /// 
        /// See [RFC4776]
        /// </summary>
        GEOCONF_CIVIC = 99,
        /// <summary>
        /// IEEE 1003.1 TZ String
        /// See [RFC4833]
        /// </summary>
        [OptionTagType(OptionTagTypes.AsciiString)]
        PCode = 100,
        /// <summary>
        /// Reference to the TZ Database
        /// See [RFC4833]
        /// </summary>
        [OptionTagType(OptionTagTypes.AsciiString)]
        TCode = 101,
        /// <summary>
        /// NetInfo Parent Server Address
        /// See [RFC3679]
        /// </summary>
        NetInfoAddress = 112,
        /// <summary>
        /// NetInfo Parent Server Tag
        /// See [RFC3679]
        /// </summary>
        NetInfoTag = 113,
        /// <summary>
        /// URL
        /// See [RFC3679]
        /// </summary>
        URL = 114,
        /// <summary>
        /// DHCP Auto-Configuration
        /// See [RFC2563]
        /// </summary>
        [OptionTagType(OptionTagTypes.Byte)]
        AutoConfiguration = 116,
        /// <summary>
        /// Name Service Search
        /// See [RFC2937]
        /// </summary>
        [OptionTagType(OptionTagTypes.UInt16List)]
        NameServiceSearch = 117,
        /// <summary>
        /// Subnet Selection Option
        /// See [RFC3011]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddress)]
        SubnetSelection = 118,
        /// <summary>
        /// DNS domain search list
        /// See [RFC3397]
        /// </summary>
        [OptionTagType(OptionTagTypes.DnsNameList)]
        DomainSearch = 119,
        /// <summary>
        /// SIP Servers DHCP Option
        /// See [RFC3361]
        /// </summary>
        [OptionTagType(OptionTagTypes.SipServers)]
        SipServers = 120,

        /// <summary>
        /// Classless Static Route Option.
        /// The code for this option is 121, and its minimum length is 5 bytes.
        /// This option can contain one or more static routes, each of which
        /// consists of a destination descriptor and the IP address of the router
        /// that should be used to reach that destination.
        /// See [RFC3442]
        /// </summary>
        ClasslessStaticRoute = 121,

        /// <summary>
        /// CableLabs Client Configuration
        /// See [RFC3495]
        /// </summary>
        CableLabsClientConfiguration = 122,
        /// <summary>
        /// GeoConf Option
        /// See [RFC6225]
        /// </summary>
        GeoConfOption = 123,
        /// <summary>
        /// Vendor-Identifying Vendor Class
        /// See [RFC3925]
        /// </summary>
        VendorIdentifyingVendorClass = 124,
        /// <summary>
        /// Vendor-Identifying Vendor-Specific Information
        /// See [RFC3925]
        /// </summary>
        VendorIdentifyingVendorSpecificInformation = 125,
        /// <summary>
        /// From IANA: PXE (vendor specific) See [RFC4578]
        /// 6 bytes: E4:45:74:68:00:00
        /// </summary>
        PxeEtherbootSignature = 128,
        /// <summary>
        /// From IANA: PXE (vendor specific) See [RFC4578]
        /// </summary>
        PxeDocsisFullSecurityServiceIpAddress = 128,
        /// <summary>
        /// From IANA: PXE (vendor specific) See [RFC4578]
        /// TFTP Server IP address (for IP Phone software load)
        /// </summary>
        PxeTftpServerIpAddress = 128,
        /// <summary>
        /// From IANA: PXE (vendor specific) See [RFC4578]
        /// Kernel Options. Variable length string
        /// </summary>
        PxeKernelOptions = 129,
        /// <summary>
        /// From IANA: PXE (vendor specific) See [RFC4578]
        /// </summary>
        PxeCallServerIPaddress = 129,
        /// <summary>
        /// From IANA: PXE (vendor specific) See [RFC4578]
        /// Ethernet Interface. Variable length string.
        /// </summary>
        PxeEthernetInterface = 130,
        /// <summary>
        /// From IANA: PXE (vendor specific) See [RFC4578]
        /// Discrimination string (to identify vendor)
        /// </summary>
        PxeDiscriminationString = 130,
        /// <summary>
        /// From IANA: PXE (vendor specific) See [RFC4578]
        /// </summary>
        PxeRemoteStatisticsServerIpAddress = 131,
        /// <summary>
        /// From IANA: PXE (vendor specific) See [RFC4578]
        /// </summary>
        PxeIeee8021QVlanId = 132,
        /// <summary>
        /// From IANA: PXE (vendor specific) See [RFC4578]
        /// </summary>
        PxeIeee8021DpLayer2Priority = 133,
        /// <summary>
        /// From IANA: PXE (vendor specific) See [RFC4578]
        /// Diffserv Code Point (DSCP) for VoIP signalling and media streams
        /// </summary>
        PxeDiffservCodePoint = 134,
        /// <summary>
        /// From IANA: PXE (vendor specific) See [RFC4578]
        /// HTTP Proxy for phone-specific applications
        /// </summary>
        PxeHttpProxy = 135,
        /// <summary>
        /// PANA Authentication Agent DHCPv4
        /// See [RFC5192]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        PanaAuthenticationAgent = 136,
        /// <summary>
        /// 
        /// See [RFC5223]
        /// </summary>
        [OptionTagType(OptionTagTypes.DnsName)]
        LostServer = 137,
        /// <summary>
        /// CAPWAP Access Controller addresses
        /// See [RFC5417]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        CapwapAc = 138,
        /// <summary>
        /// a series of suboptions
        /// See [RFC5678]
        /// </summary>
        IPv4_Address_MoS = 139,
        /// <summary>
        /// a series of suboptions
        /// See [RFC5678]
        /// </summary>
        IPv4_FQDN_MoS = 140,
        /// <summary>
        /// List of domain names to search for SIP User Agent Configuration
        /// See [RFC6011]
        /// </summary>
        SIPUAConfigurationServiceDomains = 141,
        /// <summary>
        /// ANDSF IPv4 Address Option for DHCPv4
        /// See [RFC6153]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        IPv4_Address_ANDSF = 142,
        /// <summary>
        /// Geospatial Location with Uncertainty
        /// See [RFC6225]
        /// </summary>
        GeoLoc = 144,
        /// <summary>
        /// Forcerenew Nonce Capable
        /// See [RFC6704]
        /// </summary>
        FORCERENEW_NONCE_CAPABLE = 145,
        /// <summary>
        /// Information for selecting RDNSS
        /// See [RFC6731]
        /// </summary>
        RDNSSSelection = 146,
        /// <summary>
        /// 
        /// See [RFC5859]
        /// </summary>
        [OptionTagType(OptionTagTypes.IpAddressList)]
        TftpServerAddress = 150,
        /// <summary>
        /// 
        /// See 
        /// </summary>
        Etherboot150 = 150,
        /// <summary>
        /// 
        /// See 
        /// </summary>
        GRUBconfigurationpathname = 150,
        /// <summary>
        /// Status code and optional N byte text message describing status.
        /// See [RFC6926]
        /// </summary>
        [OptionTagType(OptionTagTypes.StatusCode)]
        StatusCode = 151,
        /// <summary>
        /// Absolute time (seconds since Jan 1, 1970) message was sent.
        /// See [RFC6926]
        /// </summary>
        [OptionTagType(OptionTagTypes.UInt32)]
        BaseTime = 152,
        /// <summary>
        /// Number of seconds in the past when client entered current state.
        /// See [RFC6926]
        /// </summary>
        [OptionTagType(OptionTagTypes.UInt32)]
        StartTimeOfState = 153,
        /// <summary>
        /// Absolute time (seconds since Jan 1, 1970) for beginning of query.
        /// See [RFC6926]
        /// </summary>
        [OptionTagType(OptionTagTypes.UInt32)]
        QueryStartTime = 154,
        /// <summary>
        /// Absolute time (seconds since Jan 1, 1970) for end of query.
        /// See [RFC6926]
        /// </summary>
        [OptionTagType(OptionTagTypes.UInt32)]
        QueryEndTime = 155,
        /// <summary>
        /// State of IP address.
        /// See [RFC6926]
        /// </summary>
        [OptionTagType(OptionTagTypes.DhcpState)]
        DhcpState = 156,
        /// <summary>
        /// Indicates information came from local or remote server.
        /// See [RFC6926]
        /// </summary>
        [OptionTagType(OptionTagTypes.Byte)]
        DataSource = 157,
        /// <summary>
        /// Includes one or multiple lists of PCP server IP addresses; 
        /// each list is treated as a separate PCP server.
        /// See [RFC7291]
        /// </summary>
        OPTION_V4_PCP_SERVER = 158,
        /// <summary>
        /// This option is used to configure a set of ports bound to a 
        /// shared IPv4 address.
        /// /// See [RFC7618]
        /// </summary>
        OPTION_V4_PORTPARAMS = 159,
        /// <summary>
        /// DHCP Captive-Portal
        /// See [RFC7710]
        /// </summary>
        [OptionTagType(OptionTagTypes.Utf8String)]
        DhcpCaptivePortal = 160,
        /// <summary>
        /// Manufacturer Usage Descriptions
        /// (TEMPORARY - registered 2016-11-17, expires 2017-11-17)
        /// See [draft-ietf-opsawg-mud]
        /// </summary>
        OPTION_MUD_URL_V4 = 161,
        /// <summary>
        /// Etherboot (Tentatively Assigned - 2005-06-23)
        /// </summary>
        Etherboot175 = 175,
        /// <summary>
        /// IP Telephone (Tentatively Assigned - 2005-06-23)
        /// </summary>
        IpTelephone = 176,
        /// <summary>
        /// Etherboot (Tentatively Assigned - 2005-06-23)
        /// </summary>
        Etherboot177 = 177,
        /// <summary>
        /// PacketCable and CableHome (replaced by 122)
        /// </summary>
        PacketCableAndCableHome = 177,
        /// <summary>
        /// magic string = F1:00:74:7E
        /// See [RFC5071][Deprecated]
        /// </summary>
        [Obsolete]
        PxeLinuxMagic = 208,
        /// <summary>
        /// Configuration file
        /// See [RFC5071]
        /// </summary>
        [OptionTagType(OptionTagTypes.AsciiString)]
        ConfigurationFile = 209,
        /// <summary>
        /// Path Prefix Option
        /// See [RFC5071]
        /// </summary>
        [OptionTagType(OptionTagTypes.AsciiString)]
        PathPrefix = 210,
        /// <summary>
        /// Reboot Time
        /// See [RFC5071]
        /// </summary>
        [OptionTagType(OptionTagTypes.UInt32)]
        RebootTime = 211,
        /// <summary>
        /// OPTION_6RD with N/4 6rd BR addresses
        /// See [RFC5969]
        /// </summary>
        OPTION_6RD = 212,
        /// <summary>
        /// Access Network Domain Name
        /// See [RFC5986]
        /// </summary>
        [OptionTagType(OptionTagTypes.DnsNameList)]
        OPTION_V4_ACCESS_DOMAIN = 213,
        /// <summary>
        /// Subnet Allocation Option
        /// See [RFC6656]
        /// </summary>
        SubnetAllocationOption = 220,
        /// <summary>
        /// 
        /// See [RFC6607]
        /// </summary>
        VirtualSubnetSelection = 221,

        /// <summary>
        /// Classless Static Route Option.
        /// All Windows DHCP clients and servers prior to Windows Vista and Windows Server 2008 use Option Code 249
        /// for requesting and sending Classless Static Routes (CSRs) instead of Option Code 121, as specified in [RFC3442].
        /// These clients and servers ignore Option 121 if included in a DHCP message.
        /// Windows Vista and Windows Server 2008 DHCP clients use both Option 121 and Option 249.
        /// See https://msdn.microsoft.com/en-us/library/cc202606.aspx and https://msdn.microsoft.com/en-us/library/cc202639.aspx
        /// </summary>
        [Obsolete]
        ClasslessStaticRouteWindowsNonStandard = 249,

        /// <summary>
        /// Web Proxy Auto-Discovery Protocol URL.
        /// Non-standard, Site-local; see: https://en.wikipedia.org/wiki/Web_Proxy_Auto-Discovery_Protocol
        /// </summary>
        [OptionTagType(OptionTagTypes.Utf8String)]
        AutoProxyConfig = 252,
    }
}
