namespace AltCode.Validation

module Xunit =

  open System
  open System.Collections.Generic

  open NUnit.Framework
  open AltCode.Test.Xunit

  type Example =
    | A
    | B of int
    | C of string

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
  let DoesNotMatchShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual "Hello")
        .WithExpected "x"

    AltAssert.DoesNotMatch match1

  [<Test>]
  let DoesNotMatchShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = "Hello"
          Expected = "l" }

    Assert.Throws<Xunit.Sdk.DoesNotMatchException>(fun _ -> AltAssert.DoesNotMatch match1)
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
  let EqualEnumerablesShouldFail () =
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

  let exrefcomp =
    { new IEqualityComparer<Example> with
        member this.Equals(x, y) = Object.ReferenceEquals(x, y)
        member this.GetHashCode(x) = x.GetHashCode() }

  [<Test>]
  let EqualItemsShouldPass () =
    let item = B 1

    let match1 =
      (AssertionMatch.Create().WithActual item)
        .WithExpected item

    AltAssert.Equal<Example> match1
    AltAssert.Equal<Example>(match1, exrefcomp)

  [<Test>]
  let EqualItemsShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = A
          Expected = C "hello" }

    Assert.Throws<Xunit.Sdk.EqualException>(fun _ -> AltAssert.Equal<Example> match1)
    |> ignore

    Assert.Throws<Xunit.Sdk.EqualException>(fun _ ->
      AltAssert.Equal<Example>(match1, exrefcomp))
    |> ignore

  [<Test>]
  let EqualScalarsShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual 5.0)
        .WithExpected 5.0

    AltAssert.Equal(match1, 2)
    AltAssert.Equal(match1, 2, MidpointRounding.ToZero)
    AltAssert.Equal(match1, 0.1)

    let match2 =
      (AssertionMatch.Create().WithActual 5.0M)
        .WithExpected 5.0M

    AltAssert.Equal(match2, 2)

    let match3 =
      (AssertionMatch.Create().WithActual 5.0f)
        .WithExpected 5.0f

    AltAssert.Equal(match3, 0.1f)


  [<Test>]
  let EqualScalarsShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = 5.0
          Expected = 6.0 }

    Assert.Throws<Xunit.Sdk.EqualException>(fun _ -> AltAssert.Equal(match1, 2))
    |> ignore

    Assert.Throws<Xunit.Sdk.EqualException>(fun _ ->
      AltAssert.Equal(match1, 2, MidpointRounding.ToZero))
    |> ignore

    Assert.Throws<Xunit.Sdk.EqualException>(fun _ -> AltAssert.Equal(match1, 0.1))
    |> ignore

    let match2 =
      { AssertionMatch.Create() with
          Actual = 5.0M
          Expected = 6.0M }

    Assert.Throws<Xunit.Sdk.EqualException>(fun _ -> AltAssert.Equal(match2, 2))
    |> ignore

    let match3 =
      { AssertionMatch.Create() with
          Actual = 5.0f
          Expected = 6.0f }

    Assert.Throws<Xunit.Sdk.EqualException>(fun _ -> AltAssert.Equal(match3, 0.1f))
    |> ignore