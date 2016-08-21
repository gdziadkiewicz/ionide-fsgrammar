//---------------------------------------------------------//
// Ionide-FsGrammar - FSharp.SyntaxTools FAKE Build Script //
//---------------------------------------------------------//

// This build script is run inside VSCode when `F5` is used to Launch the Extension

#r @"packages/FAKE/tools/FakeLib.dll"

open System
open Fake
open Fake.ProcessHelper

System.IO.Directory.SetCurrentDirectory __SOURCE_DIRECTORY__

/// Stores F#, Paket, FsLex & FsYacc Syntax Definition Files  
let syntaxDir           = "grammar"
/// Location of the FSharp-SyntaxTest VSCode Extension    
let extensionDir        = "fsharp.syntaxtest"
/// FSharp-SyntaxTest will load the Syntax Definition files in this dir    
let extensionSyntaxDir  = extensionDir</>"syntaxes"
/// this files is created as a flag upon successful npm installation,
/// to prevent it from re-running every build
let setupcomplete = "packages"</>"built.md" 


let platformTool tool path =
    isUnix |> function | true -> tool | _ -> path

/// Node Package Manager
let npmTool =    
    platformTool "npm" ("packages"</>"Npm.js"</>"tools"</>"npm.cmd"|>FullName)

let run cmd args workingDir =
    match  execProcess (fun info ->
        if not (String.IsNullOrWhiteSpace workingDir) then 
            info.WorkingDirectory <- workingDir
        info.FileName <- cmd    
        info.Arguments <- args
        ) System.TimeSpan.MaxValue with
    | false -> traceError <| sprintf "Error while running '%s' with args: %s" cmd args; false
    | true -> true


let CopyGrammar & cg = "CopyGrammar" 
Target cg  (fun _ ->
    trace "Copying F# Syntax Definition Files\n"
    ensureDirectory extensionSyntaxDir
    CleanDir extensionSyntaxDir
    CopyFiles extensionSyntaxDir [
        syntaxDir</>"fsharp.fsi.json"
        syntaxDir</>"fsharp.fsl.json"
        syntaxDir</>"fsharp.fsx.json"
        syntaxDir</>"fsharp.json"
    ]
)


let Packages & dep = "Packages" 
Target dep  (fun () ->
    trace "Installing FSharp.SyntaxTools npm packages"
    // install any necessary dependencies for the FSharp-SyntaxTest Extension
    match run npmTool "install --verbose" extensionDir with
    | false -> DeleteFile setupcomplete
    | true  -> CreateFile setupcomplete
)

let FableCompile & fc = "FableCompile" 
Target fc  (fun () ->
    trace "Transpile FSharp.SyntaxTools to Javascript"
    run npmTool "run-script build" extensionDir |> ignore 
)



/// Visual Studio Code
let codeTool =
    platformTool "code" (ProgramFilesX86</>"Microsoft VS Code"</>"bin/code.cmd")

let needsInstall = not <| fileExists setupcomplete

CopyGrammar 
 ==> Packages
 =?> (FableCompile, needsInstall) 

CopyGrammar 
 ==> FableCompile

RunTargetOrDefault FableCompile 
