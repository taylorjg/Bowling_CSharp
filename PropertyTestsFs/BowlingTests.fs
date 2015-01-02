module BowlingTests

open NUnit.Framework
open FsCheck
open FsCheck.NUnit
open FsCheck.Fluent
open System.Linq
open BowlingLib

type MyArbitraries =
  static member NonShrinkingIntArray() =
      { new Arbitrary<int[]>() with
          override x.Generator = Gen.constant [||]
          override x.Shrinker _ = Seq.empty }    

[<SetUp>]
let setUp =
    DefaultArbitraries.Add<MyArbitraries>() |> ignore

let nonStrikeValidFrameRolls = 
    seq {
        for r1 in 0..9 do
        for r2 in 0..10 do
        if r1 + r2 <= 10 then yield [r1; r2]
    }

let genNonStrikeValidFrameRolls = Gen.elements nonStrikeValidFrameRolls
let genStrikeValidFrameRolls = Gen.constant [10]

let genValidFrame = Gen.frequency [(40, genNonStrikeValidFrameRolls); (60, genStrikeValidFrameRolls)]

let genRolls (genFrame:Gen<int list>) =

    let calculateNumBonusBallsNeeded rollsForlastFrame =
        match rollsForlastFrame with
            | [r1] when r1 = 10 -> 2
            | [r1; r2] when r1 + r2 = 10 -> 1
            | _ -> 0

    gen {
        let! tenFrames = Gen.listOfLength 10 genFrame
        let! bonusBallFrames = Gen.listOfLength 2 genFrame
        let rollsForlastFrame = Seq.last tenFrames
        let rollsForTenFrames = Seq.concat tenFrames
        let bonusBalls = Seq.concat bonusBallFrames
        let numBonusBallsNeeded = calculateNumBonusBallsNeeded rollsForlastFrame
        return [rollsForTenFrames; Seq.take numBonusBallsNeeded bonusBalls] |> Seq.concat |> Seq.toArray
    }

let checkFrameInvariant (f:Frame) : bool =
    f.RunningTotal.IsJust &&
    f.FirstRoll.IsJust &&
    if f.IsLastFrame then
        f.SecondRoll.IsJust &&
        if f.IsStrikeFrame || f.IsSpareFrame then f.ThirdRoll.IsJust else f.ThirdRoll.IsNothing
    else
        let r1 = f.FirstRoll.FromMaybe 0
        let r2 = f.SecondRoll.FromMaybe 0
        r1 + r2 <= 10 &&
        (if f.IsStrikeFrame then f.SecondRoll.IsNothing else true) &&
        f.ThirdRoll.IsNothing

let prop_FrameInvariantHoldsForAllFrames rolls : bool =
    let frames = Bowling.ProcessRolls rolls
    Seq.forall checkFrameInvariant frames

[<Property>]
let ``frame invariant holds for all frames``() = 
    let gen = genRolls genValidFrame
    let specBuilder = Spec.For (gen, prop_FrameInvariantHoldsForAllFrames)
    specBuilder.QuickCheckThrowOnFailure()
