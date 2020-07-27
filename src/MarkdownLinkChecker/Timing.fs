module MarkdownLinkChecker.Timing

open System
open System.Diagnostics

type Timed<'T> = Timed of 'T * TimeSpan

let time f =
    async {
        let stopwatch = Stopwatch.StartNew()
        let! result = f()
        stopwatch.Stop()
        return Timed(result, stopwatch.Elapsed)
    }
