using System;
using System.Threading;

namespace Common.DateRanges
{
    public static class DateTimeHelpers
    {
        public static DateTime FirstDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        public static int DaysInMonth(this DateTime value)
        {
            return DateTime.DaysInMonth(value.Year, value.Month);
        }

        public static DateTime LastDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.DaysInMonth());
        }

        public static DateTime FirstDayOfWeek(this DateTime value)
        {
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var daysTillCurrentDay = value.DayOfWeek - cultureInfo.DateTimeFormat.FirstDayOfWeek;
            return value.AddDays(-daysTillCurrentDay).Date;
        }
    }
}