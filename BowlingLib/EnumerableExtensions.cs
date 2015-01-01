using System.Collections.Generic;
using System.Linq;

namespace BowlingLib
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> Append<T>(this IEnumerable<T> xs, T x)
        {
            return xs.Concat(new[] { x });
        }
    }
}
