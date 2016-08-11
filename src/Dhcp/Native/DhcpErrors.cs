
namespace Dhcp.Native
{
    internal enum DhcpErrors : uint
    {
        [DhcpErrorDescription("Success")]
        SUCCESS = 0,

        [DhcpErrorDescription("More data is available.")]
        ERROR_MORE_DATA = 234,

        [DhcpErrorDescription("No more data is available.")]
        ERROR_NO_MORE_ITEMS = 259,

        [DhcpErrorDescription("There are no more endpoints available from the endpoint mapper.")]
        EPT_S_NOT_REGISTERED = 1753,

        [DhcpErrorDescription("The RPC server is unavailable.")]
        RPC_S_SERVER_UNAVAILABLE = 1722,

        [DhcpErrorDescription("The UUID type is not supported.")]
        RPC_S_UNSUPPORTED_TYPE = 1734,

        [DhcpErrorDescription("This call was performed by a client who is not a member of the \"DHCP Administrators\" security group.")]
        ERROR_ACCESS_DENIED = 5,
        [DhcpErrorDescription("The parameter is incorrect.")]
        ERROR_INVALID_PARAMETER = 87,

        [DhcpErrorDescription("The system cannot find the file specified.")]
        ERROR_FILE_NOT_FOUND = 2,

