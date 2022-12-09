namespace AltCode.Validation

open System

module NUnit =

  open NUnit.Framework
  open AltCode.Test.Nunit

  [<Test>]
  let ThatShouldPass () =
    let match1 =
      (Constraint.Create().WithActual true)
        .WithConstraint Is.True

    AltAssert.That match1

    let match2 =
      (Constraint.Create().WithActual 5.0)
        .WithConstraint(Is.GreaterThan 4.0)

    AltAssert.That(match2, (fun () -> "bang!"))

    let match3 =
      { Constraint.Create() with
          Actual = 5
          Constraint = (Is.Not.EqualTo 4) }

    AltAssert.That(match3, "bang {0} {2}", 1, 2, 3, 4)

  [<Test>]
  let ThatShouldFail () =
    let match1 =
      (Constraint.Create().WithActual true)
        .WithConstraint Is.Not.True

    Assert.Throws<AssertionException>(fun _ -> AltAssert.That match1)
    |> ignore

    let match2 =
      (Constraint.Create().WithActual 5.0)
        .WithConstraint(Is.Not.GreaterThan 4.0)

    Assert.Throws<AssertionException>(fun _ -> AltAssert.That(match2, (fun () -> "bang!")))
    |> ignore

    let match3 =
      { Constraint.Create() with
          Actual = 5
          Constraint = (Is.EqualTo 4) }

    let x =
      Assert.Throws<AssertionException>(fun _ ->
        AltAssert.That(match3, "bang {0} {2}", 1, 2., 3, 4))

    let lines =
      [ "  bang 1 3"
        "  Expected: 4"
        "  But was:  5" ]
      |> List.map (fun s -> s + Environment.NewLine)

    let matchx =
      { Constraint.Create() with
          Actual = x.Message
          Constraint = (Is.EqualTo(String.Join(String.Empty, lines))) }

    AltAssert.That matchx

  [<Test>]
  let GreaterShouldPass () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = 5
          Expected = 4 }

    AltAssert.Greater match1

    let match2 =
      { AssertionMatch.Create() with
          Actual = 5.0
          Expected = 4.0 }

    AltAssert.Greater match2

  [<Test>]
  let GreaterShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = 5
          Expected = 6 }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.Greater match1)
    |> ignore

    let match2 =
      { AssertionMatch.Create() with
          Actual = 5.0
          Expected = 6.0 }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.Greater match2)
    |> ignore