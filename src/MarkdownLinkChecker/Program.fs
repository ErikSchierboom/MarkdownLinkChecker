﻿module MarkdownLinkChecker.Program

open MarkdownLinkChecker.Options
open MarkdownLinkChecker.Files
open MarkdownLinkChecker.Parser
open MarkdownLinkChecker.Checker

type ExitCode =
    | Ok = 0
    | Error = 1

// TODO: output errors
// TODO: cleanup empty lines in loggin output
// TODO: add instructions to README
// TODO: create computation expression for timing

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
