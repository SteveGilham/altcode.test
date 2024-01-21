Q. Never mind the fluff -- how do I get started?

A. Start with the README : https://github.com/SteveGilham/altcode.test/blob/master/README.md

#????

# 2.0.48
* support Expecto ≥ 10.1.0, NUnit ≥ 4.0.1, Xunit ≥ 2.4.2, FSharp.Core ≥ 8.0.100
* runtime support driven by the consumed packages
  * Expecto support on `net6.0`
  * NUnit support on `net462` and `net6.0`
  * Xunit support on `netstandard2.0`
* NUnit support does not rely on the legacy `ClassicAssert` assembly

# 2.0.31
* [BREAKING] make the `AssertionMatch` type specific to each package to avoid name clashes if multiple test suites are in use
* [BREAKING] remove support for obsolescing Expecto `Expect.floatEqual`
* [BREAKING] remove support for obsolescing NUnit `Assert.AreEqual<Nullable<double>>` methods
* [BREAKING] rename `AltCode.Test.Xunit.AltAssert.DoesNotAssertionMatch` & `AssertionMatches` to `AltCode.Test.Xunit.AltAssert.DoesNotMatch` and `Matches` to line up with the methods being shadowed in `Xunit.Assert`
* [BREAKING] in NUnit methods with `...,message, args)` correctly declare args as a `params object[]` making the methods take variable numbers of arguments
* support `netstandard2.0` only (meaning ≥ `net472` compatible)
* various fixes to `Expecto.Expect` and `Expecto.Flip.Expect` support, including Stream subtypes to be passed w/o explicit coercion to base
* various fixes to `Xunit.Assert`, including respecting optional arguments and allowing interface subtypes to be passed w/o explicit coercion to base
* support Expecto ≥ 9.0.4, NUnit ≥ 3.13.3, Xunit ≥ 2.4.2, FSharp.Core ≥ 6.0.0
* new for `Expecto.Expect`
  * equalWithDiffPrinter
  * isFasterThan 
  * isFasterThanSub 
  * isGreaterThan
  * isGreaterThanOrEqual
  * isLessThan
  * isLessThanOrEqual

  where for comparisons, `Actual` is what's being tested and `Expected` is the benchmark value
* new for `Expecto.Flip.Expect`
  * containsAll
  * equalWithDiffPrinter diffPrinter message  (x: AssertionMatch<'a>)
  * isFasterThan 
  * isFasterThanSub 
  * isGreaterThan
  * isGreaterThanOrEqual
  * isLessThan
  * isLessThanOrEqual
* new for `Expecto.CSharp.Function`, type `AltCSharpExpect` for the type-homogenous versions of `IsFasterThan`
* new for `NUnit.Framework.Assert`
  * Greater
  * GreaterOrEqual
  * Less
  * LessOrEqual
* new for `Xunit.Assert`
  * Equal of double with MidpointRounding option
  * Equal of double with tolerance option
  * Equal of single ditto
  * Equivalent
  * NotEqual object overloads

# 1.0.11
* support Expecto 9.x
* support `net47` and `netstandard2.0` only

# 1.0.8
* `altcode.test.nunit` -- Fix classic framework support 
* `altcode.test.xunit` -- Fix classic framework support

# 1.0.6
* [BREAKING] `Match` -> `AssertionMatch`
* `altcode.test.expecto` -- add `Expecto.Flip` support
* `altcode.test.nunit` -- add `DirectoryAssert`, `FileAssert` and `StringAssert` support
* `altcode.test.xunit` -- add `AltAssert.Same`

# 1.0.4
* [NEW PACKAGE] `altcode.test.expecto` -- named argument helper for Expecto ≥ 8.12.0
* [NEW PACKAGE] `altcode.test.nunit` -- named argument helper for NUnit ≥ 3.12.0
* [NEW PACKAGE] `altcode.test.xunit` -- named argument helper for Xunit ≥ 2.4.1