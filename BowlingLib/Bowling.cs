using System.Collections.Generic;
using System.Linq;

namespace BowlingLib
{
    public static class Bowling
    {
        private class ProcessRollResult
        {
            public ProcessRollResult(Frame frame, bool rollWasConsumed)
            {
                _frame = frame;
                _rollWasConsumed = rollWasConsumed;
            }

            public Frame Frame
            {
                get { return _frame; }
            }

            public bool RollWasConsumed
            {
                get { return _rollWasConsumed; }
            }

            private readonly Frame _frame;
            private readonly bool _rollWasConsumed;
        }

        public static IEnumerable<Frame> ProcessRolls(IEnumerable<int> rolls)
        {
            var seed = Enumerable.Range(1, 10).Select(frameNumber => new InitialFrame(frameNumber) as Frame);
            return rolls.Aggregate(seed, (accumulator, roll) =>
                {
                    var rollWasConsumed = false;
                    return accumulator.Select(frame =>
                        {
                            if (rollWasConsumed) return frame;
                            var processRollResult = ProcessRoll(frame, roll);
                            rollWasConsumed = processRollResult.RollWasConsumed;
                            return processRollResult.Frame;
                        });
                });
        }

        private static ProcessRollResult ProcessRoll(Frame frame, int roll)
        {
            return new ProcessRollResult(frame.ApplyRoll(roll), frame.WillConsumeRoll);
        }
    }
}
