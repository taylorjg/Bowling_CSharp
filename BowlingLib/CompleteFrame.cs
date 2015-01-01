using MonadLib;

namespace BowlingLib
{
    public class CompleteFrame : Frame
    {
        public CompleteFrame(int frameNumber, Maybe<int> runningTotal, Maybe<int> firstRoll, Maybe<int> secondRoll, Maybe<int> thirdRoll)
            : base(frameNumber, runningTotal, firstRoll, secondRoll, thirdRoll)
        {
        }

        internal override bool WillConsumeRoll
        {
            get { return false; }
        }

        internal override Frame ApplyRoll(int _)
        {
            return this;
        }
    }
}
