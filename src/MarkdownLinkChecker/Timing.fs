module MarkdownLinkChecker.Timing

open System
open System.Diagnostics

type Timed<'T> = Timed of 'T * TimeSpan

type TimedBuilder() =
    member x.Return(value) = Timed(value, TimeSpan.Zero)

    member x.Delay(func) =
        let stopwatch = Stopwatch.StartNew()
        let delayedResult = func()
        stopwatch.Stop()
        
        match delayedResult with
        | Timed(value, elapsed) -> Timed(value, elapsed + stopwatch.Elapsed)
    
let timed = TimedBuilder()