        [DhcpErrorDescription("The DHCP server registry initialization parameters are incorrect.")]
        REGISTRY_INIT_FAILED = 20000,
        [DhcpErrorDescription("The DHCP server was unable to open the database of DHCP clients.")]
        DATABASE_INIT_FAILED = 20001,
        [DhcpErrorDescription("The DHCP server was unable to start as a Remote Procedure Call (RPC) server.")]
        RPC_INIT_FAILED = 20002,
        [DhcpErrorDescription("The DHCP server was unable to establish a socket connection.")]
        NETWORK_INIT_FAILED = 20003,
        [DhcpErrorDescription("The specified subnet already exists on the DHCP server.")]
        SUBNET_EXISTS = 20004,
        [DhcpErrorDescription("The specified subnet does not exist on the DHCP server.")]
        SUBNET_NOT_PRESENT = 20005,
        [DhcpErrorDescription("The primary host information for the specified subnet was not found on the DHCP server.")]
        PRIMARY_NOT_FOUND = 20006,
        [DhcpErrorDescription("The specified DHCP element has been used by a client and cannot be removed.")]
        ELEMENT_CANT_REMOVE = 20007,
        [DhcpErrorDescription("The specified option already exists on the DHCP server.")]
        OPTION_EXISTS = 20009,
        [DhcpErrorDescription("The specified option does not exist on the DHCP server.")]
        OPTION_NOT_PRESENT = 20010,
        [DhcpErrorDescription("The specified IP address is not available.")]
        ADDRESS_NOT_AVAILABLE = 20011,
        [DhcpErrorDescription("The specified IP address range has all of its member addresses leased.")]
        RANGE_FULL = 20012,
        [DhcpErrorDescription(@"An error occurred while accessing the DHCP JET database. For more information about this error, please look at the DHCP server event log.")]
        JET_ERROR = 20013,
        [DhcpErrorDescription("The specified client already exists in the database.")]
        CLIENT_EXISTS = 20014,
        [DhcpErrorDescription("The DHCP server received an invalid message.")]
        INVALID_DHCP_MESSAGE = 20015,
        [DhcpErrorDescription("The DHCP server received an invalid message from the client.")]
        INVALID_DHCP_CLIENT = 20016,
        [DhcpErrorDescription("The DHCP server is currently paused.")]
        SERVICE_PAUSED = 20017,
        [DhcpErrorDescription("The specified DHCP client is not a reserved client.")]
        NOT_RESERVED_CLIENT = 20018,
        [DhcpErrorDescription("The specified DHCP client is a reserved client.")]
        RESERVED_CLIENT = 20019,
        [DhcpErrorDescription("The specified IP address range is too small.")]
        RANGE_TOO_SMALL = 20020,
        [DhcpErrorDescription("The specified IP address range is already defined on the DHCP server.")]
        IPRANGE_EXISTS = 20021,
        [DhcpErrorDescription("The specified IP address is currently taken by another client.")]
        RESERVEDIP_EXISTS = 20022,
        [DhcpErrorDescription("The specified IP address range either overlaps with an existing range or is invalid.")]
        INVALID_RANGE = 20023,
        [DhcpErrorDescription("The specified IP address range is an extension of an existing range.")]
        RANGE_EXTENDED = 20024,
        [DhcpErrorDescription(@"The specified IP address range extension is too small. The number of addresses in the extension must be a multiple of 32.")]
        RANGE_EXTENSION_TOO_SMALL = 20025,
        [DhcpErrorDescription(@"An attempt was made to extend the IP address range to a value less than the specified backward extension. The number of addresses in the extension must be a multiple of 32. ")]
        WARNING_RANGE_EXTENDED_LESS = 20026,
        [DhcpErrorDescription(@"The DHCP database needs to be upgraded to a newer format. For more information, refer to the DHCP server event log.")]
        JET_CONV_REQUIRED = 20027,
        [DhcpErrorDescription(@"The format of the bootstrap protocol file table is incorrect. The correct format is:
<requested boot file name 1>,<boot file server name 1>, <boot file name 1>
<requested boot file name 2>,<boot file server name 2>, <boot file name 2>
...")]
        SERVER_INVALID_BOOT_FILE_TABLE = 20027,
        [DhcpErrorDescription("A boot file name specified in the bootstrap protocol file table is unrecognized or invalid.")]
        SERVER_UNKNOWN_BOOT_FILE_NAME = 20029,
        [DhcpErrorDescription("The specified superscope name is too long.")]
        SUPER_SCOPE_NAME_TOO_LONG = 20030,
        [DhcpErrorDescription("The specified IP address is already in use.")]
        IP_ADDRESS_IN_USE = 20032,
        [DhcpErrorDescription("The specified path to the DHCP audit log file is too long.")]
        LOG_FILE_PATH_TOO_LONG = 20033,
        [DhcpErrorDescription("The DHCP server received a request for a valid IP address not administered by the server.")]
        UNSUPPORTED_CLIENT = 20034,
        [DhcpErrorDescription(@"The DHCP server failed to receive a notification when the interface list changed, therefore some of the interfaces will not be enabled on the server.")]
        SERVER_INTERFACE_NOTIFICATION_EVENT = 20035,
        [DhcpErrorDescription(@"The DHCP database needs to be upgraded to a newer format (JET97). For more information, refer to the DHCP server event log.")]
        JET97_CONV_REQUIRED = 20036,
        [DhcpErrorDescription(@"The DHCP server cannot determine if it has the authority to run, and is not servicing clients on the network. This rogue status may be due to network problems or insufficient server resources.")]
        ROGUE_INIT_FAILED = 20037,
        [DhcpErrorDescription("The DHCP service is shutting down because another DHCP server is active on the network.")]
        ROGUE_SAMSHUTDOWN = 20038,
        [DhcpErrorDescription("The DHCP server does not have the authority to run, and is not servicing clients on the network.")]
        ROGUE_NOT_AUTHORIZED = 20039,
        [DhcpErrorDescription(@"The DHCP server is unable to contact the directory service for this domain. The DHCP server will continue to attempt to contact the directory service. During this time, no clients on the network will be serviced.")]
        ROGUE_DS_UNREACHABLE = 20040,
        [DhcpErrorDescription("The DHCP server's authorization information conflicts with that of another DHCP server on the network.")]
        ROGUE_DS_CONFLICT = 20041,
        [DhcpErrorDescription(@"The DHCP server is ignoring a request from another DHCP server because the second server is a member of a different directory service enterprise.")]
        ROGUE_NOT_OUR_ENTERPRISE = 20042,
        [DhcpErrorDescription("The DHCP server has detected a directory service environment on the network. If there is a directory service on the network, the DHCP server can only run if it is a part of the directory service. Since the server ostensibly belongs to a workgroup, it is terminating.")]
        STANDALONE_IN_DS = 20043,
        [DhcpErrorDescription("The specified DHCP class name  is unknown or invalid.")]
        CLASS_NOT_FOUND = 20044,
        [DhcpErrorDescription("The specified DHCP class name (or information) is already in use.")]
        CLASS_ALREADY_EXISTS = 20045,
        [DhcpErrorDescription("The specified DHCP scope name is too long, the scope name must not exceed 256 characters.")]
        SCOPE_NAME_TOO_LONG = 20046,
        [DhcpErrorDescription("The default scope is already configured on the server.")]
        DEFAULT_SCOPE_EXISTS = 20047,
        [DhcpErrorDescription("The Dynamic BOOTP attribute cannot be turned on or off.")]
        CANT_CHANGE_ATTRIBUTE = 20048,
        [DhcpErrorDescription("Conversion of a scope to a \"DHCP Only\" scope or to a \"BOOTP Only\" scope is not allowed when the scope contains other DHCP and BOOTP clients. Either the DHCP or BOOTP clients should be specifically deleted before converting the scope to the other type.")]
        IPRANGE_CONV_ILLEGAL = 20049,
        [DhcpErrorDescription("The network has changed. Retry this operation after checking for network changes. Network changes may be caused by interfaces that are new or invalid, or by IP addresses that are new or invalid.")]
        NETWORK_CHANGED = 20050,
        [DhcpErrorDescription("The bindings to internal IP addresses cannot be modified.")]
        CANNOT_MODIFY_BINDINGS = 20051,
        [DhcpErrorDescription("The DHCP scope parameters are incorrect. Either the scope already exists, or its properties are inconsistent with the subnet address and mask of an existing scope.")]
        BAD_SCOPE_PARAMETERS = 20052,
        [DhcpErrorDescription("The DHCP multicast scope parameters are incorrect. Either the scope already exists, or its properties are inconsistent with the subnet address and mask of an existing scope.")]
        MSCOPE_EXISTS = 20053,
        [DhcpErrorDescription("The multicast scope range must have at least 256 IP addresses.")]
        MSCOPE_RANGE_TOO_SMALL = 20054,
        [DhcpErrorDescription("The DHCP server could not contact Active Directory.")]
        DDS_NO_DS_AVAILABLE = 20070,
        [DhcpErrorDescription("The DHCP service root could not be found in  Active Directory.")]
        DDS_NO_DHCP_ROOT = 20071,
        [DhcpErrorDescription("An unexpected error occurred while accessing  Active Directory.")]
        DDS_UNEXPECTED_ERROR = 20072,
        [DhcpErrorDescription("There were too many errors to proceed.")]
        DDS_TOO_MANY_ERRORS = 20073,
        [DhcpErrorDescription("A DHCP service could not be found.")]
        DDS_DHCP_SERVER_NOT_FOUND = 20074,
        [DhcpErrorDescription("The specified DHCP options are already present in  Active Directory.")]
        DDS_OPTION_ALREADY_EXISTS = 20075,
        [DhcpErrorDescription("The specified DHCP options are not present in  Active Directory.")]
        DDS_OPTION_DOES_NOT_EXIST = 20076,
        [DhcpErrorDescription("The specified DHCP classes are already present in  Active Directory.")]
        DDS_CLASS_EXISTS = 20077,
        [DhcpErrorDescription("The specified DHCP classes are not present in  Active Directory.")]
        DDS_CLASS_DOES_NOT_EXIST = 20078,
        [DhcpErrorDescription("The specified DHCP servers are already present in  Active Directory.")]
        DDS_SERVER_ALREADY_EXISTS = 20079,
        [DhcpErrorDescription("The specified DHCP servers are not present in  Active Directory.")]
        DDS_SERVER_DOES_NOT_EXIST = 20080,
        [DhcpErrorDescription("The specified DHCP server address does not correspond to the identified DHCP server name.")]
        DDS_SERVER_ADDRESS_MISMATCH = 20081,
        [DhcpErrorDescription("The specified subnets are already present in  Active Directory.")]
        DDS_SUBNET_EXISTS = 20082,
        [DhcpErrorDescription("The specified subnet belongs to a different superscope.")]
        DDS_SUBNET_HAS_DIFF_SUPER_SCOPE = 20083,
        [DhcpErrorDescription("The specified subnet is not present in  Active Directory.")]
        DDS_SUBNET_NOT_PRESENT = 20084,
        [DhcpErrorDescription("The specified reservation is not present in  Active Directory.")]
        DDS_RESERVATION_NOT_PRESENT = 20085,
        [DhcpErrorDescription("The specified reservation conflicts with another reservation present in  Active Directory.")]
        DDS_RESERVATION_CONFLICT = 20086,
        [DhcpErrorDescription("The specified IP address range conflicts with another IP range present in  Active Directory.")]
        DDS_POSSIBLE_RANGE_CONFLICT = 20087,
        [DhcpErrorDescription("The specified IP address range is not present in  Active Directory.")]
        DDS_RANGE_DOES_NOT_EXIST = 20088,
        [DhcpErrorDescription("Windows 7 or later: This class cannot be deleted.")]
        DELETE_BUILTIN_CLASS = 20089,
        [DhcpErrorDescription("Windows 7 or later: The given subnet prefix is invalid. It represents either a non-unicast or link local address range.")]
        INVALID_SUBNET_PREFIX = 20091,
        [DhcpErrorDescription("Windows 7 or later: The given delay value is invalid. The valid value is from 0 to 1000.")]
        INVALID_DELAY = 20092,
        [DhcpErrorDescription("Windows 7 or later: Address or Address pattern is already contained in one of the list.")]
        LINKLAYER_ADDRESS_EXISTS = 20093,
        [DhcpErrorDescription("Windows 7 or later: Address to be added to Deny list or to be deleted from allow list, has an associated reservation.")]
        LINKLAYER_ADDRESS_RESERVATION_EXISTS = 20094,
        [DhcpErrorDescription("Windows 7 or later: Address or Address pattern is not contained in either list.")]
        LINKLAYER_ADDRESS_DOES_NOT_EXIST = 20095,
        [DhcpErrorDescription("Windows 7 or later: This Hardware Type is already exempt.")]
        HARDWARE_ADDRESS_TYPE_ALREADY_EXEMPT = 20101,
        [DhcpErrorDescription("Windows 7 or later: You are trying to delete an undefined Hardware Type.")]
        UNDEFINED_HARDWARE_ADDRESS_TYPE = 20102,
        [DhcpErrorDescription("Windows 7 or later: Conflict in types for the same option on Host and Added DHCP Servers.")]
        OPTION_TYPE_MISMATCH = 20103,
        [DhcpErrorDescription("Windows 8 or later: The parent expression specified does not exist.")]
        POLICY_BAD_PARENT_EXPR = 20104,
        [DhcpErrorDescription("Windows 8 or later: The DHCP server policy already exists.")]
        POLICY_EXISTS = 20105,
        [DhcpErrorDescription("Windows 8 or later: The DHCP server policy range specified already exists in the given scope.")]
        POLICY_RANGE_EXISTS = 20106,
        [DhcpErrorDescription("Windows 8 or later: The DHCP server policy range specified is invalid or does not match the given subnet.")]
        POLICY_RANGE_BAD = 20107,
        [DhcpErrorDescription("Windows 8 or later: DHCP server policy ranges can only be added to scope level policies.")]
        RANGE_INVALID_IN_SERVER_POLICY = 20108,
        [DhcpErrorDescription("Windows 8 or later: The DHCP server policy contains an invalid expression.")]
        INVALID_POLICY_EXPRESSION = 20109,
        [DhcpErrorDescription("Windows 8 or later: The processing order specified for the DHCP server policy is invalid.")]
        INVALID_PROCESSING_ORDER = 20110,
        [DhcpErrorDescription("Windows 8 or later: The DHCP server policy was not found.")]
        POLICY_NOT_FOUND = 20111,
        [DhcpErrorDescription("Windows 8 or later: There is an IP address range configured for a policy in this scope. This operation on the scope IP address range cannot be performed until the policy IP address range is suitably modified. Please change the IP address range of the policy before performing this operation.")]
        SCOPE_RANGE_POLICY_RANGE_CONFLICT = 20112,
        [DhcpErrorDescription("Windows 8 or later: The DHCP scope is already in a failover relationship.")]
        FO_SCOPE_ALREADY_IN_RELATIONSHIP = 20113,
        [DhcpErrorDescription("Windows 8 or later: The DHCP failover relationship already exists.")]
        FO_RELATIONSHIP_EXISTS = 20114,
        [DhcpErrorDescription("Windows 8 or later: The DHCP failover relationship does not exist.")]
        FO_RELATIONSHIP_DOES_NOT_EXIST = 20115,
        [DhcpErrorDescription("Windows 8 or later: The DHCP scope is not part of a failover relationship.")]
        FO_SCOPE_NOT_IN_RELATIONSHIP = 20116,
        [DhcpErrorDescription("Windows 8 or later: The DHCP failover relationship is a secondary.")]
        FO_RELATION_IS_SECONDARY = 20117,
        [DhcpErrorDescription("Windows 8 or later: The DHCP failover is not supported.")]
        FO_NOT_SUPPORTED = 20118,
        [DhcpErrorDescription("Windows 8 or later: The DHCP servers in the failover relationship have timed out of synchronization.")]
        FO_TIME_OUT_OF_SYNC = 20119,
        [DhcpErrorDescription("Windows 8 or later: The DHCP failover relationship state is not NORMAL.")]
        FO_STATE_NOT_NORMAL = 20120,
        [DhcpErrorDescription("Windows 8 or later: The user does not have administrative permissions for the DHCP server.")]
        NO_ADMIN_PERMISSION = 20121,
        [DhcpErrorDescription("Windows 8 or later: The specified DHCP server is not reachable. Please provide a DHCP server that is reachable.")]
        SERVER_NOT_REACHABLE = 20122,
        [DhcpErrorDescription("Windows 8 or later: The DHCP Server Service is not running on the specified server. Please ensure that the DHCP Server service is running on the specified computer.")]
        SERVER_NOT_RUNNING = 20123,
        [DhcpErrorDescription("Windows 8 or later: Unable to resolve DNS name.")]
        SERVER_NAME_NOT_RESOLVED = 20124,
        [DhcpErrorDescription("Windows 8 or later: The specified DHCP failover relationship name is too long. The name is limited to a maximum of 126 characters.")]
        FO_RELATIONSHIP_NAME_TOO_LONG = 20125,
        [DhcpErrorDescription("Windows 8 or later: The specified DHCP Server has reached the end of the selected range while finding the free IP address.")]
        REACHED_END_OF_SELECTION = 20126,
        [DhcpErrorDescription("Windows 8 or later: The synchronization of leases in the scopes being added to the failover relationship  failed.")]
        FO_ADDSCOPE_LEASES_NOT_SYNCED = 20127,
        [DhcpErrorDescription("Windows 8 or later: The relationship cannot be created on the DHCP server as the maximum number of allowed relationship has been exceeded.")]
        FO_MAX_RELATIONSHIPS = 20128,
        [DhcpErrorDescription("Windows 8 or later: A Scope configured for failover cannot be changed to type BOOTP or BOTH.")]
        FO_IPRANGE_TYPE_CONV_ILLEGAL = 20129,
        [DhcpErrorDescription("Windows 8 or later: The number of scopes being added to the failover relationship exceeds the max number of scopes which can be added to a failover relationship at one time.")]
        FO_MAX_ADD_SCOPES = 20130,
        [DhcpErrorDescription("Windows 8 or later: A scope supporting BOOTP clients cannot be added to a failover relationship.")]
        FO_BOOT_NOT_SUPPORTED = 20131,
        [DhcpErrorDescription("Windows 8 or later: An IP address range of a scope which is part of a failover relationship cannot be deleted. The scope will need to be removed from the failover relationship before deleting the range.")]
        FO_RANGE_PART_OF_REL = 20132,
    }
}
