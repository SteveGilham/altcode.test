namespace AltCode.Validation

module Xunit =

  open NUnit.Framework
  open AltCode.Test.Xunit

  [<Test>]
  let StringContainsShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual "Hello")
        .WithExpected "e"

    AltAssert.Contains match1
    AltAssert.Contains(match1, System.StringComparison.Ordinal)

    let match2 =
      (AssertionMatch.Create().WithActual "Hello")
        .WithExpected "?"

    AltAssert.DoesNotContain match2
    AltAssert.DoesNotContain(match2, System.StringComparison.Ordinal)


  [<Test>]
  let StringContainsShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = "Hello"
          Expected = "?" }

    Assert.Throws<Xunit.Sdk.ContainsException>(fun _ -> AltAssert.Contains match1)
    |> ignore

    Assert.Throws<Xunit.Sdk.ContainsException>(fun _ ->
      AltAssert.Contains(match1, System.StringComparison.Ordinal))
    |> ignore

    let match2 =
      { AssertionMatch.Create() with
          Actual = "Hello"
          Expected = "e" }

    Assert.Throws<Xunit.Sdk.DoesNotContainException>(fun _ ->
      AltAssert.DoesNotContain match2)
    |> ignore

    Assert.Throws<Xunit.Sdk.DoesNotContainException>(fun _ ->
      AltAssert.DoesNotContain(match2, System.StringComparison.Ordinal))
    |> ignore