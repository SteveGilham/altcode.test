namespace AltCode.Validation

open System

module NUnit =

  open NUnit.Framework
  open AltCode.Test.Nunit

  type Example =
    | A
    | B of int
    | C of string

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
  let AreEqualShouldPass () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = 5
          Expected = 5 }

    AltAssert.AreEqual match1

    let match2 =
      { AssertionMatch.Create() with
          Actual = 5.0
          Expected = 5.0 }

    AltAssert.AreEqual(match2, 0.1)

    let match3 =
      { AssertionMatch.Create() with
          Actual = 5
          Expected = 5 }

    AltAssert.AreEqual(match3, "bang {0} {2}", 1, 2., 3, 4)

    let match4 =
      { AssertionMatch.Create() with
          Actual = 5.0
          Expected = 5.0 }

    AltAssert.AreEqual(match4, 0.1, "bang {0} {2}", 1, 2., 3, 4)

  [<Test>]
  let AreEqualShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = 5
          Expected = 6 }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.AreEqual match1)
    |> ignore

    let match2 =
      { AssertionMatch.Create() with
          Actual = 5.0
          Expected = 6.0 }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.AreEqual(match2, 0.1))
    |> ignore

    let match3 =
      { AssertionMatch.Create() with
          Actual = 5
          Expected = 6 }

    Assert.Throws<AssertionException>(fun _ ->
      AltAssert.AreEqual(match3, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

    let match4 =
      { AssertionMatch.Create() with
          Actual = 5.0
          Expected = 6.0 }

    Assert.Throws<AssertionException>(fun _ ->
      AltAssert.AreEqual(match4, 0.1, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let AreNotEqualShouldPass () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = 5
          Expected = 6 }

    AltAssert.AreNotEqual match1

    let match3 =
      { AssertionMatch.Create() with
          Actual = 5
          Expected = 6 }

    AltAssert.AreNotEqual(match3, "bang {0} {2}", 1, 2., 3, 4)

  [<Test>]
  let AreNotEqualShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = 5
          Expected = 5 }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.AreNotEqual match1)
    |> ignore

    let match3 =
      { AssertionMatch.Create() with
          Actual = 5
          Expected = 5 }

    Assert.Throws<AssertionException>(fun _ ->
      AltAssert.AreNotEqual(match3, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let AreSameShouldPass () =
    let x = B 5
    let match1 =
      { AssertionMatch.Create() with
          Actual = x
          Expected = x }

    AltAssert.AreSame match1

    let match3 =
      { AssertionMatch.Create() with
          Actual = A
          Expected = A }

    AltAssert.AreSame(match3, "bang {0} {2}", 1, 2., 3, 4)

  [<Test>]
  let AreSameShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = B 5
          Expected = B 5 }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.AreSame match1)
    |> ignore

    let match3 =
      { AssertionMatch.Create() with
          Actual = A
          Expected = C "6" }

    Assert.Throws<AssertionException>(fun _ ->
      AltAssert.AreSame(match3, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let AreNotSameShouldPass () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = B 5
          Expected = B 5 }

    AltAssert.AreNotSame match1

    let match3 =
      { AssertionMatch.Create() with
          Actual = A
          Expected = C "6" }

    AltAssert.AreNotSame(match3, "bang {0} {2}", 1, 2., 3, 4)

  [<Test>]
  let AreNotSameShouldFail () =
    let x = B 5

    let match1 =
      { AssertionMatch.Create() with
          Actual = x
          Expected = x }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.AreNotSame match1)
    |> ignore

    let match3 =
      { AssertionMatch.Create() with
          Actual = A
          Expected = A }

    Assert.Throws<AssertionException>(fun _ ->
      AltAssert.AreNotSame(match3, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

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

    AltAssert.Greater (match2, "bang {0} {2}", 1, 2., 3, 4)

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

    Assert.Throws<AssertionException>(fun _ -> AltAssert.Greater (match2, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let GreaterOrEqualShouldPass () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = 5
          Expected = 4 }

    AltAssert.GreaterOrEqual match1

    let match2 =
      { AssertionMatch.Create() with
          Actual = 5.0
          Expected = 4.0 }

    AltAssert.GreaterOrEqual (match2, "bang {0} {2}", 1, 2., 3, 4)

  [<Test>]
  let GreaterOrEqualShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = 5
          Expected = 6 }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.GreaterOrEqual match1)
    |> ignore

    let match2 =
      { AssertionMatch.Create() with
          Actual = 5.0
          Expected = 6.0 }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.GreaterOrEqual (match2, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let LessShouldPass () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = 5
          Expected = 6 }

    AltAssert.Less match1

    let match2 =
      { AssertionMatch.Create() with
          Actual = 5.0
          Expected = 6.0 }

    AltAssert.Less (match2, "bang {0} {2}", 1, 2., 3, 4)

  [<Test>]
  let LessShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = 5
          Expected = 4 }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.Less match1)
    |> ignore

    let match2 =
      { AssertionMatch.Create() with
          Actual = 5.0
          Expected = 4.0 }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.Less (match2, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let LessOrEqualShouldPass () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = 5
          Expected = 6 }

    AltAssert.LessOrEqual match1

    let match2 =
      { AssertionMatch.Create() with
          Actual = 5.0
          Expected = 6.0 }

    AltAssert.LessOrEqual (match2, "bang {0} {2}", 1, 2., 3, 4)

  [<Test>]
  let LessOrEqualShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = 5
          Expected = 4 }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.LessOrEqual match1)
    |> ignore

    let match2 =
      { AssertionMatch.Create() with
          Actual = 5.0
          Expected = 4.0 }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.LessOrEqual (match2, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore        