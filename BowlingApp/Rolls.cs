using System;
using System.Collections.Generic;
using System.Linq;
using MonadLib;

namespace BowlingApp
{
    internal static class Rolls
    {
        public static Maybe<IEnumerable<int>> ChooseRolls()
        {
            Console.WriteLine();
            var currentOptionChar = 'a';

            MenuOfPredefinedRolls.ForEach(predefinedRolls =>
            {
                Console.WriteLine("{0}) {1}", currentOptionChar, RollsToString(predefinedRolls));
                currentOptionChar++;
            });

            Console.WriteLine("{0}) I want to enter my own list of rolls", currentOptionChar);
            Console.WriteLine();

            for (; ; )
            {
                Console.Write("Please make a selection ('a' - '{0}') or 'q' to quit: ", currentOptionChar);
                var line = Console.ReadLine();

                if (string.IsNullOrEmpty(line)) continue;

                if (line[0] == 'q') return Maybe.Nothing<IEnumerable<int>>();

                var choiceIndex = line[0] - 'a';

                if (choiceIndex >= 0 && choiceIndex < MenuOfPredefinedRolls.Length)
                {
                    return Maybe.Just(MenuOfPredefinedRolls[choiceIndex]);
                }

                if (choiceIndex == MenuOfPredefinedRolls.Length)
                {
                    return Maybe.Just(ReadRollsFromConsole());
                }
            }
        }

        public static string RollsToString(IEnumerable<int> predefinedRolls)
        {
            return string.Join(", ", predefinedRolls.Select(roll => Convert.ToString(roll)));
        }

        private static readonly IEnumerable<int>[] MenuOfPredefinedRolls =
            {
                new[] { 2, 8, 1, 2 },
                new[] { 10, 1, 2 },
                new[] { 10, 10, 1, 2 },
                new[] { 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0 },
                new[] { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
                new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10, 1, 2 },
                new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 3, 1 },
                new[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },
                new[] { 10, 7, 3, 7, 2, 9, 1, 10, 10, 10, 2, 3, 6, 4, 7, 3, 3 }
            };

        private static IEnumerable<int> ReadRollsFromConsole()
        {
            Console.Write("Enter your rolls (separated by commas): ");
            var line = Console.ReadLine();
            return (line == null)
                       ? Enumerable.Empty<int>()
                       : line
                             .Split(',')
                             .Select(s => s.Trim())
                             .Select(int.Parse);
        }
    }
}
