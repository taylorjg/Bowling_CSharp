using BowlingLib;
using MonadLib;

namespace BowlingApp
{
    internal class Program
    {
        private static void Main()
        {
            Rolls.ChooseRolls().Bind(rolls =>
            {
                var lines = Formatting.FormatFrames(Bowling.ProcessRolls(rolls));
                lines.ForEach(HighlightingConsole.WriteLine);
                return Maybe.Just(new Unit());
            });
        }
    }
}
