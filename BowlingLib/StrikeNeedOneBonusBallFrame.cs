using MonadLib;

namespace BowlingLib
{
    public class StrikeNeedOneBonusBallFrame : Frame
    {
        public StrikeNeedOneBonusBallFrame(int frameNumber, int firstBonusBall)
            : base(frameNumber, NothingRunningTotal, Maybe.Just(10), (frameNumber == 10) ? Maybe.Just(firstBonusBall) : NothingRoll, NothingRoll)
        {
            _firstBonusBall = firstBonusBall;
        }

        internal override bool WillConsumeRoll
        {
            get { return IsLastFrame; }
        }

        internal override Frame ApplyRoll(int secondBonusBall)
        {
            // new rt = current rt + 10 + _firstBonusBall + secondBonusBall
            return (IsLastFrame)
                       ? new CompleteFrame(FrameNumber, NothingRunningTotal, FirstRoll, Maybe.Just(_firstBonusBall), Maybe.Just(secondBonusBall))
                       : new CompleteFrame(FrameNumber, NothingRunningTotal, FirstRoll, NothingRoll, NothingRoll);
        }

        private readonly int _firstBonusBall;
    }
}
