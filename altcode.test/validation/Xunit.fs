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
        .WithExpected
        "e"

    AltAssert.Contains match1
    AltAssert.Contains(match1, StringComparison.Ordinal)

    let match2 =
      (AssertionMatch.Create().WithActual "Hello")
        .WithExpected
        "?"

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
        .WithExpected
        "x"

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
        .WithExpected
        "o"

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
      (AssertionMatch.Create().WithActual [ "Hello"; "World" ])
        .WithExpected
        [ "Hello"; "World" ]

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

    let func = Func<string, string, bool>(fun x y -> x.Equals y)
    Assert.Throws<Xunit.Sdk.EqualException>(fun _ ->
      AltAssert.Equal<String list, String>(match1, func))
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
        .WithExpected
        item

    AltAssert.Equal<Example> match1
    AltAssert.Equal<Example>(match1, exrefcomp)

    let match2 =
      { AssertionMatch.Create() with
          Actual = item.GetHashCode()
          Expected = exrefcomp.GetHashCode(item) }

    AltAssert.Equal match2

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

    let func = Func<_,_,bool>(fun x y -> Object.ReferenceEquals(x, y))
    Assert.Throws<Xunit.Sdk.EqualException>(fun _ ->
      AltAssert.Equal<Example>(match1, func))
    |> ignore

  [<Test>]
  let EqualScalarsShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual 5.0)
        .WithExpected
        5.0

    AltAssert.Equal(match1, 2)
    AltAssert.Equal(match1, 2, MidpointRounding.ToZero)
    AltAssert.Equal(match1, 0.1)

    let match2 =
      (AssertionMatch.Create().WithActual 5.0M)
        .WithExpected
        5.0M

    AltAssert.Equal(match2, 2)

    let match3 =
      (AssertionMatch.Create().WithActual 5.0f)
        .WithExpected
        5.0f

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

  [<Test>]
  let EqualDTsShouldPass () =
    let item = DateTime.UtcNow

    let match1 =
      (AssertionMatch.Create().WithActual item)
        .WithExpected
        item

    AltAssert.Equal(match1, TimeSpan(1, 0, 0))

  [<Test>]
  let EqualDTsShouldFail () =
    let item = DateTime.UtcNow
    let later = item + TimeSpan(1, 0, 0, 0)

    let match1 =
      { AssertionMatch.Create() with
          Actual = item
          Expected = later }

    Assert.Throws<Xunit.Sdk.EqualException>(fun _ ->
      AltAssert.Equal(match1, TimeSpan(1, 0, 0)))
    |> ignore

  [<Test>]
  let EqualStringsShouldPass () =
    let item = "DateTime.UtcNow"

    let match1 =
      (AssertionMatch.Create().WithActual item)
        .WithExpected
        item

    AltAssert.Equal match1
    AltAssert.Equal(match1, false, false, false)

  [<Test>]
  let EqualStringsShouldFail () =
    let item = "DateTime.UtcNow"
    let later = "item + TimeSpan(1, 0, 0, 0)"

    let match1 =
      { AssertionMatch.Create() with
          Actual = item
          Expected = later }

    Assert.Throws<Xunit.Sdk.EqualException>(fun _ -> AltAssert.Equal match1)
    |> ignore

    Assert.Throws<Xunit.Sdk.EqualException>(fun _ ->
      AltAssert.Equal(match1, false, false, false))
    |> ignore

  [<Test>]
  let EquivalentShouldPass () =
    let match1 =
      (AssertionMatch<Object>.Create().WithActual(B 1))
        .WithExpected(B 1)

    AltAssert.Equivalent(match1, true)

  [<Test>]
  let EquivalentShouldFail () =
    let match1 =
      { AssertionMatch<Object>.Create() with
          Actual = A
          Expected = C "ulater" }

    Assert.Throws<Xunit.Sdk.EquivalentException>(fun _ ->
      AltAssert.Equivalent(match1, false))
    |> ignore

  [<Test>]
  let MatchesShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual "Hello")
        .WithExpected
        "l"

    AltAssert.Matches match1

  [<Test>]
  let MatchesShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = "Hello"
          Expected = "x" }

    Assert.Throws<Xunit.Sdk.MatchesException>(fun _ -> AltAssert.Matches match1)
    |> ignore

  [<Test>]
  let NotEqualEnumerablesShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual [ "Hello"; "World" ])
        .WithExpected
        [ "hello"; "world" ]

    // How better to say this??
    AltAssert.NotEqual<String list, String> match1
    AltAssert.NotEqual<String list, String>(match1, strcomp)

  [<Test>]
  let NotEqualEnumerablesShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = [ "Hello"; "World" ]
          Expected = [ "Hello"; "World" ] }

    Assert.Throws<Xunit.Sdk.NotEqualException>(fun _ ->
      AltAssert.NotEqual<String list, String> match1)
    |> ignore

    Assert.Throws<Xunit.Sdk.NotEqualException>(fun _ ->
      AltAssert.NotEqual<String list, String>(match1, strcomp))
    |> ignore

    let func = Func<string, string, bool>(fun x y -> x.Equals y)
    Assert.Throws<Xunit.Sdk.NotEqualException>(fun _ ->
      AltAssert.NotEqual<String list, String>(match1, func))
    |> ignore

  [<Test>]
  let NotEqualItemsShouldPass () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = A
          Expected = C "hello" }

    AltAssert.NotEqual<Example> match1
    AltAssert.NotEqual<Example>(match1, exrefcomp)

  [<Test>]
  let NotEqualItemsShouldFail () =
    let item = B 1

    let match1 =
      (AssertionMatch.Create().WithActual item)
        .WithExpected
        item

    Assert.Throws<Xunit.Sdk.NotEqualException>(fun _ ->
      AltAssert.NotEqual<Example> match1)
    |> ignore

    Assert.Throws<Xunit.Sdk.NotEqualException>(fun _ ->
      AltAssert.NotEqual<Example>(match1, exrefcomp))
    |> ignore

    let func = Func<_,_,bool>(fun x y -> Object.ReferenceEquals(x, y))
    Assert.Throws<Xunit.Sdk.NotEqualException>(fun _ ->
      AltAssert.NotEqual<Example>(match1, func))
    |> ignore

  [<Test>]
  let NotEqualScalarsShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual 5.0)
        .WithExpected
        6.0

    AltAssert.NotEqual(match1, 2)

    let match2 =
      (AssertionMatch.Create().WithActual 5.0M)
        .WithExpected
        6.0M

    AltAssert.NotEqual(match2, 2)

  [<Test>]
  let NotEqualScalarsShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = 5.0
          Expected = 5.0 }

    Assert.Throws<Xunit.Sdk.NotEqualException>(fun _ -> AltAssert.NotEqual(match1, 2))
    |> ignore

    let func = Func<double,double,bool>(=)
    Assert.Throws<Xunit.Sdk.NotEqualException>(fun _ -> AltAssert.NotEqual(match1, func))
    |> ignore

    let doublecomp =
      { new IEqualityComparer<double> with
          member this.Equals(x, y) = x = y
          member this.GetHashCode(x) = x.GetHashCode() }

    Assert.Throws<Xunit.Sdk.NotEqualException>(fun _ -> AltAssert.NotEqual(match1, doublecomp))
    |> ignore

    let match2 =
      { AssertionMatch.Create() with
          Actual = 5.0M
          Expected = 5.0M }

    Assert.Throws<Xunit.Sdk.NotEqualException>(fun _ -> AltAssert.NotEqual(match2, 2))
    |> ignore

  [<Test>]
  let NotSameItemsShouldPass () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = B 1
          Expected = B 1 }

    AltAssert.NotSame match1

  [<Test>]
  let NotSameItemsShouldFail () =
    let item = B 1

    let match1 =
      (AssertionMatch.Create().WithActual item)
        .WithExpected
        item

    Assert.Throws<Xunit.Sdk.NotSameException>(fun _ -> AltAssert.NotSame match1)
    |> ignore

  [<Test>]
  let NotStrictEqualItemsShouldPass () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = B 1
          Expected = A }

    AltAssert.NotStrictEqual match1

  [<Test>]
  let NotStrictEqualItemsShouldFail () =
    let item = B 1

    let match1 =
      (AssertionMatch.Create().WithActual item)
        .WithExpected
        item

    Assert.Throws<Xunit.Sdk.NotStrictEqualException>(fun _ ->
      AltAssert.NotStrictEqual match1)
    |> ignore

  [<Test>]
  let SetsShouldPass () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = HashSet [ 1 ]
          Expected = HashSet [ 1; 2 ] }

    AltAssert.ProperSubset match1
    AltAssert.Subset match1

    let match2 =
      { AssertionMatch.Create() with
          Actual = HashSet [ 1; 2 ]
          Expected = HashSet [ 1 ] }

    AltAssert.ProperSuperset match2
    AltAssert.Superset match2

  [<Test>]
  let SetsShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = HashSet [ 1; 2 ]
          Expected = HashSet [ 1 ] }

    Assert.Throws<Xunit.Sdk.ProperSubsetException>(fun _ -> AltAssert.ProperSubset match1)
    |> ignore

    Assert.Throws<Xunit.Sdk.SubsetException>(fun _ -> AltAssert.Subset match1)
    |> ignore

    let match2 =
      { AssertionMatch.Create() with
          Actual = HashSet [ 1 ]
          Expected = HashSet [ 1; 2 ] }

    Assert.Throws<Xunit.Sdk.ProperSupersetException>(fun _ ->
      AltAssert.ProperSuperset match2)
    |> ignore

    Assert.Throws<Xunit.Sdk.SupersetException>(fun _ -> AltAssert.Superset match2)
    |> ignore

  [<Test>]
  let SameItemsShouldPass () =
    let item = B 1

    let match1 =
      (AssertionMatch.Create().WithActual item)
        .WithExpected
        item

    AltAssert.Same match1

  [<Test>]
  let SameItemsShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = B 1
          Expected = B 1 }

    Assert.Throws<Xunit.Sdk.SameException>(fun _ -> AltAssert.Same match1)
    |> ignore

  [<Test>]
  let StrictEqualItemsShouldPass () =
    let item = B 1

    let match1 =
      (AssertionMatch.Create().WithActual item)
        .WithExpected
        item

    AltAssert.StrictEqual match1

  [<Test>]
  let StrictEqualItemsShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = B 1
          Expected = A }

    Assert.Throws<Xunit.Sdk.StrictEqualException>(fun _ -> AltAssert.StrictEqual match1)
    |> ignore

  [<Test>]
  let StringStartsWithShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual "Hello")
        .WithExpected
        "H"

    AltAssert.StartsWith match1
    AltAssert.StartsWith(match1, StringComparison.Ordinal)

  [<Test>]
  let StringStartsWithShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = "Hello"
          Expected = "e" }

    Assert.Throws<Xunit.Sdk.StartsWithException>(fun _ -> AltAssert.StartsWith match1)
    |> ignore

    Assert.Throws<Xunit.Sdk.StartsWithException>(fun _ ->
      AltAssert.StartsWith(match1, StringComparison.Ordinal))
    |> ignore