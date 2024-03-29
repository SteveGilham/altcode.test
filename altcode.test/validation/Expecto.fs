namespace AltCode.Validation

module Expecto =

  open NUnit.Framework
  open AltCode.Test.Expecto

  type Example =
    | A
    | B of int
    | C of string

  [<Test>]
  let containsAllShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual [ 1; 2; 3 ])
        .WithExpected
        [ 1; 3 ]

    AltExpect.containsAll match1 "match1"
    AltFlipExpect.containsAll "flipmatch1" match1

    let match2 =
      { Actual = [ '1'; '2'; '3' ]
        Expected = [ '3'; '1' ] }

    AltExpect.containsAll match2 "match2"
    AltFlipExpect.containsAll "flipmatch2" match2

    let match3 =
      { Actual = [ "1"; "2"; "3" ]
        Expected = [ "1"; "3" ] }

    AltExpect.containsAll match3 "match3"
    AltFlipExpect.containsAll "flipmatch3" match3

    let match4 =
      { Actual = [ A; B 1; C "3" ]
        Expected = [ C "3"; A ] }

    AltExpect.containsAll match4 "match4"
    AltFlipExpect.containsAll "flipmatch4" match4

  [<Test>]
  let containsAllShouldFail () =
    let match1 =
      { Actual = [ 1; 2; 3 ]
        Expected = [ 1; 4 ] }

    Assert.Throws<Expecto.AssertException>(fun _ -> AltExpect.containsAll match1 "match1")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.containsAll "flipmatch1" match1)
    |> ignore

    let match2 =
      { Actual = [ '1'; '2'; '3' ]
        Expected = [ '4'; '1' ] }

    Assert.Throws<Expecto.AssertException>(fun _ -> AltExpect.containsAll match2 "match2")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.containsAll "flipmatch2" match2)
    |> ignore

    let match3 =
      { Actual = [ "1"; "2"; "3" ]
        Expected = [ "1"; "4" ] }

    Assert.Throws<Expecto.AssertException>(fun _ -> AltExpect.containsAll match3 "match3")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.containsAll "flipmatch3" match3)
    |> ignore

    let match4 =
      { Actual = [ A; B 1; C "3" ]
        Expected = [ C "4"; A ] }

    Assert.Throws<Expecto.AssertException>(fun _ -> AltExpect.containsAll match4 "match4")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.containsAll "flipmatch4" match4)
    |> ignore

  let diffPrinter a b = sprintf "%A.ne.%A" a b

  [<Test>]
  let equalShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual [ 1; 3 ])
        .WithExpected
        [ 1; 3 ]

    AltExpect.equal match1 "match1"
    AltFlipExpect.equal "flipmatch1" match1
    AltExpect.equalWithDiffPrinter diffPrinter match1 "match1"
    AltFlipExpect.equalWithDiffPrinter diffPrinter "flipmatch1" match1

    Assert.Throws<Expecto.AssertException>(fun _ -> AltExpect.notEqual match1 "match1")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.notEqual "flipmatch1" match1)
    |> ignore

  [<Test>]
  let equalShouldFail () =
    let match1 =
      { Actual = [ 1; 2; 3 ]
        Expected = [ 1; 4 ] }

    Assert.Throws<Expecto.AssertException>(fun _ -> AltExpect.equal match1 "match1")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.equal "flipmatch1" match1)
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltExpect.equalWithDiffPrinter diffPrinter match1 "match1")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.equalWithDiffPrinter diffPrinter "flipmatch1" match1)
    |> ignore

    AltExpect.notEqual match1 "match1"
    AltFlipExpect.notEqual "flipmatch1" match1

  let accuracy =
    { Expecto.Accuracy.absolute = 0.1
      Expecto.Accuracy.relative = 0.01 }

  [<Test>]
  let floatCloseShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual 3.0)
        .WithExpected
        3.0

    AltExpect.floatClose accuracy match1 "match1"
    AltFlipExpect.floatClose "flipmatch1" accuracy match1

  [<Test>]
  let floatCloseShouldFail () =
    let match1 =
      { Actual = 5.0; Expected = 3.0 }

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltExpect.floatClose accuracy match1 "match1")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.floatClose "flipmatch1" accuracy match1)
    |> ignore

  [<Test>]
  let floatGreaterThanOrCloseShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual 5.0)
        .WithExpected
        3.0

    AltExpect.floatGreaterThanOrClose accuracy match1 "match1"
    AltFlipExpect.floatGreaterThanOrClose "flipmatch1" accuracy match1

  [<Test>]
  let floatGreaterThanOrCloseShouldFail () =
    let match1 =
      { Actual = -5.0; Expected = 3.0 }

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltExpect.floatGreaterThanOrClose accuracy match1 "match1")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.floatGreaterThanOrClose "flipmatch1" accuracy match1)
    |> ignore

  [<Test>]
  let floatLessThanOrCloseShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual -5.0)
        .WithExpected
        3.0

    AltExpect.floatLessThanOrClose accuracy match1 "match1"
    AltFlipExpect.floatLessThanOrClose "flipmatch1" accuracy match1

  [<Test>]
  let floatLessThanOrCloseShouldFail () =
    let match1 =
      { Actual = 5.0; Expected = 3.0 }

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltExpect.floatLessThanOrClose accuracy match1 "match1")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.floatLessThanOrClose "flipmatch1" accuracy match1)
    |> ignore

  let fast () = accuracy

  let slow () =
    System.Threading.Thread.Sleep(100)
    accuracy

  let makeMeasurer f = (fun measurer -> measurer f ())

  [<Test>]
  let isFasterThanShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual fast)
        .WithExpected
        slow

    AltExpect.isFasterThan match1 "match1"
    AltFlipExpect.isFasterThan "flipmatch1" match1

    let fastfunc =
      System.Func<Expecto.Accuracy>(fun () -> fast ())

    let slowfunc =
      System.Func<Expecto.Accuracy>(fun () -> slow ())

    let noop = System.Action(fun () -> ())

    let match1a =
      (AssertionMatch.Create().WithActual fastfunc)
        .WithExpected
        slowfunc

    let mutable result = System.String.Empty

    Assert.That(AltCSharpExpect.IsFasterThan(match1a, "match1a", &result))

    let match1b =
      (AssertionMatch.Create().WithActual(noop, fastfunc))
        .WithExpected(noop, slowfunc)

    Assert.That(AltCSharpExpect.IsFasterThan(match1b, "match1b", &result))

    let match1c =
      (AssertionMatch
        .Create()
        .WithActual(noop, fastfunc, noop))
        .WithExpected(noop, slowfunc, noop)

    Assert.That(AltCSharpExpect.IsFasterThan(match1c, "match1c", &result))

    let match2 =
      { Actual = makeMeasurer fast
        Expected = makeMeasurer slow }

    AltExpect.isFasterThanSub match2 "match2"
    AltFlipExpect.isFasterThanSub "flipmatch2" match2

  [<Test>]
  let isFasterThanShouldFail () =
    let match1 =
      { Actual = slow; Expected = fast }

    Assert.Throws<Expecto.FailedException>(fun _ ->
      AltExpect.isFasterThan match1 "match1")
    |> ignore

    Assert.Throws<Expecto.FailedException>(fun _ ->
      AltFlipExpect.isFasterThan "flipmatch1" match1)
    |> ignore

    let fastfunc =
      System.Func<Expecto.Accuracy>(fun () -> fast ())

    let slowfunc =
      System.Func<Expecto.Accuracy>(fun () -> slow ())

    let noop = System.Action(fun () -> ())

    let match1a =
      (AssertionMatch.Create().WithActual slowfunc)
        .WithExpected
        fastfunc

    let mutable result = System.String.Empty

    Assert.That(
      not
      <| AltCSharpExpect.IsFasterThan(match1a, "match1a", &result)
    )

    let match1b =
      (AssertionMatch.Create().WithActual(noop, slowfunc))
        .WithExpected(noop, fastfunc)

    Assert.That(
      not
      <| AltCSharpExpect.IsFasterThan(match1b, "match1b", &result)
    )

    let match1c =
      (AssertionMatch
        .Create()
        .WithActual(noop, slowfunc, noop))
        .WithExpected(noop, fastfunc, noop)

    Assert.That(
      not
      <| AltCSharpExpect.IsFasterThan(match1c, "match1c", &result)
    )

    let match2 =
      { Actual = makeMeasurer slow
        Expected = makeMeasurer fast }

    Assert.Throws<Expecto.FailedException>(fun _ ->
      AltExpect.isFasterThanSub match2 "match2")
    |> ignore

    Assert.Throws<Expecto.FailedException>(fun _ ->
      AltFlipExpect.isFasterThanSub "flipmatch2" match2)
    |> ignore

  [<Test>]
  let isGreaterThanOrEqualShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual 5)
        .WithExpected
        3

    AltExpect.isGreaterThanOrEqual match1 "match1"
    AltFlipExpect.isGreaterThanOrEqual "flipmatch1" match1
    AltExpect.isGreaterThan match1 "match1"
    AltFlipExpect.isGreaterThan "flipmatch1" match1

  [<Test>]
  let isGreaterThanOrEqualShouldFail () =
    let match1 = { Actual = -5; Expected = 3 }

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltExpect.isGreaterThanOrEqual match1 "match1")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.isGreaterThanOrEqual "flipmatch1" match1)
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltExpect.isGreaterThan match1 "match1")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.isGreaterThan "flipmatch1" match1)
    |> ignore

  [<Test>]
  let isLessThanOrEqualShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual -5)
        .WithExpected
        3

    AltExpect.isLessThanOrEqual match1 "match1"
    AltFlipExpect.isLessThanOrEqual "flipmatch1" match1
    AltExpect.isLessThan match1 "match1"
    AltFlipExpect.isLessThan "flipmatch1" match1

  [<Test>]
  let isLessThanOrEqualShouldFail () =
    let match1 = { Actual = 5; Expected = 3 }

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltExpect.isLessThanOrEqual match1 "match1")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.isLessThanOrEqual "flipmatch1" match1)
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ -> AltExpect.isLessThan match1 "match1")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.isLessThan "flipmatch1" match1)
    |> ignore

  [<Test>]
  let IsMatchShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual "Hello")
        .WithExpected
        "o"

    AltExpect.isMatch match1 "match1"
    AltFlipExpect.isMatch "flipmatch1" match1

    AltExpect.isMatchGroups
      match1
      (fun x -> x |> Seq.exists (fun y -> y.Success))
      "match1a"

    AltFlipExpect.isMatchGroups
      "flipmatch1a"
      (fun x -> x |> Seq.exists (fun y -> y.Success))
      match1

    Assert.Throws<Expecto.AssertException>(fun _ -> AltExpect.isNotMatch match1 "match1b")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.isNotMatch "flipmatch1b" match1)
    |> ignore

  [<Test>]
  let IsMatchShouldFail () =
    let match1 =
      { Actual = "Hello"; Expected = "x" }

    Assert.Throws<Expecto.AssertException>(fun _ -> AltExpect.isMatch match1 "match1")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.isMatch "flipmatch1" match1)
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltExpect.isMatchGroups
        match1
        (fun x -> x |> Seq.exists (fun y -> y.Success))
        "match1a")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.isMatchGroups
        "flipmatch1a"
        (fun x -> x |> Seq.exists (fun y -> y.Success))
        match1)
    |> ignore

    AltExpect.isNotMatch match1 "match1b"
    AltFlipExpect.isNotMatch "match1b" match1

  [<Test>]
  let StringStartsShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual "Hello")
        .WithExpected
        "H"

    AltExpect.stringStarts match1 "match1"
    AltFlipExpect.stringStarts "flipmatch1" match1

  [<Test>]
  let StringStartsShouldFail () =
    let match1 =
      { Actual = "Hello"; Expected = "o" }

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltExpect.stringStarts match1 "match1")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.stringStarts "flipmatch1" match1)
    |> ignore

  [<Test>]
  let StringEndsShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual "Hello")
        .WithExpected
        "o"

    AltExpect.stringEnds match1 "match1"
    AltFlipExpect.stringEnds "flipmatch1" match1

  [<Test>]
  let StringEndsShouldFail () =
    let match1 =
      { Actual = "Hello"; Expected = "H" }

    Assert.Throws<Expecto.AssertException>(fun _ -> AltExpect.stringEnds match1 "match1")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.stringEnds "flipmatch1" match1)
    |> ignore

  [<Test>]
  let StringContainsShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual "Hello")
        .WithExpected
        "l"

    AltExpect.stringContains match1 "match1"
    AltFlipExpect.stringContains "flipmatch1" match1

  [<Test>]
  let StringContainsShouldFail () =
    let match1 =
      { Actual = "Hello"; Expected = "?" }

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltExpect.stringContains match1 "match1")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.stringContains "flipmatch1" match1)
    |> ignore

  [<Test>]
  let streamsEqualShouldPass () =
    let data = [| 1uy; 3uy |]
    // fsharplint:disable-next-line  RedundantNewKeyword
    use s1 = new System.IO.MemoryStream(data)
    // fsharplint:disable-next-line  RedundantNewKeyword
    use s2 = new System.IO.MemoryStream(data)

    let match1 =
      (AssertionMatch.Create().WithActual s1)
        .WithExpected
        s2

    AltExpect.streamsEqual match1 "match1"
    AltFlipExpect.streamsEqual "flipmatch1" match1

  [<Test>]
  let StreamsEqualShouldFail () =
    use s1 = // fsharplint:disable-next-line  RedundantNewKeyword
      new System.IO.MemoryStream([| 1uy; 2uy; 3uy |])

    use s2 = // fsharplint:disable-next-line  RedundantNewKeyword
      new System.IO.MemoryStream([| 1uy; 4uy |])

    let match1 = { Actual = s1; Expected = s2 }

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltExpect.streamsEqual match1 "match1")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.streamsEqual "flipmatch1" match1)
    |> ignore

  [<Test>]
  let SequenceEqualShouldPass () =
    let match1 =
      { Actual = [ B 5; B 1; B 2 ]
        Expected = [ B 5; B 1; B 2 ] }

    AltExpect.sequenceEqual match1 "match1"
    AltFlipExpect.sequenceEqual "flipmatch1" match1

  [<Test>]
  let SequenceEqualShouldFail () =
    let match1 =
      { Actual = [ B 5; B 1; B 2 ]
        Expected = [ B 1; B 5; B 3 ] }

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltExpect.sequenceEqual match1 "match1")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.sequenceEqual "flipmatch1" match1)
    |> ignore

  [<Test>]
  let SequenceStartsShouldPass () =
    let match1 =
      { Actual = [ B 5; B 1; B 2; B 2 ]
        Expected = [ B 5; B 1 ] }

    AltExpect.sequenceStarts match1 "match1"
    AltFlipExpect.sequenceStarts "flipmatch1" match1

  [<Test>]
  let SequenceStartsShouldFail () =
    let match1 =
      { Actual = [ B 5; B 1; B 2; B 3 ]
        Expected = [ B 1; B 5 ] }

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltExpect.sequenceStarts match1 "match1")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.sequenceStarts "flipmatch1" match1)
    |> ignore

  [<Test>]
  let SequenceContainsOrderShouldPass () =
    let match1 =
      { Actual = [ B 5; B 1; B 2; B 2 ]
        Expected = [ B 5; B 2 ] }

    AltExpect.sequenceContainsOrder match1 "match1"
    AltFlipExpect.sequenceContainsOrder "flipmatch1" match1

  [<Test>]
  let SequenceContainsOrderShouldFail () =
    let match1 =
      { Actual = [ B 5; B 1; B 2; B 3 ]
        Expected = [ B 1; B 5 ] }

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltExpect.sequenceContainsOrder match1 "match1")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.sequenceContainsOrder "flipmatch1" match1)
    |> ignore