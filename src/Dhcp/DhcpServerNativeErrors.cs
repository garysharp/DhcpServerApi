using Dhcp.Native;

namespace Dhcp
{
    public enum DhcpServerNativeErrors : uint
    {
        /// <summary>
        /// Success
        /// </summary>
        [DhcpServerNativeErrorDescription("Success")]
        SUCCESS = 0,

        /// <summary>
        /// More data is available.
        /// </summary>
        [DhcpServerNativeErrorDescription("More data is available.")]
        ERROR_MORE_DATA = 234,

        /// <summary>
        /// No more data is available.
        /// </summary>
        [DhcpServerNativeErrorDescription("No more data is available.")]
        ERROR_NO_MORE_ITEMS = 259,

        /// <summary>
        /// There are no more endpoints available from the endpoint mapper.
        /// </summary>
        [DhcpServerNativeErrorDescription("There are no more endpoints available from the endpoint mapper.")]
        EPT_S_NOT_REGISTERED = 1753,

        /// <summary>
        /// The RPC server is unavailable.
        /// </summary>
        [DhcpServerNativeErrorDescription("The RPC server is unavailable.")]
        RPC_S_SERVER_UNAVAILABLE = 1722,

        /// <summary>
        /// The UUID type is not supported.
        /// </summary>
        [DhcpServerNativeErrorDescription("The UUID type is not supported.")]
        RPC_S_UNSUPPORTED_TYPE = 1734,

        /// <summary>
        /// This call was performed by a client who is not a member of the "DHCP Administrators" security group.
        /// </summary>
        [DhcpServerNativeErrorDescription("This call was performed by a client who is not a member of the \"DHCP Administrators\" security group.")]
        ERROR_ACCESS_DENIED = 5,
        /// <summary>
        /// The parameter is incorrect.
        /// </summary>
        [DhcpServerNativeErrorDescription("The parameter is incorrect.")]
        ERROR_INVALID_PARAMETER = 87,

        /// <summary>
        /// The system cannot find the file specified.
        /// </summary>
        [DhcpServerNativeErrorDescription("The system cannot find the file specified.")]
        ERROR_FILE_NOT_FOUND = 2,

        /// <summary>
        /// The DHCP server registry initialization parameters are incorrect.
        /// </summary>
        [DhcpServerNativeErrorDescription("The DHCP server registry initialization parameters are incorrect.")]
        REGISTRY_INIT_FAILED = 20000,

        /// <summary>
        /// The DHCP server was unable to open the database of DHCP clients.
        /// </summary>
        [DhcpServerNativeErrorDescription("The DHCP server was unable to open the database of DHCP clients.")]
        DATABASE_INIT_FAILED = 20001,

        /// <summary>
        /// The DHCP server was unable to start as a Remote Procedure Call (RPC) server.
        /// </summary>
        [DhcpServerNativeErrorDescription("The DHCP server was unable to start as a Remote Procedure Call (RPC) server.")]
        RPC_INIT_FAILED = 20002,

        /// <summary>
        /// The DHCP server was unable to establish a socket connection.
        /// </summary>
        [DhcpServerNativeErrorDescription("The DHCP server was unable to establish a socket connection.")]
        NETWORK_INIT_FAILED = 20003,

        /// <summary>
        /// The specified subnet already exists on the DHCP server.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified subnet already exists on the DHCP server.")]
        SUBNET_EXISTS = 20004,

        /// <summary>
        /// The specified subnet does not exist on the DHCP server.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified subnet does not exist on the DHCP server.")]
        SUBNET_NOT_PRESENT = 20005,

        /// <summary>
        /// The primary host information for the specified subnet was not found on the DHCP server.
        /// </summary>
        [DhcpServerNativeErrorDescription("The primary host information for the specified subnet was not found on the DHCP server.")]
        PRIMARY_NOT_FOUND = 20006,

        /// <summary>
        /// The specified DHCP element has been used by a client and cannot be removed.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified DHCP element has been used by a client and cannot be removed.")]
        ELEMENT_CANT_REMOVE = 20007,

        /// <summary>
        /// The specified option already exists on the DHCP server.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified option already exists on the DHCP server.")]
        OPTION_EXISTS = 20009,

        /// <summary>
        /// The specified option does not exist on the DHCP server.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified option does not exist on the DHCP server.")]
        OPTION_NOT_PRESENT = 20010,

