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

        internal override Frame ApplyRoll(int bonusBall, Maybe<int> runningTotal)
        {
            var newRunningTotal = runningTotal.Bind(rt => Maybe.Just(rt + 10 + bonusBall));
            return new CompleteFrame(FrameNumber, newRunningTotal, FirstRoll, SecondRoll, IsLastFrame ? Maybe.Just(bonusBall) : NothingRoll);
        }
    }
}
