open System
open System.IO
open System.Xml.Linq

open Actions

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.DotNet.NuGet.NuGet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing
open Fake.IO.Globbing.Operators

open FSharpLint.Application
open FSharpLint.Framework

open NUnit.Framework

let Copyright = ref String.Empty
let Version = ref String.Empty
let consoleBefore = (Console.ForegroundColor, Console.BackgroundColor)
let programFiles = Environment.environVar "ProgramFiles"
let programFiles86 = Environment.environVar "ProgramFiles(x86)"
let dotnetPath = "dotnet" |> Fake.Core.ProcessUtils.tryFindFileOnPath

let dotnetOptions (o : DotNet.Options) =
  match dotnetPath with
  | Some f -> { o with DotNetCliPath = f }
  | None -> o

let nugetCache =
  Path.Combine
    (Environment.GetFolderPath Environment.SpecialFolder.UserProfile, ".nuget/packages")

let cliArguments =
  { MSBuild.CliArguments.Create() with ConsoleLogParameters = []
                                       DistributedLoggers = None
                                       DisableInternalBinLog = true }

let withWorkingDirectoryVM dir o =
  { dotnetOptions o with WorkingDirectory = Path.getFullName dir
                         Verbosity = Some DotNet.Verbosity.Minimal }

let withWorkingDirectoryOnly dir o =
  { dotnetOptions o with WorkingDirectory = Path.getFullName dir }
let withCLIArgs (o : Fake.DotNet.DotNet.TestOptions) =
  { o with MSBuildParams = cliArguments }
let withMSBuildParams (o : Fake.DotNet.DotNet.BuildOptions) =
  { o with MSBuildParams = cliArguments }

let toolPackages =
  let xml =
    "./Build/dotnet-cli.csproj"
    |> Path.getFullName
    |> XDocument.Load
  xml.Descendants(XName.Get("PackageReference"))
  |> Seq.map
       (fun x ->
       (x.Attribute(XName.Get("Include")).Value, x.Attribute(XName.Get("version")).Value))
  |> Map.ofSeq

let packageVersion (p: string) = p.ToLowerInvariant() + "/" + (toolPackages.Item p)

let _Target s f =
  Target.description s
  Target.create s f

// Preparation
_Target "Preparation" ignore

_Target "Clean" (fun _ ->
  printfn "Cleaning the build and deploy folders"
  Actions.Clean())

_Target "SetVersion" (fun _ ->
  let appveyor = Environment.environVar "APPVEYOR_BUILD_VERSION"
  let travis = Environment.environVar "TRAVIS_BUILD_NUMBER"
  let version = Actions.GetVersionFromYaml()

  let ci =
    if String.IsNullOrWhiteSpace appveyor then
      if String.IsNullOrWhiteSpace travis then String.Empty
      else version.Replace("{build}", travis + "-travis")
    else appveyor

  let (v, majmin, y) = Actions.LocalVersion ci version
  Version := v
  let copy = sprintf "© 2019-%d by Steve Gilham <SteveGilham@users.noreply.github.com>" y
  Copyright := "Copyright " + copy
  Directory.ensure "./_Generated"
  Actions.InternalsVisibleTo(!Version)
  let v' = !Version
  [ "./_Generated/AssemblyVersion.fs"; "./_Generated/AssemblyVersion.cs" ]
  |> List.iter
       (fun file ->
       AssemblyInfoFile.create file [ AssemblyInfo.Product "AltCode.Test"
                                      AssemblyInfo.Version(majmin + ".0.0")
                                      AssemblyInfo.FileVersion v'
                                      AssemblyInfo.Company "Steve Gilham"
                                      AssemblyInfo.Trademark ""
                                      AssemblyInfo.Copyright copy ]
         (Some AssemblyInfoFileConfig.Default))
  let hack = """namespace AltCover
module SolutionRoot =
  let location = """ + "\"\"\"" + (Path.getFullName ".") + "\"\"\""
  let path = "_Generated/SolutionRoot.fs"

  // Update the file only if it would change
  let old =
    if File.Exists(path) then File.ReadAllText(path)
    else String.Empty
  if not (old.Equals(hack)) then File.WriteAllText(path, hack))

// Basic compilation

_Target "Compilation" ignore

