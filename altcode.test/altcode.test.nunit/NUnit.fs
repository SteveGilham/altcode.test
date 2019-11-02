namespace AltCode.Test.Nunit

open System
open AltCode.Test.Common
open NUnit.Framework

type Constraint<'a> =
  {
    Actual : 'a
    Constraint : NUnit.Framework.Constraints.IResolveConstraint
  }

type AltAssert =
  static member That(x: Constraint<'a>) =
    Assert.That(x.Actual, x.Constraint)
  static member That(x: Constraint<'a>, message, args) =
    Assert.That(x.Actual, x.Constraint, message, args)
  static member That(x: Constraint<'a>, getExceptionMessage) =
    Assert.That(x.Actual, x.Constraint, getExceptionMessage)
  static member AreEqual(x : Match<double>, delta, message, args) =
    Assert.AreEqual(x.Expected, x.Actual, delta, message, args)
  static member AreEqual(x : Match<double>, delta) =
    Assert.AreEqual(x.Expected, x.Actual, delta)
  static member AreEqual(x : Match<Nullable<double>>, delta, message, args) =
    Assert.AreEqual(x.Expected.Value, x.Actual, delta, message, args)
  static member AreEqual(x : Match<Nullable<double>>, delta) =
    Assert.AreEqual(x.Expected.Value, x.Actual, delta)
  static member AreEqual(x : Match<'a>, message, args) =
    Assert.AreEqual(x.Expected, x.Actual, message, args)
  static member AreEqual(x : Match<'a>) =
    Assert.AreEqual(x.Expected, x.Actual)
  static member AreNotEqual(x : Match<'a>, message, args) =
    Assert.AreNotEqual(x.Expected, x.Actual, message, args)
  static member AreNotEqual(x : Match<'a>) =
    Assert.AreNotEqual(x.Expected, x.Actual)
  static member AreSame(x : Match<'a>, message, args) =
    Assert.AreSame(x.Expected, x.Actual, message, args)
  static member AreSame(x : Match<'a>) =
    Assert.AreSame(x.Expected, x.Actual)
  static member AreNotSame(x : Match<'a>, message, args) =
    Assert.AreNotSame(x.Expected, x.Actual, message, args)
  static member AreNotSame(x : Match<'a>) =
    Assert.AreNotSame(x.Expected, x.Actual)