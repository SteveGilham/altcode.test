namespace AltCode.Test.Nunit

open System
open System.Collections.Generic
open AltCode.Test.Common
open Xunit

type AltAssert =
  static member Contains (x : Match<string>) =
    Assert.Contains(x.Expected, x.Actual)
  static member Contains (x : Match<string>, comparison) =
    Assert.Contains(x.Expected, x.Actual, comparison)
  static member DoesNotContain (x : Match<string>) =
    Assert.DoesNotContain(x.Expected, x.Actual)
  static member DoesNotContain (x : Match<string>, comparison) =
    Assert.DoesNotContain(x.Expected, x.Actual, comparison)
  static member DoesNotMatch (x : Match<string>) =
    Assert.DoesNotMatch(x.Expected, x.Actual)
  static member EndsWith (x : Match<string>) =
    Assert.EndsWith(x.Expected, x.Actual)
  static member EndsWith (x : Match<string>, comparison) =
    Assert.EndsWith(x.Expected, x.Actual, comparison)
  static member Equal(x : Match<IEnumerable<'a>>) =
    Assert.Equal<'a>(x.Expected, x.Actual)
  static member Equal(x : Match<IEnumerable<'a>>, comparer) =
    Assert.Equal<'a>(x.Expected, x.Actual, comparer)
  static member Equal<'a>(x : Match<'a>) =
    Assert.Equal<'a>(x.Expected, x.Actual)
  static member Equal<'a>(x : Match<'a>, comparer) =
    Assert.Equal<'a>(x.Expected, x.Actual, comparer)
  static member Equal(x : Match<double>, precision) =
    Assert.Equal(x.Expected, x.Actual, precision)
  static member Equal(x : Match<decimal>, precision) =
    Assert.Equal(x.Expected, x.Actual, precision)
  static member Equal(x : Match<DateTime>, precision) =
    Assert.Equal(x.Expected, x.Actual, precision)
  static member Equal(x : Match<string>) =
    Assert.Equal(x.Expected, x.Actual)
  static member Equal(x : Match<string>, ignoreCase, ignoreLineEndingDifferences, ignoreWhiteSpaceDifferences) = // TODO
    Assert.Equal(x.Expected, x.Actual, ignoreCase, ignoreLineEndingDifferences, ignoreWhiteSpaceDifferences)
  static member Matches (x : Match<string>) =
    Assert.Matches(x.Expected, x.Actual)
  static member NotEqual(x : Match<IEnumerable<'a>>) =
    Assert.NotEqual<'a>(x.Expected, x.Actual)
  static member NotEqual(x : Match<IEnumerable<'a>>, comparer) =
    Assert.NotEqual<'a>(x.Expected, x.Actual, comparer)
  static member NotEqual(x : Match<double>, precision) =
    Assert.NotEqual(x.Expected, x.Actual, precision)
  static member NotEqual(x : Match<decimal>, precision) =
    Assert.NotEqual(x.Expected, x.Actual, precision)
  static member NotSame(x : Match<'a>) =
    Assert.NotSame(x.Expected, x.Actual)
  static member NotStrictEqual(x : Match<'a>) =
    Assert.NotStrictEqual(x.Expected, x.Actual)
  static member ProperSubset(x : Match<ISet<'a>>) =
    Assert.ProperSubset(x.Expected, x.Actual)
  static member ProperSuperset(x : Match<ISet<'a>>) =
    Assert.ProperSuperset(x.Expected, x.Actual)
  static member StartsWith (x : Match<string>) =
    Assert.StartsWith(x.Expected, x.Actual)
  static member StartsWith (x : Match<string>, comparison) =
    Assert.StartsWith(x.Expected, x.Actual, comparison)
  static member StrictEqual(x : Match<'a>) =
    Assert.StrictEqual(x.Expected, x.Actual)
  static member Subset(x : Match<ISet<'a>>) =
    Assert.Subset(x.Expected, x.Actual)
  static member Superset(x : Match<ISet<'a>>) =
    Assert.Superset(x.Expected, x.Actual)