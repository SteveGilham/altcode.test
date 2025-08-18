namespace AltCode.Test.Nunit

open System
open System.Collections
open System.IO
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
type AltAssertBase =
  static member ConvertMessageWithArgs(message: string, [<ParamArray>] args: Object[]) =
    NUnitString(
      if (isNull args) || (Array.isEmpty args) then
        message
      else
        String.Format(message, args)
    )

[<AbstractClass; Sealed>]
type AltAssert =
  static member That(x: Constraint<'a>) = Assert.That(x.Actual, x.Constraint)

  static member That(x: Constraint<'a>, message: string) =
    Assert.That<'a>(x.Actual, x.Constraint, message)

  static member AreEqual
    (x: AssertionMatch<double>, delta, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Is.EqualTo(x.Expected).Within(delta),
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member AreEqual(x: AssertionMatch<double>, delta) =
    Assert.That(x.Actual, Is.EqualTo(x.Expected).Within(delta))

  static member AreEqual(x: AssertionMatch<'a>, message, [<ParamArray>] args: Object[]) =
    Assert.That(
      x.Actual,
      Is.EqualTo<'a>(x.Expected),
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member AreEqual(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.EqualTo<'a>(x.Expected))

  static member AreNotEqual
    (x: AssertionMatch<'a>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Is.Not.EqualTo x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member AreNotEqual(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.Not.EqualTo(x.Expected))

  static member AreSame(x: AssertionMatch<'a>, message, [<ParamArray>] args: Object[]) =
    Assert.That(
      x.Actual,
      Is.SameAs x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member AreSame(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.SameAs x.Expected)

  static member AreNotSame
    (x: AssertionMatch<'a>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Is.Not.SameAs x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member AreNotSame(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.Not.SameAs x.Expected)

  static member Greater(x: AssertionMatch<'a>, message, [<ParamArray>] args: Object[]) =
    Assert.That(
      x.Actual,
      Is.GreaterThan x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member Greater(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.GreaterThan x.Expected)

  static member GreaterOrEqual
    (x: AssertionMatch<'a>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Is.GreaterThanOrEqualTo x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member GreaterOrEqual(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.GreaterThanOrEqualTo x.Expected)

  static member Less(x: AssertionMatch<'a>, message, [<ParamArray>] args: Object[]) =
    Assert.That(
      x.Actual,
      Is.LessThan x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member Less(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.LessThan x.Expected)

  static member LessOrEqual
    (x: AssertionMatch<'a>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Is.LessThanOrEqualTo x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member LessOrEqual(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.LessThanOrEqualTo x.Expected)

[<AbstractClass; Sealed>]
type AltCollectionAssert =
  static member AreEqual<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.EqualTo x.Expected)

  static member AreEqual<'a when 'a :> IEnumerable>
    (x: AssertionMatch<'a>, comparer: IComparer)
    =
    Assert.That(x.Actual, Is.EqualTo(x.Expected).Using(comparer))

  static member AreEqual<'a when 'a :> IEnumerable>
    (x: AssertionMatch<'a>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Is.EqualTo x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member AreEqual<'a when 'a :> IEnumerable>
    (x: AssertionMatch<'a>, comparer: IComparer, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Is.EqualTo(x.Expected).Using(comparer),
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member AreEquivalent<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.EquivalentTo x.Expected)

  static member AreEquivalent<'a when 'a :> IEnumerable>
    (x: AssertionMatch<'a>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Is.EquivalentTo x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member AreNotEqual<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.Not.EqualTo x.Expected)

  static member AreNotEqual<'a when 'a :> IEnumerable>
    (x: AssertionMatch<'a>, comparer: IComparer)
    =
    Assert.That(x.Actual, Is.Not.EqualTo(x.Expected).Using(comparer))

  static member AreNotEqual<'a when 'a :> IEnumerable>
    (x: AssertionMatch<'a>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Is.Not.EqualTo x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member AreNotEqual<'a when 'a :> IEnumerable>
    (x: AssertionMatch<'a>, comparer: IComparer, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Is.Not.EqualTo(x.Expected).Using(comparer),
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member AreNotEquivalent<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.Not.EquivalentTo x.Expected)

  static member AreNotEquivalent<'a when 'a :> IEnumerable>
    (x: AssertionMatch<'a>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Is.Not.EquivalentTo x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member IsNotSubsetOf<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.Not.SubsetOf x.Expected)

  static member IsNotSubsetOf<'a when 'a :> IEnumerable>
    (x: AssertionMatch<'a>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Is.Not.SubsetOf x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member IsNotSupersetOf<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.Not.SupersetOf x.Expected)

  static member IsNotSupersetOf<'a when 'a :> IEnumerable>
    (x: AssertionMatch<'a>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Is.Not.SupersetOf x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member IsSubsetOf<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.SubsetOf x.Expected)

  static member IsSubsetOf<'a when 'a :> IEnumerable>
    (x: AssertionMatch<'a>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Is.SubsetOf x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member IsSupersetOf<'a when 'a :> IEnumerable>(x: AssertionMatch<'a>) =
    Assert.That(x.Actual, Is.SupersetOf x.Expected)

  static member IsSupersetOf<'a when 'a :> IEnumerable>
    (x: AssertionMatch<'a>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Is.SupersetOf x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

[<AbstractClass; Sealed>]
type AltDirectoryAssert =
  static member AreEqual(x: AssertionMatch<DirectoryInfo>) =
    Assert.That(x.Actual, Is.EqualTo x.Expected)

  static member AreEqual
    (x: AssertionMatch<DirectoryInfo>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Is.EqualTo x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member AreNotEqual(x: AssertionMatch<DirectoryInfo>) =
    Assert.That(x.Actual, Is.Not.EqualTo x.Expected)

  static member AreNotEqual
    (x: AssertionMatch<DirectoryInfo>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Is.Not.EqualTo x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

[<AbstractClass; Sealed>]
type AltFileAssert =
  static member AreEqual(x: AssertionMatch<FileInfo>) =
    AltFileAssert.AreEqual(x, String.Empty, null)

  static member AreEqual
    (x: AssertionMatch<FileInfo>, message, [<ParamArray>] args: Object[])
    =
    use expected2 =
      x.Expected.OpenRead() :> Stream

    use actual2 = x.Actual.OpenRead()

    AltFileAssert.AreEqual(
      { Expected = expected2
        Actual = actual2 },
      message,
      args
    )

  static member AreEqual(x: AssertionMatch<Stream>) =
    AltFileAssert.AreEqual(x, String.Empty, null)

  static member AreEqual
    (x: AssertionMatch<Stream>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Is.EqualTo x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member AreEqual(x: AssertionMatch<String>) =
    AltFileAssert.AreEqual(x, String.Empty, null)

  static member AreEqual
    (x: AssertionMatch<String>, message, [<ParamArray>] args: Object[])
    =
    use expected2 =
      File.OpenRead(x.Expected) :> Stream

    use actual2 = File.OpenRead(x.Actual)

    AltFileAssert.AreEqual(
      { Expected = expected2
        Actual = actual2 },
      message,
      args
    )

  static member AreNotEqual(x: AssertionMatch<FileInfo>) =
    AltFileAssert.AreNotEqual(x, String.Empty, null)

  static member AreNotEqual
    (x: AssertionMatch<FileInfo>, message, [<ParamArray>] args: Object[])
    =
    use expected2 =
      x.Expected.OpenRead() :> Stream

    use actual2 = x.Actual.OpenRead()

    AltFileAssert.AreNotEqual(
      { Expected = expected2
        Actual = actual2 },
      message,
      args
    )

  static member AreNotEqual(x: AssertionMatch<Stream>) =
    AltFileAssert.AreNotEqual(x, String.Empty, null)

  static member AreNotEqual
    (x: AssertionMatch<Stream>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Is.Not.EqualTo x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member AreNotEqual(x: AssertionMatch<String>) =
    AltFileAssert.AreNotEqual(x, String.Empty, null)

  static member AreNotEqual
    (x: AssertionMatch<String>, message, [<ParamArray>] args: Object[])
    =
    use expected2 =
      File.OpenRead(x.Expected) :> Stream

    use actual2 = File.OpenRead(x.Actual)

    AltFileAssert.AreNotEqual(
      { Expected = expected2
        Actual = actual2 },
      message,
      args
    )

[<AbstractClass; Sealed>]
type AltStringAssert =
  static member AreEqualIgnoringCase(x: AssertionMatch<String>) =
    Assert.That(x.Actual, Is.EqualTo(x.Expected).IgnoreCase)

  static member AreEqualIgnoringCase
    (x: AssertionMatch<String>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Is.EqualTo(x.Expected).IgnoreCase,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member AreNotEqualIgnoringCase(x: AssertionMatch<String>) =
    Assert.That(x.Actual, Is.Not.EqualTo(x.Expected).IgnoreCase)

  static member AreNotEqualIgnoringCase
    (x: AssertionMatch<String>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Is.Not.EqualTo(x.Expected).IgnoreCase,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member Contains(x: AssertionMatch<String>) =
    Assert.That(x.Actual, Does.Contain x.Expected)

  static member Contains
    (x: AssertionMatch<String>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Does.Contain x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member DoesNotContain(x: AssertionMatch<String>) =
    Assert.That(x.Actual, Does.Not.Contain x.Expected)

  static member DoesNotContain
    (x: AssertionMatch<String>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Does.Not.Contain x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member DoesNotEndWith(x: AssertionMatch<String>) =
    Assert.That(x.Actual, Does.Not.EndWith x.Expected)

  static member DoesNotEndWith
    (x: AssertionMatch<String>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Does.Not.EndWith x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member DoesNotMatch(x: AssertionMatch<String>) =
    Assert.That(x.Actual, Does.Not.Match x.Expected)

  static member DoesNotMatch
    (x: AssertionMatch<String>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Does.Not.Match x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member DoesNotStartWith(x: AssertionMatch<String>) =
    Assert.That(x.Actual, Does.Not.StartWith x.Expected)

  static member DoesNotStartWith
    (x: AssertionMatch<String>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Does.Not.StartWith x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member EndsWith(x: AssertionMatch<String>) =
    Assert.That(x.Actual, Does.EndWith x.Expected)

  static member EndsWith
    (x: AssertionMatch<String>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Does.EndWith x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member IsMatch(x: AssertionMatch<String>) =
    Assert.That(x.Actual, Does.Match x.Expected)

  static member IsMatch
    (x: AssertionMatch<String>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Does.Match x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

  static member StartsWith(x: AssertionMatch<String>) =
    Assert.That(x.Actual, Does.StartWith x.Expected)

  static member StartsWith
    (x: AssertionMatch<String>, message, [<ParamArray>] args: Object[])
    =
    Assert.That(
      x.Actual,
      Does.StartWith x.Expected,
      AltAssertBase.ConvertMessageWithArgs(message, args)
    )

//