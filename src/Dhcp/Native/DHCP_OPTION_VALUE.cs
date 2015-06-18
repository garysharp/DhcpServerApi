using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_OPTION_VALUE structure defines a DHCP option value (just the option data with an associated ID tag).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_OPTION_VALUE
    {
        /// <summary>
        /// DHCP_OPTION_ID value that specifies a unique ID number for the option.
        /// </summary>
        public int OptionID;
        /// <summary>
        /// DHCP_OPTION_DATA structure that contains the data for a DHCP server option.
        /// </summary>
        public DHCP_OPTION_DATA Value;
    }
}