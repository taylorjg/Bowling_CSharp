using System;
using System.Collections.Generic;
using System.Linq;
using MonadLib;

namespace BowlingLib
{
    using OuterAccumulator = Tuple<IEnumerable<Frame>>;
    using InnerAccumulator = Tuple<IEnumerable<Frame>, Maybe<int>, bool>;

    public static class Bowling
    {
        public const int NumFrames = 10;
        public const int MaxPins = 10;

        public static IEnumerable<Frame> ProcessRolls(IEnumerable<int> rolls)
        {
            var initialFrames = Enumerable.Range(1, NumFrames).Select(frameNumber => new InitialFrame(frameNumber) as Frame);
            var outerSeed = Tuple.Create(initialFrames);
            var outerAggregateResult = rolls.Aggregate(outerSeed, ApplyRollToFrames);
            var resultantFrames = outerAggregateResult.Item1;
            return resultantFrames;
        }

        private static OuterAccumulator ApplyRollToFrames(OuterAccumulator outerAccumulator, int roll)
        {
            Func<InnerAccumulator, Frame, InnerAccumulator> partiallyAppliedApplyRollToFrame =
                (innerAccumulator, frame) => ApplyRollToFrame(roll, innerAccumulator, frame);

            var frames = outerAccumulator.Item1;
            var innerSeed = Tuple.Create(Enumerable.Empty<Frame>(), Maybe.Just(0), false);
            var innerAggregateResult = frames.Aggregate(innerSeed, partiallyAppliedApplyRollToFrame);
            var newFrames = innerAggregateResult.Item1;
            return Tuple.Create(newFrames);
        }

        private static InnerAccumulator ApplyRollToFrame(int roll, InnerAccumulator innerAccumulator, Frame oldFrame)
        {
            var newFrames = innerAccumulator.Item1;
            var runningTotal = innerAccumulator.Item2;
            var rollWasConsumed = innerAccumulator.Item3;

            if (rollWasConsumed)
            {
                return Tuple.Create(newFrames.Append(oldFrame), Maybe.Nothing<int>(), true);
            }

            var newFrame = oldFrame.ApplyRoll(roll, runningTotal);
            var newRunningTotal = newFrame.RunningTotal;
            var newRollWasConsumed = oldFrame.WillConsumeRoll;

            return Tuple.Create(newFrames.Append(newFrame), newRunningTotal, newRollWasConsumed);
        }
    }
}
