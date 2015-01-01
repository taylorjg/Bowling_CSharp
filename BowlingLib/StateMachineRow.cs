using System;
using System.Collections.Generic;
using MonadLib;

namespace BowlingLib
{
    internal class StateMachineRow
    {
        public Func<Frame, int, FrameState> StateFn = (f, _) => f.FrameState;
        public Func<Frame, int, Maybe<int>, Maybe<int>> RunningTotalFn = (f, _, __) => f.RunningTotal;
        public Func<Frame, int, Maybe<int>> FirstRollFn = (f, _) => f.FirstRoll; // may need to use _firstRoll
        public Func<Frame, int, Maybe<int>> SecondRollFn = (f, _) => f.SecondRoll; // may need to use _secondRoll
        public Func<Frame, int, int> NumBonusBallsNeededFn = (f, _) => f.NumBonusBallsNeeded;
        public Func<Frame, int, IEnumerable<int>> BonusBallsFn = (f, _) => f.BonusBalls;
        public Func<Frame, int, bool> ConsumedFn = (_, __) => false;
    }
}
