module BowlingTests

open FsCheck
open FsCheck.NUnit
open BowlingLib

let seqNormalFrames = 
    seq {
        for r1 in 0..9 do
        for r2 in 0..10 do
        if r1 + r2 < 10 then yield [r1; r2]
    }

let seqSpareFrames = 
    seq {
        for r1 in 0..9 do
        for r2 in 0..10 do
        if r1 + r2 = 10 then yield [r1; r2]
    }

let strikeFrame = [10]

let genFrame =
    Gen.frequency [
        (20, Gen.elements seqNormalFrames);
        (20, Gen.elements seqSpareFrames);
        (60, Gen.constant strikeFrame)
    ]

let genRolls =

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

let checkFrameInvariant (f:Frame) =
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

let checkFrameInvariantHoldsForAllFrames rolls =
    let frames = Bowling.ProcessRolls rolls
    Seq.forall checkFrameInvariant frames

// let dontShrinkIntArrays =
//     new System.Func<int[], seq<int[]>>(fun _ -> Seq.empty) 

[<Property>]
let ``frame invariant holds for all frames``() = 
    let arb = Arb.fromGen(genRolls)
    Check.One(Config.VerboseThrowOnFailure, Prop.forAll arb checkFrameInvariantHoldsForAllFrames)
