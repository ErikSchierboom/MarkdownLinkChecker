module MarkdownLinkChecker.Logging

type Verbosity =
    | Quiet
    | Minimal
    | Normal
    | Detailed
    | Diagnostic

type Logger(verbosity: Verbosity) =
    let log supportedLevels (message: string) =
        if List.contains verbosity supportedLevels then
            printfn "%s" message
    
    member __.Minimal = log [Minimal; Normal; Detailed; Diagnostic]

    member __.Normal = log [Normal; Detailed; Diagnostic]
            
    member __.Detailed = log [Detailed; Diagnostic]
            
    member __.Diagnostic = log [Diagnostic]