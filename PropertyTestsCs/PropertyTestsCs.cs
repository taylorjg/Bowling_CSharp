using System.Collections.Generic;
using System.Linq;
using BowlingLib;
using FsCheck;
using FsCheck.Fluent;
using FsCheckUtils;
using Microsoft.FSharp.Core;
using NUnit.Framework;

namespace PropertyTestsCs
{
    using Property = Gen<Rose<Result>>;

    [TestFixture]
    public class PropertyTestsCs
    {
        private static readonly Config MyConfig = Config.VerboseThrowOnFailure;

        private static readonly IEnumerable<int[]> NormalFrames =
            from r1 in Enumerable.Range(0, 10)
            from r2 in Enumerable.Range(0, 11)
            where r1 + r2 < 10
            select new[] {r1, r2};

        private static readonly IEnumerable<int[]> SpareFrames =
            from r1 in Enumerable.Range(0, 10)
            from r2 in Enumerable.Range(0, 11)
            where r1 + r2 == 10
            select new[] {r1, r2};

        private static readonly int[] StrikeFrame = new[] {10};

        private static readonly Gen<int[]> GenFrame = Any.WeighedGeneratorIn(
                new WeightAndValue<Gen<int[]>>(20, Any.ValueIn(NormalFrames)),
                new WeightAndValue<Gen<int[]>>(20, Any.ValueIn(SpareFrames)),
                new WeightAndValue<Gen<int[]>>(60, Any.Value(StrikeFrame)));

        private static int CalculateNumBonusBallsNeeded(IList<int> rollsForlastFrame)
        {
            if (rollsForlastFrame.Count == 1 && rollsForlastFrame[0] == 10) return 2;
            if (rollsForlastFrame.Count == 2 && rollsForlastFrame[0] + rollsForlastFrame[1] == 10) return 1;
            return 0;
        }

        private static readonly Gen<int[]> GenRolls =
            from tenFrames in GenFrame.MakeListOfLength(10)
            from bonusBallFrames in GenFrame.MakeListOfLength(2)
            let rollsForlastFrame = tenFrames.Last()
            let rollsForTenFrames = tenFrames.SelectMany(x => x)
            let bonusBalls = bonusBallFrames.SelectMany(x => x)
            let numBonusBallsNeeded = CalculateNumBonusBallsNeeded(rollsForlastFrame)
            select rollsForTenFrames.Concat(bonusBalls.Take(numBonusBallsNeeded)).ToArray();

        private static Property CheckFrameInvariant(Frame f)
        {
            var p1 = PropExtensions.Label(f.RunningTotal.IsJust, "RunningTotal");
            var p2 = PropExtensions.Label(f.FirstRoll.IsJust, "FirstRoll");

            Property p3;

            if (f.IsLastFrame)
            {
                var pa = PropExtensions.Label(f.SecondRoll.IsJust, "SecondRoll when last frame");
                var pb = PropExtensions.Label((f.IsStrikeFrame || f.IsSpareFrame) ? f.ThirdRoll.IsJust : f.ThirdRoll.IsNothing, "ThirdRoll when last frame");
                p3 = PropExtensions.And(pa, pb);
            }
            else
            {
                var r1 = f.FirstRoll.FromMaybe(0);
                var r2 = f.SecondRoll.FromMaybe(0);
                var pa = PropExtensions.Label(r1 + r2 <= 10, "r1 + r2 <= 10 when not last frame");
                var pb = PropExtensions.Label(f.ThirdRoll.IsNothing, "ThirdRoll when not last frame");
                p3 = PropExtensions.And(pa, pb);
            }

            return PropExtensions.AndAll(p1, p2, p3);
        }

        private static Property CheckFrameInvariantHoldsForAllFrames(IEnumerable<int> rolls)
        {
            var frames = Bowling.ProcessRolls(rolls);
            var properties = frames.Select(CheckFrameInvariant);
            return PropExtensions.AndAll(properties.ToArray());
        }

        [Test]
        public void FrameInvariantHoldsForAllFrames()
        {
            var arb = Arb.fromGen(GenRolls);
            var body = FSharpFunc<int[], Property>.FromConverter(CheckFrameInvariantHoldsForAllFrames);
            var property = Prop.forAll(arb, body);
            Check.One(MyConfig, property);
        }
    }
}
