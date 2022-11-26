using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Net7CoreApiBoilerplate.Utility.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNull(this object o)
        {
            return o == null || o == DBNull.Value;
        }


        public static string ToJson(this object o)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            return JsonConvert.SerializeObject(o, Formatting.Indented, settings);
        }

        public static string ToJsonNoFormat(this object o)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            return JsonConvert.SerializeObject(o, Formatting.None, settings);
        }

        public static void EnsureNotNull(this object o,
            string paramName = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (o == null)
            {
                var s = $"{memberName} {sourceFilePath} {sourceLineNumber}";

                if (paramName == null)
                    throw new ArgumentNullException($"Value is null. {s}");
                throw new ArgumentNullException(paramName, s);
            }
        }

        //public static object GetPropertyValue(this object obj, string propertyName)
        //{
        //    var o1 = obj.GetType();

        //    var o2 =
        //        o1.GetProperties()
        //            .Where(current => string.Compare(current.Name, propertyName, true) == 0)
        //            .FirstOrDefault();

        //    object value = o2.GetValue(obj, null);

        //    return value;
        //}
    }
}
