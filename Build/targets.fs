namespace AltCode.Test

module Targets =

  open System
  open System.IO
  open System.Xml.Linq

  open Fake.Core
  open Fake.Core.TargetOperators
  open Fake.DotNet
  open Fake.DotNet.NuGet.NuGet
  open Fake.IO
  open Fake.IO.Globbing.Operators
  open Fake.Tools.Git

  open NUnit.Framework

  let mutable Copyright = String.Empty
  let mutable Version = String.Empty
  let mutable VersionBase = String.Empty
  let mutable VersionSuffix = String.Empty

  let currentBranch =
    "."
    |> Path.getFullName
    |> Information.getBranchName

  let consoleBefore =
    (Console.ForegroundColor, Console.BackgroundColor)

  let programFiles =
    Environment.environVar "ProgramFiles"

  let programFiles86 =
    Environment.environVar "ProgramFiles(x86)"

  let dotnetPath =
    "dotnet"
    |> Fake.Core.ProcessUtils.tryFindFileOnPath

  let dotnetOptions (o: DotNet.Options) =
    match dotnetPath with
    | Some f -> { o with DotNetCliPath = f }
    | None -> o

  let nugetCache =
    Path.Combine(
      Environment.GetFolderPath Environment.SpecialFolder.UserProfile,
      ".nuget/packages"
    )

  let cliArguments =
    { MSBuild.CliArguments.Create() with
        ConsoleLogParameters = []
        DistributedLoggers = None
        DisableInternalBinLog = true }

  let withWorkingDirectoryVM dir o =
    { dotnetOptions o with
        WorkingDirectory = Path.getFullName dir
        Verbosity = Some DotNet.Verbosity.Minimal }

  let withWorkingDirectoryOnly dir o =
    { dotnetOptions o with WorkingDirectory = Path.getFullName dir }

  let withCLIArgs (o: Fake.DotNet.DotNet.TestOptions) =
    { o with MSBuildParams = cliArguments }

  let withMSBuildParams (o: Fake.DotNet.DotNet.BuildOptions) =
    { o with MSBuildParams = cliArguments }

  let toolPackages =
    let xml =
      "./Directory.Packages.props"
      |> Path.getFullName
      |> XDocument.Load

    xml.Descendants()
    |> Seq.filter (fun x -> x.Attribute(XName.Get("Include")) |> isNull |> not)
    |> Seq.map (fun x ->
      (x.Attribute(XName.Get("Include")).Value, x.Attribute(XName.Get("Version")).Value))
    |> Map.ofSeq

  let packageVersion (p: string) =
    p.ToLowerInvariant() + "/" + (toolPackages.Item p)

  let commitHash =
    Information.getCurrentSHA1 (".")

  let infoV =
    Information.showName "." commitHash

  let buildWithCLIArguments (o: Fake.DotNet.DotNet.BuildOptions) =
    { o with MSBuildParams = cliArguments }

  let withEnvironment (o: Fake.DotNet.DotNet.BuildOptions) =
    let before = o.Environment |> Map.toList

    let l =
      [ "Copyright", Copyright //.Replace("©", "(c)
        "PackageVersion", Version
        "VersionSuffix", VersionSuffix
        "PackageReleaseNotes", // TODO max length warning
        "This build from https://github.com/SteveGilham/altcode.test/tree/"
        + commitHash
        + Environment.NewLine
        + Environment.NewLine
        + (Path.getFullName "ReleaseNotes.md"
           |> File.ReadAllText) ]

    let after =
      [ l; before ] |> List.concat |> Map.ofList

    o.WithEnvironment after

  let dotnetBuildRelease proj =
    DotNet.build
      (fun p ->
        { p.WithCommon dotnetOptions with
            Configuration = DotNet.BuildConfiguration.Release }
        |> buildWithCLIArguments
        |> withEnvironment)
      (Path.GetFullPath proj)

  let dotnetBuildDebug proj =
    DotNet.build
      (fun p ->
        { p.WithCommon dotnetOptions with Configuration = DotNet.BuildConfiguration.Debug }
        |> buildWithCLIArguments
        |> withEnvironment)
      (Path.GetFullPath proj)

  let _Target s f =
    let doTarget s f =
      let banner x =
        printfn ""
        printfn " ****************** %s ******************" s
        f x

      Target.create s banner

    Target.description s
    doTarget s f

    let s2 = "Replay" + s
    Target.description s2
    doTarget s2 f

  // Preparation
  //_Target "Preparation" ignore

  let Clean =
    (fun _ ->
      printfn "Cleaning the build and deploy folders"
      Actions.Clean())

  let SetVersion =
    (fun _ ->
      let github =
        Environment.environVar "GITHUB_RUN_NUMBER"

      let version = "1.0.{build}"

      let ci =
        if String.IsNullOrWhiteSpace github then
          String.Empty
        else
          version.Replace("{build}", github)

      let (v, majmin, y) =
        Actions.LocalVersion ci version

      let trailer =
        if currentBranch.StartsWith "release/" then
          String.Empty
        else
          "-pre"

      VersionBase <- v
      VersionSuffix <- trailer
      Version <- v + trailer
      printfn "Version %A" Version

      let copy =
        sprintf "© 2019-%d by Steve Gilham <SteveGilham@users.noreply.github.com>" y

      Copyright <- "Copyright " + copy
      printfn "Copyright %A" Copyright
      Directory.ensure "./_Generated"
      Actions.InternalsVisibleTo(Version)
      let v' = Version

      [ "./_Generated/AssemblyVersion.fs"
        "./_Generated/AssemblyVersion.cs" ]
      |> List.iter (fun file ->
        AssemblyInfoFile.create
          file
          [ AssemblyInfo.Product "AltCode.Test"
            AssemblyInfo.Version(majmin + ".0.0")
            AssemblyInfo.FileVersion v'
            AssemblyInfo.Company "Steve Gilham"
            AssemblyInfo.Trademark ""
            AssemblyInfo.InformationalVersion(infoV)
            AssemblyInfo.Copyright copy ]
          (Some AssemblyInfoFileConfig.Default))

      let common =
        File.ReadAllText("./altcode.test/altcode.test.common/Common.fs")

      [ "Expecto"; "Nunit"; "Xunit" ]
      |> List.iter (fun k ->
        let text = common.Replace("Common", k)
        File.WriteAllText("./_Generated/Common." + k + ".fs", text))

      let hack =
        """namespace AltCover
  module SolutionRoot =
    let location = """
        + "\"\"\""
        + (Path.getFullName ".")
        + "\"\"\""

      let path = "_Generated/SolutionRoot.fs"

      // Update the file only if it would change
      let old =
        if File.Exists(path) then
          File.ReadAllText(path)
        else
          String.Empty

      if not (old.Equals(hack)) then
        File.WriteAllText(path, hack))

  // Basic compilation

  //_Target "Compilation" ignore

  let BuildRelease =
    (fun _ ->
      try
        DotNet.restore
          (fun o -> o.WithCommon(withWorkingDirectoryVM "."))
          "./altcode.test/altcode.test.sln"

        "./altcode.test/altcode.test.sln"
        |> dotnetBuildRelease
      with x ->
        printfn "%A" x
        reraise ())

  let BuildDebug =
    (fun _ ->
      DotNet.restore
        (fun o -> o.WithCommon(withWorkingDirectoryVM "."))
        "./altcode.test/altcode.test.sln"

      "./altcode.test/altcode.test.sln"
      |> dotnetBuildDebug)

  // Code Analysis

  //_Target "Analysis" ignore

  let Lint =
    (fun _ ->
      let cfg =
        Path.getFullName "./fsharplint.json"

      let doLint f =
        CreateProcess.fromRawCommand "dotnet" [ "fsharplint"; "lint"; "-l"; cfg; f ]
        |> CreateProcess.ensureExitCodeWithMessage "Lint issues were found"
        |> Proc.run

      let doLintAsync f = async { return (doLint f).ExitCode }

      let throttle x =
        Async.Parallel(x, System.Environment.ProcessorCount)

      let failOnIssuesFound (issuesFound: bool) =
        Assert.That(issuesFound, Is.False, "Lint issues were found")

      [ !! "./**/*.fsproj"
        |> Seq.sortBy (Path.GetFileName)
        !! "./Build/*.fsx" |> Seq.map Path.GetFullPath ]
      |> Seq.concat
      |> Seq.map doLintAsync
      |> throttle
      |> Async.RunSynchronously
      |> Seq.exists (fun x -> x <> 0)
      |> failOnIssuesFound)

  // Packaging

  let Packaging =
    (fun _ ->
      [ "expecto"; "nunit"; "xunit" ]
      |> List.iter (fun test ->
        let output =
          Path.getFullName ("_Packaging." + test)

        Directory.ensure output

        !!("./_Binaries/altcode.test."
           + test
           + "/Release+AnyCPU/*.nupkg")
        |> Seq.iter (fun f -> Shell.copyFile output f)))

  let PrepareReadMe =
    (fun _ ->
      Directory.ensure "./_Binaries"

      Actions.PrepareReadMe(
        (Copyright)
          .Replace("©", "&#xa9;")
          .Replace("<", "&lt;")
          .Replace(">", "&gt;")
      ))

  //let Deployment ignore

  // AOB
  let All =
    (fun _ ->
      if
        Environment.isWindows
        && currentBranch.StartsWith "release/"
        && "NUGET_API_TOKEN"
           |> Environment.environVar
           |> String.IsNullOrWhiteSpace
           |> not
      then
        (!! "./_Packagin*/*.nupkg")
        |> Seq.iter (fun f ->
          printfn "Publishing %A from %A" f currentBranch

          Actions.Run
            ("dotnet",
             ".",
             [ "nuget"
               "push"
               f
               "--api-key"
               Environment.environVar "NUGET_API_TOKEN"
               "--source"
               "https://api.nuget.org/v3/index.json" ])
            ("NuGet upload failed " + f)))

  let resetColours _ =
    Console.ForegroundColor <- consoleBefore |> fst
    Console.BackgroundColor <- consoleBefore |> snd

  Target.description "ResetConsoleColours"
  Target.createFinal "ResetConsoleColours" resetColours
  Target.activateFinal "ResetConsoleColours"

  let initTargets () =
    _Target "Preparation" ignore
    _Target "Clean" Clean
    _Target "SetVersion" SetVersion
    _Target "Compilation" ignore
    _Target "BuildRelease" BuildRelease
    _Target "BuildDebug" BuildDebug
    _Target "Analysis" ignore
    _Target "Lint" Lint
    _Target "Packaging" Packaging
    _Target "PrepareReadMe" PrepareReadMe
    _Target "Deployment" ignore
    _Target "All" All

    // Dependencies
    "Clean" ==> "SetVersion" ==> "Preparation"
    |> ignore

    "Preparation" ==> "BuildRelease" |> ignore

    "BuildRelease" ==> "BuildDebug" ==> "Compilation"
    |> ignore

    "BuildRelease" ==> "Lint" ==> "Analysis" |> ignore

    "Compilation" ==> "Analysis" |> ignore

    "Compilation" ?=> "Packaging" |> ignore

    "Compilation" ==> "Packaging" ==> "Deployment" |> ignore

    "Clean" ==> "PrepareReadMe" ==> "Preparation"
    |> ignore

    "Analysis" ==> "All" |> ignore

    "Deployment" ==> "All" |> ignore

  let defaultTarget () =
    resetColours ()
    "All"