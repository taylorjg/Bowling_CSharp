using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace BowlingApp
{
    internal static class EnumerableExtensions
    {
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            source.Select(t => { action(t); return 0; }).ToList();
        }
    }
}
