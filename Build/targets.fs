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

  let Copyright = ref String.Empty
  let Version = ref String.Empty

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

  let dotnetBuildRelease proj =
    DotNet.build
      (fun p ->
        { p.WithCommon dotnetOptions with
            Configuration = DotNet.BuildConfiguration.Release }
        |> buildWithCLIArguments)
      (Path.GetFullPath proj)

  let dotnetBuildDebug proj =
    DotNet.build
      (fun p ->
        { p.WithCommon dotnetOptions with Configuration = DotNet.BuildConfiguration.Debug }
        |> buildWithCLIArguments)
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

      Version.Value <- v + trailer

      let copy =
        sprintf "© 2019-%d by Steve Gilham <SteveGilham@users.noreply.github.com>" y

      Copyright.Value <- "Copyright " + copy
      Directory.ensure "./_Generated"
      Actions.InternalsVisibleTo(Version.Value)
      let v' = Version.Value

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
      let packable =
        Path.getFullName "./_Binaries/README.html"

      let extras =
        [ (packable, Some "", None)
          (Path.getFullName "./Build/Icon_128x.png", Some "Icon_128x.png", None)
          (Path.getFullName "./LICENS*", Some "", None) ]

      [ "Expecto"; "NUnit"; "Xunit" ]
      |> List.iter (fun utest ->
        let test = utest.ToLowerInvariant()

        let nuspec =
          Path.getFullName (
            "./_Intermediate/altcode.test."
            + test
            + "/Release/Release/altcode.test."
            + test
            + ".1.0.0.nuspec"
          )

        let fileroot =
          Path.getFullName ("./_Publish/" + test + "/lib")

        let chop = fileroot.Length

        let files =
          !!(fileroot + "/**/*")
          |> Seq.map (fun f ->
            (f, Some("lib" + ((Path.GetDirectoryName f).Substring chop)), None))
          |> Seq.toList

        let output =
          Path.getFullName ("_Packaging." + test)

        let workingDir =
          Path.getFullName ("./altcode.test/_Binaries/" + test)

        Directory.ensure workingDir
        Directory.ensure output

        NuGet
          (fun p ->
            { p with
                Authors = [ "Steve Gilham" ]
                Description =
                  "A named-argument helper wrapper for unit tests with "
                  + utest
                OutputPath = output
                WorkingDir = workingDir
                Files = files @ extras
                Version = Version.Value
                Copyright = (Copyright.Value).Replace("©", "(c)")
                Publish = false
                ReleaseNotes =
                  "This build from https://github.com/SteveGilham/altcode.test/tree/"
                  + commitHash
                  + Environment.NewLine
                  + Environment.NewLine
                  + (Path.getFullName "ReleaseNotes.md"
                     |> File.ReadAllText)
                ToolPath =
                  if Environment.isWindows then
                    ("./packages/"
                     + (packageVersion "NuGet.CommandLine")
                     + "/tools/NuGet.exe")
                    |> Path.getFullName
                  else
                    "/usr/bin/nuget" })
          nuspec))

  let PrepareDotNetBuild =
    (fun _ ->
      let packaging =
        Path.getFullName "_Packaging"

      Directory.ensure packaging
      let publish = Path.getFullName "./_Publish"

      !!("./altcode.test/_Binaries/*/Release+AnyCPU/*.nupkg")
      |> Seq.iter (fun f ->
        let name = Path.GetFileName f

        let unpack =
          Path.Combine(publish, name.Split('.').[2])

        System.IO.Compression.ZipFile.ExtractToDirectory(f, unpack)

      // let dotnetNupkg = XDocument.Load path

      // dotnetNupkg.Descendants()
      // |> Seq.filter (fun x ->
      //   x.Name.LocalName = "dependency"
      //   && x.Attributes()
      //      |> Seq.exists (fun node ->
      //        node.Name.LocalName = "id"
      //        && node.Value = "altcode.test.common"))
      // |> Seq.toList
      // |> List.iter (fun n -> n.Remove())

      // dotnetNupkg.Save(
      //   (Path.Combine(packaging, Path.GetFileName path)),
      //   SaveOptions.None
      // )))
      ))

  let PrepareReadMe =
    (fun _ ->
      Directory.ensure "./_Binaries"

      Actions.PrepareReadMe(
        (Copyright.Value)
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
    _Target "PrepareDotNetBuild" PrepareDotNetBuild
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

    "Compilation"
    ==> "PrepareDotNetBuild"
    ==> "Packaging"
    |> ignore

    "Clean" ==> "PrepareReadMe" ==> "Preparation"
    |> ignore

    "Analysis" ==> "All" |> ignore

    "Deployment" ==> "All" |> ignore

  let defaultTarget () =
    resetColours ()
    "All"