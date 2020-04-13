module MarkdownLinkChecker.Options

open MarkdownLinkChecker.Logging

open CommandLine

type Options =
    { Exclude: string option
      Include: string
      Directory: string
      Logger: Logger }

type CommandLineOptions =
    { [<Option('v', "verbosity", Required = false, Default = "normal", HelpText = "Set the verbosity")>]
      Verbosity: string
      
      [<Option('d', "directory", Required = false, HelpText = "Directory to search in")>]
      Directory: string option

      [<Option('e', "exclude", Required = false, HelpText = "Files to exclude")>]
      Exclude: string option
      
      [<Value(0, Required = false, Default = "**/*.md", HelpText = "Files to check")>]
      Include: string }
    
let private parseVerbosity (verbosity: string) =
    match verbosity.ToLower() with
    | "q" | "quiet" -> Quiet
    | "m" | "minimal" -> Minimal
    | "n" | "normal" -> Normal
    | "d" | "detailed" -> Detailed
    | "g" | "diagnostic" -> Diagnostic
    | _ -> Normal
    
let private fromCommandLineOptions (options: CommandLineOptions) =
    { Include = options.Include
      Exclude = options.Exclude
      Directory = options.Directory |> Option.defaultWith System.IO.Directory.GetCurrentDirectory
      Logger = Logger(parseVerbosity options.Verbosity) }

let (|ParseSuccess|ParseFailure|) (result: ParserResult<CommandLineOptions>) =
    match result with
    | :? (Parsed<CommandLineOptions>) as options -> ParseSuccess (fromCommandLineOptions options.Value) 
    | _ -> ParseFailure

let parseOptions argv = CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(argv)