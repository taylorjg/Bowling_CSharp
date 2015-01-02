module BowlingTests

open NUnit.Framework
open FsCheck
open FsCheck.NUnit
open FsCheck.Fluent
open System.Linq
open BowlingLib

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
        return [rollsForTenFrames; Seq.take numBonusBallsNeeded bonusBalls] |> Seq.concat
    }
