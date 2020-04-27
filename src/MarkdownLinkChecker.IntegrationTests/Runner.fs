module MarkdownLinkChecker.IntegrationTests.Runner

open System
open System.IO

open MarkdownLinkChecker.Program

type CheckResults =
    { ExitCode: int
      Output: string }

let run argv =
    use stringWriter = new StringWriter()
    Console.SetOut(stringWriter)
    Console.SetError(stringWriter)
    
    { ExitCode = main argv
      Output = stringWriter.ToString() }
    
let runOnSingleFile file = run [| "--files"; file |]

let runOnMultipleFiles files = run (Array.append [| "--files" |] files)
    
let runOnDirectory directory = run [| "--directory"; directory |]
    