_Target "BuildRelease" (fun _ ->
  try
    DotNet.restore (fun o -> o.WithCommon(withWorkingDirectoryVM ".")) "./altcode.test/altcode.test.sln"
    "./altcode.test/altcode.test.sln"
    |> MSBuild.build (fun p ->
         { p with Verbosity = Some MSBuildVerbosity.Normal
                  ConsoleLogParameters = []
                  DistributedLoggers = None
                  DisableInternalBinLog = true
                  Properties =
                    [ "Configuration", "Release"
                      "DebugSymbols", "True" ] })
  with x ->
    printfn "%A" x
    reraise())

_Target "BuildDebug" (fun _ ->
  DotNet.restore (fun o -> o.WithCommon(withWorkingDirectoryVM ".")) "./altcode.test/altcode.test.sln"
  "./altcode.test/altcode.test.sln"
  |> MSBuild.build (fun p ->
       { p with Verbosity = Some MSBuildVerbosity.Normal
                ConsoleLogParameters = []
                DistributedLoggers = None
                DisableInternalBinLog = true
                Properties =
                  [ "Configuration", "Debug"
                    "DebugSymbols", "True" ] }))

// Code Analysis

_Target "Analysis" ignore

_Target "Lint" (fun _ ->
  let failOnIssuesFound (issuesFound: bool) =
    Assert.That(issuesFound, Is.False, "Lint issues were found")
  try
    let settings =
      Configuration.SettingsFileName
      |> Path.getFullName
      |> File.ReadAllText

    let lintConfig =
      FSharpLint.Application.ConfigurationManagement.loadConfigurationFile settings
    let options =
      { Lint.OptionalLintParameters.Default with Configuration = Some lintConfig }

    !!"**/*.fsproj"
    |> Seq.collect (fun n -> !!(Path.GetDirectoryName n @@ "*.fs"))
    |> Seq.distinct
    |> Seq.map (fun f ->
         match Lint.lintFile options f with
         | Lint.LintResult.Failure x -> failwithf "%A" x
         | Lint.LintResult.Success w ->
           w
           |> Seq.filter (fun x ->
                match x.Fix with
                | None -> false
                | Some fix -> fix.FromText <> "AltCover_Fake")) // special case
    |> Seq.concat
    |> Seq.fold (fun _ x ->
         printfn "Info: %A\r\n Range: %A\r\n Fix: %A\r\n====" x.Info x.Range x.Fix
         true) false
    |> failOnIssuesFound
  with ex ->
    printfn "%A" ex
    reraise())

// Packaging

_Target "Packaging" (fun _ ->
  let gendarmeDir =
    Path.getFullName "_Binaries/AltCode.Fake.DotNet.Gendarme/Release+AnyCPU"
  let packable = Path.getFullName "./_Binaries/README.html"

  let gendarmeFiles =
    [ (gendarmeDir @@ "AltCode.Fake.DotNet.Gendarme.dll", Some "lib/net462", None)
      (packable, Some "", None) ]

  let gendarmeNetcoreFiles =
    (!!(gendarmeDir @@ "netstandard2.0/AltCode.Fake.DotNet.Gendarme.*"))
    |> Seq.map (fun x -> (x, Some "lib/netstandard2.0", None))
    |> Seq.toList

  let publishWhat = (Path.getFullName "./_Publish.vsWhat").Length
  let whatFiles where =
    (!!"./_Publish.vsWhat/**/*.*")
    |> Seq.map
         (fun x ->
           (x, Some(where + Path.GetDirectoryName(x).Substring(publishWhat).Replace("\\", "/")), None))
    |> Seq.toList

  [ (List.concat [ gendarmeFiles; gendarmeNetcoreFiles ], "_Packaging.Gendarme",
     "./_Generated/altcode.fake.dotnet.gendarme.nuspec", "AltCode.Fake.DotNet.Gendarme",
     "A helper task for running Mono.Gendarme from FAKE ( >= 5.9.3 )",
     "Gendarme")
    (whatFiles "tools/netcoreapp2.1/any", "_Packaging.VsWhat",
     "./_Generated/altcode.vswhat.nuspec", "AltCode.VsWhat",
     "A tool to list Visual Studio instances and their installed packages",
     "VsWhat") ]
  |> List.iter (fun (files, output, nuspec, project, description, what) ->
       let outputPath = "./" + output
       let workingDir = "./_Binaries/" + output
       Directory.ensure workingDir
       Directory.ensure outputPath
       NuGet (fun p ->
         { p with Authors = [ "Steve Gilham" ]
                  Project = project
                  Description = description
                  OutputPath = outputPath
                  WorkingDir = workingDir
                  Files = files
                  Version = !Version
                  Copyright = (!Copyright).Replace("©", "(c)")
                  Publish = false
                  ReleaseNotes = Path.getFullName ("ReleaseNotes." + what + ".md") |> File.ReadAllText
                  ToolPath =
                    if Environment.isWindows then
                      ("./packages/" + (packageVersion "NuGet.CommandLine")
                       + "/tools/NuGet.exe") |> Path.getFullName
                    else "/usr/bin/nuget" }) nuspec))

