module MarkdownLinkChecker.Program

open MarkdownLinkChecker.Options
open MarkdownLinkChecker.Files
open MarkdownLinkChecker.Parser
open MarkdownLinkChecker.Checker

type ExitCode =
    | Ok = 0
    | Error = 1

// TODO: add instructions to README
// TODO: check links in parallel
// TODO: do performance analysis (e.g. using Seq/Array/List)

[<EntryPoint>]
let main argv =
    match parseOptions argv with
    | ParseSuccess options ->
        let status =
            findFiles options
            |> parseDocuments options
            |> checkDocuments options

        match status with
        | Valid -> int ExitCode.Ok
        | Invalid -> int ExitCode.Error
    | ParseFailure -> int ExitCode.Error
