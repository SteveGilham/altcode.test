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
      { AssertionMatch.Create() with
          Actual = [ 1; 2; 3 ]
          Expected = [ 1; 3 ] }

    AltExpect.containsAll match1 "match1"

    let match2 =
      { AssertionMatch.Create() with
          Actual = [ '1'; '2'; '3' ]
          Expected = [ '3'; '1' ] }

    AltExpect.containsAll match2 "match2"

    let match3 =
      { AssertionMatch.Create() with
          Actual = [ "1"; "2"; "3" ]
          Expected = [ "1"; "3" ] }

    AltExpect.containsAll match3 "match3"

    let match4 =
      { AssertionMatch.Create() with
          Actual = [ A; B 1; C "3" ]
          Expected = [ C "3"; A ] }

    AltExpect.containsAll match4 "match4"

  [<Test>]
  let containsAllShouldFail () =
    let match1 =
      { AssertionMatch.Create() with
          Actual = [ 1; 2; 3 ]
          Expected = [ 1; 4 ] }

    Assert.Throws<Expecto.AssertException>(fun _ -> AltExpect.containsAll match1 "match1")
    |> ignore

    let match2 =
      { AssertionMatch.Create() with
          Actual = [ '1'; '2'; '3' ]
          Expected = [ '4'; '1' ] }

    Assert.Throws<Expecto.AssertException>(fun _ -> AltExpect.containsAll match2 "match2")
    |> ignore

    let match3 =
      { AssertionMatch.Create() with
          Actual = [ "1"; "2"; "3" ]
          Expected = [ "1"; "4" ] }

    Assert.Throws<Expecto.AssertException>(fun _ -> AltExpect.containsAll match3 "match3")
    |> ignore

    let match4 =
      { AssertionMatch.Create() with
          Actual = [ A; B 1; C "3" ]
          Expected = [ C "4"; A ] }

    Assert.Throws<Expecto.AssertException>(fun _ -> AltExpect.containsAll match4 "match4")
    |> ignore