namespace AltCode.Test

module Targets =

  open System
  open System.IO
  open System.Xml.Linq

  open AltCoverFake.DotNet.DotNet
  open AltCoverFake.DotNet.Testing

  open Fake.Core
  open Fake.Core.TargetOperators
  open Fake.DotNet
  open Fake.DotNet.NuGet.NuGet
  open Fake.IO
  open Fake.IO.FileSystemOperators
  open Fake.IO.Globbing.Operators
  open Fake.Testing
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

  let dotnetOptionsWithRollForwards (o: DotNet.Options) =
    let env =
      o.Environment
        .Add("DOTNET_ROLL_FORWARD_ON_NO_CANDIDATE_FX", "2")
        .Add("DOTNET_ROLL_FORWARD", "Major")

    o.WithEnvironment env

  let nugetCache =
    Path.Combine(
      Environment.GetFolderPath Environment.SpecialFolder.UserProfile,
      ".nuget/packages"
    )

  let AltCoverFilter (p: Primitive.PrepareOptions) =
    { p with
        AssemblyExcludeFilter =
          @"NUnit3\."
          :: (@"\.Tests"
              :: (p.AssemblyExcludeFilter |> Seq.toList))
        AssemblyFilter =
          "FSharp"
          :: @"\.Placeholder"
          :: (p.AssemblyFilter |> Seq.toList)
        LocalSource = true
        TypeFilter =
          [ @"System\."
            "Microsoft"
            "Program"
            @"\$RepoRoot" ]
          @ (p.TypeFilter |> Seq.toList) }

  let mutable misses = 0

  let uncovered (path: string) =
    misses <- 0

    !!path
    |> Seq.collect (fun f ->
      let xml = XDocument.Load f

      xml.Descendants(XName.Get("Uncoveredlines"))
      |> Seq.filter (fun x ->
        match String.IsNullOrWhiteSpace x.Value with
        | false -> true
        | _ ->
          sprintf "No coverage from '%s'" f
          |> Trace.traceImportant

          misses <- 1 + misses
          false)
      |> Seq.map (fun e ->
        let coverage = e.Value

        match Int32.TryParse coverage with
        | (false, _) ->
          printfn "%A" xml

          Assert.Fail(
            "Could not parse uncovered line value '"
            + coverage
            + "'"
          )

          0
        | (_, numeric) ->
          printfn "%s : %A" (f |> Path.GetDirectoryName |> Path.GetFileName) numeric
          numeric))
    |> Seq.toList

  let cliArguments =
    { MSBuild.CliArguments.Create() with
        ConsoleLogParameters = []
        DistributedLoggers = None
        Properties = []
        DisableInternalBinLog = true }

  let withWorkingDirectoryVM dir o =
    { dotnetOptions o with
        WorkingDirectory = Path.getFullName dir
        Verbosity = Some DotNet.Verbosity.Minimal }

  let withWorkingDirectoryOnly dir o =
    { dotnetOptions o with
        WorkingDirectory = Path.getFullName dir }

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

  let ReleaseNotes () =
    let source =
      Path.getFullName "ReleaseNotes.md"
      |> File.ReadAllLines
      |> Seq.map (fun s ->
        let sv = s.Replace("#????", "# " + Version)

        let t =
          System.Text.RegularExpressions.Regex.Replace(sv, "^\*\s", "* •\u00A0")

        let u =
          System.Text.RegularExpressions.Regex.Replace(
            t,
            "^\s\s\*\s", // ◦ U+25E6 WHITE BULLET
            "  * \u00A0\u00A0\u25E6\u00A0"
          )

        let v =
          System.Text.RegularExpressions.Regex.Replace(
            u,
            "^\s\s\s+\*\s", // ⁃ U+2043 HYPHEN BULLET,
            "    * \u00A0\u00A0\u00A0\u00A0\u2043\u00A0"
          )

        System.Text.RegularExpressions.Regex.Replace(
          v,
          "^#\s", // ⁋ U+204B REVERSED PILCROW SIGN
          "# \u204B"
        ))
      |> (fun s -> String.Join(Environment.NewLine, s))

    use w = // fsharplint:disable-next-line  RedundantNewKeyword
      new StringWriter()
    // printfn "tweaked = %A" source
    Markdig.Markdown.ToPlainText(source, w) |> ignore

    let releaseNotes =
      "This build from https://github.com/SteveGilham/altcode.test/tree/"
      + commitHash
      + Environment.NewLine
      + Environment.NewLine
      + w.ToString().Replace("\u204B", Environment.NewLine)

    printfn "release notes are %A characters" releaseNotes.Length
    Assert.That(releaseNotes.Length, Is.LessThan 35000)
    releaseNotes

  let buildWithCLIArguments (o: Fake.DotNet.DotNet.BuildOptions) =
    { o with MSBuildParams = cliArguments }

  let withEnvironment (o: Fake.DotNet.DotNet.BuildOptions) =
    let before = o.Environment |> Map.toList

    let l =
      [ "Copyright", Copyright //.Replace("©", "(c)
        "PackageVersion", Version
        "VersionSuffix", VersionSuffix
        "PackageReleaseNotes", ReleaseNotes() ]

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
        { p.WithCommon dotnetOptions with
            Configuration = DotNet.BuildConfiguration.Debug }
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

      let version = "2.0.{build}" // <+++++++++++++++++

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

      [ "Expecto"
        "Common"
        "Nunit"
        "Xunit" ]
      |> List.iter (fun k ->
        let text = common.Replace("Common", k)
        File.WriteAllText("./_Generated/Common." + k + ".fs", text))

      let hack =
        """namespace AltCode.Test
  module RepoRoot =
    let location = """
        + "\"\"\""
        + (Path.getFullName ".")
        + "\"\"\""

      let path = "_Generated/RepoRoot.fs"

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
        "./altcode.test/altcode.test.slnx"
        |> dotnetBuildRelease
      with x ->
        printfn "%A" x
        reraise ())

  let BuildDebug =
    (fun _ ->
      "./altcode.test/altcode.test.slnx"
      |> dotnetBuildDebug)

  let Validation =
    (fun _ ->
      DotNet.test
        (fun p ->
          { p.WithCommon dotnetOptions with
              MSBuildParams = cliArguments
              Configuration = DotNet.BuildConfiguration.Debug
              Framework = Some "net8.0"
              NoBuild = true })
        "./altcode.test/validation")

  let Coverage =
    (fun _ ->
      let reports = Path.getFullName "./_Reports"
      Directory.ensure reports

      let report = "./_Reports/Coverage"

      Directory.ensure report

      let coverage =
        !!(@"./**/validation.fsproj")
        |> Seq.fold
          (fun l test ->
            printfn "%A" test

            let tname =
              test |> Path.GetFileNameWithoutExtension

            let testDirectory =
              test |> Path.getFullName |> Path.GetDirectoryName

            let altReport =
              reports @@ ("Coverage." + tname + ".xml")

            let collect =
              AltCover.CollectOptions.Primitive(Primitive.CollectOptions.Create()) // FSApi

            let prepare =
              AltCover.PrepareOptions.Primitive(
                { Primitive.PrepareOptions.Create() with
                    Report = altReport
                    All = false
                }
                |> AltCoverFilter
              )

            let forceTrue = DotNet.CLIOptions.Force true

            let setBaseOptions (o: DotNet.Options) =
              { o with
                  WorkingDirectory = Path.getFullName testDirectory
                  Verbosity = Some DotNet.Verbosity.Minimal }

            try
              DotNet.test
                (fun to' ->
                  { to'.WithCommon(setBaseOptions) with
                      MSBuildParams = cliArguments
                      Framework = Some "net8.0"
                      NoBuild = true }
                    .WithAltCoverOptions
                    prepare
                    collect
                    forceTrue)
                test
            with x ->
              printfn "%A" x
              reraise () // argue either way

            altReport :: l)
          []

      ReportGenerator.generateReports
        (fun p ->
          { p with
              ToolType = ToolType.CreateLocalTool()
              ReportTypes =
                [ ReportGenerator.ReportType.Html
                  ReportGenerator.ReportType.XmlSummary ]
              TargetDir = report })
        coverage

      if
        Environment.isWindows
        && "COVERALLS_REPO_TOKEN"
           |> Environment.environVar
           |> String.IsNullOrWhiteSpace
           |> not
      then
        let maybe envvar fallback =
          let x = Environment.environVar envvar

          if String.IsNullOrWhiteSpace x then
            fallback
          else
            x

        let log = Information.shortlog "."
        let gap = log.IndexOf ' '
        let commit = log.Substring gap

        Actions.Run
          ("dotnet",
           "_Reports",
           [ "csmacnz.Coveralls"
             "--opencover"
             "-i"
             (Seq.head coverage)
             "--repoToken"
             Environment.environVar "COVERALLS_REPO_TOKEN"
             "--commitId"
             commitHash
             "--commitBranch"
             Information.getBranchName (".")
             "--commitAuthor"
             maybe "COMMIT_AUTHOR" "" // TODO
             "--commitEmail"
             maybe "COMMIT_AUTHOR_EMAIL" "" //
             "--commitMessage"
             commit
             "--jobId"
             DateTime.UtcNow.ToString("yyMMdd-HHmmss") ])
          "Coveralls upload failed"

      (report @@ "Summary.xml")
      |> uncovered
      |> (fun u ->
        u |> (printfn "%A uncovered lines")
        // printfn "%A" (u.GetType().FullName)
        Assert.That<int list>(
          u,
          Is.EqualTo<int list> [ 0 ],
          "All lines should be covered"
        )))

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

      [ !!"./**/*.fsproj" |> Seq.sortBy (Path.GetFileName)
        !!"./Build/*.fsx" |> Seq.map Path.GetFullPath ]
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
        (Copyright).Replace("©", "&#xa9;").Replace("<", "&lt;").Replace(">", "&gt;")
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
        (!!"./_Packagin*/*.nupkg")
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
    _Target "Validation" Validation
    _Target "Coverage" Coverage
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

    "Compilation"
    ==> "Validation"
    ==> "Coverage"
    ==> "All"
    |> ignore

    "Compilation" ?=> "Packaging" |> ignore

    "Compilation" ==> "Packaging" ==> "Deployment"
    |> ignore

    "Clean" ==> "PrepareReadMe" ==> "Preparation"
    |> ignore

    "Analysis" ==> "All" |> ignore

    "Deployment" ==> "All" |> ignore

  let defaultTarget () =
    resetColours ()
    "All"