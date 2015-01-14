using System;
using System.Collections.Generic;
using System.Linq;

namespace BowlingApp
{
    internal static class ConsoleUtils
    {
        private const Char Sep = '`';

        public static void WriteLine(string line)
        {
            var bits = LineToBits(line);
            WriteBits(bits);
        }

        private static IEnumerable<string> LineToBits(string line)
        {
            var pos = line.IndexOf(Sep);
            if (pos < 0) return Enumerable.Repeat(line, 1);
            var leftBit = line.Substring(0, pos);
            var remainderOfLine = line.Substring(pos + 1);
            return Enumerable.Repeat(leftBit, 1).Concat(LineToBits(remainderOfLine));
        }

        private static void WriteBits(IEnumerable<string> bits)
        {
            var x = ZipBits(bits);
            x.ForEach((tuple, _) =>
            {
                var text = tuple.Item1;
                var action = tuple.Item2;
                action(text);
            });
            Console.WriteLine();
        }

        private static IEnumerable<Tuple<string, Action<string>>> ZipBits(IEnumerable<string> bits)
        {
            Action<string> actionNormal = WriteNormal;
            Action<string> actionYellow = WriteYellow;
            Func<int, bool> isEven = i => i % 2 == 0;
            Func<int, Action<string>> chooseAction = index => isEven(index) ? actionNormal : actionYellow;

            return bits.Zip(
                Enumerable.Range(0, int.MaxValue),
                (bit, index) => Tuple.Create(bit, chooseAction(index)));
        }

        private static void WriteNormal(string text)
        {
            Console.Write(text);
        }

        private static void WriteYellow(string text)
        {
            var oldValue = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(text);
            Console.ForegroundColor = oldValue;
        }
    }
}
