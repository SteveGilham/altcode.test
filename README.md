# altcode.test
Named argument overloads for unit test frameworks

## What's in the box?

A core type
```
namespace AltCode.Test.Common

type Match<'a> =
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

Module `AltCode.Test.Expecto.AltExpect` which provides wrappers for `Expecto.Expect` with an appropriate `Match`-typed argument in place of actual and template expectation

# For Xunit

Module `AltCode.Test.Xunit.AltAssert` which provides wrappers for `Xunit.Assert` with an appropriate `Match`-typed argument in place of actual and template expectation

# For NUnit

Module `AltCode.Test.NUnit.AltAssert` which provides wrappers for `NUnit.Framework.Assert` with an appropriate `Match`-typed argument in place of actual and template expectation; also 
```
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
and wrappers for some `NUnit.Framework.Assert.That` overloads

## Continuous Integration

| | | |
| --- | --- | --- | 
| **Build** | <sup>AppVeyor</sup> [![Build status](https://img.shields.io/appveyor/ci/SteveGilham/altcode-test/master.svg)](https://ci.appveyor.com/project/SteveGilham/altcode-test) ![Build history](https://buildstats.info/appveyor/chart/SteveGilham/altcode-test?branch=master) 

## Building

Windows build only for Framework support; the .net core part of the artifacts are platform independent

### Tooling

It is assumed that .net core 3.0.100 is available  (`dotnet`) -- try https://www.microsoft.com/net/download  

### Bootstrapping

Start by setting up `dotnet fake` with `dotnet tool restore `
Then `dotnet fake run ./Build/setup.fsx` to do the rest of the set-up.

### Normal builds

Running `dotnet fake run ./Build/build.fsx` performs a full build/test/package process.

Use `dotnet fake run ./Build/build.fsx --target <targetname>` to run to a specific target.

## Thanks to

* [AppVeyor](https://ci.appveyor.com/project/SteveGilham/altcode-fake) for allowing free build CI services for Open Source projects
