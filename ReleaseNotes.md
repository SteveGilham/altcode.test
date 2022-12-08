Q. Never mind the fluff -- how do I get started?

A. Start with the README : https://github.com/SteveGilham/altcode.test/blob/master/README.md

# 1.0.xx
* [BREAKING] make the `AssertionMatch` type specific to each package to avoid name clashes if multiple test suites are in use
* [BREAKING] remove support for obsolescing Expecto `Expect.floatEqual`
* [BREAKING] remove support for obsolescing NUnit `Assert.AreEqual<Nullable<double>>` methods
* [BREAKING] rename `AltCode.Test.Xunit.AltAssert.DoesNotAssertionMatch` & `AssterionMatches` to `AltCode.Test.Xunit.AltAssert.DoesNotMatch` and `Matches` to line up with the methods being shadowed in `Xunit.Assert`
* support `netstandard2.0` only (meaning ≥ `net472` compatible)
* support Expecto ≥ 9.0.4, NUnit ≥ 3.13.3, Xunit ≥ 2.4.2
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