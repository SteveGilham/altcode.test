namespace AltCode.Test.Expecto

open System
open System.Collections.Generic
open System.Runtime.InteropServices
open System.IO
open Expecto

[<RequireQualifiedAccess>]
module AltExpect =
  let containsAll (x: AssertionMatch<IEnumerable<'a>>) message =
    Expect.containsAll x.Actual x.Expected message

  let equal (x: AssertionMatch<'a>) message =
    Expect.equal x.Actual x.Expected message

  let equalWithDiffPrinter diffPrinter (x: AssertionMatch<'a>) message =
    Expect.equalWithDiffPrinter diffPrinter x.Actual x.Expected message

  let floatClose accuracy (x: AssertionMatch<double>) message =
    Expect.floatClose accuracy x.Actual x.Expected message

  let floatGreaterThanOrClose accuracy (x: AssertionMatch<double>) message =
    Expect.floatGreaterThanOrClose accuracy x.Actual x.Expected message

  let floatLessThanOrClose accuracy (x: AssertionMatch<double>) message =
    Expect.floatLessThanOrClose accuracy x.Actual x.Expected message

  let isFasterThan (x: AssertionMatch<unit -> 'a>) message =
    Expect.isFasterThan x.Actual x.Expected message

  let isFasterThanSub (x: AssertionMatch<Performance.Measurer<_, _> -> 'a>) message =
    Expect.isFasterThanSub x.Actual x.Expected message

  let isGreaterThan (x: AssertionMatch<'a>) message =
    Expect.isGreaterThan x.Actual x.Expected message

  let isGreaterThanOrEqual (x: AssertionMatch<'a>) message =
    Expect.isGreaterThanOrEqual x.Actual x.Expected message

  let isLessThan (x: AssertionMatch<'a>) message =
    Expect.isLessThan x.Actual x.Expected message

  let isLessThanOrEqual (x: AssertionMatch<'a>) message =
    Expect.isLessThanOrEqual x.Actual x.Expected message

  let isMatch (x: AssertionMatch<string>) message =
    Expect.isMatch x.Actual x.Expected message

  let isMatchGroups (x: AssertionMatch<string>) matchesOperator message =
    Expect.isMatchGroups x.Actual x.Expected matchesOperator message

  let isNotMatch (x: AssertionMatch<string>) message =
    Expect.isNotMatch x.Actual x.Expected message

  let notEqual (x: AssertionMatch<'a>) message =
    Expect.notEqual x.Actual x.Expected message

  let sequenceContainsOrder (x: AssertionMatch<IEnumerable<'a>>) message =
    Expect.sequenceContainsOrder x.Actual x.Expected message

  let sequenceEqual (x: AssertionMatch<IEnumerable<'a>>) message =
    Expect.sequenceEqual x.Actual x.Expected message

  let sequenceStarts (x: AssertionMatch<IEnumerable<'a>>) message =
    Expect.sequenceStarts x.Actual x.Expected message

  let streamsEqual (x: AssertionMatch<Stream>) message =
    Expect.streamsEqual x.Actual x.Expected message

  let stringContains (x: AssertionMatch<string>) message =
    Expect.stringContains x.Actual x.Expected message

  let stringEnds (x: AssertionMatch<string>) message =
    Expect.stringEnds x.Actual x.Expected message

  let stringStarts (x: AssertionMatch<IEnumerable<char>>) message =
    Expect.stringStarts x.Actual x.Expected message

[<RequireQualifiedAccess>]
module AltFlipExpect =
  let equal message (x: AssertionMatch<'a>) =
    Expecto.Flip.Expect.equal message x.Expected x.Actual

  let floatClose message accuracy (x: AssertionMatch<double>) =
    Expecto.Flip.Expect.floatClose message accuracy x.Expected x.Actual

  let floatGreaterThanOrClose message accuracy (x: AssertionMatch<double>) =
    Expecto.Flip.Expect.floatGreaterThanOrClose message accuracy x.Expected x.Actual

  let floatLessThanOrClose message accuracy (x: AssertionMatch<double>) =
    Expecto.Flip.Expect.floatLessThanOrClose message accuracy x.Expected x.Actual

  let isFasterThan message (x: AssertionMatch<unit -> 'a>) =
    Expecto.Flip.Expect.isFasterThan message x.Actual x.Expected

  let isFasterThanSub message (x: AssertionMatch<Performance.Measurer<_, _> -> 'a>) =
    Expecto.Flip.Expect.isFasterThanSub message x.Actual x.Expected

  let isGreaterThan message (x: AssertionMatch<'a>) =
    Expecto.Flip.Expect.isGreaterThan message (x.Actual, x.Expected)

  let isGreaterThanOrEqual message (x: AssertionMatch<'a>) =
    Expecto.Flip.Expect.isGreaterThanOrEqual message (x.Actual, x.Expected)

  let isLessThan message (x: AssertionMatch<'a>) =
    Expecto.Flip.Expect.isLessThan message (x.Actual, x.Expected)

  let isLessThanOrEqual message (x: AssertionMatch<'a>) =
    Expecto.Flip.Expect.isLessThanOrEqual message (x.Actual, x.Expected)

  let isMatch message (x: AssertionMatch<string>) =
    Expecto.Flip.Expect.isMatch message x.Expected x.Actual

  let isMatchGroups message matchesOperator (x: AssertionMatch<string>) =
    Expecto.Flip.Expect.isMatchGroups message matchesOperator x.Expected x.Actual

  let isNotMatch message (x: AssertionMatch<string>) =
    Expecto.Flip.Expect.isNotMatch message x.Expected x.Actual

  let notEqual message (x: AssertionMatch<'a>) =
    Expecto.Flip.Expect.notEqual message x.Expected x.Actual

  let sequenceContainsOrder message (x: AssertionMatch<IEnumerable<'a>>) =
    Expecto.Flip.Expect.sequenceContainsOrder message x.Expected x.Actual

  let sequenceEqual message (x: AssertionMatch<IEnumerable<'a>>) =
    Expecto.Flip.Expect.sequenceEqual message x.Expected x.Actual

  let sequenceStarts message (x: AssertionMatch<IEnumerable<'a>>) =
    Expecto.Flip.Expect.sequenceStarts message x.Expected x.Actual

  let streamsEqual message (x: AssertionMatch<Stream>) =
    Expecto.Flip.Expect.streamsEqual message x.Expected x.Actual

  let stringContains message (x: AssertionMatch<string>) =
    Expecto.Flip.Expect.stringContains message x.Expected x.Actual

  let stringEnds message (x: AssertionMatch<string>) =
    Expecto.Flip.Expect.stringEnds message x.Expected x.Actual

  let stringStarts message (x: AssertionMatch<IEnumerable<char>>) =
    Expecto.Flip.Expect.stringStarts message x.Expected x.Actual

[<RequireQualifiedAccess>]
type AltCSharpExpect() =

  static member IsFasterThan
    (
      (x: AssertionMatch<Func<'a>>),
      message: string,
      [<Out>] result: string byref
    ) =
    Expecto.CSharp.Function.IsFasterThan(x.Actual, x.Expected, message, &result)

  static member IsFasterThan
    (
      (x: AssertionMatch<Action * Func<'a>>),
      message: string,
      [<Out>] result: string byref
    ) =
    Expecto.CSharp.Function.IsFasterThan(
      fst x.Actual,
      snd x.Actual,
      fst x.Expected,
      snd x.Expected,
      message,
      &result
    )

  static member IsFasterThan
    (
      (x: AssertionMatch<Action * Func<'a> * Action>),
      message: string,
      [<Out>] result: string byref
    ) =
    let (a1, b1, c1) = x.Actual
    let (a2, b2, c2) = x.Expected

    Expecto.CSharp.Function.IsFasterThan(a1, b1, c1, a2, b2, c2, message, &result)


//