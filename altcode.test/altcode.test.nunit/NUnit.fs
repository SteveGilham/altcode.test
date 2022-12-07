namespace AltCode.Test.Nunit

open System
open System.Collections
open System.IO
open AltCode.Test.Common
open NUnit.Framework

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

  static member That(x: Constraint<'a>, message, args) =
    Assert.That(x.Actual, x.Constraint, message, args)

  static member That(x: Constraint<'a>, getExceptionMessage) =
    Assert.That(x.Actual, x.Constraint, getExceptionMessage)

  static member AreEqual(x: AssertionMatch<double>, delta, message, args) =
    Assert.AreEqual(x.Expected, x.Actual, delta, message, args)

  static member AreEqual(x: AssertionMatch<double>, delta) =
    Assert.AreEqual(x.Expected, x.Actual, delta)

  static member AreEqual(x: AssertionMatch<'a>, message, args) =
    Assert.AreEqual(x.Expected, x.Actual, message, args)

  static member AreEqual(x: AssertionMatch<'a>) = Assert.AreEqual(x.Expected, x.Actual)

  static member AreNotEqual(x: AssertionMatch<'a>, message, args) =
    Assert.AreNotEqual(x.Expected, x.Actual, message, args)

  static member AreNotEqual(x: AssertionMatch<'a>) =
    Assert.AreNotEqual(x.Expected, x.Actual)

  static member AreSame(x: AssertionMatch<'a>, message, args) =
    Assert.AreSame(x.Expected, x.Actual, message, args)

  static member AreSame(x: AssertionMatch<'a>) = Assert.AreSame(x.Expected, x.Actual)

  static member AreNotSame(x: AssertionMatch<'a>, message, args) =
    Assert.AreNotSame(x.Expected, x.Actual, message, args)

  static member AreNotSame(x: AssertionMatch<'a>) =
    Assert.AreNotSame(x.Expected, x.Actual)

[<AbstractClass; Sealed>]
type AltCollectionAssert =
  static member AreEqual<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    CollectionAssert.AreEqual(x.Expected, x.Actual)

  static member AreEqual<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>, comparer) =
    CollectionAssert.AreEqual(x.Expected, x.Actual, comparer)

  static member AreEqual<'a when 'a :> IEnumerable>
    (
      x: AssertionMatch<'a>,
      message,
      args
    ) =
    CollectionAssert.AreEqual(x.Expected, x.Actual, message, args)

  static member AreEqual<'a when 'a :> IEnumerable>
    (
      x: AssertionMatch<'a>,
      comparer,
      message,
      args
    ) =
    CollectionAssert.AreEqual(x.Expected, x.Actual, comparer, message, args)

  static member AreEquivalent<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    CollectionAssert.AreEquivalent(x.Expected, x.Actual)

  static member AreEquivalent<'a when 'a :> IEnumerable>
    (
      x: AssertionMatch<'a>,
      message,
      args
    ) =
    CollectionAssert.AreEquivalent(x.Expected, x.Actual, message, args)

  static member AreNotEqual<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    CollectionAssert.AreNotEqual(x.Expected, x.Actual)

  static member AreNotEqual<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>, comparer) =
    CollectionAssert.AreNotEqual(x.Expected, x.Actual, comparer)

  static member AreNotEqual<'a when 'a :> IEnumerable>
    (
      x: AssertionMatch<'a>,
      message,
      args
    ) =
    CollectionAssert.AreNotEqual(x.Expected, x.Actual, message, args)

  static member AreNotEqual<'a when 'a :> IEnumerable>
    (
      x: AssertionMatch<'a>,
      comparer,
      message,
      args
    ) =
    CollectionAssert.AreNotEqual(x.Expected, x.Actual, comparer, message, args)

  static member AreNotEquivalent<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    CollectionAssert.AreNotEquivalent(x.Expected, x.Actual)

  static member AreNotEquivalent<'a when 'a :> IEnumerable>
    (
      x: AssertionMatch<'a>,
      message,
      args
    ) =
    CollectionAssert.AreNotEquivalent(x.Expected, x.Actual, message, args)

  static member IsNotSubsetOf<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    CollectionAssert.IsNotSubsetOf(x.Actual, x.Expected)

  static member IsNotSubsetOf<'a when 'a :> IEnumerable>
    (
      x: AssertionMatch<'a>,
      message,
      args
    ) =
    CollectionAssert.IsNotSubsetOf(x.Actual, x.Expected, message, args)

  static member IsNotSupersetOf<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    CollectionAssert.IsNotSupersetOf(x.Actual, x.Expected)

  static member IsNotSupersetOf<'a when 'a :> IEnumerable>
    (
      x: AssertionMatch<'a>,
      message,
      args
    ) =
    CollectionAssert.IsNotSupersetOf(x.Actual, x.Expected, message, args)

  static member IsSubsetOf<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    CollectionAssert.IsSubsetOf(x.Actual, x.Expected)

  static member IsSubsetOf<'a when 'a :> IEnumerable>
    (
      x: AssertionMatch<'a>,
      message,
      args
    ) =
    CollectionAssert.IsSubsetOf(x.Actual, x.Expected, message, args)

  static member IsSupersetOf<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    CollectionAssert.IsSupersetOf(x.Actual, x.Expected)

  static member IsSupersetOf<'a when 'a :> IEnumerable>
    (
      x: AssertionMatch<'a>,
      message,
      args
    ) =
    CollectionAssert.IsSupersetOf(x.Actual, x.Expected, message, args)

[<AbstractClass; Sealed>]
type AltDirectoryAssert =
  static member AreEqual(x: AssertionMatch<DirectoryInfo>) =
    DirectoryAssert.AreEqual(x.Expected, x.Actual)

  static member AreEqual(x: AssertionMatch<DirectoryInfo>, message, args) =
    DirectoryAssert.AreEqual(x.Expected, x.Actual, message, args)

  static member AreNotEqual(x: AssertionMatch<DirectoryInfo>) =
    DirectoryAssert.AreNotEqual(x.Expected, x.Actual)

  static member AreNotEqual(x: AssertionMatch<DirectoryInfo>, message, args) =
    DirectoryAssert.AreNotEqual(x.Expected, x.Actual, message, args)

[<AbstractClass; Sealed>]
type AltFileAssert =
  static member AreEqual(x: AssertionMatch<FileInfo>) =
    FileAssert.AreEqual(x.Expected, x.Actual)

  static member AreEqual(x: AssertionMatch<FileInfo>, message, args) =
    FileAssert.AreEqual(x.Expected, x.Actual, message, args)

  static member AreEqual(x: AssertionMatch<Stream>) =
    FileAssert.AreEqual(x.Expected, x.Actual)

  static member AreEqual(x: AssertionMatch<Stream>, message, args) =
    FileAssert.AreEqual(x.Expected, x.Actual, message, args)

  static member AreEqual(x: AssertionMatch<String>) =
    FileAssert.AreEqual(x.Expected, x.Actual)

  static member AreEqual(x: AssertionMatch<String>, message, args) =
    FileAssert.AreEqual(x.Expected, x.Actual, message, args)

  static member AreNotEqual(x: AssertionMatch<FileInfo>) =
    FileAssert.AreNotEqual(x.Expected, x.Actual)

  static member AreNotEqual(x: AssertionMatch<FileInfo>, message, args) =
    FileAssert.AreNotEqual(x.Expected, x.Actual, message, args)

  static member AreNotEqual(x: AssertionMatch<Stream>) =
    FileAssert.AreNotEqual(x.Expected, x.Actual)

  static member AreNotEqual(x: AssertionMatch<Stream>, message, args) =
    FileAssert.AreNotEqual(x.Expected, x.Actual, message, args)

  static member AreNotEqual(x: AssertionMatch<String>) =
    FileAssert.AreNotEqual(x.Expected, x.Actual)

  static member AreNotEqual(x: AssertionMatch<String>, message, args) =
    FileAssert.AreNotEqual(x.Expected, x.Actual, message, args)

[<AbstractClass; Sealed>]
type AltStringAssert =
  static member AreEqualIgnoringCase(x: AssertionMatch<String>) =
    StringAssert.AreEqualIgnoringCase(x.Expected, x.Actual)

  static member AreEqualIgnoringCase(x: AssertionMatch<String>, message, args) =
    StringAssert.AreEqualIgnoringCase(x.Expected, x.Actual, message, args)

  static member AreNotEqualIgnoringCase(x: AssertionMatch<String>) =
    StringAssert.AreNotEqualIgnoringCase(x.Expected, x.Actual)

  static member AreNotEqualIgnoringCase(x: AssertionMatch<String>, message, args) =
    StringAssert.AreNotEqualIgnoringCase(x.Expected, x.Actual, message, args)

  static member Contains(x: AssertionMatch<String>) =
    StringAssert.Contains(x.Expected, x.Actual)

  static member Contains(x: AssertionMatch<String>, message, args) =
    StringAssert.Contains(x.Expected, x.Actual, message, args)

  static member DoesNotContain(x: AssertionMatch<String>) =
    StringAssert.DoesNotContain(x.Expected, x.Actual)

  static member DoesNotContain(x: AssertionMatch<String>, message, args) =
    StringAssert.DoesNotContain(x.Expected, x.Actual, message, args)

  static member DoesNotEndWith(x: AssertionMatch<String>) =
    StringAssert.DoesNotEndWith(x.Expected, x.Actual)

  static member DoesNotEndWith(x: AssertionMatch<String>, message, args) =
    StringAssert.DoesNotEndWith(x.Expected, x.Actual, message, args)

  static member DoesNotMatch(x: AssertionMatch<String>) =
    StringAssert.DoesNotMatch(x.Expected, x.Actual)

  static member DoesNotMatch(x: AssertionMatch<String>, message, args) =
    StringAssert.DoesNotMatch(x.Expected, x.Actual, message, args)

  static member DoesNotStartWith(x: AssertionMatch<String>) =
    StringAssert.DoesNotStartWith(x.Expected, x.Actual)

  static member DoesNotStartWith(x: AssertionMatch<String>, message, args) =
    StringAssert.DoesNotStartWith(x.Expected, x.Actual, message, args)

  static member EndsWith(x: AssertionMatch<String>) =
    StringAssert.EndsWith(x.Expected, x.Actual)

  static member EndsWith(x: AssertionMatch<String>, message, args) =
    StringAssert.EndsWith(x.Expected, x.Actual, message, args)

  static member IsMatch(x: AssertionMatch<String>) =
    StringAssert.IsMatch(x.Expected, x.Actual)

  static member IsMatch(x: AssertionMatch<String>, message, args) =
    StringAssert.IsMatch(x.Expected, x.Actual, message, args)

  static member StartsWith(x: AssertionMatch<String>) =
    StringAssert.StartsWith(x.Expected, x.Actual)

  static member StartsWith(x: AssertionMatch<String>, message, args) =
    StringAssert.StartsWith(x.Expected, x.Actual, message, args)

//