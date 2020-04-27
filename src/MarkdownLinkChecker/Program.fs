module MarkdownLinkChecker.Program

open MarkdownLinkChecker.Options
open MarkdownLinkChecker.Globbing
    
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

// TODO: add format script

[<EntryPoint>]
let main argv =
    match parseOptions argv with
    | ParseSuccess options ->
        logOptions options
        
        let files = findMarkdownFiles options
        
        int ExitCode.Ok
    | ParseFailure ->
        int ExitCode.Error 
