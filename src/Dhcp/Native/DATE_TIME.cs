using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DATE_TIME structure defines a 64-bit integer value that contains a date/time, expressed as the number of ticks (100-nanosecond increments) since 12:00 midnight, January 1, 1 C.E. in the Gregorian calendar. 
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct DATE_TIME
    {
        /// <summary>
        /// Specifies the lower 32 bits of the time value.
        /// </summary>
        [FieldOffset(0)]
        public readonly uint dwLowDateTime;
        /// <summary>
        /// Specifies the upper 32 bits of the time value.
        /// </summary>
        [FieldOffset(4)]
        public readonly uint dwHighDateTime;

        [FieldOffset(0)]
        public readonly long dwDateTime;

        public DateTime ToDateTime()
        {
            if (dwDateTime == 0)
                return DateTime.MinValue;
            else if (dwDateTime == long.MaxValue)
                return DateTime.MaxValue;
            else
                return DateTime.FromFileTimeUtc(dwDateTime);
        }
    }
}
