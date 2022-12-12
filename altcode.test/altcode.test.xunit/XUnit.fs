namespace AltCode.Test.Xunit

open System
open System.Collections.Generic
open Xunit

[<AbstractClass; Sealed>]
type AltAssert =
  static member Contains(x: AssertionMatch<string>) =
    Assert.Contains(x.Expected, x.Actual)

  static member Contains(x: AssertionMatch<string>, comparison) =
    Assert.Contains(x.Expected, x.Actual, comparison)

  static member DoesNotContain(x: AssertionMatch<string>) =
    Assert.DoesNotContain(x.Expected, x.Actual)

  static member DoesNotContain(x: AssertionMatch<string>, comparison) =
    Assert.DoesNotContain(x.Expected, x.Actual, comparison)

  static member DoesNotMatch(x: AssertionMatch<string>) =
    Assert.DoesNotMatch(x.Expected, x.Actual)

  static member EndsWith(x: AssertionMatch<string>) =
    Assert.EndsWith(x.Expected, x.Actual)

  static member EndsWith(x: AssertionMatch<string>, comparison) =
    Assert.EndsWith(x.Expected, x.Actual, comparison)

  static member Equal(x: AssertionMatch<#IEnumerable<'a>>) =
    Assert.Equal<'a>(x.Expected, x.Actual)

  static member Equal(x: AssertionMatch<#IEnumerable<'a>>, comparer) =
    Assert.Equal<'a>(x.Expected, x.Actual, comparer)

  static member Equal<'a>(x: AssertionMatch<'a>) = Assert.Equal<'a>(x.Expected, x.Actual)

  static member Equal<'a>(x: AssertionMatch<'a>, comparer) =
    Assert.Equal<'a>(x.Expected, x.Actual, comparer)

  static member Equal(x: AssertionMatch<double>, (precision: int)) =
    Assert.Equal(x.Expected, x.Actual, precision)

  static member Equal(x: AssertionMatch<double>, (precision: int), rounding) =
    Assert.Equal(x.Expected, x.Actual, precision, rounding)

  static member Equal(x: AssertionMatch<double>, (tolerance: double)) =
    Assert.Equal(x.Expected, x.Actual, tolerance)

  static member Equal(x: AssertionMatch<single>, (tolerance: single)) =
    Assert.Equal(x.Expected, x.Actual, tolerance)

  static member Equal(x: AssertionMatch<decimal>, precision) =
    Assert.Equal(x.Expected, x.Actual, precision)

  static member Equal(x: AssertionMatch<DateTime>, precision) =
    Assert.Equal(x.Expected, x.Actual, precision)

  static member Equal(x: AssertionMatch<string>) = Assert.Equal(x.Expected, x.Actual)

  static member Equal
    (
      x: AssertionMatch<string>,
      ?ignoreCase,
      ?ignoreLineEndingDifferences,
      ?ignoreWhiteSpaceDifferences
    ) =
    Assert.Equal(
      x.Expected,
      x.Actual,
      defaultArg ignoreCase false,
      defaultArg ignoreLineEndingDifferences false,
      defaultArg ignoreWhiteSpaceDifferences false
    )

  static member Equivalent(x: AssertionMatch<Object>, ?strict) =
    Assert.Equivalent(x.Expected, x.Actual, defaultArg strict false)

  static member Matches(x: AssertionMatch<string>) = Assert.Matches(x.Expected, x.Actual)

  static member NotEqual(x: AssertionMatch<#IEnumerable<'a>>) =
    Assert.NotEqual<'a>(x.Expected, x.Actual)

  static member NotEqual(x: AssertionMatch<#IEnumerable<'a>>, comparer) =
    Assert.NotEqual<'a>(x.Expected, x.Actual, comparer)

  static member NotEqual<'a>(x: AssertionMatch<'a>) =
    Assert.NotEqual<'a>(x.Expected, x.Actual)

  static member NotEqual<'a>(x: AssertionMatch<'a>, comparer) =
    Assert.NotEqual<'a>(x.Expected, x.Actual, comparer)

  static member NotEqual(x: AssertionMatch<double>, precision) =
    Assert.NotEqual(x.Expected, x.Actual, precision)

  static member NotEqual(x: AssertionMatch<decimal>, precision) =
    Assert.NotEqual(x.Expected, x.Actual, precision)

  static member NotSame(x: AssertionMatch<'a>) = Assert.NotSame(x.Expected, x.Actual)

  static member NotStrictEqual(x: AssertionMatch<'a>) =
    Assert.NotStrictEqual(x.Expected, x.Actual)

  static member ProperSubset(x: AssertionMatch<#ISet<'a>>) =
    Assert.ProperSubset(x.Expected, x.Actual)

  static member ProperSuperset(x: AssertionMatch<#ISet<'a>>) =
    Assert.ProperSuperset(x.Expected, x.Actual)

  static member Same(x: AssertionMatch<'a>) = Assert.Same(x.Expected, x.Actual)

  static member StartsWith(x: AssertionMatch<string>) =
    Assert.StartsWith(x.Expected, x.Actual)

  static member StartsWith(x: AssertionMatch<string>, comparison) =
    Assert.StartsWith(x.Expected, x.Actual, comparison)

  static member StrictEqual(x: AssertionMatch<'a>) =
    Assert.StrictEqual(x.Expected, x.Actual)

  static member Subset(x: AssertionMatch<#ISet<'a>>) = Assert.Subset(x.Expected, x.Actual)

  static member Superset(x: AssertionMatch<#ISet<'a>>) =
    Assert.Superset(x.Expected, x.Actual)