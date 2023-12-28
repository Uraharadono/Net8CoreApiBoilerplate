using System;
using System.Collections.Generic;
using System.Linq;

namespace Net8CoreApiBoilerplate.Utility.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || !list.Any();
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> list, T item)
        {
            return list.Except(new[] { item });
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static IEnumerable<T> Push<T>(this IEnumerable<T> items, T newItem)
        {
            return newItem == null ? items : items.Concat(new[] { newItem });
        }

        public static IEnumerable<T> Push<T>(this IEnumerable<T> items, T newItem, bool onlyIf)
        {
            return onlyIf ? items.Push(newItem) : items;
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> items, T newItem)
        {
            return new[] { newItem }.Concat(items);
        }

        public static T[] ToArrayOrEmpty<T>(this IEnumerable<T> items)
        {
            return items?.ToArray() ?? Enumerable.Empty<T>().ToArray();
        }
    }
}