        /// <summary>
        /// The specified IP address is not available.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified IP address is not available.")]
        ADDRESS_NOT_AVAILABLE = 20011,

        /// <summary>
        /// The specified IP address range has all of its member addresses leased.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified IP address range has all of its member addresses leased.")]
        RANGE_FULL = 20012,

        /// <summary>
        /// "An error occurred while accessing the DHCP JET database. For more information about this error, please look at the DHCP server event log.
        /// </summary>
        [DhcpServerNativeErrorDescription(@"An error occurred while accessing the DHCP JET database. For more information about this error, please look at the DHCP server event log.")]
        JET_ERROR = 20013,

        /// <summary>
        /// The specified client already exists in the database.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified client already exists in the database.")]
        CLIENT_EXISTS = 20014,

        /// <summary>
        /// The DHCP server received an invalid message.
        /// </summary>
        [DhcpServerNativeErrorDescription("The DHCP server received an invalid message.")]
        INVALID_DHCP_MESSAGE = 20015,

        /// <summary>
        /// The DHCP server received an invalid message from the client.
        /// </summary>
        [DhcpServerNativeErrorDescription("The DHCP server received an invalid message from the client.")]
        INVALID_DHCP_CLIENT = 20016,

        /// <summary>
        /// The DHCP server is currently paused.
        /// </summary>
        [DhcpServerNativeErrorDescription("The DHCP server is currently paused.")]
        SERVICE_PAUSED = 20017,

        /// <summary>
        /// The specified DHCP client is not a reserved client.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified DHCP client is not a reserved client.")]
        NOT_RESERVED_CLIENT = 20018,

        /// <summary>
        /// The specified DHCP client is a reserved client.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified DHCP client is a reserved client.")]
        RESERVED_CLIENT = 20019,

        /// <summary>
        /// The specified IP address range is too small.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified IP address range is too small.")]
        RANGE_TOO_SMALL = 20020,

        /// <summary>
        /// The specified IP address range is already defined on the DHCP server.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified IP address range is already defined on the DHCP server.")]
        IPRANGE_EXISTS = 20021,

        /// <summary>
        /// The specified IP address is currently taken by another client.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified IP address is currently taken by another client.")]
        RESERVEDIP_EXISTS = 20022,

        /// <summary>
        /// The specified IP address range either overlaps with an existing range or is invalid.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified IP address range either overlaps with an existing range or is invalid.")]
        INVALID_RANGE = 20023,

        /// <summary>
        /// The specified IP address range is an extension of an existing range.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified IP address range is an extension of an existing range.")]
        RANGE_EXTENDED = 20024,

        /// <summary>
        /// "The specified IP address range extension is too small. The number of addresses in the extension must be a multiple of 32.
        /// </summary>
        [DhcpServerNativeErrorDescription(@"The specified IP address range extension is too small. The number of addresses in the extension must be a multiple of 32.")]
        RANGE_EXTENSION_TOO_SMALL = 20025,

        /// <summary>
        /// "An attempt was made to extend the IP address range to a value less than the specified backward extension. The number of addresses in the extension must be a multiple of 32. 
        /// </summary>
        [DhcpServerNativeErrorDescription(@"An attempt was made to extend the IP address range to a value less than the specified backward extension. The number of addresses in the extension must be a multiple of 32. ")]
        WARNING_RANGE_EXTENDED_LESS = 20026,

        /// <summary>
        /// "The DHCP database needs to be upgraded to a newer format. For more information, refer to the DHCP server event log.
        /// </summary>
        [DhcpServerNativeErrorDescription(@"The DHCP database needs to be upgraded to a newer format. For more information, refer to the DHCP server event log.")]
        JET_CONV_REQUIRED = 20027,

