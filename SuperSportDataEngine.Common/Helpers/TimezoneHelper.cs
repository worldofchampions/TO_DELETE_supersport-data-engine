using System;

namespace SuperSportDataEngine.Common.Helpers
{
    public static class TimezoneHelper
    {
        // ReSharper disable once InconsistentNaming
        public static TimeZoneInfo LocalSAST => TimeZoneInfo.FindSystemTimeZoneById("South Africa Standard Time");

        // ReSharper disable once InconsistentNaming
        public static DateTimeOffset FromUtcToSastDateTimeOffset(this DateTimeOffset utcDateTimeOffset)
        {
            if(utcDateTimeOffset.Offset != TimeSpan.Zero)
                throw new ArgumentException(
                    "The DateTimeOffset passed into this method has " +
                    "an offset. This should be in UTC time.");

            // Add the offset from UTC of SAST to UTC time and return it.
            return utcDateTimeOffset.Add(
                LocalSAST.BaseUtcOffset);
        }

        // ReSharper disable once InconsistentNaming
        public static DateTime FromUtcToSastDateTime(this DateTime utcDateTime)
        {
            // Add the offset from UTC of SAST to UTC time and return it.
            return utcDateTime.Add(
                LocalSAST.BaseUtcOffset);
        }
    }
}
