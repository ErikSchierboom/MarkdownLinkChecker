module MarkdownLinkChecker.Logging

type Verbosity =
    | Quiet
    | Normal

type Logger(verbosity: Verbosity) =

    member __.Log(message) =
        if verbosity = Normal then printfn "%s" message

    member __.Verbosity = verbosity
