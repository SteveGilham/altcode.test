namespace AltCode.Validation

module NUnit =

  open NUnit.Framework

  [<SetUp>]
  let Setup () = ()

  [<Test>]
  let Test1 () = Assert.Pass()