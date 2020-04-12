module MarkdownLinkChecker.Program

open CommandLine
open MarkdownLinkChecker.Documents
open MarkdownLinkChecker.Checker

type Options =
    { [<Option('e', "exclude", Required = false, HelpText = "Files to exclude")>]
      Exclude: string option
      [<Value(0, Required = false, Default = "**/*.md", HelpText = "Files to check")>]
      Include: string }

type ExitCode =
    | Success
    | Error
    
let private toExitCode exitCode =
    match exitCode with
    | Success -> 0
    | Error -> 1

let private onSuccess (options: Options) =
    let findOptions: FindOptions =
        { Include = options.Include
          Exclude = options.Exclude }

    let (CheckedDocuments checkedDocuments) =
        findOptions
        |> findUncheckedDocuments
        |> checkDocuments

    let anyInvalidDocuments = checkedDocuments |> List.exists (fun doc -> doc.Status = Invalid)
    if anyInvalidDocuments then ExitCode.Error else ExitCode.Success 

let private onFailure = ExitCode.Error

let private (|ParseSuccess|ParseFailure|) (result: ParserResult<Options>) =
    match result with
    | :? (Parsed<Options>) as options -> ParseSuccess options.Value
    | _ -> ParseFailure

let private parse argv = CommandLine.Parser.Default.ParseArguments<Options>(argv)

[<EntryPoint>]
let main argv =
    match parse argv with
    | ParseSuccess options -> onSuccess options |> toExitCode
    | ParseFailure -> onFailure |> toExitCode
