using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    internal static class Api
    {
        /// <summary>
        /// The DhcpEnumServers function returns an enumerated list of DHCP servers found in the directory service. 
        /// </summary>
        /// <param name="Flags">Reserved for future use. This field should be set to 0.</param>
        /// <param name="IdInfo">Pointer to an address containing the server's ID block. This field should be set to null.</param>
        /// <param name="Servers">Pointer to a <see cref="DHCPDS_SERVERS"/> structure that contains the output list of DHCP servers.</param>
        /// <param name="CallbackFn">Pointer to the callback function that will be called when the server add operation completes. This field should be null.</param>
        /// <param name="CallbackData">Pointer to a data block containing the formatted structure for callback information. This field should be null.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        [DllImport("dhcpsapi.dll", SetLastError = true)]
        public static extern DhcpErrors DhcpEnumServers(uint Flags, IntPtr IdInfo, out IntPtr Servers, IntPtr CallbackFn, IntPtr CallbackData);

        /// <summary>
        /// The DhcpGetVersion function returns the major and minor version numbers of the DHCP server.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="MajorVersion">Specifies the major version number of the DHCP server.</param>
        /// <param name="MinorVersion">Specifies the minor version number of the DHCP server.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpGetVersion(string ServerIpAddress, out int MajorVersion, out int MinorVersion);

        /// <summary>
        /// The DhcpServerGetConfig function returns the specific configuration settings of a DHCP server. Configuration information includes information on the JET database used to store subnet and client lease information, and the supported protocols.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="ConfigInfo">Pointer to a <see cref="DHCP_SERVER_CONFIG_INFO"/> structure that contains the specific configuration information for the DHCP server. Note: The memory for this parameter must be free using <see cref="DhcpRpcFreeMemory"/>.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpServerGetConfig(string ServerIpAddress, out IntPtr ConfigInfo);

        /// <summary>
        /// The DhcpGetServerSpecificStrings function retrieves the names of the default vendor class and user class.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IPv4 address of the DHCPv4 server.</param>
        /// <param name="ServerSpecificStrings">Pointer to a DHCP_SERVER_SPECIFIC_STRINGS structure that receives the information for the default vendor class and user class name strings. Note: The memory for this parameter must be free using DhcpRpcFreeMemory.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpGetServerSpecificStrings(string ServerIpAddress, out IntPtr ServerSpecificStrings);

        /// <summary>
        /// The DhcpGetServerBindingInfo function returns endpoint bindings set on the DHCP server.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="Flags">Specifies a set of flags describing the endpoints to return.
        /// DHCP_ENDPOINT_FLAG_CANT_MODIFY (0x01) = Returns unmodifiable endpoints only.
        /// </param>
        /// <param name="BindElementsInfo">Pointer to a DHCP_BIND_ELEMENT_ARRAY structure that contains the server network endpoint bindings. Note: The memory for this parameter must be free using DhcpRpcFreeMemory.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        /// <remarks>This function requires network byte ordering for all DHCP_IP_ADDRESS values in parameter structures.</remarks>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpGetServerBindingInfo(string ServerIpAddress, uint Flags, out IntPtr BindElementsInfo);

        /// <summary>
        /// The DhcpGetSubnetDelayOffer function obtains the delay period for DHCP OFFER messages after a DISCOVER message is received.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="SubnetAddress">DHCP_IP_ADDRESS value that contains the IP address of the subnet gateway.</param>
        /// <param name="TimeDelayInMilliseconds">Unsigned 16-bit integer value that receive the time to delay an OFFER message after receiving a DISCOVER message as configured on the DHCP server, in milliseconds.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpGetSubnetDelayOffer(string ServerIpAddress, DHCP_IP_ADDRESS SubnetAddress, out ushort TimeDelayInMilliseconds);

        /// <summary>
        /// The DhcpEnumOptions function returns an enumerated set of options stored on the DHCPv4 server.
        /// </summary>
        /// <param name="ServerIpAddress">Pointer to a Unicode string that specifies the IPv4 address of the DHCP server.</param>
        /// <param name="ResumeHandle">Pointer to a DHCP_RESUME_HANDLE value that identifies the enumeration operation. Initially, this value should be zero, with a successful call returning the handle value used for subsequent enumeration requests. For example, if PreferredMaximum is set to 1000 bytes, and 2000 bytes worth of options are stored on the server, the resume handle can be used after the first 1000 bytes are retrieved to obtain the next 1000 on a subsequent call, and so forth. 
        /// The presence of additional enumerable data is indicated when this function returns ERROR_MORE_DATA. If no additional enumerable data is available on the DHCPv4 server, ERROR_NO_MORE_ITEMS is returned.</param>
        /// <param name="PreferredMaximum">Specifies the preferred maximum number of bytes of options to return. If the number of remaining unenumerated options (in bytes) is less than this value, then that amount will be returned.
        /// To retrieve all the option definitions for the default user and vendor class, set this parameter to 0xFFFFFFFF.</param>
        /// <param name="Options">Pointer to a <see cref="DHCP_OPTION_ARRAY"/> structure containing the returned options. If there are no options available on the DHCPv4 server, this parameter will return null.</param>
        /// <param name="OptionsRead">Pointer to a DWORD value that specifies the number of options returned in Options.</param>
        /// <param name="OptionsTotal">Pointer to a DWORD value that specifies the total number of remaining options stored on the DHCPv4 server.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpEnumOptions(string ServerIpAddress, ref IntPtr ResumeHandle, uint PreferredMaximum, out IntPtr Options, out int OptionsRead, out int OptionsTotal);

        /// <summary>
        /// The DhcpEnumOptions function returns an enumerated set of options stored on the DHCPv4 server.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="Flags">A set of flags that indicate the option definition for enumeration. 0x00000000 = The option definitions are enumerated for a default vendor class; DHCP_FLAGS_OPTION_IS_VENDOR (0x00000003) = The option definitions are enumerated for a specific vendor class.</param>
        /// <param name="ClassName">Pointer to a Unicode string that contains the name of the class whose options will be enumerated. This parameter is optional. </param>
        /// <param name="VendorName">Pointer to a Unicode string that contains the name of the vendor for the class. This parameter is optional. If a vendor class name is not provided, the default vendor class name is used.</param>
        /// <param name="ResumeHandle">Pointer to a DHCP_RESUME_HANDLE value that identifies the enumeration operation. Initially, this value should be zero, with a successful call returning the handle value used for subsequent enumeration requests. For example, if PreferredMaximum is set to 1000 bytes, and 2000 bytes of option definitions are stored on the server, the resume handle can be used after the first 1000 bytes are retrieved to obtain the next 1000 on a subsequent call, and so forth.</param>
        /// <param name="PreferredMaximum">Specifies the preferred maximum number of bytes of options to return. If the number of remaining unenumerated option definitions (in bytes) is less than this value, all option definitions are returned.</param>
        /// <param name="Options">Pointer to a DHCP_OPTION_ARRAY structure containing the returned option definitions. If there are no option definitions available on the DHCP server, this parameter will return null.</param>
        /// <param name="OptionsRead">Pointer to a DWORD value that specifies the number of option definitions returned in Options.</param>
        /// <param name="OptionsTotal">Pointer to a DWORD value that specifies the total number of unenumerated option definitions on the DHCP server.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpEnumOptionsV5(string ServerIpAddress, uint Flags, string ClassName, string VendorName, ref IntPtr ResumeHandle, uint PreferredMaximum, out IntPtr Options, out int OptionsRead, out int OptionsTotal);

        /// <summary>
        /// The DhcpEnumOptionValues function returns an enumerated list of option values (just the option data and the associated ID number) for a given scope.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="ScopeInfo">DHCP_OPTION_SCOPE_INFO structure that contains the level (specifically: default, server, scope, or IPv4 reservation level) for which the option values are defined and should be enumerated. </param>
        /// <param name="ResumeHandle">Pointer to a DHCP_RESUME_HANDLE value that identifies the enumeration operation. Initially, this value should be zero, with a successful call returning the handle value used for subsequent enumeration requests. For example, if PreferredMaximum is set to 1000 bytes, and 2000 bytes worth of option values are stored on the server, the resume handle can be used after the first 1000 bytes are retrieved to obtain the next 1000 on a subsequent call, and so forth.
        /// The presence of additional enumerable data is indicated when this function returns ERROR_MORE_DATA. If no additional enumerable data is available on the DHCPv4 server, ERROR_NO_MORE_ITEMS is returned.</param>
        /// <param name="PreferredMaximum">Specifies the preferred maximum number of bytes of option values to return. If the number of remaining unenumerated options (in bytes) is less than this value, then that amount will be returned.
        /// To retrieve all the option values for the default user and vendor class at the specified level, set this parameter to 0xFFFFFFFF.</param>
        /// <param name="OptionValues">Pointer to a <see cref="DHCP_OPTION_VALUE_ARRAY"/> structure that contains the enumerated option values returned for the specified scope. If there are no option values available for this scope on the DHCP server, this parameter will return null.</param>
        /// <param name="OptionsRead">Pointer to a DWORD value that specifies the number of option values returned in OptionValues.</param>
        /// <param name="OptionsTotal">Pointer to a DWORD value that specifies the total number of remaining option values for this scope stored on the DHCP server.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpEnumOptionValues(string ServerIpAddress, IntPtr ScopeInfo, ref IntPtr ResumeHandle, uint PreferredMaximum, out IntPtr OptionValues, out int OptionsRead, out int OptionsTotal);

        /// <summary>
        /// The DhcpEnumOptionValuesV5 function returns an enumerated list of option values (just the option data and the associated ID number) for a specific scope within a given user or vendor class.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="Flags">Specifies a bit flag that indicates whether or not the option is vendor specific. If it is not vendor specific, this parameter must be 0. 0x00000000 = The option values are enumerated for a default vendor class; DHCP_FLAGS_OPTION_IS_VENDOR (0x00000003) = The option values are enumerated for a specific vendor class.</param>
        /// <param name="ClassName">Pointer to a Unicode string that contains the name of the class whose scope option values will be enumerated.</param>
        /// <param name="VendorName">Pointer to a Unicode string that contains the name of the vendor for the class. This parameter is optional. If a vendor class name is not provided, the option values enumerated for a default vendor class.</param>
        /// <param name="ScopeInfo">Pointer to a DHCP_OPTION_SCOPE_INFO structure that contains the scope for which the option values are defined. This value defines the option values that will be retrieved from the server, scope, or default level, or for an IPv4 reservation.</param>
        /// <param name="ResumeHandle">Pointer to a DHCP_RESUME_HANDLE value that identifies the enumeration operation. Initially, this value should be zero, with a successful call returning the handle value used for subsequent enumeration requests. For example, if PreferredMaximum is set to 1000 bytes, and 2000 bytes' worth of option values are stored on the server, the resume handle can be used after the first 1000 bytes are retrieved to obtain the next 1000 on a subsequent call, and so forth.</param>
        /// <param name="PreferredMaximum">Specifies the preferred maximum number of bytes of option values to return. If the number of remaining unenumerated options (in bytes) is less than this value, all option values are returned.</param>
        /// <param name="OptionValues">Pointer to a DHCP_OPTION_VALUE_ARRAY structure that contains the enumerated option values returned for the specified scope. If there are no option values available for this scope on the DHCP server, this parameter will return null.</param>
        /// <param name="OptionsRead">Pointer to a DWORD value that specifies the number of option values returned in OptionValues.</param>
        /// <param name="OptionsTotal">Pointer to a DWORD value that specifies the total number of as-yet unenumerated option values for this scope stored on the DHCP server.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpEnumOptionValuesV5(string ServerIpAddress, uint Flags, string ClassName, string VendorName, IntPtr ScopeInfo, ref IntPtr ResumeHandle, uint PreferredMaximum, out IntPtr OptionValues, out int OptionsRead, out int OptionsTotal);

        /// <summary>
        /// The DhcpGetOptionValue function retrieves a DHCP option value (the option code and associated data) for a particular scope.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="OptionID">DHCP_OPTION_ID value that specifies the code for the option value to retrieve.</param>
        /// <param name="ScopeInfo">DHCP_OPTION_SCOPE_INFO structure that contains information on the scope where the option value is set.</param>
        /// <param name="OptionValue">DHCP_OPTION_VALUE structure that contains the returned option code and data. NOTE: The memory for this parameter must be free using DhcpRpcFreeMemory.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpGetOptionValue(string ServerIpAddress, int OptionID, IntPtr ScopeInfo, out IntPtr OptionValue);

        /// <summary>
        /// The DhcpGetOptionValueV5 function retrieves a DHCP option value (the option code and associated data) for a particular scope. This function extends the functionality provided by DhcpGetOptionValue by allowing the caller to specify a class and/or vendor for the option.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="Flags">Flag value that indicates whether the option is for a specific or default vendor class. 0x00000000 = The option value is retrieved for a default vendor class; DHCP_FLAGS_OPTION_IS_VENDOR (0x00000003) = The option value is retrieved for a specific vendor class. The vendor name is supplied in VendorName.</param>
        /// <param name="OptionID">DHCP_OPTION_ID value that specifies the code for the option value to retrieve.</param>
        /// <param name="ClassName">Unicode string that specifies the DHCP class name of the option. This parameter is optional.</param>
        /// <param name="VendorName">Unicode string that specifies the vendor of the option. This parameter is optional, and should be null when Flags is not set to DHCP_FLAGS_OPTION_IS_VENDOR. If the vendor class is not specified, the option value is returned for the default vendor class.</param>
        /// <param name="ScopeInfo">DHCP_OPTION_SCOPE_INFO structure that contains information on the scope where the option value is set.</param>
        /// <param name="OptionValue">DHCP_OPTION_VALUE structure that contains the returned option code and data.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpGetOptionValueV5(string ServerIpAddress, uint Flags, int OptionID, string ClassName, string VendorName, IntPtr ScopeInfo, out IntPtr OptionValue);

        /// <summary>
        /// The DhcpGetClassInfo function returns the user or vendor class information configured on a specific DHCP server.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="ReservedMustBeZero">Reserved. This parameter must be set to 0.</param>
        /// <param name="PartialClassInfo">DHCP_CLASS_INFO structure that contains data provided by the caller for the following members, with all other fields initialized.
        /// - ClassName;
        /// - ClassData;
        /// - ClassDataLength;
        /// These fields must not be null.</param>
        /// <param name="FilledClassInfo">DHCP_CLASS_INFO structure returned after lookup that contains the complete class information.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpGetClassInfo(string ServerIpAddress, uint ReservedMustBeZero, DHCP_CLASS_INFO_Local PartialClassInfo, out IntPtr FilledClassInfo);

        /// <summary>
        /// The DhcpEnumClasses function enumerates the user or vendor classes configured for the DHCP server.
        /// </summary>
        /// <param name="ServerIpAddress">Pointer to a Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="ReservedMustBeZero">Reserved. This field must be set to zero.</param>
        /// <param name="ResumeHandle">Pointer to a DHCP_RESUME_HANDLE value that identifies the enumeration operation. Initially, this value should be zero, with a successful call returning the handle value used for subsequent enumeration requests. For example, if PreferredMaximum is set to 100 classes, and 200 classes are stored on the server, the resume handle can be used after the first 100 classes are retrieved to obtain the next 100 on a subsequent call, and so forth.</param>
        /// <param name="PreferredMaximum">Specifies the preferred maximum number of classes to return. If the number of remaining unenumerated classes is less than this value, then that amount will be returned. To retrieve all classes available on the DHCP server, set this parameter to 0xFFFFFFFF.</param>
        /// <param name="ClassInfoArray">Pointer to a DHCP_CLASS_INFO_ARRAY structure that contains the returned classes. If there are no classes available on the DHCP server, this parameter will return null.</param>
        /// <param name="nRead">Pointer to a DWORD value that specifies the number of classes returned in ClassInfoArray.</param>
        /// <param name="nTotal">Pointer to a DWORD value that specifies the total number of classes stored on the DHCP server.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        /// <remarks>A DHCP class is a specific category of client, defined either by the vendor or by a user. An example of a vendor-defined class would be all Windows 8 clients, with Microsoft as the vendor. A user-defined class consists of those clients with specific attributes selected by a user or administrator, such as all laptops or clients that support wireless connections.</remarks>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpEnumClasses(string ServerIpAddress, uint ReservedMustBeZero, ref IntPtr ResumeHandle, uint PreferredMaximum, out IntPtr ClassInfoArray, out int nRead, out int nTotal);

        /// <summary>
        /// The DhcpCreateSubnet function creates a new subnet on the DHCP server.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="SubnetAddress"><see cref="DHCP_IP_ADDRESS"/> value that contains the IP address of the subnet's gateway.</param>
        /// <param name="SubnetInfo"><see cref="DHCP_SUBNET_INFO"/> structure that contains specific settings for the subnet, including the subnet mask and IP address of the subnet gateway.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpCreateSubnet(string ServerIpAddress, DHCP_IP_ADDRESS SubnetAddress, ref DHCP_SUBNET_INFO SubnetInfo);

        /// <summary>
        /// The DhcpCreateSubnet function creates a new subnet on the DHCP server.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="SubnetAddress"><see cref="DHCP_IP_ADDRESS"/> value that contains the IP address of the subnet's gateway.</param>
        /// <param name="SubnetInfo"><see cref="DHCP_SUBNET_INFO"/> structure that contains specific settings for the subnet, including the subnet mask and IP address of the subnet gateway.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpCreateSubnetVQ(string ServerIpAddress, DHCP_IP_ADDRESS SubnetAddress, ref DHCP_SUBNET_INFO_VQ SubnetInfo);

        /// <summary>
        /// The DhcpEnumSubnets function returns an enumerated list of subnets defined on the DHCP server.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="ResumeHandle">Pointer to a DHCP_RESUME_HANDLE value that identifies the enumeration operation. Initially, this value should be zero, with a successful call returning the handle value used for subsequent enumeration requests. For example, if PreferredMaximum is set to 100, and 200 subnet addresses are stored on the server, the resume handle can be used after the first 100 subnets are retrieved to obtain the next 100 on a subsequent call, and so forth.</param>
        /// <param name="PreferredMaximum">Specifies the preferred maximum number of subnet addresses to return. If the number of remaining unenumerated options is less than this value, then that amount will be returned.</param>
        /// <param name="EnumInfo">Pointer to a <see cref="DHCP_IP_ARRAY"/> structure that contains the subnet IDs available on the DHCP server. If no subnets are defined, this value will be null.</param>
        /// <param name="ElementsRead">Pointer to a DWORD value that specifies the number of subnet addresses returned in EnumInfo.</param>
        /// <param name="ElementsTotal">Pointer to a DWORD value that specifies the number of subnets defined on the DHCP server that have not yet been enumerated.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. If a call is made with the same ResumeHandle value and all items on the server have been enumerated, this method returns ERROR_NO_MORE_ITEMS with ElementsRead and ElementsTotal set to 0. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        /// <remarks>When no longer needed, the resources consumed for the enumerated data, and all pointers containted within, should be released with DhcpRpcFreeMemory. This function requires host byte ordering for all DHCP_IP_ADDRESS values in parameter structures.</remarks>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpEnumSubnets(string ServerIpAddress, ref IntPtr ResumeHandle, uint PreferredMaximum, out IntPtr EnumInfo, out int ElementsRead, out int ElementsTotal);

        /// <summary>
        /// The DhcpGetSubnetInfo function returns information on a specific subnet.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="SubnetAddress">DHCP_IP_ADDRESS value that specifies the subnet ID.</param>
        /// <param name="SubnetInfo"><see cref="DHCP_SUBNET_INFO"/> structure that contains the returned information for the subnet matching the ID specified by SubnetAddress. Note: The memory for this parameter must be free using <see cref="DhcpRpcFreeMemory"/>.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        /// <remarks>This function uses host byte ordering for all DHCP_IP_ADDRESS values in the <see cref="DHCP_SUBNET_INFO"/> structure passed back to the caller in the SubnetInfo parameter. However, this function uses network byte order for the IpAddress member of the DHCP_HOST_INFO structure within the <see cref="DHCP_SUBNET_INFO"/> structure.</remarks>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpGetSubnetInfo(string ServerIpAddress, DHCP_IP_ADDRESS SubnetAddress, out IntPtr SubnetInfo);

        /// <summary>
        /// The DhcpGetSubnetInfoVQ function retrieves the information about a specific IPv4 subnet defined on the DHCP server.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="SubnetAddress">DHCP_IP_ADDRESS value that specifies the subnet ID.</param>
        /// <param name="SubnetInfo"><see cref="DHCP_SUBNET_INFO"/> structure that contains the returned information for the subnet matching the IPv4 address specified by SubnetAddress. Note: The memory for this parameter must be free using <see cref="DhcpRpcFreeMemory"/>.</param>        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpGetSubnetInfoVQ(string ServerIpAddress, DHCP_IP_ADDRESS SubnetAddress, out IntPtr SubnetInfo);

        /// <summary>
        /// The DhcpGetAllOptions function returns an array that contains all options defined on the DHCP server.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="Flags">Specifies a bit flag that indicates whether or not the options are vendor-specific. If the qualification of vendor options is not necessary, this parameter should be 0. DHCP_FLAGS_OPTION_IS_VENDOR = This flag should be set if vendor-specific options are desired.</param>
        /// <param name="OptionStruct">Pointer to a DHCP_ALL_OPTIONS structure containing every option defined for a vendor or default class. If there are no options available on the server, this value will be null. Note: The memory for this parameter must be free using DhcpRpcFreeMemory.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        /// <remarks>There will be one option element in the array specified by OptionStruct for each vendor/class pair defined on the DHCP server.</remarks>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpGetAllOptions(string ServerIpAddress, uint Flags, out IntPtr OptionStruct);

        /// <summary>
        /// The DhcpGetAllOptionValues function returns an array that contains all option values defined for a specific scope on the DHCP server.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="Flags">Specifies a bit flag that indicates whether the options are vendor-specific. If the qualification of vendor options is not necessary, this parameter should be 0.</param>
        /// <param name="ScopeInfo">Pointer to a DHCP_OPTION_SCOPE_INFO structure that contains information on the specific scope whose option values will be returned. This information defines the option values that are being retrieved from the default, server, or scope level, or for a specific IPv4 reservation.</param>
        /// <param name="Values">Pointer to a DHCP_ALL_OPTION_VALUES structure that contains the returned option values for the scope specified in ScopeInfo.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        /// <remarks>There will be one option value in the array specified by Values for each vendor/class pair defined on the DHCP server.</remarks>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpGetAllOptionValues(string ServerIpAddress, uint Flags, IntPtr ScopeInfo, out IntPtr Values);

        /// <summary>
        /// The DhcpGetOptionInfo function returns information on a specific DHCP option for the default user and vendor class.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IPv4 address of the DHCP server.</param>
        /// <param name="OptionID">DHCP_OPTION_ID value that specifies the code for the option to retrieve information on.</param>
        /// <param name="OptionInfo">Pointer to a DHCP_OPTION structure that contains the returned information on the option specified by OptionID. Note: The memory for this parameter must be free using DhcpRpcFreeMemory.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.
        /// ERROR_DHCP_OPTION_NOT_PRESENT = The specified option definition could not be found in the DHCP server database.</returns>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpGetOptionInfo(string ServerIpAddress, int OptionID, out IntPtr OptionInfo);

        /// <summary>
        /// The DhcpGetOptionInfoV5 function returns information on a specific DHCP option.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="Flags">Specifies a bit flag that indicates whether or not the option is vendor-specific. If it is not, this parameter should be 0.</param>
        /// <param name="OptionID">DHCP_OPTION_ID value that specifies the code for the option to retrieve information on.</param>
        /// <param name="ClassName">Unicode string that specifies the DHCP class name of the option. This parameter is optional.</param>
        /// <param name="VendorName">Unicode string that specifies the vendor of the option. This parameter is optional, and must be null when Flags is not set to DHCP_FLAGS_OPTION_IS_VENDOR. If it is not set, then the option definition for the default vendor class is returned.</param>
        /// <param name="OptionInfo">Pointer to a DHCP_OPTION structure that contains the returned information on the option specified by OptionID.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpGetOptionInfoV5(string ServerIpAddress, uint Flags, int OptionID, string ClassName, string VendorName, out IntPtr OptionInfo);

        /// <summary>
        /// The DhcpEnumSubnetClients function returns an enumerated list of clients with served IP addresses in the specified subnet.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="SubnetAddress">DHCP_IP_ADDRESS value that contains the subnet ID. See RFC 950 for more information about subnet ID.</param>
        /// <param name="ResumeHandle">Pointer to a DHCP_RESUME_HANDLE value that identifies the enumeration operation. Initially, this value should be zero, with a successful call returning the handle value used for subsequent enumeration requests. For example, if PreferredMaximum is set to 1000 bytes, and 2000 bytes worth of subnet client information structures are stored on the server, the resume handle can be used after the first 1000 bytes are retrieved to obtain the next 1000 on a subsequent call, and so forth.</param>
        /// <param name="PreferredMaximum">Specifies the preferred maximum number of bytes of subnet client information structures to return. If the number of remaining unenumerated options (in bytes) is less than this value, then that amount will be returned.  The minimum value is 1024 bytes (1KB), and the maximum value is 65536 bytes (64KB); if the input value is greater or less than this range, it will be set to the maximum or minimum value, respectively.</param>
        /// <param name="ClientInfo">Pointer to a DHCP_CLIENT_INFO_ARRAY structure that contains information on the clients served under this specific subnet. If no clients are available, this field will be null.</param>
        /// <param name="ClientsRead">Pointer to a DWORD value that specifies the number of clients returned in ClientInfo.</param>
        /// <param name="ClientsTotal">Pointer to a DWORD value that specifies the number of clients for the specified subnet that have not yet been enumerated. Note  This value is set to the correct value during the final enumeration call; however, prior calls to this function set the value as "0x7FFFFFFF".</param>
        /// <returns>This function returns ERROR_MORE_DATA upon a successful call. The final call to this method with the last set of subnet clients returns ERROR_SUCCESS. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        /// <remarks>This function requires host byte ordering for all DHCP_IP_ADDRESS values in parameter structures.</remarks>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpEnumSubnetClients(string ServerIpAddress, DHCP_IP_ADDRESS SubnetAddress, ref IntPtr ResumeHandle, uint PreferredMaximum, out IntPtr ClientInfo, out int ClientsRead, out int ClientsTotal);

        /// <summary>
        /// The DhcpEnumSubnetClientsV4 function returns an enumerated list of client lease records with served IP addresses in the specified subnet. This function extends the functionality provided in DhcpEnumSubnetClients by returning a list of DHCP_CLIENT_INFO_V4 structures that contain the specific client type (DHCP and/or BOOTP).
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="SubnetAddress">DHCP_IP_ADDRESS value containing the IP address of the subnet gateway.</param>
        /// <param name="ResumeHandle">Pointer to a DHCP_RESUME_HANDLE value that identifies the enumeration operation. Initially, this value should be zero, with a successful call returning the handle value used for subsequent enumeration requests. This parameter contains the last last IPv4 address retrieved from the DHCPv4 client. The presence of additional enumerable data is indicated when this function returns ERROR_MORE_DATA. If no additional enumerable data is available on the DHCPv4 server, ERROR_NO_MORE_ITEMS is returned.</param>
        /// <param name="PreferredMaximum">Specifies the preferred maximum number of bytes of subnet client elements to return. If the number of remaining unenumerated elements (in bytes) is less than this value, then that amount will be returned. The minimum value is 1024 bytes, and the maximum value is 65536 bytes. To retrieve all the subnet client elements for the default user and vendor class at the specified level, set this parameter to 0xFFFFFFFF.</param>
        /// <param name="ClientInfo">Pointer to a DHCP_CLIENT_INFO_ARRAY_V4 structure that contains the DHCPv4 client lease record array. If no clients are available, this field will be null.</param>
        /// <param name="ClientsRead">Pointer to a DWORD value that specifies the number of client lease records returned in ClientInfo.</param>
        /// <param name="ClientsTotal">Pointer to a DWORD value that specifies the total number of client lease records remaining on the DHCPv4 server. For example, if there are 100 DHCPv4 lease records for an IPv4 subnet, and if 10 DHCPv4 lease records are enumerated per call, then this parameter would return a value of 90 after the first call.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        /// <remarks>The caller of this function must free the memory for ClientInfo after the call completes. </remarks>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpEnumSubnetClientsV4(string ServerIpAddress, DHCP_IP_ADDRESS SubnetAddress, ref IntPtr ResumeHandle, uint PreferredMaximum, out IntPtr ClientInfo, out int ClientsRead, out int ClientsTotal);

        /// <summary>
        /// The DhcpEnumSubnetClientsV5 function returns an enumerated list of clients with served IP addresses in the specified subnet. This function extends the features provided in the DhcpEnumSubnetClients function by returning a list of DHCP_CLIENT_INFO_V5 structures that contain the specific client type (DHCP and/or BOOTP) and the IP address state.
        /// </summary>
        /// <param name="ServerIpAddress">A UNICODE string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="SubnetAddress">A value containing the IP address of the subnet gateway. If this parameter is set to 0, then the DHCP clients for all IPv4 subnets defined on the DHCP server are returned.</param>
        /// <param name="ResumeHandle">A pointer to a handle that identifies the enumeration operation. Initially, this value should be zero, with a successful call returning the handle value used for subsequent enumeration requests. For example, if PreferredMaximum is set to 1000 bytes, and 2000 bytes worth of subnet client information structures are stored on the server, the resume handle can be used after the first 1000 bytes are retrieved to obtain the next 1000 on a subsequent call, and so forth.</param>
        /// <param name="PreferredMaximum">The preferred maximum number of bytes of subnet client information structures to return. If the number of remaining unenumerated options (in bytes) is less than this value, then that amount will be returned.</param>
        /// <param name="ClientInfo">A pointer to a DHCP_CLIENT_INFO_ARRAY_V5 structure containing information on the clients served under this specific subnet. If no clients are available, this field will be null.</param>
        /// <param name="ClientsRead">A pointer to a value that specifies the number of clients returned in ClientInfo.</param>
        /// <param name="ClientsTotal">A pointer to a value that specifies the total number of clients for the specified subnet stored on the DHCP server.</param>
        /// <returns>The DhcpEnumSubnetClientsV5 function returns ERROR_SUCCESS upon success. </returns>
        /// <remarks>The caller of this function must release the memory used by the DHCP_CLIENT_INFO_ARRAY_V5 structure returned in buffer pointed to by the ClientInfo parameter when the information is no longer needed.</remarks>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpEnumSubnetClientsV5(string ServerIpAddress, DHCP_IP_ADDRESS SubnetAddress, ref IntPtr ResumeHandle, uint PreferredMaximum, out IntPtr ClientInfo, out int ClientsRead, out int ClientsTotal);

        /// <summary>
        /// The DhcpEnumSubnetClientsVQ function retrieves all DHCP clients serviced from the specified IPv4 subnet.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="SubnetAddress">DHCP_IP_ADDRESS value that contains the IPv4 subnet for which the DHCP clients are returned. If this parameter is set to 0, the DHCP clients for all known IPv4 subnets are returned.</param>
        /// <param name="ResumeHandle">Pointer to a DHCP_RESUME_HANDLE value that identifies the enumeration operation on the DHCP server. Initially, this value must be set to 0. A successful call will return a handle value in this parameter, which can be passed to subsequent enumeration requests. The returned handle value is the last IPv4 address retrieved in the enumeration operation.</param>
        /// <param name="PreferredMaximum">Specifies the preferred maximum number of bytes to return in the enumeration operation. the minimum value is 1024 bytes, and the maximum value is 65536 bytes.</param>
        /// <param name="ClientInfo">Pointer to a DHCP_CLIENT_INFO_ARRAY_VQ structure that contains the DHCP client lease record set returned by the enumeration operation.</param>
        /// <param name="ClientsRead">Pointer to a value that specifies the number of DHCP client records returned in ClientInfo.</param>
        /// <param name="ClientsTotal">Pointer to a value that specifies the number of DHCP client record remaining and as-yet unreturned. For example, if there are 100 DHCP client records for a given IPv4 subnet, and if 10 client records are enumerated per call, then after the first call this value would return 90.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        /// <remarks>If SubnetAddress is set to zero (0), then all of the DHCP clients from all known IPv4 subnets. The caller of this function must free the data pointed to by ClientInfo.</remarks>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpEnumSubnetClientsVQ(string ServerIpAddress, DHCP_IP_ADDRESS SubnetAddress, ref IntPtr ResumeHandle, uint PreferredMaximum, out IntPtr ClientInfo, out int ClientsRead, out int ClientsTotal);

        /// <summary>
        /// The DhcpEnumSubnetElementsV5 function returns an enumerated list of elements for a specific DHCP subnet.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="SubnetAddress">DHCP_IP_ADDRESS value that specifies the address of the IP subnet whose elements will be enumerated.</param>
        /// <param name="EnumElementType">DHCP_SUBNET_ELEMENT_TYPE enumeration value that indicates the type of subnet element to enumerate.</param>
        /// <param name="ResumeHandle">Pointer to a DHCP_RESUME_HANDLE value that identifies the enumeration operation. Initially, this value should be zero, with a successful call returning the handle value used for subsequent enumeration requests. For example, if PreferredMaximum is set to 1000 bytes, and 2000 bytes worth of subnet elements are stored on the server, the resume handle can be used after the first 1000 bytes are retrieved to obtain the next 1000 on a subsequent call, and so forth.
        /// The presence of additional enumerable data is indicated when this function returns ERROR_MORE_DATA. If no additional enumerable data is available on the DHCPv4 server, ERROR_NO_MORE_ITEMS is returned.</param>
        /// <param name="PreferredMaximum">Specifies the preferred maximum number of bytes of subnet elements to return. If the number of remaining unenumerated options (in bytes) is less than this value, then that amount will be returned.
        /// To retrieve all the subnet client elements for the default user and vendor class at the specified level, set this parameter to 0xFFFFFFFF.</param>
        /// <param name="EnumElementInfo">Pointer to a DHCP_SUBNET_ELEMENT_INFO_ARRAY_V5 structure containing an enumerated list of all elements available for the specified subnet. If no elements are available for enumeration, this value will be null.</param>
        /// <param name="ElementsRead">Pointer to a DWORD value that specifies the number of subnet elements returned in EnumElementInfo.</param>
        /// <param name="ElementsTotal">Pointer to a DWORD value that specifies the total number of elements for the specified subnet.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes. Common errors include the following:</returns>
        /// <remarks>When no longer needed, the resources consumed for the enumerated data, and all pointers contained within, should be released with DhcpRpcFreeMemory.</remarks>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpEnumSubnetElementsV5(string ServerIpAddress, DHCP_IP_ADDRESS SubnetAddress, DHCP_SUBNET_ELEMENT_TYPE_V5 EnumElementType, ref IntPtr ResumeHandle, uint PreferredMaximum, out IntPtr EnumElementInfo, out int ElementsRead, out int ElementsTotal);

        /// <summary>
        /// The DhcpGetClientInfo function returns information about a specific DHCP client.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="SearchInfo">DHCP_SEARCH_INFO structure that contains the parameters for the search.</param>
        /// <param name="ClientInfo">Pointer to a DHCP_CLIENT_INFO structure that contains information describing the DHCP client that most closely matches the provided search parameters. If no client is found, this parameter will be null. Note: The memory for this parameter must be free using DhcpRpcFreeMemory.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        /// <remarks>This function requires host byte ordering for all DHCP_IP_ADDRESS values in parameter structures.</remarks>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpGetClientInfo(string ServerIpAddress, IntPtr SearchInfo, out IntPtr ClientInfo);

        /// <summary>
        /// The DhcpGetClientInfoVQ function retrieves DHCP client lease record information from the DHCP server database.
        /// </summary>
        /// <param name="ServerIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="SearchInfo">Pointer to a DHCP_SEARCH_INFO structure that defines the key used to search the client lease record database on the DHCP server for a particular client record.</param>
        /// <param name="ClientInfo">Pointer to the DHCP_CLIENT_INFO_VQ structure returned by a successful search operation.</param>
        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
        /// <remarks>The caller of this function must release the memory used by the DHCP_CLIENT_INFO_VQ structure returned in ClientInfo.</remarks>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpGetClientInfoVQ(string ServerIpAddress, IntPtr SearchInfo, out IntPtr ClientInfo);

        /// <summary>
        /// The DhcpAuditLogGetParams function returns the audit log configuration settings from the DHCP server.
        /// </summary>
        /// <param name="ServerIpAddress">Pointer to a Unicode string that specifies the IP address or hostname of the DHCP server.</param>
        /// <param name="Flags">Specifies a set of bit flags for filtering the audit log. Currently, this parameter is reserved and should be set to 0.</param>
        /// <param name="AuditLogDir">Unicode string that contains the directory where the audit log is stored as an absolute path within the file system.</param>
        /// <param name="DiskCheckInterval">Specifies the disk check interval for attempting to write the audit log to the specified file as the number of logged DHCP server events that should occur between checks. The default is 50 DHCP server events between checks.</param>
        /// <param name="MaxLogFilesSize">Specifies the maximum log file size, in bytes.</param>
        /// <param name="MinSpaceOnDisk">Specifies the minimum required disk space, in bytes, for audit log storage.</param>
        /// <returns></returns>
        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern DhcpErrors DhcpAuditLogGetParams(string ServerIpAddress, int Flags, [MarshalAs(UnmanagedType.LPWStr)] out string AuditLogDir, out int DiskCheckInterval, out int MaxLogFilesSize, out int MinSpaceOnDisk);

        /// <summary>
        /// The DhcpRpcFreeMemory function frees a block of buffer space returned as a parameter.
        /// </summary>
        /// <param name="BufferPointer">Pointer to an address that contains a structure (or structures, in the case of an array) returned as a parameter. </param>
        /// <remarks>This function should be called to release the memory consumed by any structures.</remarks>
        [DllImport("dhcpsapi.dll", SetLastError = true)]
        public static extern void DhcpRpcFreeMemory(IntPtr BufferPointer);

        /// <summary>
        /// Helper which calls <see cref="DhcpRpcFreeMemory(IntPtr)"/> and clears the pointer if != <see cref="IntPtr.Zero"/>
        /// </summary>
        public static void FreePointer(ref IntPtr Pointer)
        {
            if (Pointer != IntPtr.Zero)
            {
                DhcpRpcFreeMemory(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Helper which calls <see cref="DhcpRpcFreeMemory(IntPtr)"/> if the pointer != <see cref="IntPtr.Zero"/>
        /// </summary>
        public static void FreePointer(IntPtr Pointer)
        {
            if (Pointer != IntPtr.Zero)
            {
                DhcpRpcFreeMemory(Pointer);
            }
        }
    }
}
