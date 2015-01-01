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
            var frames = actual.ToArray();
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

        [Test]
        public void Test2()
        {
            var actual = Bowling.ProcessRolls(Enumerable.Repeat(10, 12));
            var frames = actual.ToArray();
            AssertFrame(frames[0], 1, Maybe.Just(30), Maybe.Just(10), NothingRoll);
            AssertFrame(frames[1], 2, Maybe.Just(60), Maybe.Just(10), NothingRoll);
            AssertFrame(frames[2], 3, Maybe.Just(90), Maybe.Just(10), NothingRoll);
            AssertFrame(frames[3], 4, Maybe.Just(120), Maybe.Just(10), NothingRoll);
            AssertFrame(frames[4], 5, Maybe.Just(150), Maybe.Just(10), NothingRoll);
            AssertFrame(frames[5], 6, Maybe.Just(180), Maybe.Just(10), NothingRoll);
            AssertFrame(frames[6], 7, Maybe.Just(210), Maybe.Just(10), NothingRoll);
            AssertFrame(frames[7], 8, Maybe.Just(240), Maybe.Just(10), NothingRoll);
            AssertFrame(frames[8], 9, Maybe.Just(270), Maybe.Just(10), NothingRoll);
            AssertFrame(frames[9], 10, Maybe.Just(300), Maybe.Just(10), Maybe.Just(10), Maybe.Just(10));
        }

        [Test]
        public void Test3()
        {
            var actual = Bowling.ProcessRolls(Enumerable.Repeat(5, 21));
            var frames = actual.ToArray();
            AssertFrame(frames[0], 1, Maybe.Just(15), Maybe.Just(5), Maybe.Just(5));
            AssertFrame(frames[1], 2, Maybe.Just(30), Maybe.Just(5), Maybe.Just(5));
            AssertFrame(frames[2], 3, Maybe.Just(45), Maybe.Just(5), Maybe.Just(5));
            AssertFrame(frames[3], 4, Maybe.Just(60), Maybe.Just(5), Maybe.Just(5));
            AssertFrame(frames[4], 5, Maybe.Just(75), Maybe.Just(5), Maybe.Just(5));
            AssertFrame(frames[5], 6, Maybe.Just(90), Maybe.Just(5), Maybe.Just(5));
            AssertFrame(frames[6], 7, Maybe.Just(105), Maybe.Just(5), Maybe.Just(5));
            AssertFrame(frames[7], 8, Maybe.Just(120), Maybe.Just(5), Maybe.Just(5));
            AssertFrame(frames[8], 9, Maybe.Just(135), Maybe.Just(5), Maybe.Just(5));
            AssertFrame(frames[9], 10, Maybe.Just(150), Maybe.Just(5), Maybe.Just(5), Maybe.Just(5));
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
            //Assert.That(frame.RunningTotal, Is.EqualTo(runningTotal));
            Assert.That(frame.FirstRoll, Is.EqualTo(firstRoll));
            Assert.That(frame.SecondRoll, Is.EqualTo(secondRoll));
            Assert.That(frame.ThirdRoll, Is.EqualTo(thirdRoll));
        }
    }
}
