using System.Collections.Generic;
using System.Linq;

namespace BowlingLib
{
    public static class Bowling
    {
        public static IEnumerable<Frame> ProcessRolls(IEnumerable<int> rolls)
        {
            var seed = Enumerable.Range(1, 10).Select(frameNumber => new InitialFrame(frameNumber) as Frame);
            return rolls.Aggregate(seed, (accumulator, roll) => accumulator.Select(frame => ProcessRoll(frame, roll)));
        }

        private static Frame ProcessRoll(Frame frame, int roll)
        {
            return frame.ApplyRoll(roll);
        }
    }
}
