using System;
using System.Text.RegularExpressions;

namespace Lib.Base
{
    public static class DateTimeExtensions
    {
        public static bool CompareIntersection(DateTime t1Start, DateTime t1End, DateTime t2Start, DateTime t2End)
        {
            TimeSpan ts1 = t2Start - t1Start;
            TimeSpan ts2;
            if (ts1.Ticks > 0)
            {
                ts2 = t2Start - t1End;
                if (ts2.Ticks >= 0)
                    return false;
                else
                    return true;
            }
            else
            {
                ts2 = t1Start - t2End;
                if (ts2.Ticks >= 0)
                    return false;
                else
                    return true;
            }
        }

        public static bool IsDate(this string source)
        {
            return Regex.IsMatch(source, @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-9]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$");
        }

        public static bool IsTime(this string source)
        {
            return Regex.IsMatch(source, @"^((20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$");
        }

        public static bool IsDateAndTime(this string source)
        {
            return Regex.IsMatch(source, @"^(((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$ ");
        }

        public static DateTime ToUtcDateTime(this string s)
        {
            return DateTime.Now;
        }

        public static string ToStringWithStandard(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string ToStringWithT(this DateTime dt)
        {
            return dt.ToString("'yyyy'-'MM'-'dd'T'HH'L'mm':'ss'");
        }

        public static bool IsWeekend(this DateTime dt)
        {
            return dt.DayOfWeek == DayOfWeek.Sunday || dt.DayOfWeek == DayOfWeek.Saturday;
        }
    }
}
