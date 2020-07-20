module MarkdownLinkChecker.Timing

open System.Diagnostics

let time f =
    let stopwatch = Stopwatch.StartNew()
    let result = f()
    stopwatch.Stop()
    result, stopwatch.Elapsed

let timeAsync f =
    async {
        let stopwatch = Stopwatch.StartNew()
        let! result = f()
        stopwatch.Stop()
        return result, stopwatch.Elapsed
    }
