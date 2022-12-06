namespace AltCode.Test.Common

type AssertionMatch<'a> =
  { Actual: 'a
    Expected: 'a }
  static member Create() =
    { Actual = Unchecked.defaultof<'a>
      Expected = Unchecked.defaultof<'a> }

  member this.WithActual e = { this with Actual = e }
  member this.WithExpected e = { this with Expected = e }