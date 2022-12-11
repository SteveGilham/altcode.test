namespace AltCode.Validation

module Xunit =

  open System
  open System.Collections.Generic

  open NUnit.Framework
  open AltCode.Test.Xunit

  [<Test>]
  let StringContainsShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual "Hello")
        .WithExpected "e"

    AltAssert.Contains match1
    AltAssert.Contains(match1, StringComparison.Ordinal)

    let match2 =
      (AssertionMatch.Create().WithActual "Hello")
        .WithExpected "?"

    AltAssert.DoesNotContain match2
    AltAssert.DoesNotContain(match2, StringComparison.Ordinal)

  [<Test>]
  let StringContainsShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = "Hello"
          Expected = "?" }

    Assert.Throws<Xunit.Sdk.ContainsException>(fun _ -> AltAssert.Contains match1)
    |> ignore

    Assert.Throws<Xunit.Sdk.ContainsException>(fun _ ->
      AltAssert.Contains(match1, StringComparison.Ordinal))
    |> ignore

    let match2 =
      { AssertionMatch.Create() with
          Actual = "Hello"
          Expected = "e" }

    Assert.Throws<Xunit.Sdk.DoesNotContainException>(fun _ ->
      AltAssert.DoesNotContain match2)
    |> ignore

    Assert.Throws<Xunit.Sdk.DoesNotContainException>(fun _ ->
      AltAssert.DoesNotContain(match2, StringComparison.Ordinal))
    |> ignore

  [<Test>]
  let StringEndsWithShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual "Hello")
        .WithExpected "o"

    AltAssert.EndsWith match1
    AltAssert.EndsWith(match1, StringComparison.Ordinal)

  [<Test>]
  let StringEndsWithShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = "Hello"
          Expected = "H" }

    Assert.Throws<Xunit.Sdk.EndsWithException>(fun _ -> AltAssert.EndsWith match1)
    |> ignore

    Assert.Throws<Xunit.Sdk.EndsWithException>(fun _ ->
      AltAssert.EndsWith(match1, StringComparison.Ordinal))
    |> ignore

  let strcomp =
    StringComparer.Create(Globalization.CultureInfo.InvariantCulture, false)

  [<Test>]
  let EqualEnumerablesShouldPass () =
    let match1 =
      (AssertionMatch
        .Create()
        .WithActual [ "Hello"; "World" ])
        .WithExpected [ "Hello"; "World" ]

    // How better to say this??
    AltAssert.Equal<String list, String> match1
    AltAssert.Equal<String list, String>(match1, strcomp)

  [<Test>]
  let EqualEnumerablesWithShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = [ "Hello"; "World" ]
          Expected = [ "hello"; "world" ] }

    Assert.Throws<Xunit.Sdk.EqualException>(fun _ ->
      AltAssert.Equal<String list, String> match1)
    |> ignore

    Assert.Throws<Xunit.Sdk.EqualException>(fun _ ->
      AltAssert.Equal<String list, String>(match1, strcomp))
    |> ignore