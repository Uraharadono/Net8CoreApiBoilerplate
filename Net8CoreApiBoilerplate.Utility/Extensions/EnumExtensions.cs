using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Net8CoreApiBoilerplate.Utility.Attributes;
using Net8CoreApiBoilerplate.Utility.Exceptions;

namespace Net8CoreApiBoilerplate.Utility.Extensions
{
    public static class EnumExtensions
    {
        public static TEnum[] EnumerateFlags<TEnum>(this TEnum mask)
        {
            var enumMask = mask as Enum;
            if (enumMask == null)
                throw new AppException("Expected an enum.");

            return Enum.GetValues(typeof(TEnum)).Cast<Enum>()
                .Where(enumMask.HasFlag).Cast<TEnum>()
                .ToArray();
        }

        public static TEnum MergeFlags<TEnum>(this IEnumerable<TEnum> flags) where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
                throw new AppException("Expected an enum.");

            int result = flags.Aggregate(0, (current, f) => current | (int)(object)f);
            return (TEnum)(object)result;
        }

        public static bool IsDefined(this Enum value)
        {
            return Enum.IsDefined(value.GetType(), value);
        }

        public static int ToInt(this Enum value)
        {
            return Convert.ToInt32(value);
        }

        public static IEnumerable<T> ToEnumerable<T>()
        {
            return Enum.GetValues(typeof (T)).Cast<T>();
        }

        public static string GetDisplayNameOrDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null)
                return "";

            var dna = Attribute.GetCustomAttribute(field, typeof(DisplayNameAttribute)) as DisplayNameAttribute;
            if (dna != null)
                return dna.DisplayName;

            var da = Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) as DisplayAttribute;
            if (da != null)
                return da.Name;

            var desca = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return desca == null ? value.ToString() : desca.Description;
        }

        public static string GetEnumDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null)
                return "";

            var ed = Attribute.GetCustomAttribute(field, typeof(EnumDescription)) as EnumDescription;
            if (ed != null)
                return ed.Description;

            return value.ToString();
        }
    }
}