        /// <summary>
        /// The format of the bootstrap protocol file table is incorrect. The correct format is:
        ///   &lt;requested boot file name 1&gt;,&lt;boot file server name 1&gt;, &lt;boot file name 1&gt;
        ///   &lt;requested boot file name 2&gt;,&lt;boot file server name 2&gt;, &lt;boot file name 2&gt;
        /// </summary>
        [DhcpServerNativeErrorDescription(@"The format of the bootstrap protocol file table is incorrect. The correct format is:
<requested boot file name 1>,<boot file server name 1>, <boot file name 1>
<requested boot file name 2>,<boot file server name 2>, <boot file name 2>
...")]
        SERVER_INVALID_BOOT_FILE_TABLE = 20027,

        /// <summary>
        /// A boot file name specified in the bootstrap protocol file table is unrecognized or invalid.
        /// </summary>
        [DhcpServerNativeErrorDescription("A boot file name specified in the bootstrap protocol file table is unrecognized or invalid.")]
        SERVER_UNKNOWN_BOOT_FILE_NAME = 20029,

        /// <summary>
        /// The specified superscope name is too long.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified superscope name is too long.")]
        SUPER_SCOPE_NAME_TOO_LONG = 20030,

        /// <summary>
        /// The specified IP address is already in use.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified IP address is already in use.")]
        IP_ADDRESS_IN_USE = 20032,

        /// <summary>
        /// The specified path to the DHCP audit log file is too long.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified path to the DHCP audit log file is too long.")]
        LOG_FILE_PATH_TOO_LONG = 20033,

        /// <summary>
        /// The DHCP server received a request for a valid IP address not administered by the server.
        /// </summary>
        [DhcpServerNativeErrorDescription("The DHCP server received a request for a valid IP address not administered by the server.")]
        UNSUPPORTED_CLIENT = 20034,

        /// <summary>
        /// "The DHCP server failed to receive a notification when the interface list changed, therefore some of the interfaces will not be enabled on the server.
        /// </summary>
        [DhcpServerNativeErrorDescription(@"The DHCP server failed to receive a notification when the interface list changed, therefore some of the interfaces will not be enabled on the server.")]
        SERVER_INTERFACE_NOTIFICATION_EVENT = 20035,

        /// <summary>
        /// "The DHCP database needs to be upgraded to a newer format (JET97). For more information, refer to the DHCP server event log.
        /// </summary>
        [DhcpServerNativeErrorDescription(@"The DHCP database needs to be upgraded to a newer format (JET97). For more information, refer to the DHCP server event log.")]
        JET97_CONV_REQUIRED = 20036,

        /// <summary>
        /// "The DHCP server cannot determine if it has the authority to run, and is not servicing clients on the network. This rogue status may be due to network problems or insufficient server resources.
        /// </summary>
        [DhcpServerNativeErrorDescription(@"The DHCP server cannot determine if it has the authority to run, and is not servicing clients on the network. This rogue status may be due to network problems or insufficient server resources.")]
        ROGUE_INIT_FAILED = 20037,

        /// <summary>
        /// The DHCP service is shutting down because another DHCP server is active on the network.
        /// </summary>
        [DhcpServerNativeErrorDescription("The DHCP service is shutting down because another DHCP server is active on the network.")]
        ROGUE_SAMSHUTDOWN = 20038,

        /// <summary>
        /// The DHCP server does not have the authority to run, and is not servicing clients on the network.
        /// </summary>
        [DhcpServerNativeErrorDescription("The DHCP server does not have the authority to run, and is not servicing clients on the network.")]
        ROGUE_NOT_AUTHORIZED = 20039,

        /// <summary>
        /// "The DHCP server is unable to contact the directory service for this domain. The DHCP server will continue to attempt to contact the directory service. During this time, no clients on the network will be serviced.
        /// </summary>
        [DhcpServerNativeErrorDescription(@"The DHCP server is unable to contact the directory service for this domain. The DHCP server will continue to attempt to contact the directory service. During this time, no clients on the network will be serviced.")]
        ROGUE_DS_UNREACHABLE = 20040,

        /// <summary>
        /// The DHCP server's authorization information conflicts with that of another DHCP server on the network.
        /// </summary>
        [DhcpServerNativeErrorDescription("The DHCP server's authorization information conflicts with that of another DHCP server on the network.")]
        ROGUE_DS_CONFLICT = 20041,

        /// <summary>
        /// "The DHCP server is ignoring a request from another DHCP server because the second server is a member of a different directory service enterprise.
        /// </summary>
        [DhcpServerNativeErrorDescription(@"The DHCP server is ignoring a request from another DHCP server because the second server is a member of a different directory service enterprise.")]
        ROGUE_NOT_OUR_ENTERPRISE = 20042,

        /// <summary>
        /// The DHCP server has detected a directory service environment on the network. If there is a directory service on the network, the DHCP server can only run if it is a part of the directory service. Since the server ostensibly belongs to a workgroup, it is terminating.
        /// </summary>
        [DhcpServerNativeErrorDescription("The DHCP server has detected a directory service environment on the network. If there is a directory service on the network, the DHCP server can only run if it is a part of the directory service. Since the server ostensibly belongs to a workgroup, it is terminating.")]
        STANDALONE_IN_DS = 20043,

        /// <summary>
        /// The specified DHCP class name is unknown or invalid.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified DHCP class name is unknown or invalid.")]
        CLASS_NOT_FOUND = 20044,

        /// <summary>
        /// The specified DHCP class name (or information) is already in use.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified DHCP class name (or information) is already in use.")]
        CLASS_ALREADY_EXISTS = 20045,

        /// <summary>
        /// The specified DHCP scope name is too long, the scope name must not exceed 256 characters.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified DHCP scope name is too long, the scope name must not exceed 256 characters.")]
        SCOPE_NAME_TOO_LONG = 20046,

        /// <summary>
        /// The default scope is already configured on the server.
        /// </summary>
        [DhcpServerNativeErrorDescription("The default scope is already configured on the server.")]
        DEFAULT_SCOPE_EXISTS = 20047,

        /// <summary>
        /// The Dynamic BOOTP attribute cannot be turned on or off.
        /// </summary>
        [DhcpServerNativeErrorDescription("The Dynamic BOOTP attribute cannot be turned on or off.")]
        CANT_CHANGE_ATTRIBUTE = 20048,

        /// <summary>
        /// Conversion of a scope to a \"DHCP Only\" scope or to a \"BOOTP Only\" scope is not allowed when the scope contains other DHCP and BOOTP clients. Either the DHCP or BOOTP clients should be specifically deleted before converting the scope to the other type.
        /// </summary>
        [DhcpServerNativeErrorDescription("Conversion of a scope to a \"DHCP Only\" scope or to a \"BOOTP Only\" scope is not allowed when the scope contains other DHCP and BOOTP clients. Either the DHCP or BOOTP clients should be specifically deleted before converting the scope to the other type.")]
        IPRANGE_CONV_ILLEGAL = 20049,

        /// <summary>
        /// The network has changed. Retry this operation after checking for network changes. Network changes may be caused by interfaces that are new or invalid, or by IP addresses that are new or invalid.
        /// </summary>
        [DhcpServerNativeErrorDescription("The network has changed. Retry this operation after checking for network changes. Network changes may be caused by interfaces that are new or invalid, or by IP addresses that are new or invalid.")]
        NETWORK_CHANGED = 20050,

        /// <summary>
        /// The bindings to internal IP addresses cannot be modified.
        /// </summary>
        [DhcpServerNativeErrorDescription("The bindings to internal IP addresses cannot be modified.")]
        CANNOT_MODIFY_BINDINGS = 20051,

        /// <summary>
        /// The DHCP scope parameters are incorrect. Either the scope already exists, or its properties are inconsistent with the subnet address and mask of an existing scope.
        /// </summary>
        [DhcpServerNativeErrorDescription("The DHCP scope parameters are incorrect. Either the scope already exists, or its properties are inconsistent with the subnet address and mask of an existing scope.")]
        BAD_SCOPE_PARAMETERS = 20052,

        /// <summary>
        /// The DHCP multicast scope parameters are incorrect. Either the scope already exists, or its properties are inconsistent with the subnet address and mask of an existing scope.
        /// </summary>
        [DhcpServerNativeErrorDescription("The DHCP multicast scope parameters are incorrect. Either the scope already exists, or its properties are inconsistent with the subnet address and mask of an existing scope.")]
        MSCOPE_EXISTS = 20053,

        /// <summary>
        /// The multicast scope range must have at least 256 IP addresses.
        /// </summary>
        [DhcpServerNativeErrorDescription("The multicast scope range must have at least 256 IP addresses.")]
        MSCOPE_RANGE_TOO_SMALL = 20054,

        /// <summary>
        /// The DHCP server could not contact Active Directory.
        /// </summary>
        [DhcpServerNativeErrorDescription("The DHCP server could not contact Active Directory.")]
        DDS_NO_DS_AVAILABLE = 20070,

        /// <summary>
        /// The DHCP service root could not be found in Active Directory.
        /// </summary>
        [DhcpServerNativeErrorDescription("The DHCP service root could not be found in Active Directory.")]
        DDS_NO_DHCP_ROOT = 20071,

        /// <summary>
        /// An unexpected error occurred while accessing Active Directory.
        /// </summary>
        [DhcpServerNativeErrorDescription("An unexpected error occurred while accessing Active Directory.")]
        DDS_UNEXPECTED_ERROR = 20072,

        /// <summary>
        /// There were too many errors to proceed.
        /// </summary>
        [DhcpServerNativeErrorDescription("There were too many errors to proceed.")]
        DDS_TOO_MANY_ERRORS = 20073,

        /// <summary>
        /// A DHCP service could not be found.
        /// </summary>
        [DhcpServerNativeErrorDescription("A DHCP service could not be found.")]
        DDS_DHCP_SERVER_NOT_FOUND = 20074,

        /// <summary>
        /// The specified DHCP options are already present in Active Directory.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified DHCP options are already present in Active Directory.")]
        DDS_OPTION_ALREADY_EXISTS = 20075,

        /// <summary>
        /// The specified DHCP options are not present in Active Directory.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified DHCP options are not present in Active Directory.")]
        DDS_OPTION_DOES_NOT_EXIST = 20076,

        /// <summary>
        /// The specified DHCP classes are already present in Active Directory.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified DHCP classes are already present in Active Directory.")]
        DDS_CLASS_EXISTS = 20077,

        /// <summary>
        /// The specified DHCP classes are not present in Active Directory.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified DHCP classes are not present in Active Directory.")]
        DDS_CLASS_DOES_NOT_EXIST = 20078,

        /// <summary>
        /// The specified DHCP servers are already present in Active Directory.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified DHCP servers are already present in Active Directory.")]
        DDS_SERVER_ALREADY_EXISTS = 20079,

        /// <summary>
        /// The specified DHCP servers are not present in Active Directory.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified DHCP servers are not present in Active Directory.")]
        DDS_SERVER_DOES_NOT_EXIST = 20080,

        /// <summary>
        /// The specified DHCP server address does not correspond to the identified DHCP server name.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified DHCP server address does not correspond to the identified DHCP server name.")]
        DDS_SERVER_ADDRESS_MISMATCH = 20081,

        /// <summary>
        /// The specified subnets are already present in Active Directory.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified subnets are already present in Active Directory.")]
        DDS_SUBNET_EXISTS = 20082,

        /// <summary>
        /// The specified subnet belongs to a different superscope.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified subnet belongs to a different superscope.")]
        DDS_SUBNET_HAS_DIFF_SUPER_SCOPE = 20083,

        /// <summary>
        /// The specified subnet is not present in Active Directory.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified subnet is not present in Active Directory.")]
        DDS_SUBNET_NOT_PRESENT = 20084,

        /// <summary>
        /// The specified reservation is not present in Active Directory.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified reservation is not present in Active Directory.")]
        DDS_RESERVATION_NOT_PRESENT = 20085,

        /// <summary>
        /// The specified reservation conflicts with another reservation present in Active Directory.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified reservation conflicts with another reservation present in Active Directory.")]
        DDS_RESERVATION_CONFLICT = 20086,

        /// <summary>
        /// The specified IP address range conflicts with another IP range present in Active Directory.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified IP address range conflicts with another IP range present in Active Directory.")]
        DDS_POSSIBLE_RANGE_CONFLICT = 20087,

        /// <summary>
        /// The specified IP address range is not present in Active Directory.
        /// </summary>
        [DhcpServerNativeErrorDescription("The specified IP address range is not present in Active Directory.")]
        DDS_RANGE_DOES_NOT_EXIST = 20088,

        /// <summary>
        /// Windows 7 or later: This class cannot be deleted.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 7 or later: This class cannot be deleted.")]
        DELETE_BUILTIN_CLASS = 20089,

        /// <summary>
        /// Windows 7 or later: The given subnet prefix is invalid. It represents either a non-unicast or link local address range.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 7 or later: The given subnet prefix is invalid. It represents either a non-unicast or link local address range.")]
        INVALID_SUBNET_PREFIX = 20091,

        /// <summary>
        /// Windows 7 or later: The given delay value is invalid. The valid value is from 0 to 1000.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 7 or later: The given delay value is invalid. The valid value is from 0 to 1000.")]
        INVALID_DELAY = 20092,

        /// <summary>
        /// Windows 7 or later: Address or Address pattern is already contained in one of the list.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 7 or later: Address or Address pattern is already contained in one of the list.")]
        LINKLAYER_ADDRESS_EXISTS = 20093,

        /// <summary>
        /// Windows 7 or later: Address to be added to Deny list or to be deleted from allow list, has an associated reservation.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 7 or later: Address to be added to Deny list or to be deleted from allow list, has an associated reservation.")]
        LINKLAYER_ADDRESS_RESERVATION_EXISTS = 20094,

        /// <summary>
        /// Windows 7 or later: Address or Address pattern is not contained in either list.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 7 or later: Address or Address pattern is not contained in either list.")]
        LINKLAYER_ADDRESS_DOES_NOT_EXIST = 20095,

        /// <summary>
        /// Windows 7 or later: This Hardware Type is already exempt.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 7 or later: This Hardware Type is already exempt.")]
        HARDWARE_ADDRESS_TYPE_ALREADY_EXEMPT = 20101,

        /// <summary>
        /// Windows 7 or later: You are trying to delete an undefined Hardware Type.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 7 or later: You are trying to delete an undefined Hardware Type.")]
        UNDEFINED_HARDWARE_ADDRESS_TYPE = 20102,

        /// <summary>
        /// Windows 7 or later: Conflict in types for the same option on Host and Added DHCP Servers.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 7 or later: Conflict in types for the same option on Host and Added DHCP Servers.")]
        OPTION_TYPE_MISMATCH = 20103,

        /// <summary>
        /// Windows 8 or later: The parent expression specified does not exist.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The parent expression specified does not exist.")]
        POLICY_BAD_PARENT_EXPR = 20104,

        /// <summary>
        /// Windows 8 or later: The DHCP server policy already exists.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The DHCP server policy already exists.")]
        POLICY_EXISTS = 20105,

        /// <summary>
        /// Windows 8 or later: The DHCP server policy range specified already exists in the given scope.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The DHCP server policy range specified already exists in the given scope.")]
        POLICY_RANGE_EXISTS = 20106,

        /// <summary>
        /// Windows 8 or later: The DHCP server policy range specified is invalid or does not match the given subnet.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The DHCP server policy range specified is invalid or does not match the given subnet.")]
        POLICY_RANGE_BAD = 20107,

        /// <summary>
        /// Windows 8 or later: DHCP server policy ranges can only be added to scope level policies.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: DHCP server policy ranges can only be added to scope level policies.")]
        RANGE_INVALID_IN_SERVER_POLICY = 20108,

        /// <summary>
        /// Windows 8 or later: The DHCP server policy contains an invalid expression.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The DHCP server policy contains an invalid expression.")]
        INVALID_POLICY_EXPRESSION = 20109,

        /// <summary>
        /// Windows 8 or later: The processing order specified for the DHCP server policy is invalid.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The processing order specified for the DHCP server policy is invalid.")]
        INVALID_PROCESSING_ORDER = 20110,

        /// <summary>
        /// Windows 8 or later: The DHCP server policy was not found.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The DHCP server policy was not found.")]
        POLICY_NOT_FOUND = 20111,

        /// <summary>
        /// Windows 8 or later: There is an IP address range configured for a policy in this scope. This operation on the scope IP address range cannot be performed until the policy IP address range is suitably modified. Please change the IP address range of the policy before performing this operation.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: There is an IP address range configured for a policy in this scope. This operation on the scope IP address range cannot be performed until the policy IP address range is suitably modified. Please change the IP address range of the policy before performing this operation.")]
        SCOPE_RANGE_POLICY_RANGE_CONFLICT = 20112,

        /// <summary>
        /// Windows 8 or later: The DHCP scope is already in a failover relationship.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The DHCP scope is already in a failover relationship.")]
        FO_SCOPE_ALREADY_IN_RELATIONSHIP = 20113,

        /// <summary>
        /// Windows 8 or later: The DHCP failover relationship already exists.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The DHCP failover relationship already exists.")]
        FO_RELATIONSHIP_EXISTS = 20114,

        /// <summary>
        /// Windows 8 or later: The DHCP failover relationship does not exist.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The DHCP failover relationship does not exist.")]
        FO_RELATIONSHIP_DOES_NOT_EXIST = 20115,

        /// <summary>
        /// Windows 8 or later: The DHCP scope is not part of a failover relationship.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The DHCP scope is not part of a failover relationship.")]
        FO_SCOPE_NOT_IN_RELATIONSHIP = 20116,

        /// <summary>
        /// Windows 8 or later: The DHCP failover relationship is a secondary.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The DHCP failover relationship is a secondary.")]
        FO_RELATION_IS_SECONDARY = 20117,

        /// <summary>
        /// Windows 8 or later: The DHCP failover is not supported.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The DHCP failover is not supported.")]
        FO_NOT_SUPPORTED = 20118,

        /// <summary>
        /// Windows 8 or later: The DHCP servers in the failover relationship have timed out of synchronization.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The DHCP servers in the failover relationship have timed out of synchronization.")]
        FO_TIME_OUT_OF_SYNC = 20119,

        /// <summary>
        /// Windows 8 or later: The DHCP failover relationship state is not NORMAL.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The DHCP failover relationship state is not NORMAL.")]
        FO_STATE_NOT_NORMAL = 20120,

        /// <summary>
        /// Windows 8 or later: The user does not have administrative permissions for the DHCP server.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The user does not have administrative permissions for the DHCP server.")]
        NO_ADMIN_PERMISSION = 20121,

        /// <summary>
        /// Windows 8 or later: The specified DHCP server is not reachable. Please provide a DHCP server that is reachable.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The specified DHCP server is not reachable. Please provide a DHCP server that is reachable.")]
        SERVER_NOT_REACHABLE = 20122,

        /// <summary>
        /// Windows 8 or later: The DHCP Server Service is not running on the specified server. Please ensure that the DHCP Server service is running on the specified computer.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The DHCP Server Service is not running on the specified server. Please ensure that the DHCP Server service is running on the specified computer.")]
        SERVER_NOT_RUNNING = 20123,

        /// <summary>
        /// Windows 8 or later: Unable to resolve DNS name.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: Unable to resolve DNS name.")]
        SERVER_NAME_NOT_RESOLVED = 20124,

        /// <summary>
        /// Windows 8 or later: The specified DHCP failover relationship name is too long. The name is limited to a maximum of 126 characters.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The specified DHCP failover relationship name is too long. The name is limited to a maximum of 126 characters.")]
        FO_RELATIONSHIP_NAME_TOO_LONG = 20125,

        /// <summary>
        /// Windows 8 or later: The specified DHCP Server has reached the end of the selected range while finding the free IP address.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The specified DHCP Server has reached the end of the selected range while finding the free IP address.")]
        REACHED_END_OF_SELECTION = 20126,

        /// <summary>
        /// Windows 8 or later: The synchronization of leases in the scopes being added to the failover relationship failed.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The synchronization of leases in the scopes being added to the failover relationship failed.")]
        FO_ADDSCOPE_LEASES_NOT_SYNCED = 20127,

        /// <summary>
        /// Windows 8 or later: The relationship cannot be created on the DHCP server as the maximum number of allowed relationship has been exceeded.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The relationship cannot be created on the DHCP server as the maximum number of allowed relationship has been exceeded.")]
        FO_MAX_RELATIONSHIPS = 20128,

        /// <summary>
        /// Windows 8 or later: A Scope configured for failover cannot be changed to type BOOTP or BOTH.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: A Scope configured for failover cannot be changed to type BOOTP or BOTH.")]
        FO_IPRANGE_TYPE_CONV_ILLEGAL = 20129,

        /// <summary>
        /// Windows 8 or later: The number of scopes being added to the failover relationship exceeds the max number of scopes which can be added to a failover relationship at one time.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: The number of scopes being added to the failover relationship exceeds the max number of scopes which can be added to a failover relationship at one time.")]
        FO_MAX_ADD_SCOPES = 20130,

        /// <summary>
        /// Windows 8 or later: A scope supporting BOOTP clients cannot be added to a failover relationship.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: A scope supporting BOOTP clients cannot be added to a failover relationship.")]
        FO_BOOT_NOT_SUPPORTED = 20131,

        /// <summary>
        /// Windows 8 or later: An IP address range of a scope which is part of a failover relationship cannot be deleted. The scope will need to be removed from the failover relationship before deleting the range.
        /// </summary>
        [DhcpServerNativeErrorDescription("Windows 8 or later: An IP address range of a scope which is part of a failover relationship cannot be deleted. The scope will need to be removed from the failover relationship before deleting the range.")]
        FO_RANGE_PART_OF_REL = 20132,

    }
}
