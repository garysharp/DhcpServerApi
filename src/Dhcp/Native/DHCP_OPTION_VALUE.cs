using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DHCP_OPTION_VALUE structure defines a DHCP option value (just the option data with an associated ID tag).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_OPTION_VALUE : IDisposable
    {
        /// <summary>
        /// DHCP_OPTION_ID value that specifies a unique ID number for the option.
        /// </summary>
        public readonly int OptionID;
        /// <summary>
        /// DHCP_OPTION_DATA structure that contains the data for a DHCP server option.
        /// </summary>
        public readonly DHCP_OPTION_DATA Value;

        public void Dispose()
        {
            Value.Dispose();
        }
    }

    /// <summary>
    /// The DHCP_OPTION_VALUE structure defines a DHCP option value (just the option data with an associated ID tag).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_OPTION_VALUE_Managed : IDisposable
    {
        /// <summary>
        /// DHCP_OPTION_ID value that specifies a unique ID number for the option.
        /// </summary>
        public readonly int OptionID;
        /// <summary>
        /// DHCP_OPTION_DATA structure that contains the data for a DHCP server option.
        /// </summary>
        public readonly DHCP_OPTION_DATA_Managed Value;

        public DHCP_OPTION_VALUE_Managed(int optionID, DHCP_OPTION_DATA_Managed value)
        {
            OptionID = optionID;
            Value = value;
        }

        public void Dispose()
        {
            //Value.Dispose();
        }
    }
}
