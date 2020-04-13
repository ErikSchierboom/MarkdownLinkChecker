module MarkdownLinkChecker.Program

open CommandLine
open MarkdownLinkChecker.Logging
open MarkdownLinkChecker.Documents
open MarkdownLinkChecker.Checker

type CommandLineOptions =
    { [<Option('v', "verbosity", Required = false, Default = "normal", HelpText = "Set the verbosity")>]
      Verbosity: string
      
      [<Option('d', "directory", Required = false, HelpText = "Directory to search in")>]
      Directory: string option

      [<Option('e', "exclude", Required = false, HelpText = "Files to exclude")>]
      Exclude: string option
      
      [<Value(0, Required = false, Default = "**/*.md", HelpText = "Files to check")>]
      Include: string }
    
type ExitCode =
    | Ok
    | Error
    
let private parseVerbosity (verbosity: string) =
    match verbosity.ToLower() with
    | "q" | "quiet" -> Quiet
    | "m" | "minimal" -> Minimal
    | "n" | "normal" -> Normal
    | "d" | "detailed" -> Detailed
    | "g" | "diagnostic" -> Diagnostic
    | _ -> Normal
        
let exitCodeToInt exitCode =
    match exitCode with
    | Ok -> 0
    | Error -> 1
    
let private toContext (options: CommandLineOptions) =
    { Include = options.Include
      Exclude = options.Exclude
      Directory = options.Directory |> Option.defaultWith System.IO.Directory.GetCurrentDirectory
      Logger = Logger(parseVerbosity options.Verbosity) }

let private logContext context =
    context.Logger.Normal("Running Markdown link checker using:")
    context.Logger.Normal(sprintf "Directory: %s" context.Directory)    
    context.Logger.Normal(sprintf "Include pattern: %s" context.Include)    
    context.Logger.Normal(sprintf "Exclude pattern: %s" (context.Exclude |> Option.defaultValue "<not specified>"))

let private logUncheckedDocuments context (uncheckedDocuments: UncheckedDocuments) =
    ()
    
let private logCheckedDocuments context (checkedDocuments: CheckedDocuments) =
    ()

let private onSuccess options =
    let context = toContext options
    logContext context
    
    let uncheckedDocuments = findUncheckedDocuments context
    logUncheckedDocuments context uncheckedDocuments
    
    let checkedDocuments = checkDocuments context uncheckedDocuments
    logCheckedDocuments context checkedDocuments

    if checkedDocuments.Status = Valid then ExitCode.Ok else ExitCode.Error 

let private onFailure = ExitCode.Error

let private (|Success|Failure|) (result: ParserResult<CommandLineOptions>) =
    match result with
    | :? (Parsed<CommandLineOptions>) as options -> Success options.Value
    | _ -> Failure

let private parse argv = CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(argv)

[<EntryPoint>]
let main argv =
    match parse argv with
    | Success options -> onSuccess options |> exitCodeToInt
    | Failure -> onFailure |> exitCodeToInt
