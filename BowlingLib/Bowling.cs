using System;
using System.Collections.Generic;
using System.Linq;

namespace BowlingLib
{
    public static class Bowling
    {
        private class Accumulator
        {
            public Accumulator(IEnumerable<Frame> frames, bool rollWasConsumed)
            {
                _frames = frames;
                _rollWasConsumed = rollWasConsumed;
            }

            public IEnumerable<Frame> Frames
            {
                get { return _frames; }
            }

            public bool RollWasConsumed
            {
                get { return _rollWasConsumed; }
            }

            private readonly IEnumerable<Frame> _frames;
            private readonly bool _rollWasConsumed;
        }

        public static IEnumerable<Frame> ProcessRolls(IEnumerable<int> rolls)
        {
            var initialFrames = Enumerable.Range(1, 10).Select(frameNumber => new InitialFrame(frameNumber) as Frame);
            var seed = new Accumulator(initialFrames, false);
            var aggregateResult = rolls.Aggregate(seed, (accumulator, roll) =>
                {
                    var rollWasConsumed = false;
                    var newFrames = accumulator.Frames.Select(frame =>
                        {
                            if (rollWasConsumed) return frame;
                            var processRollResult = ProcessRoll(frame, roll);
                            rollWasConsumed = processRollResult.Item2;
                            return processRollResult.Item1;
                        });
                    return new Accumulator(newFrames, rollWasConsumed);
                });
            return aggregateResult.Frames;
        }

        private static Tuple<Frame, bool> ProcessRoll(Frame frame, int roll)
        {
            var rollWasConsumed = frame.WillConsumeRoll;
            var newFrame = frame.ApplyRoll(roll);
            return Tuple.Create(newFrame, rollWasConsumed);
        }
    }
}
