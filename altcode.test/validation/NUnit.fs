namespace AltCode.Validation

open System
open System.IO

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
        .WithConstraint
        Is.True

    AltAssert.That match1
    AltAssert.That(match1, "message")

  [<Test>]
  let ThatShouldFail () =
    let match1 =
      (Constraint.Create().WithActual true)
        .WithConstraint
        Is.Not.True

    let x1 =
      Assert.Throws<AssertionException>(fun _ -> AltAssert.That match1)

    let expected =
      """  Assert.That(, )
  Expected: not True
  But was:  True
"""

    Assert.That(x1.Message, Is.EqualTo expected)

    let match2 =
      (Constraint.Create().WithActual true)
        .WithConstraint
        Is.Not.True

    let x2 =
      Assert.Throws<AssertionException>(fun _ -> AltAssert.That(match2, "some text"))

    let expected2 =
      "  some text\n"
      + """Assert.That(, )
  Expected: not True
  But was:  True
"""

    Assert.That(x2.Message, Is.EqualTo expected2)

  [<Test>]
  let AreEqualShouldPass () =
    let match1 = { Actual = 5; Expected = 5 }

    AltAssert.AreEqual match1

    let match2 =
      { Actual = 5.0; Expected = 5.0 }

    AltAssert.AreEqual(match2, 0.1)

    let match3 = { Actual = 5; Expected = 5 }

    AltAssert.AreEqual(match3, "bang {0} {2}", 1, 2., 3, 4)

    let match4 =
      { Actual = 5.0; Expected = 5.0 }

    AltAssert.AreEqual(match4, 0.1, "bang {0} {2}", 1, 2., 3, 4)

  [<Test>]
  let AreEqualShouldFail () =
    let match1 = { Actual = 5; Expected = 6 }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.AreEqual match1)
    |> ignore

    let match2 =
      { Actual = 5.0; Expected = 6.0 }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.AreEqual(match2, 0.1))
    |> ignore

    let match3 = { Actual = 5; Expected = 6 }

    Assert.Throws<AssertionException>(fun _ ->
      AltAssert.AreEqual(match3, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

    let match4 =
      { Actual = 5.0; Expected = 6.0 }

    Assert.Throws<AssertionException>(fun _ ->
      AltAssert.AreEqual(match4, 0.1, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let AreNotEqualShouldPass () =
    let match1 = { Actual = 5; Expected = 6 }

    AltAssert.AreNotEqual match1

    let match3 = { Actual = 5; Expected = 6 }

    AltAssert.AreNotEqual(match3, "bang {0} {2}", 1, 2., 3, 4)

  [<Test>]
  let AreNotEqualShouldFail () =
    let match1 = { Actual = 5; Expected = 5 }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.AreNotEqual match1)
    |> ignore

    let match3 = { Actual = 5; Expected = 5 }

    Assert.Throws<AssertionException>(fun _ ->
      AltAssert.AreNotEqual(match3, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let AreSameShouldPass () =
    let x = B 5

    let match1 = { Actual = x; Expected = x }

    AltAssert.AreSame match1

    let match3 = { Actual = A; Expected = A }

    AltAssert.AreSame(match3, "bang {0} {2}", 1, 2., 3, 4)

  [<Test>]
  let AreSameShouldFail () =
    let match1 =
      { Actual = B 5; Expected = B 5 }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.AreSame match1)
    |> ignore

    let match3 =
      { Actual = A; Expected = C "6" }

    Assert.Throws<AssertionException>(fun _ ->
      AltAssert.AreSame(match3, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let AreNotSameShouldPass () =
    let match1 =
      { Actual = B 5; Expected = B 5 }

    AltAssert.AreNotSame match1

    let match3 =
      { Actual = A; Expected = C "6" }

    AltAssert.AreNotSame(match3, "bang {0} {2}", 1, 2., 3, 4)

  [<Test>]
  let AreNotSameShouldFail () =
    let x = B 5

    let match1 = { Actual = x; Expected = x }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.AreNotSame match1)
    |> ignore

    let match3 = { Actual = A; Expected = A }

    Assert.Throws<AssertionException>(fun _ ->
      AltAssert.AreNotSame(match3, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let GreaterShouldPass () =
    let match1 = { Actual = 5; Expected = 4 }

    AltAssert.Greater match1

    let match2 =
      { Actual = 5.0; Expected = 4.0 }

    AltAssert.Greater(match2, "bang {0} {2}", 1, 2., 3, 4)

  [<Test>]
  let GreaterShouldFail () =
    let match1 = { Actual = 5; Expected = 6 }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.Greater match1)
    |> ignore

    let match2 =
      { Actual = 5.0; Expected = 6.0 }

    Assert.Throws<AssertionException>(fun _ ->
      AltAssert.Greater(match2, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let GreaterOrEqualShouldPass () =
    let match1 = { Actual = 5; Expected = 4 }

    AltAssert.GreaterOrEqual match1

    let match2 =
      { Actual = 5.0; Expected = 4.0 }

    AltAssert.GreaterOrEqual(match2, "bang {0} {2}", 1, 2., 3, 4)

  [<Test>]
  let GreaterOrEqualShouldFail () =
    let match1 = { Actual = 5; Expected = 6 }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.GreaterOrEqual match1)
    |> ignore

    let match2 =
      { Actual = 5.0; Expected = 6.0 }

    Assert.Throws<AssertionException>(fun _ ->
      AltAssert.GreaterOrEqual(match2, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let LessShouldPass () =
    let match1 = { Actual = 5; Expected = 6 }

    AltAssert.Less match1

    let match2 =
      { Actual = 5.0; Expected = 6.0 }

    AltAssert.Less(match2, "bang {0} {2}", 1, 2., 3, 4)

  [<Test>]
  let LessShouldFail () =
    let match1 = { Actual = 5; Expected = 4 }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.Less match1)
    |> ignore

    let match2 =
      { Actual = 5.0; Expected = 4.0 }

    Assert.Throws<AssertionException>(fun _ ->
      AltAssert.Less(match2, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let LessOrEqualShouldPass () =
    let match1 = { Actual = 5; Expected = 6 }

    AltAssert.LessOrEqual match1

    let match2 =
      { Actual = 5.0; Expected = 6.0 }

    AltAssert.LessOrEqual(match2, "bang {0} {2}", 1, 2., 3, 4)

  [<Test>]
  let LessOrEqualShouldFail () =
    let match1 = { Actual = 5; Expected = 4 }

    Assert.Throws<AssertionException>(fun _ -> AltAssert.LessOrEqual match1)
    |> ignore

    let match2 =
      { Actual = 5.0; Expected = 4.0 }

    Assert.Throws<AssertionException>(fun _ ->
      AltAssert.LessOrEqual(match2, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let DirectoryAssertsShouldPass () =
    let d1 = DirectoryInfo "."
    let d2 = DirectoryInfo ".."

    let match1 =
      (AssertionMatch.Create().WithActual d1)
        .WithExpected
        d1

    AltDirectoryAssert.AreEqual match1

    let match2 = { Actual = d2; Expected = d2 }

    AltDirectoryAssert.AreEqual(match2, "bang {0} {2}", 1, 2., 3, 4)

    let match3 = { Actual = d1; Expected = d2 }

    AltDirectoryAssert.AreNotEqual match3

    let match4 = { Actual = d2; Expected = d1 }

    AltDirectoryAssert.AreNotEqual(match4, "bang {0} {2}", 1, 2., 3, 4)

  [<Test>]
  let DirectoryAssertsShouldFail () =
    let d1 = DirectoryInfo "."
    let d2 = DirectoryInfo ".."

    let match1 =
      (AssertionMatch.Create().WithActual d1)
        .WithExpected
        d2

    Assert.Throws<AssertionException>(fun _ -> AltDirectoryAssert.AreEqual match1)
    |> ignore

    let match2 = { Actual = d2; Expected = d1 }

    Assert.Throws<AssertionException>(fun _ ->
      AltDirectoryAssert.AreEqual(match2, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

    let match3 = { Actual = d1; Expected = d1 }

    Assert.Throws<AssertionException>(fun _ -> AltDirectoryAssert.AreNotEqual match3)
    |> ignore

    let match4 = { Actual = d2; Expected = d2 }

    Assert.Throws<AssertionException>(fun _ ->
      AltDirectoryAssert.AreNotEqual(match4, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let StringContainsShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual "Hello")
        .WithExpected
        "e"

    AltStringAssert.Contains match1
    AltStringAssert.Contains(match1, "bang {0} {2}", 1, 2., 3, 4)

    let match2 =
      (AssertionMatch.Create().WithActual "Hello")
        .WithExpected
        "?"

    AltStringAssert.DoesNotContain match2
    AltStringAssert.DoesNotContain(match2, "bang {0} {2}", 1, 2., 3, 4)

  [<Test>]
  let StringContainsShouldFail () =
    let match1 =
      { Actual = "Hello"; Expected = "?" }

    Assert.Throws<AssertionException>(fun _ -> AltStringAssert.Contains match1)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltStringAssert.Contains(match1, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

    let match2 =
      { Actual = "Hello"; Expected = "e" }

    Assert.Throws<AssertionException>(fun _ -> AltStringAssert.DoesNotContain match2)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltStringAssert.DoesNotContain(match2, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let DoesNotMatchShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual "Hello")
        .WithExpected
        "x"

    AltStringAssert.DoesNotMatch match1
    AltStringAssert.DoesNotMatch(match1, "bang {0} {2}", 1, 2., 3, 4)

    Assert.Throws<AssertionException>(fun _ -> AltStringAssert.IsMatch match1)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltStringAssert.IsMatch(match1, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let DoesNotMatchShouldFail () =
    let match1 =
      { Actual = "Hello"; Expected = "l" }

    Assert.Throws<AssertionException>(fun _ -> AltStringAssert.DoesNotMatch match1)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltStringAssert.DoesNotMatch(match1, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

    AltStringAssert.IsMatch match1
    AltStringAssert.IsMatch(match1, "bang {0} {2}", 1, 2., 3, 4)

  [<Test>]
  let StringEndsWithShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual "Hello")
        .WithExpected
        "o"

    AltStringAssert.EndsWith match1
    AltStringAssert.EndsWith(match1, "bang {0} {2}", 1, 2., 3, 4)

    Assert.Throws<AssertionException>(fun _ -> AltStringAssert.DoesNotEndWith match1)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltStringAssert.DoesNotEndWith(match1, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let StringEndsWithShouldFail () =
    let match1 =
      { Actual = "Hello"; Expected = "H" }

    Assert.Throws<AssertionException>(fun _ -> AltStringAssert.EndsWith match1)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltStringAssert.EndsWith(match1, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

    AltStringAssert.DoesNotEndWith match1
    AltStringAssert.DoesNotEndWith(match1, "bang {0} {2}", 1, 2., 3, 4)

  [<Test>]
  let StringStartsWithShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual "Hello")
        .WithExpected
        "H"

    AltStringAssert.StartsWith match1
    AltStringAssert.StartsWith(match1, "bang {0} {2}", 1, 2., 3, 4)

    Assert.Throws<AssertionException>(fun _ -> AltStringAssert.DoesNotStartWith match1)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltStringAssert.DoesNotStartWith(match1, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let StringStartsWithShouldFail () =
    let match1 =
      { Actual = "Hello"; Expected = "o" }

    Assert.Throws<AssertionException>(fun _ -> AltStringAssert.StartsWith match1)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltStringAssert.StartsWith(match1, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

    AltStringAssert.DoesNotStartWith match1
    AltStringAssert.DoesNotStartWith(match1, "bang {0} {2}", 1, 2., 3, 4)

  [<Test>]
  let AreEqualIgnoringCaseStringsShouldPass () =
    let item = "DateTime.UtcNow"

    let match1 =
      (AssertionMatch.Create().WithActual item)
        .WithExpected(item.ToUpperInvariant())

    AltStringAssert.AreEqualIgnoringCase match1
    AltStringAssert.AreEqualIgnoringCase(match1, "bang {0} {2}", 1, 2., 3, 4)

    Assert.Throws<AssertionException>(fun _ ->
      AltStringAssert.AreNotEqualIgnoringCase match1)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltStringAssert.AreNotEqualIgnoringCase(match1, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let AreEqualIgnoringCaseStringsShouldFail () =
    let item = "DateTime.UtcNow"
    let later = "item + TimeSpan(1, 0, 0, 0)"

    let match1 =
      { Actual = item; Expected = later }

    Assert.Throws<AssertionException>(fun _ ->
      AltStringAssert.AreEqualIgnoringCase match1)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltStringAssert.AreEqualIgnoringCase(match1, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

    AltStringAssert.AreNotEqualIgnoringCase match1
    AltStringAssert.AreNotEqualIgnoringCase(match1, "bang {0} {2}", 1, 2., 3, 4)

  [<Test>]
  let AreEqualFilesShouldPass () =
    let left =
      Path.Combine(
        AltCode.Test.RepoRoot.location,
        "altcode.test/altcode.test.common/Common.fs"
      )

    let right =
      Path.Combine(AltCode.Test.RepoRoot.location, "_Generated/Common.Common.fs")

    // let l1 = File.ReadAllText left
    // let r1 = File.ReadAllText right
    // Assert.That(l1, Is.EqualTo r1)

    let match1 =
      (AssertionMatch.Create().WithActual left)
        .WithExpected(right)

    AltFileAssert.AreEqual match1
    AltFileAssert.AreEqual(match1, "bang {0} {2}", 1, 2., 3, 4)

    Assert.Throws<AssertionException>(fun _ -> AltFileAssert.AreNotEqual match1)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltFileAssert.AreNotEqual(match1, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

    let match2 =
      (AssertionMatch.Create().WithActual(FileInfo left))
        .WithExpected(FileInfo right)

    AltFileAssert.AreEqual match2
    AltFileAssert.AreEqual(match2, "bang {0} {2}", 1, 2., 3, 4)

    Assert.Throws<AssertionException>(fun _ -> AltFileAssert.AreNotEqual match2)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltFileAssert.AreNotEqual(match2, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

    use l2 = (File.OpenRead left) :> Stream
    use r2 = (File.OpenRead right) :> Stream

    let match3 =
      (AssertionMatch.Create().WithActual l2)
        .WithExpected(r2)

    AltFileAssert.AreEqual match3
    AltFileAssert.AreEqual(match3, "bang {0} {2}", 1, 2., 3, 4)

    Assert.Throws<AssertionException>(fun _ -> AltFileAssert.AreNotEqual match3)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltFileAssert.AreNotEqual(match3, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let AreEqualFilesShouldFail () =
    let left =
      Path.Combine(
        AltCode.Test.RepoRoot.location,
        "altcode.test/altcode.test.common/Common.fs"
      )

    let right =
      Path.Combine(AltCode.Test.RepoRoot.location, "_Generated/RepoRoot.fs")

    let match1 =
      (AssertionMatch.Create().WithActual left)
        .WithExpected(right)

    AltFileAssert.AreNotEqual match1
    AltFileAssert.AreNotEqual(match1, "bang {0} {2}", 1, 2., 3, 4)

    Assert.Throws<AssertionException>(fun _ -> AltFileAssert.AreEqual match1)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltFileAssert.AreEqual(match1, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

    let match2 =
      (AssertionMatch.Create().WithActual(FileInfo left))
        .WithExpected(FileInfo right)

    AltFileAssert.AreNotEqual match2
    AltFileAssert.AreNotEqual(match2, "bang {0} {2}", 1, 2., 3, 4)

    Assert.Throws<AssertionException>(fun _ -> AltFileAssert.AreEqual match2)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltFileAssert.AreEqual(match2, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

    use l2 = (File.OpenRead left) :> Stream
    use r2 = (File.OpenRead right) :> Stream

    let match3 =
      (AssertionMatch.Create().WithActual l2)
        .WithExpected(r2)

    AltFileAssert.AreNotEqual match3
    AltFileAssert.AreNotEqual(match3, "bang {0} {2}", 1, 2., 3, 4)

    Assert.Throws<AssertionException>(fun _ -> AltFileAssert.AreEqual match3)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltFileAssert.AreEqual(match3, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let SetsShouldPass () =
    let match1 =
      { Actual = [ B 1; B 5 ]
        Expected = [ B 5; B 1; B 2 ] }

    AltCollectionAssert.IsSubsetOf match1
    AltCollectionAssert.IsSubsetOf(match1, "bang {0} {2}", 1, 2., 3, 4)

    Assert.Throws<AssertionException>(fun _ -> AltCollectionAssert.IsNotSubsetOf match1)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltCollectionAssert.IsNotSubsetOf(match1, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

    let match2 =
      { Actual = [ B 5; B 1; B 2 ]
        Expected = [ B 1; B 5 ] }

    AltCollectionAssert.IsSupersetOf match2
    AltCollectionAssert.IsSupersetOf(match2, "bang {0} {2}", 1, 2., 3, 4)

    Assert.Throws<AssertionException>(fun _ -> AltCollectionAssert.IsNotSupersetOf match2)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltCollectionAssert.IsNotSupersetOf(match2, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let SetsShouldFail () =
    let match1 =
      { Actual = [ B 5; B 1; B 2 ]
        Expected = [ B 1; B 5 ] }

    Assert.Throws<AssertionException>(fun _ -> AltCollectionAssert.IsSubsetOf match1)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltCollectionAssert.IsSubsetOf(match1, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

    AltCollectionAssert.IsNotSubsetOf match1
    AltCollectionAssert.IsNotSubsetOf(match1, "bang {0} {2}", 1, 2., 3, 4)

    let match2 =
      { Actual = [ B 1; B 5 ]
        Expected = [ B 5; B 1; B 2 ] }

    Assert.Throws<AssertionException>(fun _ -> AltCollectionAssert.IsSupersetOf match2)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltCollectionAssert.IsSupersetOf(match2, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

    AltCollectionAssert.IsNotSupersetOf match2
    AltCollectionAssert.IsNotSupersetOf(match2, "bang {0} {2}", 1, 2., 3, 4)

  let excomp =
    System.Collections.Comparer(System.Globalization.CultureInfo.InvariantCulture)

  [<Test>]
  let EqualCollectionsShouldPass () =
    let match1 =
      { Actual = [ B 5; B 1; B 2 ]
        Expected = [ B 5; B 1; B 2 ] }

    AltCollectionAssert.AreEqual match1
    AltCollectionAssert.AreEqual(match1, "bang {0} {2}", 1, 2., 3, 4)
    AltCollectionAssert.AreEqual(match1, excomp)
    AltCollectionAssert.AreEqual(match1, excomp, "bang {0} {2}", 1, 2., 3, 4)

    Assert.Throws<AssertionException>(fun _ -> AltCollectionAssert.AreNotEqual match1)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltCollectionAssert.AreNotEqual(match1, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltCollectionAssert.AreNotEqual(match1, excomp))
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltCollectionAssert.AreNotEqual(match1, excomp, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

    let match2 =
      { Actual = [ B 5; B 1; B 2 ]
        Expected = [ B 1; B 5; B 2 ] }

    AltCollectionAssert.AreEquivalent match2
    AltCollectionAssert.AreEquivalent(match2, "bang {0} {2}", 1, 2., 3, 4)

    Assert.Throws<AssertionException>(fun _ ->
      AltCollectionAssert.AreNotEquivalent match2)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltCollectionAssert.AreNotEquivalent(match2, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

  [<Test>]
  let EqualCollectionsShouldFail () =
    let match1 =
      { Actual = [ B 5; B 1; B 2 ]
        Expected = [ B 1; B 5; B 3 ] }

    AltCollectionAssert.AreNotEqual match1
    AltCollectionAssert.AreNotEqual(match1, "bang {0} {2}", 1, 2., 3, 4)
    AltCollectionAssert.AreNotEqual(match1, excomp)
    AltCollectionAssert.AreNotEqual(match1, excomp, "bang {0} {2}", 1, 2., 3, 4)

    Assert.Throws<AssertionException>(fun _ -> AltCollectionAssert.AreEqual match1)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltCollectionAssert.AreEqual(match1, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltCollectionAssert.AreEqual(match1, excomp))
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltCollectionAssert.AreEqual(match1, excomp, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore

    let match2 =
      { Actual = [ B 1; B 5 ]
        Expected = [ B 5; B 1; B 2 ] }

    AltCollectionAssert.AreNotEquivalent match2
    AltCollectionAssert.AreNotEquivalent(match2, "bang {0} {2}", 1, 2., 3, 4)

    Assert.Throws<AssertionException>(fun _ -> AltCollectionAssert.AreEquivalent match2)
    |> ignore

    Assert.Throws<AssertionException>(fun _ ->
      AltCollectionAssert.AreEquivalent(match2, "bang {0} {2}", 1, 2., 3, 4))
    |> ignore