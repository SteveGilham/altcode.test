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
  let packable = Path.getFullName "./altcode.test/_Binaries/README.html"
  let extras = [
                (packable, Some "", None)
                (Path.getFullName "./Build/Icon_128x.png", Some "Icon_128x.png", None)
                ]

  [
    "Expecto"
    "NUnit"
    "Xunit"
  ]
  |> List.iter (fun utest ->
    let test = utest.ToLowerInvariant()
    let nuspec = Path.getFullName ("./_Packaging/altcode.test." + test + ".nuspec")
    let fileroot = Path.getFullName ("./_Publish/" + test + "/lib")
    let chop = fileroot.Length
    let files = !!(fileroot + "/**/*")
                |> Seq.map (fun f -> (f, Some ("lib" + ((Path.GetDirectoryName f).Substring chop)), None))
                |> Seq.toList

    let output = Path.getFullName ("_Packaging." + test)          
    
    let workingDir = Path.getFullName ("./altcode.test/_Binaries/" + test)
    Directory.ensure workingDir
    Directory.ensure output
    NuGet (fun p ->
         { p with Authors = [ "Steve Gilham" ]
                  Description = "A named-argument helper wrapper for unit tests with " + utest
                  OutputPath = output
                  WorkingDir = workingDir
                  Files = files @ extras
                  Version = !Version
                  Copyright = (!Copyright).Replace("©", "(c)")
                  Publish = false
                  ReleaseNotes = Path.getFullName ("ReleaseNotes.md") |> File.ReadAllText
                  ToolPath =
                    if Environment.isWindows then
                      ("./packages/" + (packageVersion "NuGet.CommandLine")
                       + "/tools/NuGet.exe") |> Path.getFullName
                    else "/usr/bin/nuget" }) nuspec))

_Target "PrepareDotNetBuild" (fun _ ->
  let packaging = Path.getFullName "_Packaging"
  Directory.ensure packaging
  let publish = Path.getFullName "./_Publish"

  !! ("./altcode.test/_Binaries/*/Release+AnyCPU/*.nupkg")
  |> Seq.iter (fun f -> let name = Path.GetFileName f
                        let unpack = Path.Combine(publish, name.Split('.').[2])
                        System.IO.Compression.ZipFile.ExtractToDirectory(f, unpack))

  [ ("./_Publish/expecto/altcode.test.expecto.nuspec",
     "AltCode.Test.Expecto (Expecto helper)",
     false)
    ("./_Publish/nunit/altcode.test.nunit.nuspec",
     "AltCode.Test.NUnit (NUnit helper)",
     true)
    ("./_Publish/xunit/altcode.test.xunit.nuspec",
     "AltCode.Test.Xunit (Xunit helper)",
     true)
      ]
  |> List.iter (fun (path, caption, staticlink) ->
       printfn "Processing %A" path
       let x s = XName.Get(s, "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd")
       let dotnetNupkg = XDocument.Load path
       let desc = dotnetNupkg.Descendants(x "description") |> Seq.head
       "@description@" |> XText |> desc.ReplaceAll

       [ "authors"; "owners" ]
       |> List.iter (fun tag -> let title = dotnetNupkg.Descendants(x tag) |> Seq.head
                                title.ReplaceNodes "Steve Gilham")
       let id = dotnetNupkg.Descendants(x "id") |> Seq.head
       let title = XElement(x "title", caption)
       id.AddAfterSelf title
       let repo = dotnetNupkg.Descendants(x "repository") |> Seq.head
       [ 
         ("copyright", "@copyright@")
         ("releaseNotes", "@releaseNotes@")
         ("icon", "Icon_128x.png")
       ]
       |> Seq.iter (fun (a,b) -> let node = XElement(x a, b)
                                 repo.AddAfterSelf node)
       let meta = dotnetNupkg.Descendants(x "metadata") |> Seq.head
       "@files@" |> XText |> meta.AddAfterSelf

       dotnetNupkg.Descendants(x "dependency")
       |> Seq.filter (fun node -> printfn "%A -> %A" node node.Parent
                                  let id = node.Attribute(XName.Get "id").Value
                                  id = "altcode.test.common" ||
                                  (staticlink && 
                                   id = "FSharp.Core" && 
                                   node.Parent.Attribute(XName.Get "targetFramework").Value.StartsWith(".NETFramework", StringComparison.Ordinal)))
       |> Seq.toList
       |> List.iter (fun n -> printfn "nix %A" n
                              n.Remove())

       dotnetNupkg.Save ((Path.Combine(packaging, Path.GetFileName path)), SaveOptions.None)  ))

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