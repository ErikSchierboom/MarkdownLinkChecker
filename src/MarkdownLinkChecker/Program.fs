module MarkdownLinkChecker.Program

open MarkdownLinkChecker.Options
open MarkdownLinkChecker.Files
open MarkdownLinkChecker.Parser
open MarkdownLinkChecker.Checker

type ExitCode =
    | Ok = 0
    | Error = 1

// TODO: output errors
// TODO: add instructions to README
// TODO: create computation expression for timing
// TODO: check if status icons work on windows
// TODO: borrow output from https://glebbahmutov.com/blog/check-markdown-links/

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
