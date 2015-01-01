using System.Collections.Generic;
using System.Linq;
using MonadLib;

namespace BowlingLib
{
    public class Frame
    {
        public const int MinPins = 0;
        public const int MaxPins = 10;
        public const int NumFrames = 10;

        public Frame(
            int frameNumber,
            FrameState frameState,
            Maybe<int> runningTotal,
            Maybe<int> firstRoll,
            Maybe<int> secondRoll,
            int numBonusBallsNeeded,
            IEnumerable<int> bonusBalls)
        {
            _frameNumber = frameNumber;
            _frameState = frameState;
            _runningTotal = runningTotal;
            _firstRoll = firstRoll;
            _secondRoll = secondRoll;
            _numBonusBallsNeeded = numBonusBallsNeeded;
            _bonusBalls = bonusBalls;
        }

        public bool IsLastFrame {
            get { return FrameNumber == NumFrames; }
        }

        public bool IsStrikeFrame {
            get { return FirstRoll.Match(r => r == MaxPins, () => false); }
        }

        public bool IsSpareFrame {
            get
            {
                var r1 = _firstRoll.FromMaybe(0);
                var r2 = _secondRoll.FromMaybe(0);
                return r1 < MaxPins && r1 + r2 == MaxPins;
            }
        }

        public int FrameNumber
        {
            get { return _frameNumber; }
        }

        internal FrameState FrameState
        {
            get { return _frameState; }
        }

        public Maybe<int> RunningTotal
        {
            get { return _runningTotal; }
        }

        public Maybe<int> FirstRoll
        {
            get { return _firstRoll; }
        }

        public Maybe<int> SecondRoll
        {
            get
            {
                if (IsStrikeFrame && IsLastFrame && _bonusBalls.Any())
                {
                    return Maybe.Just(_bonusBalls.First());
                }

                return _secondRoll;
            }
        }

        public Maybe<int> ThirdRoll
        {
            get
            {
                var numBonusBalls = _bonusBalls.Count();

                if (IsStrikeFrame && IsLastFrame && numBonusBalls == 2)
                {
                    return Maybe.Just(_bonusBalls.Last());
                }

                if (IsSpareFrame && IsLastFrame && numBonusBalls == 1)
                {
                    return Maybe.Just(_bonusBalls.First());
                }

                return Maybe.Nothing<int>();
            }
        }

        internal int NumBonusBallsNeeded
        {
            get { return _numBonusBallsNeeded; }
        }

        internal IEnumerable<int> BonusBalls
        {
            get { return _bonusBalls; }
        }

        public int FrameScore
        {
            get
            {
                return
                    _firstRoll.FromMaybe(0) +
                    _secondRoll.FromMaybe(0) +
                    _bonusBalls.Sum();
            }
        }

        private readonly int _frameNumber;
        private readonly FrameState _frameState;
        private readonly Maybe<int> _runningTotal;
        private readonly Maybe<int> _firstRoll;
        private readonly Maybe<int> _secondRoll;
        private readonly int _numBonusBallsNeeded;
        private readonly IEnumerable<int> _bonusBalls;
    }
}
