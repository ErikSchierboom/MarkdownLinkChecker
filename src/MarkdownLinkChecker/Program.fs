module MarkdownLinkChecker.Program

open System.Diagnostics

open MarkdownLinkChecker.Options
open MarkdownLinkChecker.Files
open MarkdownLinkChecker.Parser
open MarkdownLinkChecker.Checker
    
type ExitCode =
    | Ok = 0
    | Error = 1

let private logOptions options =
    let logFilesOption files = if List.isEmpty files then "(not specified)" else String.concat ", " files
    
    options.Logger.Normal("Running Markdown link checker using:")
    options.Logger.Normal(sprintf "Verbosity: %A" options.Logger.Verbosity)    
    options.Logger.Normal(sprintf "Directory: %s" options.Directory)    
    options.Logger.Normal(sprintf "Files: %s" (logFilesOption options.Files))
    options.Logger.Normal(sprintf "Exclude: %s" (logFilesOption options.Exclude))
    options.Logger.Normal("")

// TODO: add format script

[<EntryPoint>]
let main argv =
    let stopwatch = Stopwatch.StartNew();
    
    match parseOptions argv with
    | ParseSuccess options ->
        logOptions options
        
        let files = findFiles options
        let documents = parseDocuments files
        let status = checkDocuments documents

        match status with
        | Valid -> int ExitCode.Ok
        | Invalid -> int ExitCode.Error
    | ParseFailure ->
        int ExitCode.Error 
