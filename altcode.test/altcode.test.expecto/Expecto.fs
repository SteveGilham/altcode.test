namespace AltCode.Test.Expecto

open System
open System.Collections.Generic
open System.IO
open AltCode.Test.Common
open Expecto

module AltExpect =
  let containsAll (x : Match<IEnumerable<'a>>) message =
    Expect.containsAll x.Actual x.Expected message
  let equal (x : Match<'a>) message =
    Expect.equal x.Actual x.Expected message
  let floatClose accuracy (x : Match<double>) message =
    Expect.floatClose accuracy x.Actual x.Expected message
  let floatGreaterThanOrClose accuracy (x : Match<double>) message =
    Expect.floatGreaterThanOrClose accuracy x.Actual x.Expected message
  let floatLessThanOrClose accuracy (x : Match<double>) message =
    Expect.floatLessThanOrClose accuracy x.Actual x.Expected message
  let isMatch (x : Match<string>) message =
    Expect.isMatch x.Actual x.Expected message
  let isMatchGroups (x : Match<string>) matchesOperator message =
    Expect.isMatchGroups x.Actual x.Expected matchesOperator message
  let isNotMatch (x : Match<string>) message =
    Expect.isNotMatch x.Actual x.Expected message
  let notEqual (x : Match<'a>) message =
    Expect.notEqual x.Actual x.Expected message
  let sequenceContainsOrder (x : Match<IEnumerable<'a>>) message =
    Expect.sequenceContainsOrder x.Actual x.Expected message
  let sequenceEqual (x : Match<IEnumerable<'a>>) message =
    Expect.sequenceEqual x.Actual x.Expected message
  let sequenceStarts (x : Match<IEnumerable<'a>>) message =
    Expect.sequenceStarts x.Actual x.Expected message
  let streamsEqual (x : Match<Stream>) message =
    Expect.streamsEqual x.Actual x.Expected message
  let stringContains (x : Match<string>) message =
    Expect.stringContains x.Actual x.Expected message
  let stringEnds (x : Match<string>) message =
    Expect.stringEnds x.Actual x.Expected message
  let stringStarts (x : Match<IEnumerable<char>>) message =
    Expect.stringStarts x.Actual x.Expected message
  [<Obsolete("Please use the more general AltExpect.floatClose")>]
  let floatEqual (x : Match<double>) epsilon message =
    let epsilon2 = match epsilon with
                   | Some d -> d
                   | None -> 0.001
    Expect.floatClose {absolute = epsilon2; relative =  0.0} x.Actual x.Expected message