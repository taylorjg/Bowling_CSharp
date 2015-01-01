using MonadLib;

namespace BowlingLib
{
    public class StrikeNeedOneBonusBallFrame : Frame
    {
        public StrikeNeedOneBonusBallFrame(int frameNumber, int firstBonusBall)
            : base(frameNumber, NothingRunningTotal, Maybe.Just(Bowling.MaxPins), (frameNumber == Bowling.NumFrames) ? Maybe.Just(firstBonusBall) : NothingRoll, NothingRoll)
        {
            _firstBonusBall = firstBonusBall;
        }

        internal override bool WillConsumeRoll
        {
            get { return IsLastFrame; }
        }

        internal override Frame ApplyRoll(int secondBonusBall, Maybe<int> runningTotal)
        {
            var newRunningTotal = runningTotal.Bind(rt => Maybe.Just(rt + Bowling.MaxPins + _firstBonusBall + secondBonusBall));
            return (IsLastFrame)
                       ? new CompleteFrame(FrameNumber, newRunningTotal, FirstRoll, Maybe.Just(_firstBonusBall), Maybe.Just(secondBonusBall))
                       : new CompleteFrame(FrameNumber, newRunningTotal, FirstRoll, NothingRoll, NothingRoll);
        }

        private readonly int _firstBonusBall;
    }
}
