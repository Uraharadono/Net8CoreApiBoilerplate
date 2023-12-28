using System;
using System.Globalization;
using System.Xml.Linq;

namespace Net8CoreApiBoilerplate.Utility.Extensions
{
    public static class XElementExtensions
    {
        public static string ElementAsString(this XElement parent, XName name)
        {
            return (string)parent.Element(name);
        }

        public static DateTime ElementAsDateTime(this XElement parent, XName name, string format = "yyyy-MM-ddTHH:mm:ss.fff")
        {
            var s = parent.ElementAsString(name);
            return DateTime.ParseExact(s, format, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        public static decimal ElementAsDecimal(this XElement parent, XName name)
        {
            var s = parent.ElementAsString(name);
            return decimal.Parse(s, NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo);
        }
    }
}
