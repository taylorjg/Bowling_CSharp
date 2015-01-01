using MonadLib;

namespace BowlingLib
{
    public class InitialFrame : Frame
    {
        public InitialFrame(int frameNumber)
            : base(frameNumber, NothingRunningTotal, NothingRoll, NothingRoll, NothingRoll)
        {
        }

        internal override bool WillConsumeRoll
        {
            get { return true; }
        }

        internal override Frame ApplyRoll(int firstRoll, Maybe<int> _)
        {
            return (firstRoll == 10)
                       ? new StrikeNeedTwoBonusBallsFrame(FrameNumber) as Frame
                       : new NeedSecondBallFrame(FrameNumber, firstRoll);
        }
    }
}
