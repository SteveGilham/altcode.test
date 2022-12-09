namespace AltCode.Validation

module NUnit =

  open NUnit.Framework
  open AltCode.Test.Nunit

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