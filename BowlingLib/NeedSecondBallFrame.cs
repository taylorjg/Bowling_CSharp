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

        internal override Frame ApplyRoll(int secondRoll, Maybe<int> runningTotal)
        {
            var firstRoll = FirstRoll.FromJust;
            var newRunningTotal = runningTotal.Bind(rt => Maybe.Just(rt + firstRoll + secondRoll));
            return (firstRoll + secondRoll == Bowling.MaxPins)
                       ? new SpareNeedOneBonusBallFrame(FrameNumber, firstRoll, secondRoll) as Frame
                       : new CompleteFrame(FrameNumber, newRunningTotal, FirstRoll, Maybe.Just(secondRoll), NothingRoll);
        }
    }
}
