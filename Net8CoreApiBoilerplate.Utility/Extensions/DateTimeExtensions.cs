using System;
using System.Globalization;
using System.Linq;

namespace Net8CoreApiBoilerplate.Utility.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToDutchDateFormat(this DateTime dt)
        {
            DateTimeFormatInfo nlDtfi = new CultureInfo("nl-NL", false).DateTimeFormat;
            string result = dt.ToString(nlDtfi.ShortDatePattern);
            return result;
        }

        public static DateTime FromDutchDateFormat(this string dt)
        {
            DateTimeFormatInfo nlDtfi = new CultureInfo("nl-NL", false).DateTimeFormat;
            DateTime result = Convert.ToDateTime(dt, nlDtfi);
            return result;
        }

        public static string Timestamp(this DateTime dt)
        {
            return new DateTimeOffset(dt.ToUniversalTime())
                .ToUnixTimeSeconds()
                .ToString();
        }

        public static string ToReadableFormat(this DateTime dt)
        {
            string suffix;

            if (new[] { 11, 12, 13 }.Contains(dt.Day))
            {
                suffix = "th";
            }
            else switch (dt.Day % 10)
                {
                    case 1:
                        suffix = "st";
                        break;
                    case 2:
                        suffix = "nd";
                        break;
                    case 3:
                        suffix = "rd";
                        break;
                    default:
                        suffix = "th";
                        break;
                }

            return string.Format(
                dt.ToString("dddd, d{0} MMMM yyyy", CultureInfo.InvariantCulture),
                suffix
            );
        }
    }
}
