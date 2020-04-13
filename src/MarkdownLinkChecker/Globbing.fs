module MarkdownLinkChecker.Globbing

open System.IO
open Microsoft.Extensions.FileSystemGlobbing
open Microsoft.Extensions.FileSystemGlobbing.Abstractions

open MarkdownLinkChecker.Options

type File =
    | MarkdownFile of string
    | NonMarkdownFile of string

let private toFile (options: Options) (fileMatch: FilePatternMatch) =
    let path = Path.Combine(options.Directory, fileMatch.Path)
    let extension = Path.GetExtension(fileMatch.Path).ToLower()    
    if extension = ".md" then MarkdownFile path else NonMarkdownFile path

let private createMatcher (options: Options) =
    Matcher()
        .AddInclude(options.Include)
        .AddExclude(options.Exclude |> Option.defaultValue "")
    
let findFiles (options: Options): File list =
    let matcher = createMatcher options
    let root = DirectoryInfoWrapper(DirectoryInfo(options.Directory))
    let matchResults = matcher.Execute(root)

    matchResults.Files
    |> Seq.map (toFile options)
    |> Seq.toList
        