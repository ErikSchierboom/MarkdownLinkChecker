module MarkdownLinkChecker.Logging

type Verbosity =
    | Quiet
    | Normal

type Logger(verbosity: Verbosity) =
    
    do 
        System.Console.OutputEncoding <- System.Text.Encoding.UTF8

    member __.Normal(message) =
        if verbosity = Normal || verbosity = Detailed then printfn "%s" message
        
    member __.Detailed(message) =
        if verbosity = Detailed then printfn "%s" message

    member __.Verbosity = verbosity
