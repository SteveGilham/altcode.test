namespace AltCode.Validation

module Xunit =

  open NUnit.Framework

  [<SetUp>]
  let Setup () = ()

  [<Test>]
  let Test1 () = Assert.Pass()