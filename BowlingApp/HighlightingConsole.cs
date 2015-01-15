using System;
using System.Collections.Generic;
using System.Linq;

namespace BowlingApp
{
    using Writer = Action<string>;

    internal static class HighlightingConsole
    {
        private const Char HighlightMarker = '`';
        private const ConsoleColor HighlightColour = ConsoleColor.Yellow;

        public static string Highlight(this string s)
        {
            return string.Format("{0}{1}{0}", HighlightMarker, s);
        }

        public static void WriteLine(string line)
        {
            WriteTextSegments(LineToTextSegments(line));
        }

        private static IEnumerable<string> LineToTextSegments(string line)
        {
            var pos = line.IndexOf(HighlightMarker);
            if (pos < 0) return Enumerable.Repeat(line, 1);
            var textSegment = line.Substring(0, pos);
            var remainderOfLine = line.Substring(pos + 1);
            return Enumerable.Repeat(textSegment, 1).Concat(LineToTextSegments(remainderOfLine));
        }

        private static void WriteTextSegments(IEnumerable<string> textSegments)
        {
            var categorisedTextSegments = CategoriseTextSegments(textSegments);
            categorisedTextSegments.ForEach(tuple =>
            {
                var textSegment = tuple.Item1;
                var write = tuple.Item2;
                write(textSegment);
            });
            Console.WriteLine();
        }

        private static IEnumerable<Tuple<string, Writer>> CategoriseTextSegments(IEnumerable<string> textSegments)
        {
            Func<int, bool> isEven = i => i % 2 == 0;
            Func<int, Writer> chooseWriter = index => isEven(index) ? NormalWriter : HighlightingWriter;

            return textSegments.Zip(
                Enumerable.Range(0, int.MaxValue),
                (textSegment, index) => Tuple.Create(textSegment, chooseWriter(index)));
        }

        private static readonly Writer NormalWriter = text =>
        {
            Console.Write(text);
        };

        private static readonly Writer HighlightingWriter = text =>
        {
            var oldValue = Console.ForegroundColor;
            Console.ForegroundColor = HighlightColour;
            Console.Write(text);
            Console.ForegroundColor = oldValue;
        };
    }
}
