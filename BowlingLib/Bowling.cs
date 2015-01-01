using System;
using System.Collections.Generic;
using System.Linq;
using MonadLib;

namespace BowlingLib
{
    public static class Bowling
    {
        public const int NumFrames = 10;
        public const int MaxPins = 10;

        public static IEnumerable<Frame> ProcessRolls(IEnumerable<int> rolls)
        {
            var initialFrames = Enumerable.Range(1, NumFrames).Select(frameNumber => new InitialFrame(frameNumber) as Frame);

            var seed1 = Tuple.Create(initialFrames);
            var aggregateResult = rolls.Aggregate(seed1, (accumulator1, roll) =>
                {
                    var currentFrames = accumulator1.Item1;

                    var seed2 = Tuple.Create(Enumerable.Empty<Frame>(), Maybe.Just(0), false);
                    var aggregateResult2 = currentFrames.Aggregate(seed2, (accumulator2, oldFrame) =>
                        {
                            var newFrames = accumulator2.Item1;
                            var runningTotal = accumulator2.Item2;
                            var rollWasConsumed = accumulator2.Item3;

                            if (rollWasConsumed)
                            {
                                return Tuple.Create(newFrames.Append(oldFrame), Maybe.Nothing<int>(), true);
                            }

                            var newFrame = oldFrame.ApplyRoll(roll, runningTotal);
                            var newRunningTotal = newFrame.RunningTotal;
                            var newRollWasConsumed = oldFrame.WillConsumeRoll;

                            return Tuple.Create(newFrames.Append(newFrame), newRunningTotal, newRollWasConsumed);
                        });

                    var latestFrames = aggregateResult2.Item1;
                    return Tuple.Create(latestFrames);
                });

            var resultantFrames = aggregateResult.Item1;
            return resultantFrames;
        }
    }
}
