namespace AltCode.Test.Nunit

open System
open System.Collections
open System.IO
open NUnit.Framework
open NUnit.Framework.Legacy

type Constraint<'a> =
  { Actual: 'a
    Constraint: NUnit.Framework.Constraints.IResolveConstraint }
  static member Create() =
    { Actual = Unchecked.defaultof<'a>
      Constraint = null }

  member this.WithActual e = { this with Actual = e }
  member this.WithConstraint e = { this with Constraint = e }

[<AbstractClass; Sealed>]
type AltAssert =
  static member That(x: Constraint<'a>) = Assert.That(x.Actual, x.Constraint)

  static member That(x: Constraint<'a>, message: string) =
    Assert.That<'a>(x.Actual, x.Constraint, message)

  static member AreEqual
    (
      x: AssertionMatch<double>,
      delta,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Is.EqualTo(x.Expected).Within(delta), String.Format(message, args))

  static member AreEqual(x: AssertionMatch<double>, delta) =
    Assert.That(x.Actual, Is.EqualTo(x.Expected).Within(delta))

  static member AreEqual(x: AssertionMatch<'a>, message, [<ParamArray>] args: Object[]) =
    Assert.That(x.Actual, Is.EqualTo(x.Expected),NUnitString(String.Format(message, args)))

  static member AreEqual(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.EqualTo(x.Expected))

  static member AreNotEqual
    (
      x: AssertionMatch<'a>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Is.Not.EqualTo x.Expected,NUnitString(String.Format(message, args)))

  static member AreNotEqual(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.Not.EqualTo(x.Expected))

  static member AreSame(x: AssertionMatch<'a>, message, [<ParamArray>] args: Object[]) =
    Assert.That(x.Actual, Is.SameAs x.Expected, NUnitString(String.Format(message, args)))

  static member AreSame(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.SameAs x.Expected)

  static member AreNotSame
    (
      x: AssertionMatch<'a>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Is.Not.SameAs x.Expected, NUnitString(String.Format(message, args)))

  static member AreNotSame(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.Not.SameAs x.Expected)

  static member Greater(x: AssertionMatch<'a>, message, [<ParamArray>] args: Object[]) =
    Assert.That(x.Actual, Is.GreaterThan x.Expected, NUnitString(String.Format(message, args)))

  static member Greater(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.GreaterThan x.Expected)

  static member GreaterOrEqual
    (
      x: AssertionMatch<'a>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Is.GreaterThanOrEqualTo x.Expected, NUnitString(String.Format(message, args)))

  static member GreaterOrEqual(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.GreaterThanOrEqualTo x.Expected)

  static member Less(x: AssertionMatch<'a>, message, [<ParamArray>] args: Object[]) =
    Assert.That(x.Actual, Is.LessThan x.Expected, NUnitString(String.Format(message, args)))

  static member Less(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.LessThan x.Expected)

  static member LessOrEqual
    (
      x: AssertionMatch<'a>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Is.LessThanOrEqualTo x.Expected, NUnitString(String.Format(message, args)))

  static member LessOrEqual(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.LessThanOrEqualTo x.Expected)

[<AbstractClass; Sealed>]
type AltCollectionAssert =
  static member AreEqual<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.EqualTo x.Expected)

  static member AreEqual<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>, comparer:IComparer) =
    Assert.That(x.Actual, Is.EqualTo(x.Expected).Using(comparer))

  static member AreEqual<'a when 'a :> IEnumerable>
    (
      x: AssertionMatch<'a>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Is.EqualTo x.Expected, NUnitString(String.Format(message, args)))

  static member AreEqual<'a when 'a :> IEnumerable>
    (
      x: AssertionMatch<'a>,
      comparer:IComparer,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Is.EqualTo(x.Expected).Using(comparer), NUnitString(String.Format(message, args)))

  static member AreEquivalent<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.EquivalentTo x.Expected)

  static member AreEquivalent<'a when 'a :> IEnumerable>
    (
      x: AssertionMatch<'a>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Is.EquivalentTo x.Expected, NUnitString(String.Format(message, args)))

  static member AreNotEqual<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.Not.EqualTo x.Expected)

  static member AreNotEqual<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>, comparer:IComparer) =
    Assert.That(x.Actual, Is.Not.EqualTo(x.Expected).Using(comparer))

  static member AreNotEqual<'a when 'a :> IEnumerable>
    (
      x: AssertionMatch<'a>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Is.Not.EqualTo x.Expected, NUnitString(String.Format(message, args)))

  static member AreNotEqual<'a when 'a :> IEnumerable>
    (
      x: AssertionMatch<'a>,
      comparer:IComparer,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Is.Not.EqualTo(x.Expected).Using(comparer), NUnitString(String.Format(message, args)))

  static member AreNotEquivalent<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.Not.EquivalentTo x.Expected)

  static member AreNotEquivalent<'a when 'a :> IEnumerable>
    (
      x: AssertionMatch<'a>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Is.Not.EquivalentTo x.Expected, NUnitString(String.Format(message, args)))

  static member IsNotSubsetOf<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.Not.SubsetOf x.Expected)

  static member IsNotSubsetOf<'a when 'a :> IEnumerable>
    (
      x: AssertionMatch<'a>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Is.Not.SubsetOf x.Expected, NUnitString(String.Format(message, args)))

  static member IsNotSupersetOf<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.Not.SupersetOf x.Expected)

  static member IsNotSupersetOf<'a when 'a :> IEnumerable>
    (
      x: AssertionMatch<'a>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Is.Not.SupersetOf x.Expected, NUnitString(String.Format(message, args)))

  static member IsSubsetOf<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.SubsetOf x.Expected)

  static member IsSubsetOf<'a when 'a :> IEnumerable>
    (
      x: AssertionMatch<'a>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Is.SubsetOf x.Expected, NUnitString(String.Format(message, args)))

  static member IsSupersetOf<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.SupersetOf x.Expected)

  static member IsSupersetOf<'a when 'a :> IEnumerable>
    (
      x: AssertionMatch<'a>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Is.SupersetOf x.Expected, NUnitString(String.Format(message, args)))

[<AbstractClass; Sealed>]
type AltDirectoryAssert =
  static member AreEqual(x: AssertionMatch<DirectoryInfo>) =
    DirectoryAssert.AreEqual(x.Expected, x.Actual)

  static member AreEqual
    (
      x: AssertionMatch<DirectoryInfo>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    DirectoryAssert.AreEqual(x.Expected, x.Actual, message, args)

  static member AreNotEqual(x: AssertionMatch<DirectoryInfo>) =
    DirectoryAssert.AreNotEqual(x.Expected, x.Actual)

  static member AreNotEqual
    (
      x: AssertionMatch<DirectoryInfo>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    DirectoryAssert.AreNotEqual(x.Expected, x.Actual, message, args)

[<AbstractClass; Sealed>]
type AltFileAssert =
  static member AreEqual(x: AssertionMatch<FileInfo>) =
    FileAssert.AreEqual(x.Expected, x.Actual)

  static member AreEqual
    (
      x: AssertionMatch<FileInfo>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    FileAssert.AreEqual(x.Expected, x.Actual, message, args)

  static member AreEqual(x: AssertionMatch<Stream>) =
    FileAssert.AreEqual(x.Expected, x.Actual)

  static member AreEqual
    (
      x: AssertionMatch<Stream>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    FileAssert.AreEqual(x.Expected, x.Actual, message, args)

  static member AreEqual(x: AssertionMatch<String>) =
    FileAssert.AreEqual(x.Expected, x.Actual)

  static member AreEqual
    (
      x: AssertionMatch<String>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    FileAssert.AreEqual(x.Expected, x.Actual, message, args)

  static member AreNotEqual(x: AssertionMatch<FileInfo>) =
    FileAssert.AreNotEqual(x.Expected, x.Actual)

  static member AreNotEqual
    (
      x: AssertionMatch<FileInfo>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    FileAssert.AreNotEqual(x.Expected, x.Actual, message, args)

  static member AreNotEqual(x: AssertionMatch<Stream>) =
    FileAssert.AreNotEqual(x.Expected, x.Actual)

  static member AreNotEqual
    (
      x: AssertionMatch<Stream>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    FileAssert.AreNotEqual(x.Expected, x.Actual, message, args)

  static member AreNotEqual(x: AssertionMatch<String>) =
    FileAssert.AreNotEqual(x.Expected, x.Actual)

  static member AreNotEqual
    (
      x: AssertionMatch<String>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    FileAssert.AreNotEqual(x.Expected, x.Actual, message, args)

[<AbstractClass; Sealed>]
type AltStringAssert =
  static member AreEqualIgnoringCase(x: AssertionMatch<String>) =
    StringAssert.AreEqualIgnoringCase(x.Expected, x.Actual)

  static member AreEqualIgnoringCase
    (
      x: AssertionMatch<String>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    StringAssert.AreEqualIgnoringCase(x.Expected, x.Actual, message, args)

  static member AreNotEqualIgnoringCase(x: AssertionMatch<String>) =
    StringAssert.AreNotEqualIgnoringCase(x.Expected, x.Actual)

  static member AreNotEqualIgnoringCase
    (
      x: AssertionMatch<String>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    StringAssert.AreNotEqualIgnoringCase(x.Expected, x.Actual, message, args)

  static member Contains(x: AssertionMatch<String>) =
    Assert.That(x.Actual, Does.Contain x.Expected)

  static member Contains
    (
      x: AssertionMatch<String>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Does.Contain x.Expected, NUnitString(String.Format(message, args)))

  static member DoesNotContain(x: AssertionMatch<String>) =
    Assert.That(x.Actual, Does.Not.Contain x.Expected)

  static member DoesNotContain
    (
      x: AssertionMatch<String>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Does.Not.Contain x.Expected, NUnitString(String.Format(message, args)))

  static member DoesNotEndWith(x: AssertionMatch<String>) =
    Assert.That(x.Actual, Does.Not.EndWith x.Expected)

  static member DoesNotEndWith
    (
      x: AssertionMatch<String>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Does.Not.EndWith x.Expected, NUnitString(String.Format(message, args)))

  static member DoesNotMatch(x: AssertionMatch<String>) =
    Assert.That(x.Actual, Does.Not.Match x.Expected)

  static member DoesNotMatch
    (
      x: AssertionMatch<String>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Does.Not.Match x.Expected, NUnitString(String.Format(message, args)))

  static member DoesNotStartWith(x: AssertionMatch<String>) =
    Assert.That(x.Actual, Does.Not.StartWith x.Expected)

  static member DoesNotStartWith
    (
      x: AssertionMatch<String>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Does.Not.StartWith x.Expected, NUnitString(String.Format(message, args)))

  static member EndsWith(x: AssertionMatch<String>) =
    Assert.That(x.Actual, Does.EndWith x.Expected)

  static member EndsWith
    (
      x: AssertionMatch<String>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Does.EndWith x.Expected, NUnitString(String.Format(message, args)))

  static member IsMatch(x: AssertionMatch<String>) =
   Assert.That(x.Actual, Does.Match x.Expected)

  static member IsMatch
    (
      x: AssertionMatch<String>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Does.Match x.Expected, NUnitString(String.Format(message, args)))

  static member StartsWith(x: AssertionMatch<String>) =
    Assert.That(x.Actual, Does.StartWith x.Expected)

  static member StartsWith
    (
      x: AssertionMatch<String>,
      message,
      [<ParamArray>] args: Object[]
    ) =
    Assert.That(x.Actual, Does.StartWith x.Expected, NUnitString(String.Format(message, args)))

//