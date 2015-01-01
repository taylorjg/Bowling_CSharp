using MonadLib;

namespace BowlingLib
{
    public abstract class Frame
    {
        public static readonly Maybe<int> NothingRunningTotal = Maybe.Nothing<int>();
        public static readonly Maybe<int> NothingRoll = Maybe.Nothing<int>();

        protected Frame(
            int frameNumber,
            Maybe<int> runningTotal,
            Maybe<int> firstRoll,
            Maybe<int> secondRoll,
            Maybe<int> thirdRoll)
        {
            _frameNumber = frameNumber;
            _runningTotal = runningTotal;
            _firstRoll = firstRoll;
            _secondRoll = secondRoll;
            _thirdRoll = thirdRoll;
        }

        public int FrameNumber
        {
            get { return _frameNumber; }
        }

        public Maybe<int> RunningTotal
        {
            get { return _runningTotal; }
        }

        public virtual Maybe<int> FirstRoll
        {
            get { return _firstRoll; }
        }

        public virtual Maybe<int> SecondRoll
        {
            get { return _secondRoll; }
        }

        public virtual Maybe<int> ThirdRoll
        {
            get { return _thirdRoll; }
        }

        public bool IsLastFrame {
            get { return _frameNumber == Bowling.NumFrames; }
        }

        public bool IsStrikeFrame {
            get { return _firstRoll.FromMaybe(0) == Bowling.MaxPins; }
        }

        public bool IsSpareFrame {
            get
            {
                var r1 = _firstRoll.FromMaybe(0);
                var r2 = _secondRoll.FromMaybe(0);
                return r1 != Bowling.MaxPins && r1 + r2 == Bowling.MaxPins;
            }
        }

        internal abstract bool WillConsumeRoll { get; }
        internal abstract Frame ApplyRoll(int roll, Maybe<int> runningTotal);

        private readonly int _frameNumber;
        private readonly Maybe<int> _runningTotal;
        private readonly Maybe<int> _firstRoll;
        private readonly Maybe<int> _secondRoll;
        private readonly Maybe<int> _thirdRoll;
    }
}
