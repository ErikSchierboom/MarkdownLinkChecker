module MarkdownLinkChecker.Options

open MarkdownLinkChecker.Logging

open CommandLine

type Mode =
    | CheckAllLinks
    | CheckFileLinks
    | CheckUrlLinks
    with
        member this.CheckUrls = this = CheckAllLinks || this = CheckUrlLinks 
        member this.CheckFiles = this = CheckAllLinks || this = CheckFileLinks 

type Options =
    { Directory: string
      Files: string[]
      Exclude: string[]
      Logger: Logger
      Mode: Mode }

type CommandLineOptions =
    { [<Option('v', "verbosity", Required = false,
               HelpText = "Set the verbosity level. Allowed values are q[uiet], n[ormal] (default) and [d]etailed.")>]
      Verbosity: string option

      [<Option('d', "directory", Required = false,
               HelpText =
                   "The directory to operate on. Any relative file or directory paths specified in other options will be relative to this directory. If not specified, the working directory is used.")>]
      Directory: string option

      [<Option('e', "exclude", Required = false,
               HelpText = "A list of relative Markdown file or directory paths to exclude from checking.")>]
      Exclude: string seq

      [<Option('f', "files", Required = false,
               HelpText =
                   "A list of relative Markdown file or directory paths to checking. All Markdown files are checked if empty.")>]
      Files: string seq
      
      [<Option('m', "mode", Required = false,
               HelpText = "Which links to check. Allowed values are f[iles], u[rls] and a[ll] (default).")>]
      Mode: string option }

let private parseVerbosity (verbosity: string) =
    match verbosity.ToLower() with
    | "q"
    | "quiet" -> Quiet
    | "d"
    | "detailed" -> Detailed
    | _ -> Normal

let private parseMode (mode: string) =
    match mode.ToLower() with
    | "f"
    | "files" -> CheckFileLinks
    | "u"
    | "urls" -> CheckUrlLinks
    | _ -> CheckAllLinks

let private fromCommandLineOptions (options: CommandLineOptions) =
    { Files = Array.ofSeq options.Files
      Exclude = Array.ofSeq options.Exclude
      Directory = options.Directory |> Option.defaultWith System.IO.Directory.GetCurrentDirectory
      Logger =
          options.Verbosity
          |> Option.map parseVerbosity
          |> Option.defaultValue Normal
          |> Logger
      Mode =
          options.Mode
          |> Option.map parseMode
          |> Option.defaultValue CheckAllLinks }

let (|ParseSuccess|ParseFailure|) (result: ParserResult<CommandLineOptions>) =
    match result with
    | :? (Parsed<CommandLineOptions>) as parsedOptions ->
        let options = fromCommandLineOptions parsedOptions.Value
        ParseSuccess options
    | _ -> ParseFailure

let parseOptions argv = CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(argv)
