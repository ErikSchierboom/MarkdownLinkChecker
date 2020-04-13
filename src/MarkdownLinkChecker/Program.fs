module MarkdownLinkChecker.Program

open MarkdownLinkChecker.Options
open MarkdownLinkChecker.Globbing
    
type ExitCode =
    | Ok = 0
    | Error = 1

let private logOptions options =
    options.Logger.Normal("Running Markdown link checker using:")
    options.Logger.Normal(sprintf "Directory: %s" options.Directory)    
    options.Logger.Normal(sprintf "Include pattern: %s" options.Include)    
    options.Logger.Normal(sprintf "Exclude pattern: %s" (options.Exclude |> Option.defaultValue "<not specified>"))

// TODO: add format script

[<EntryPoint>]
let main argv =
    match parseOptions argv with
    | ParseSuccess options ->
        logOptions options
        
        let files = findFiles options
        
        int ExitCode.Ok
    | ParseFailure ->
        int ExitCode.Error 
