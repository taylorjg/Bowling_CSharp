module PropWithLabels

open FsCheck
open FsCheck.NUnit

[<Property>]
let ``property that uses labels``() =
    let arb = Arb.from<int list>

    let property1 xs = List.rev(List.rev xs) = xs |@ "my label"
    Check.Quick(Prop.forAll arb property1)

    let property2 xs = List.rev xs = xs |@ "my label"
    Check.Quick(Prop.forAll arb property2)
