using System.Collections.Generic;
using System.Linq;

namespace BowlingLib
{
    public static class Bowling
    {
        public static IEnumerable<Frame> ProcessRolls(IEnumerable<int> rolls)
        {
            var seed = Enumerable.Range(1, 10).Select(frameNumber => new InitialFrame(frameNumber) as Frame);
            return rolls.Aggregate(seed, (accumulator, roll) =>
                {
                    var rollWasConsumed = false;
                    return accumulator.Select(frame =>
                        {
                            if (rollWasConsumed) return frame;
                            rollWasConsumed = frame.WillConsumeRoll;
                            return frame.ApplyRoll(roll);
                        });
                });
        }
    }
}
