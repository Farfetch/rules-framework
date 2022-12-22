namespace Rules.Framework.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class LinqExtension
    {
        public static K MaxOrDefault<T, K>(this IEnumerable<T> enumeration, Func<T, K> selector)
        {
            return enumeration.Any() ? enumeration.Max(selector) : default;
        }

        public static K MinOrDefault<T, K>(this IEnumerable<T> enumeration, Func<T, K> selector)
        {
            return enumeration.Any() ? enumeration.Min(selector) : default;
        }
    }
}
