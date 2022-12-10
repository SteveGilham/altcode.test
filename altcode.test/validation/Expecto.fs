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
        .WithExpected [ 1; 3 ]

    AltExpect.containsAll match1 "match1"
    AltFlipExpect.containsAll "flipmatch1" match1

    let match2 =
      { AssertionMatch.Create() with
          Actual = [ '1'; '2'; '3' ]
          Expected = [ '3'; '1' ] }

    AltExpect.containsAll match2 "match2"
    AltFlipExpect.containsAll "flipmatch2" match2

    let match3 =
      { AssertionMatch.Create() with
          Actual = [ "1"; "2"; "3" ]
          Expected = [ "1"; "3" ] }

    AltExpect.containsAll match3 "match3"
    AltFlipExpect.containsAll "flipmatch3" match3

    let match4 =
      { AssertionMatch.Create() with
          Actual = [ A; B 1; C "3" ]
          Expected = [ C "3"; A ] }

    AltExpect.containsAll match4 "match4"
    AltFlipExpect.containsAll "flipmatch4" match4

  [<Test>]
  let containsAllShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = [ 1; 2; 3 ]
          Expected = [ 1; 4 ] }

    Assert.Throws<Expecto.AssertException>(fun _ -> AltExpect.containsAll match1 "match1")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.containsAll "flipmatch1" match1)
    |> ignore

    let match2 =
      { AssertionMatch.Create() with
          Actual = [ '1'; '2'; '3' ]
          Expected = [ '4'; '1' ] }

    Assert.Throws<Expecto.AssertException>(fun _ -> AltExpect.containsAll match2 "match2")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.containsAll "flipmatch2" match2)
    |> ignore

    let match3 =
      { AssertionMatch.Create() with
          Actual = [ "1"; "2"; "3" ]
          Expected = [ "1"; "4" ] }

    Assert.Throws<Expecto.AssertException>(fun _ -> AltExpect.containsAll match3 "match3")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.containsAll "flipmatch3" match3)
    |> ignore

    let match4 =
      { AssertionMatch.Create() with
          Actual = [ A; B 1; C "3" ]
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
        .WithExpected [ 1; 3 ]

    AltExpect.equal match1 "match1"
    AltFlipExpect.equal "flipmatch1" match1
    AltExpect.equalWithDiffPrinter diffPrinter match1 "match1"
    AltFlipExpect.equalWithDiffPrinter diffPrinter "flipmatch1" match1


  [<Test>]
  let equalShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = [ 1; 2; 3 ]
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

  let accuracy =
    { Expecto.Accuracy.absolute = 0.1
      Expecto.Accuracy.relative = 0.01 }

  [<Test>]
  let floatCloseShouldPass () =
    let match1 =
      (AssertionMatch.Create().WithActual 3.0)
        .WithExpected 3.0

    AltExpect.floatClose accuracy match1 "match1"
    AltFlipExpect.floatClose "flipmatch1" accuracy match1

  [<Test>]
  let floatCloseShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = 5.0
          Expected = 3.0 }

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
        .WithExpected 3.0

    AltExpect.floatGreaterThanOrClose accuracy match1 "match1"
    AltFlipExpect.floatGreaterThanOrClose "flipmatch1" accuracy match1

  [<Test>]
  let floatGreaterThanOrCloseShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = -5.0
          Expected = 3.0 }

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
        .WithExpected 3.0

    AltExpect.floatLessThanOrClose accuracy match1 "match1"
    AltFlipExpect.floatLessThanOrClose "flipmatch1" accuracy match1

  [<Test>]
  let floatLessThanOrCloseShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = 5.0
          Expected = 3.0 }

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltExpect.floatLessThanOrClose accuracy match1 "match1")
    |> ignore

    Assert.Throws<Expecto.AssertException>(fun _ ->
      AltFlipExpect.floatLessThanOrClose "flipmatch1" accuracy match1)
    |> ignore