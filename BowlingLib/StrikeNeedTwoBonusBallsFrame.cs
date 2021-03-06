﻿using MonadLib;

namespace BowlingLib
{
    public class StrikeNeedTwoBonusBallsFrame : Frame
    {
        public StrikeNeedTwoBonusBallsFrame(int frameNumber)
            : base(frameNumber, NothingRunningTotal, Maybe.Just(Bowling.MaxPins), NothingRoll, NothingRoll)
        {
        }

        internal override bool WillConsumeRoll
        {
            get { return false; }
        }

        internal override Frame ApplyRoll(int firstBonusBall, Maybe<int> runningTotal)
        {
            return new StrikeNeedOneBonusBallFrame(FrameNumber, firstBonusBall);
        }
    }
}
