using MonadLib;

namespace BowlingLib
{
    public class StrikeNeedTwoBonusBallsFrame : Frame
    {
        public StrikeNeedTwoBonusBallsFrame(int frameNumber)
            : base(frameNumber, NothingRunningTotal, Maybe.Just(10), NothingRoll, NothingRoll)
        {
        }

        internal override bool WillConsumeRoll
        {
            get { return false; }
        }

        internal override Frame ApplyRoll(int firstBonusBall)
        {
            return new StrikeNeedOneBonusBallFrame(FrameNumber, firstBonusBall);
        }
    }
}
