module MarkdownLinkChecker.Options

open MarkdownLinkChecker.Logging

open CommandLine

type Options =
    { Directory: string
      Files: string list
      Exclude: string list
      Logger: Logger }

type CommandLineOptions =
    { [<Option('v', "verbosity", Required = false,
               HelpText = "Set the verbosity level. Allowed values are q[uiet] and n[ormal] (default).")>]
      Verbosity: string option

      [<Option('d', "directory", Required = false,
               HelpText =
                   "The directory to operate on. Any relative file or directory paths specified in other options will be relative to this directory. If not specified, the working directory is used.")>]
      Directory: string option

      [<Option('e', "exclude", Required = false,
               HelpText = "A list of relative Markdown file or directory paths to exclude from formatting.")>]
      Exclude: string seq

      [<Option('f', "files", Required = false,
               HelpText =
                   "A list of relative Markdown file or directory paths to include in formatting. All Markdown files are formatted if empty.")>]
      Files: string seq }

let private parseVerbosity (verbosity: string) =
    match verbosity.ToLower() with
    | "q"
    | "quiet" -> Quiet
    | "n"
    | "normal" -> Normal
    | _ -> Normal

let private fromCommandLineOptions (options: CommandLineOptions) =
    { Files = options.Files |> List.ofSeq
      Exclude = options.Exclude |> List.ofSeq
      Directory = options.Directory |> Option.defaultWith System.IO.Directory.GetCurrentDirectory
      Logger =
          options.Verbosity
          |> Option.map parseVerbosity
          |> Option.defaultValue Normal
          |> Logger }

let (|ParseSuccess|ParseFailure|) (result: ParserResult<CommandLineOptions>) =
    match result with
    | :? (Parsed<CommandLineOptions>) as parsedOptions ->
        let options = fromCommandLineOptions parsedOptions.Value
        ParseSuccess options
    | _ -> ParseFailure

let parseOptions argv = CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(argv)
