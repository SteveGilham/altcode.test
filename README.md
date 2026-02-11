# altcode.test
Named argument wrappers for unit test frameworks to disambiguate between which argument of type `'a` is `expected` and which `actual` as there's no consistent ordering between libraries, and even within them (e.g. Expecto and Expecto.Flip).

## What's in the box?

A core type
```fsharp
namespace AltCode.Test.[Expecto|NUnit|Xunit]

type AssertionMatch<'a> =
  {
    Actual : 'a
    Expected : 'a
  }
  static member Create() =
    {
      Actual = Unchecked.defaultof<'a>
      Expected = Unchecked.defaultof<'a>
    }
  member this.WithActual e = { this with Actual = e }
  member this.WithExpected e = { this with Expected = e }
```
providing a F# and C#-friendly API for naming arguments

# For Expecto

[`AltCode.Test.Expecto` ![Nuget](https://buildstats.info/nuget/altcode.test.expecto)](http://nuget.org/packages/altcode.test.expecto)

Contains module `AltCode.Test.Expecto.AltExpect` which provides wrappers for `Expecto.Expect` and `AltCode.Test.Expecto.AltFlipExpect` for `Expecto.Flip.Expect` with an appropriate `AltCode.Test.Expecto.AssertionMatch`-typed argument in place of actual and template expectation

# For Xunit

[`AltCode.Test.Xunit` ![Nuget](https://buildstats.info/nuget/altcode.test.xunit)](http://nuget.org/packages/altcode.test.xunit)

Contains class `AltCode.Test.Xunit.AltAssert` which provides wrappers for `Xunit.Assert` with an appropriate `AltCode.Test.Xunit.AssertionMatch`-typed argument in place of actual and template expectation

# For NUnit

[`AltCode.Test.NUnit` ![Nuget](https://buildstats.info/nuget/altcode.test.nunit)](http://nuget.org/packages/altcode.test.nunit)

Contains classes `AltCode.Test.NUnit.Alt*Assert` which provide emulators for the corresponding `NUnit.Framework.Legacy.*Assert` for `*` = `'Classic'` (just `AltAssert`, not `AltClassicAssert`), `'Collection'`, `'Directory'`, `'File'` and `'String'` types with an appropriate `AltCode.Test.Nunit.AssertionMatch`-typed argument in place of actual and template expectation; also 
```fsharp
type Constraint<'a> =
  {
    Actual : 'a
    Constraint : NUnit.Framework.Constraints.IResolveConstraint
  }
  static member Create() =
    {
      Actual = Unchecked.defaultof<'a>
      Constraint = null
    }
  member this.WithActual e = { this with Actual = e }
  member this.WithConstraint e = { this with Constraint = e }
```
and wrappers for `NUnit.Framework.Assert.That` overloads
```fsharp
  static member That(x: Constraint<'a>)
  static member That(x: Constraint<'a>, message: string)

```

## Continuous Integration

| | | |
| --- | --- | --- | 
| **Build** | <sup>GitHub</sup> [![CI](https://github.com/SteveGilham/altcode.test/workflows/CI/badge.svg)](https://github.com/SteveGilham/altcode.test/actions?query=workflow%3ACI) | [![Build history](https://buildstats.info/github/chart/SteveGilham/altcode.test?branch=master)](https://github.com/SteveGilham/altcode.test/actions?query=workflow%3ACI)
| **Coverage** | <sup>Coveralls</sup> [![Coverage Status](https://coveralls.io/repos/github/SteveGilham/altcode.test/badge.svg?branch=master)](https://coveralls.io/github/SteveGilham/altcode.test?branch=master) |


## Building

Cross platform, dotnet code throughout.

### Tooling

It is assumed that .net 8.0.100 or later is available  (`dotnet`) -- try https://www.microsoft.com/net/download  

### Bootstrapping

Start by setting up with `dotnet tool restore `
Then `dotnet run --project ./Build/Setup.fsproj` to do the rest of the set-up.

### Normal builds

Running `dotnet run --project ./Build/Build.fsproj` performs a full build/package process.

Use `dotnet run --project ./Build/Build.fsproj --target <targetname>` to run to a specific target.

## Thanks to

* [Coveralls](https://coveralls.io/r/SteveGilham/altcover) for allowing free services for Open Source projects