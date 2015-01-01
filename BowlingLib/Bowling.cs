using System;
using System.Collections.Generic;
using System.Linq;
using MonadLib;

using Frames = System.Collections.Generic.IEnumerable<BowlingLib.Frame>;
using Rolls = System.Collections.Generic.IEnumerable<int>;
using BowlingError = System.String;

namespace BowlingLib
{
    using EitherBowlingError = Either<BowlingError>;
    using BowlingResult = Either<BowlingError, Frames>;
    using OpAcc = Tuple<IEnumerable<Frame>, bool, Maybe<int>, Maybe<BowlingError>>;
    using ApplyRollToFrameResult = Tuple<Frame, bool, Maybe<int>, Maybe<BowlingError>>;

    public static class Bowling
    {
        public static BowlingResult ProcessRolls(Rolls rolls)
        {
            var initialFrames =
                Enumerable.Range(1, Frame.NumFrames)
                          .Select(frameNumber => new Frame(
                              frameNumber,
                              FrameState.ReadyForFirstRoll,
                              Maybe.Nothing<int>(),
                              Maybe.Nothing<int>(),
                              Maybe.Nothing<int>(),
                              0,
                              Enumerable.Empty<int>()));

            return Flinq.Enumerable.FoldLeft(
                rolls,
                EitherBowlingError.Right(initialFrames),
                ProcessRoll);
        }

        private static BowlingResult ProcessRoll(BowlingResult bowlingResult, int r)
        {
            if (bowlingResult.IsLeft) return bowlingResult;

            var framesIn = bowlingResult.Right;
            var seed = Tuple.Create(Enumerable.Empty<Frame>(), false, Maybe.Just(0), Maybe.Nothing<BowlingError>());

            Func<OpAcc, Frame, OpAcc> op =
                        (acc, f) =>
                            {
                                var fs1 = acc.Item1;
                                var consumed1 = acc.Item2;
                                var rt1 = acc.Item3;
                                var be1 = acc.Item4;
                                if (consumed1 || be1.IsJust)
                                {
                                    return Tuple.Create(Cons(f, fs1), consumed1, rt1, be1);
                                }

                                var applyRollToFrameResult = ApplyRollToFrame(f, r, rt1);
                                var f2 = applyRollToFrameResult.Item1;
                                var consumed2 = applyRollToFrameResult.Item2;
                                var rt2 = applyRollToFrameResult.Item3;
                                var be2 = applyRollToFrameResult.Item4;
                                return Tuple.Create(Cons(f2, fs1), consumed2, rt2, be2);
                            };

            var foldLeftResult = Flinq.Enumerable.FoldLeft(framesIn, seed, op);
            var fsOut = foldLeftResult.Item1.ToList();
            var consumed3 = foldLeftResult.Item2;
            var be3 = foldLeftResult.Item4;

            return be3.Match(EitherBowlingError.Left<Frames>, () =>
                {
                    if (fsOut.First().FrameState == FrameState.Complete && !consumed3)
                    {
                        return EitherBowlingError.Left<Frames>("Unconsumed rolls at the end of the list");
                    }

                    return EitherBowlingError.Right(fsOut.AsEnumerable().Reverse());
                });
        }

        private static IEnumerable<T> Cons<T>(T x, IEnumerable<T> xs)
        {
            return new[] {x}.Concat(xs);
        }

        private static ApplyRollToFrameResult ApplyRollToFrame(Frame f, int r, Maybe<int> rt)
        {
            if (r < Frame.MinPins || r > Frame.MaxPins)
            {
                var be = string.Format("Invalid roll: {0}", r);
                return Tuple.Create(f, false, Maybe.Nothing<int>(), Maybe.Just(be));
            }

            var smr = StateMachine[f.FrameState];
            var f2 = new Frame(
                    f.FrameNumber,
                    smr.StateFn(f, r),
                    smr.RunningTotalFn(f, r, rt),
                    smr.FirstRollFn(f, r),
                    smr.SecondRollFn(f, r),
                    smr.NumBonusBallsNeededFn(f, r),
                    smr.BonusBallsFn(f, r));
            var r1 = f2.FirstRoll.FromMaybe(0); // may need to use _firstRoll
            var r2 = f2.SecondRoll.FromMaybe(0); // may need to use _secondRoll
            var consumed = smr.ConsumedFn(f, r);

            if (r1 + r2 > Frame.MaxPins)
            {
                var be = string.Format("First and second rolls of frame number {0} have a total greater than {1}", f.FrameNumber, Frame.MaxPins);
                return Tuple.Create(f, false, Maybe.Nothing<int>(), Maybe.Just(be));
            }

            return Tuple.Create(f2, consumed, f2.RunningTotal, Maybe.Nothing<BowlingError>());
        }

        private static bool IsStrikeRoll(int roll)
        {
            return roll == Frame.MaxPins;
        }

        private static bool TwoRollsMakeSpare(Frame frame, int r2)
        {
            var r1 = frame.FirstRoll.FromMaybe(0);
            return r1 + r2 == Frame.MaxPins;
        }

        private static Maybe<int> CalcNewRunningTotal(Frame f, int r, Maybe<int> runningTotal)
        {
            return runningTotal.Match(rt => Maybe.Just(f.FrameScore + r + rt), Maybe.Nothing<int>);
        }

        private static readonly IDictionary<FrameState, StateMachineRow> StateMachine = new Dictionary<FrameState, StateMachineRow>
            {
                {
                    FrameState.ReadyForFirstRoll,
                    new StateMachineRow
                        {
                            StateFn = (_, r) => IsStrikeRoll(r) ? FrameState.NeedBonusBalls : FrameState.ReadyForSecondRoll,
                            FirstRollFn = (_, r) => Maybe.Just(r),
                            NumBonusBallsNeededFn = (_, r) => IsStrikeRoll(r) ? 2 : 0,
                            ConsumedFn = (_, __) => true
                        }
                },
                {
                    FrameState.ReadyForSecondRoll,
                    new StateMachineRow
                        {
                            StateFn = (f, r) => TwoRollsMakeSpare(f, r) ? FrameState.NeedBonusBalls : FrameState.Complete,
                            RunningTotalFn = (f, r, rt) => TwoRollsMakeSpare(f, r) ? Maybe.Nothing<int>() : CalcNewRunningTotal(f, r, rt),
                            SecondRollFn = (_, r) => Maybe.Just(r),
                            NumBonusBallsNeededFn = (f, r) => TwoRollsMakeSpare(f, r) ? 1 : 0,
                            ConsumedFn = (_, __) => true
                        }
                },
                {
                    FrameState.NeedBonusBalls,
                    new StateMachineRow
                        {
                            StateFn = (f, _) => f.NumBonusBallsNeeded == 1 ? FrameState.Complete : FrameState.NeedBonusBalls,
                            RunningTotalFn = (f, r, rt) => f.NumBonusBallsNeeded == 1 ? CalcNewRunningTotal(f, r, rt) : Maybe.Nothing<int>(),
                            NumBonusBallsNeededFn = (f, _) => f.NumBonusBallsNeeded - 1,
                            BonusBallsFn = (f, r) => f.BonusBalls.Concat(new[]{r}),
                            ConsumedFn = (f, _) => f.IsLastFrame
                        }
                },
                {
                    FrameState.Complete,
                    new StateMachineRow()
                }
            };
    }
}
