using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Net7CoreApiBoilerplate.Utility.Exceptions;

namespace Net7CoreApiBoilerplate.Utility.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNumeric(this string s)
        {
            return float.TryParse(s, out float _);
        }

        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNullOrWhiteSpace(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        public static Guid? TryParseGuid(this string s)
        {
            if (s == null)
                return null;
            return Guid.TryParse(s, out Guid guid) ? (Guid?)guid : null;
        }

        public static decimal? TryParseDecimal(this string s)
        {
            if (s == null)
                return null;

            return decimal.TryParse(s.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal d)
                ? (decimal?)d
                : null;
        }

        public static decimal ParseDecimal(this string s)
        {
            var d = s.TryParseDecimal();
            if (!d.HasValue)
                throw new AppException("Unable to parse {0} as decimal.", s);
            return d.Value;
        }

        public static decimal ParseDecimal(this string s, decimal defaultValue)
        {
            return s.TryParseDecimal().GetValueOrDefault(defaultValue);
        }

        public static int? TryParseInt(this string s)
        {
            if (s.IsNullOrWhiteSpace())
                return null;

            return int.TryParse(s.Replace(".", string.Empty).Replace(",", string.Empty), out var i)
                ? (int?)i
                : null;
        }

        public static long? TryParseLong(this string s)
        {
            if (s == null)
                return null;

            return long.TryParse(s.Replace(".", string.Empty).Replace(",", string.Empty), out long l)
                ? (long?)l
                : null;
        }

        public static int ParseInt(this string s)
        {
            var number = s.TryParseInt();
            if (!number.HasValue)
                throw new AppException("Unable to parse {0} as integer.", s);
            return number.Value;
        }

        public static string ReplaceWhiteSpacesWithSingleSpace(this string s)
        {
            return Regex.Replace(s, @"\s\s+", " ", RegexOptions.Multiline);
        }

        public static string RemoveNumeric(this string s)
        {
            return s == null ? null : string.Concat(s.Where(c => !char.IsNumber(c)));
        }

        public static string RemoveNonNumeric(this string s)
        {
            return s == null ? null : string.Concat(s.Where(char.IsNumber));
        }

        public static string RemoveNonLetterChars(this string s, bool preserveWhiteSpace)
        {
            if (s == null)
                return null;

            var reCharToDelete = (preserveWhiteSpace)
                ? new Regex("[\\P{L}-[\\s]]")
                : new Regex("[\\P{L}]");

            return reCharToDelete.Replace(s, "");
        }

        public static bool Contains(this string s, char c)
        {
            return s != null && s.IndexOf(c) >= 0;
        }

        public static bool ContainsAny(this string s, IEnumerable<char> characters)
        {
            return s != null && characters.Any(s.Contains);
        }

        public static string RemoveWhiteSpaces(this string s)
        {
            return s == null
                ? null
                : string.Concat(s.Where(c => !char.IsWhiteSpace(c)));
        }

        public static string JoinToString(this IEnumerable<string> items, string separator)
        {
            var local = items.ToArray();

            return !local.IsNullOrEmpty()
                ? string.Join(separator, local)
                : string.Empty;
        }

        public static string Combine(this string s1, string s2, string seperator)
        {
            if (s1 == null) return s2;
            if (s2 == null) return s1;

            if (s1.EndsWith(seperator))
                return s2.StartsWith(seperator) ? s1 + s2.Substring(1) : s1 + s2;

            return s2.StartsWith(seperator)
                ? s1 + s2
                : s1 + seperator + s2;
        }

        public static string PathCombine(this string s1, string s2)
        {
            return Combine(s1, s2, "\\");
        }

        public static string HttpCombine(this string s1, string s2)
        {
            return Combine(s1, s2, "/");
        }

        public static string GetExtension(this string fileName)
        {
            return fileName.Split('.').Last();
        }

        // TODO
        //public static string Base36Decode(this string s)
        //{
        //    return Base36Encoder.Base36.Decode(s).ToString();
        //}

        //public static string Base36Encode(this long s)
        //{
        //    return Base36Encoder.Base36.Encode(s);
        //}

        //public static string Base36Encode(this int s)
        //{
        //    return Convert.ToInt64(s).Base36Encode();
        //}

        public static void EnsureNotNullOrWhitespace(this string s,
            string paramName = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                var message = $"{memberName} {sourceFilePath} {sourceLineNumber}";

                if (paramName == null)
                    throw new ArgumentNullException(nameof(paramName));
                throw new ArgumentNullOrEmptyException(paramName, message);
            }
        }

        public static List<string> FindMentionsInString(this string source, string delimiter)
        {
            var mentionedUsers = new List<string>();
            foreach (var word in source.Split(" "))
            {
                if (word.Contains(delimiter))
                {
                    mentionedUsers.Add(word.Substring(word.IndexOf('@') + 1));
                }
            }
            return mentionedUsers;
        }
    }
}
