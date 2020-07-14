module MarkdownLinkChecker.Timing

open System.Diagnostics

let time f =
    let stopwatch = Stopwatch.StartNew()
    let result = f()
    result, stopwatch.Elapsed
