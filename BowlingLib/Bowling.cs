using System.Collections.Generic;
using System.Linq;
using MonadLib;

namespace BowlingLib
{
    public static class Bowling
    {
        private class Accumulator
        {
            public Accumulator(IEnumerable<Frame> frames, Maybe<int> runningTotal)
            {
                _frames = frames;
                _runningTotal = runningTotal;
            }

            public IEnumerable<Frame> Frames
            {
                get { return _frames; }
            }

            public Maybe<int> RunningTotal
            {
                get { return _runningTotal; }
            }

            private readonly IEnumerable<Frame> _frames;
            private readonly Maybe<int> _runningTotal;
        }

        public static IEnumerable<Frame> ProcessRolls(IEnumerable<int> rolls)
        {
            var initialFrames = Enumerable.Range(1, 10).Select(frameNumber => new InitialFrame(frameNumber) as Frame);
            var seed = new Accumulator(initialFrames, Maybe.Just(0));
            var aggregateResult = rolls.Aggregate(seed, (accumulator, roll) =>
                {
                    var rollWasConsumed = false;
                    var runningTotal = accumulator.RunningTotal;
                    var newFrames = accumulator.Frames.Select(frame =>
                        {
                            if (rollWasConsumed) return frame;
                            rollWasConsumed = frame.WillConsumeRoll;
                            var newFrame = frame.ApplyRoll(roll, runningTotal);
                            runningTotal = newFrame.RunningTotal;
                            return newFrame;
                        });
                    return new Accumulator(newFrames, runningTotal);
                });
            return aggregateResult.Frames;
        }
    }
}
