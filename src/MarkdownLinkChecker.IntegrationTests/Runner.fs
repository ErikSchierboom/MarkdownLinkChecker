module MarkdownLinkChecker.IntegrationTests.Runner

open System
open System.IO
open System.Reflection

open MarkdownLinkChecker.Program
open Xunit
open Xunit.Sdk

type CheckResults =
    { ExitCode: int
      Output: string }
    
[<AttributeUsage(AttributeTargets.Class ||| AttributeTargets.Method, AllowMultiple = false, Inherited = true)>]
type ExecuteInDirectory(directory) =
    inherit BeforeAfterTestAttribute()

    let mutable currentDirectory = ""
    
    override _.Before(_) =
        currentDirectory <- Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        Directory.SetCurrentDirectory(Path.Combine(currentDirectory, directory))

    override _.After(_) =
        Directory.SetCurrentDirectory(currentDirectory)

// Required due to some tests temporarily setting the current directory
[<assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)>]
do ()

let run argv =
    use stringWriter = new StringWriter()
    Console.SetOut(stringWriter)
    Console.SetError(stringWriter)
    
    { ExitCode = main argv
      Output = stringWriter.ToString() }
    
let runOnSingleFile file = run [| "--files"; file |]

let runOnMultipleFiles files = run (Array.append [| "--files" |] files)
    
let runOnDirectory directory = run [| "--directory"; directory |]
