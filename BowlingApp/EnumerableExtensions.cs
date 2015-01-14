using System;
using System.Collections.Generic;
using System.Linq;

namespace BowlingApp
{
    internal static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            source.Select((t, index) =>
                {
                    action(t, index);
                    return 0;
                }).ToList();
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }
    }
}
