using MonadLib;

namespace BowlingLib
{
    public class NeedSecondBallFrame : Frame
    {
        public NeedSecondBallFrame(int frameNumber, int firstRoll)
            : base(frameNumber, NothingRunningTotal, Maybe.Just(firstRoll), NothingRoll, NothingRoll)
        {
        }

        internal override bool WillConsumeRoll
        {
            get { return true; }
        }

        internal override Frame ApplyRoll(int secondRoll)
        {
            var firstRoll = FirstRoll.FromJust;
            return (firstRoll + secondRoll == 10)
                       ? new SpareNeedOneBonusBallFrame(FrameNumber, firstRoll, secondRoll) as Frame
                         // new rt = current rt + firstRoll + secondRoll
                       : new CompleteFrame(FrameNumber, NothingRunningTotal, FirstRoll, Maybe.Just(secondRoll), NothingRoll);
        }
    }
}
