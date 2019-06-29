using System;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    /// <summary>
    /// The DATE_TIME structure defines a 64-bit integer value that contains a date/time, expressed as the number of ticks (100-nanosecond increments) since 12:00 midnight, January 1, 1 C.E. in the Gregorian calendar. 
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal readonly struct DATE_TIME
    {
        [FieldOffset(0)]
        public readonly long dwDateTime;

        private DATE_TIME(long fileTimeUtc)
        {
            dwDateTime = fileTimeUtc;
        }

        private DateTime ToDateTime()
        {
            if (dwDateTime == 0)
                return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
            else if (dwDateTime == long.MaxValue)
                return DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
            else
                return DateTime.FromFileTimeUtc(dwDateTime);
        }

        public static DATE_TIME FromDateTime(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
                return new DATE_TIME(0);
            else if (dateTime == DateTime.MaxValue)
                return new DATE_TIME(long.MaxValue);
            else
                return new DATE_TIME(dateTime.ToFileTimeUtc());
        }

        public static implicit operator DateTime(DATE_TIME dateTime)
            => dateTime.ToDateTime();

        public static implicit operator DATE_TIME(DateTime dateTime)
            => FromDateTime(dateTime);
    }
}