_Target "PrepareDotNetBuild" (fun _ ->
  let publish = Path.getFullName "./_Publish"

  DotNet.publish (fun options ->
    { options with OutputPath = Some(publish + ".vsWhat")
                   Configuration = DotNet.BuildConfiguration.Release
                   Framework = Some "netcoreapp2.1" })
    (Path.getFullName "./AltCode.VsWhat/AltCode.VsWhat.fsproj")

  [ (String.Empty, "./_Generated/altcode.fake.dotnet.gendarme.nuspec",
     "AltCode.Fake.DotNet.Gendarme (FAKE task helper)", None,
     Some "FAKE build Gendarme") 
    ("DotnetTool", "./_Generated/altcode.vswhat.nuspec",
     "AltCode.VsWhat (Visual Studio package listing tool)", Some "Build/AltCode.VsWhat_128.png",
     Some "Visual Studio") ]
  |> List.iter (fun (ptype, path, caption, icon, tags) ->
       let x s = XName.Get(s, "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd")
       let dotnetNupkg = XDocument.Load "./Build/AltCode.Fake.nuspec"
       let title = dotnetNupkg.Descendants(x "title") |> Seq.head
       title.ReplaceNodes caption
       if ptype
          |> String.IsNullOrWhiteSpace
          |> not
       then
         let tag = dotnetNupkg.Descendants(x "tags") |> Seq.head
         let insert = XElement(x "packageTypes")
         insert.Add(XElement(x "packageType", XAttribute(XName.Get "name", ptype)))
         tag.AddAfterSelf insert
       match icon with
       | None -> ()
       | Some logo ->
         let tag = dotnetNupkg.Descendants(x "iconUrl") |> Seq.head
         let text = String.Concat(tag.Nodes()).Replace("Build/AltCode.Fake_128.png", logo)
         tag.Value <- text
       match tags with
       | None -> ()
       | Some line ->
         let tagnode = dotnetNupkg.Descendants(x "tags") |> Seq.head
         tagnode.Value <- line
       dotnetNupkg.Save path))

_Target "PrepareReadMe"
  (fun _ ->
  Actions.PrepareReadMe
    ((!Copyright).Replace("©", "&#xa9;").Replace("<", "&lt;").Replace(">", "&gt;")))

_Target "Deployment" ignore

// AOB
_Target "All" ignore

let resetColours _ =
  Console.ForegroundColor <- consoleBefore |> fst
  Console.BackgroundColor <- consoleBefore |> snd

Target.description "ResetConsoleColours"
Target.createFinal "ResetConsoleColours" resetColours
Target.activateFinal "ResetConsoleColours"

// Dependencies
"Clean"
==> "SetVersion"
==> "Preparation"

"Preparation"
==> "BuildRelease"

"BuildRelease"
==> "BuildDebug"
==> "Compilation"

"BuildRelease"
==> "Lint"
==> "Analysis"

"Compilation"
==> "Analysis"

"Compilation"
?=> "Packaging"

"Compilation"
==> "PrepareDotNetBuild"
==> "Packaging"

"Compilation"
==> "PrepareReadMe"
==> "Packaging"
==> "Deployment"

"Analysis" ==> "All"

"Deployment" 
==> "All"

let defaultTarget() =
  resetColours()
  "All"

Target.runOrDefault <| defaultTarget()