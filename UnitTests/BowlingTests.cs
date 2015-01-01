using System.Linq;
using BowlingLib;
using MonadLib;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    internal class BowlingTests
    {
        private static readonly Maybe<int> NothingRoll = Maybe.Nothing<int>();

        [Test]
        public void Test1()
        {
            var actual = Bowling.ProcessRolls(new[] {10, 7, 3, 7, 2, 9, 1, 10, 10, 10, 2, 3, 6, 4, 7, 3, 3});
            Assert.That(actual.IsRight, Is.True);
            var frames = actual.Right.ToArray();
            AssertFrame(frames[0], 1, Maybe.Just(20), Maybe.Just(10), NothingRoll);
            AssertFrame(frames[1], 2, Maybe.Just(37), Maybe.Just(7), Maybe.Just(3));
            AssertFrame(frames[2], 3, Maybe.Just(46), Maybe.Just(7), Maybe.Just(2));
            AssertFrame(frames[3], 4, Maybe.Just(66), Maybe.Just(9), Maybe.Just(1));
            AssertFrame(frames[4], 5, Maybe.Just(96), Maybe.Just(10), NothingRoll);
            AssertFrame(frames[5], 6, Maybe.Just(118), Maybe.Just(10), NothingRoll);
            AssertFrame(frames[6], 7, Maybe.Just(133), Maybe.Just(10), NothingRoll);
            AssertFrame(frames[7], 8, Maybe.Just(138), Maybe.Just(2), Maybe.Just(3));
            AssertFrame(frames[8], 9, Maybe.Just(155), Maybe.Just(6), Maybe.Just(4));
            AssertFrame(frames[9], 10, Maybe.Just(168), Maybe.Just(7), Maybe.Just(3), Maybe.Just(3));
        }

        private static void AssertFrame(
            Frame frame,
            int frameNumber,
            Maybe<int> runningTotal,
            Maybe<int> firstRoll,
            Maybe<int> secondRoll,
            Maybe<int> thirdRoll = null)
        {
            if (thirdRoll == null) thirdRoll = NothingRoll;

            Assert.That(frame.FrameNumber, Is.EqualTo(frameNumber));
            Assert.That(frame.FirstRoll, Is.EqualTo(firstRoll));
            Assert.That(frame.SecondRoll, Is.EqualTo(secondRoll));
            Assert.That(frame.ThirdRoll, Is.EqualTo(thirdRoll));
            Assert.That(frame.RunningTotal, Is.EqualTo(runningTotal));
        }
    }
}
