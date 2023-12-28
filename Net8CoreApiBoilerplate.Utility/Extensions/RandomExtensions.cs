using System;
using System.Collections.Generic;
using System.Linq;

namespace Net8CoreApiBoilerplate.Utility.Extensions
{
    public static class RandomExtensions
    {
        /// <summary>
        /// Returns a random bool.
        /// </summary>
        /// <param name="r">Object of type random</param>
        /// <param name="percentage">Chance that the returned bool is true.</param>
        public static bool NextChance(this Random r, int percentage)
        {
            return r.Next(100) < percentage;
        }

        /// <summary>
        /// Returns a random valid value of the specified enum.
        /// </summary>
        /// <typeparam name="T">Must be an enum.</typeparam>
        public static T NextEnumValue<T>(this Random r) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be of type enum");

            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(r.Next(0, values.Length));
        }

        /// <summary>
        /// Returns a random valid value of the specified enum.
        /// </summary>
        /// <param name="r">Object of type Random</param>
        /// <param name="enumType">Must be an enum.</param>
        public static object NextEnumValue(this Random r, Type enumType)
        {
            if (!enumType.IsEnum) throw new ArgumentException("Property must be of type enum", "enumType");

            var values = Enum.GetValues(enumType);
            return values.GetValue(r.Next(0, values.Length));
        }

        /// <summary>
        /// Returns a random decimal between 0.0 and 1.0.
        /// </summary>
        public static decimal NextDecimal(this Random r)
        {
            return (decimal) r.NextDouble();
        }

        /// <summary>
        /// Returns a random decimal between 0.0 and max (exclusive).
        /// </summary>
        /// <param name="r">Object of type Random</param>
        /// <param name="max">Max value (exclusive).</param>
        public static decimal NextDecimal(this Random r, decimal max)
        {
            return (decimal) r.NextDouble() * max;
        }

        /// <summary>
        /// Returns a random decimal between min (inclusive) and max (exclusive).
        /// </summary>
        /// <param name="r">Object of type Random</param>
        /// <param name="min">Min value (inclusive).</param>
        /// <param name="max">Max value (exclusive).</param>
        public static decimal NextDecimal(this Random r, decimal min, decimal max)
        {
            return (decimal) r.NextDouble() * (max - min) + min;
        }

        /// <summary>
        /// Returns a random decimal between min (inclusive) and max (exclusive).
        /// </summary>
        /// <param name="r">Object of type Random</param>
        /// <param name="min">Min value (inclusive).</param>
        /// <param name="max">Max value (exclusive).</param>
        /// <param name="decimals">Number of decimal places.</param>
        public static decimal NextDecimal(this Random r, decimal min, decimal max, int decimals)
        {
            return Math.Round((decimal) r.NextDouble() * (max - min) + min, decimals);
        }


        /// <summary>
        /// Returns an array of random decimals that add up to sum.
        /// </summary>
        /// <param name="r">Object of type Random</param>
        /// <param name="count">Length of the array.</param>
        /// <param name="sum">Adding up all decimals in the array will result in this value.</param>
        /// <param name="decimals">Number of decimal places for each decimal in the array.</param>
        public static decimal[] NextDecimalArray(this Random r, int count, decimal sum, int? decimals = null)
        {
            decimal max = (sum / count) * 2, total = 0;
            decimal[] array = new decimal[count];

            for (int i = 0; i < count - 1; i++)
            {
                decimal val = r.NextDecimal(Math.Min(max, sum - total));
                if (decimals.HasValue) val = Math.Round(val, decimals.Value);
                array[i] = val;
                total += val;
            }

            array[count - 1] = sum - total;
            r.Shuffle(array);

            return array;
        }

        /// <summary>
        /// Shuffles the order of an array in place.
        /// </summary>
        /// <param name="r">Object of type Random</param>
        /// <param name="array">The array to shuffle.</param>
        public static void Shuffle<T>(this Random r, T[] array)
        {
            int n = array.Length;

            while (n > 1)
            {
                int k = r.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        /// <summary>
        /// Returns a list of the specified type with a random amount of elements.
        /// </summary>
        /// <param name="r">Object of type Random</param>
        /// <param name="minSize">Min size of the resulting list (inclusive).</param>
        /// <param name="maxSize">Max size of the resulting list (exclusive).</param>
        /// <param name="action">Action to create a new element for the list.</param>
        public static List<T> NextList<T>(this Random r, int minSize, int maxSize, Func<T> action)
        {
            List<T> list = new List<T>();
            int length = r.Next(minSize, maxSize);
            for (int i = 0; i < length; i++) list.Add(action.Invoke());

            return list;
        }

        /// <summary>
        /// Returns a list of the specified type with a random amount of elements.
        /// </summary>
        /// <param name="r">Object of type Random</param>
        /// <param name="minSize">Min size of the resulting list (inclusive).</param>
        /// <param name="maxSize">Max size of the resulting list (exclusive).</param>
        /// <param name="useValueOnlyOnce">If true, any value from values will only be used once in the resulting list.</param>
        /// <param name="values">Possible element values to use.</param>
        public static List<T> NextList<T>(this Random r, int minSize, int maxSize, bool useValueOnlyOnce, params T[] values)
        {
            List<T> list = new List<T>();
            List<T> availableValues = values.ToList();
            if (useValueOnlyOnce && maxSize > values.Length)
                maxSize = values.Length;

            int length = r.Next(minSize, maxSize);
            for (int i = 0; i < length; i++)
            {
                int index = r.Next(availableValues.Count);
                list.Add(availableValues[index]);
                if (useValueOnlyOnce) availableValues.RemoveAt(index);
            }

            return list;
        }

        /// <summary>
        /// Returns a list of the specified type with a random amount of elements.
        /// </summary>
        /// <param name="r">Object of type Random</param>
        /// <param name="minSize">Min size of the resulting list (inclusive).</param>
        /// <param name="maxSize">Max size of the resulting list (exclusive).</param>
        /// <param name="useActionOnlyOnce">If true, any action from actions will only be used once in the resulting list.</param>
        /// <param name="actions">Possible element values to use.</param>
        public static List<T> NextList<T>(this Random r, int minSize, int maxSize, bool useActionOnlyOnce, params Func<T>[] actions)
        {
            List<T> list = new List<T>();
            List<Func<T>> availableActions = actions.ToList();
            if (useActionOnlyOnce && maxSize > actions.Length) maxSize = actions.Length;

            int length = r.Next(minSize, maxSize);

            for (int i = 0; i < length; i++)
            {
                int index = r.Next(availableActions.Count);
                list.Add(availableActions[index].Invoke());
                
                if (useActionOnlyOnce) availableActions.RemoveAt(index);
            }

            return list;
        }

        /// <summary>
        /// Returns a random element from the supplied list.
        /// </summary>
        /// <param name="r">Object of type Random</param>
        /// <param name="e">List of elements to choose from.</param>
        public static T NextElement<T>(this Random r, IEnumerable<T> e)
        {
            IEnumerable<T> enumerable = e as T[] ?? e.ToArray();
            return enumerable.ElementAt(r.Next(enumerable.Count()));
        }

        /// <summary>
        /// Returns a random DateTime between from and to.
        /// </summary>
        /// <param name="r">Object of type Random</param>
        /// <param name="from">Min DateTime (inclusive)</param>
        /// <param name="to">Max DateTime (exclusive)</param>
        public static DateTime NextDateTime(this Random r, DateTime from, DateTime to)
        {
            var ts = to - from;
            return from.AddMilliseconds(r.NextDouble() * ts.TotalMilliseconds);
        }
    }
}
