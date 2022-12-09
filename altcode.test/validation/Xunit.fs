namespace AltCode.Validation

module Xunit =

  open NUnit.Framework
  open AltCode.Test.Xunit

  [<Test>]
  let StringContainsShouldPass () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = "Hello"
          Expected = "e" }

    AltAssert.Contains match1

  [<Test>]
  let StringContainsShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = "Hello"
          Expected = "?" }

    Assert.Throws<Xunit.Sdk.ContainsException>(fun _ -> AltAssert.Contains match1)
    |> ignore