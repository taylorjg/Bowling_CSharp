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
            var seed = Tuple.Create(initialFrames, Maybe.Just(0));

            var aggregateResult = rolls.Aggregate(seed, (accumulator, roll) =>
                {
                    var currentFrames = accumulator.Item1;
                    var runningTotal = accumulator.Item2;
                    var rollWasConsumed = false;

                    var newFrames = currentFrames.Select(frame =>
                        {
                            if (rollWasConsumed) return frame;
                            rollWasConsumed = frame.WillConsumeRoll;
                            var newFrame = frame.ApplyRoll(roll, runningTotal);
                            runningTotal = newFrame.RunningTotal;
                            return newFrame;
                        });

                    return Tuple.Create(newFrames, runningTotal);
                });

            var resultFrames = aggregateResult.Item1;
            return resultFrames;
        }
    }
}
