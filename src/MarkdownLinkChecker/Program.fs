module MarkdownLinkChecker.Program

open CommandLine
open MarkdownLinkChecker.Documents
open MarkdownLinkChecker.Checker

type Options =
    { [<Option('e', "exclude", Required = false, HelpText = "Files to exclude")>]
      Exclude: string option
      [<Value(0, Required = false, Default = "**/*.md", HelpText = "Files to check")>]
      Include: string }

let private parseSuccess (options: Options) =
    let findOptions: FindOptions =
        { Include = options.Include
          Exclude = options.Exclude }
    
    let (CheckedDocuments checkedDocuments) =
        findOptions
        |> findUncheckedDocuments
        |> checkDocuments

    if checkedDocuments |> List.exists (fun doc -> doc.Status = Invalid) then 1 else 0

let private parseFailure = 1

let private (|Success|Failure|) (result: ParserResult<Options>) =
    match result with
    | :? (Parsed<Options>) as options -> Success options.Value
    | _ -> Failure
    
let private parse argv = CommandLine.Parser.Default.ParseArguments<Options>(argv)

[<EntryPoint>]
let main argv =
    match parse argv with
    | Success options -> parseSuccess options
    | Failure -> parseFailure
