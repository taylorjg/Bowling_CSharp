using MonadLib;

namespace BowlingLib
{
    public class SpareNeedOneBonusBallFrame : Frame
    {
        public SpareNeedOneBonusBallFrame(int frameNumber, int firstRoll, int secondRoll)
            : base(frameNumber, NothingRunningTotal, Maybe.Just(firstRoll), Maybe.Just(secondRoll), NothingRoll)
        {
        }

        internal override bool WillConsumeRoll
        {
            get { return IsLastFrame; }
        }

        internal override Frame ApplyRoll(int bonusBall)
        {
            // new rt = current rt + 10 + bonusBall
            return new CompleteFrame(FrameNumber, NothingRunningTotal, FirstRoll, SecondRoll, IsLastFrame ? Maybe.Just(bonusBall) : NothingRoll);
        }
    }
}
