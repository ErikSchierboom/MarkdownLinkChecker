module MarkdownLinkChecker.Logging

type Verbosity =
    | Quiet
    | Normal
    | Detailed

type Logger(verbosity: Verbosity) =
    let log supportedLevels (message: string) =
        if List.contains verbosity supportedLevels then
            printfn "%s" message
    
    member __.Normal = log [Normal; Detailed]
            
    member __.Detailed = log [Detailed]
    
    member __.Verbosity = verbosity
    