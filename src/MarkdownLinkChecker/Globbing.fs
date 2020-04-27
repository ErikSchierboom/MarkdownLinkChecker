module MarkdownLinkChecker.Globbing

open System
open System.IO
open Microsoft.Extensions.FileSystemGlobbing
open Microsoft.Extensions.FileSystemGlobbing.Abstractions

open MarkdownLinkChecker.Options

type MarkdownFile = MarkdownFile of string

module Path =
    open System.Runtime.InteropServices
    
    let getFullPath (options: Options) (path: string) =
        Path.Combine(options.Directory, path) |> Path.GetFullPath
        
    let stringComparison =
        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
            StringComparison.OrdinalIgnoreCase
        else
            StringComparison.Ordinal

let private isMarkdownFile (path: string) =
    Path.GetExtension(path) = ".md"
    
let private toMarkdownFile (path: string) =
    if isMarkdownFile path then
        Some (MarkdownFile path) 
    else
        None

let private markdownFilesInDirectory (options: Options) =
    let matcher = Matcher().AddInclude("**/*.md")
    let root = DirectoryInfoWrapper(DirectoryInfo(options.Directory))
    let matchResults = matcher.Execute(root)

    matchResults.Files
    |> Seq.map (fun fileMatch -> Path.getFullPath options fileMatch.Path)
    |> Seq.choose toMarkdownFile
    
let private markdownFilesAsIncluded (options: Options) =
    options.Files
    |> Seq.map (Path.getFullPath options)
    |> Seq.choose toMarkdownFile
    
let private markdownFilesAsExcluded (options: Options) =
    options.Exclude
    |> Seq.map (Path.getFullPath options)
    |> Seq.choose toMarkdownFile

let private checkAllMarkdownFilesInDirectory (options: Options) =
    List.isEmpty options.Files

let private filterExcludedMarkdownFiles (options: Options) markdownFiles =
    let isExcludedFile (MarkdownFile path) =
        markdownFilesAsExcluded options
        |> Seq.exists (fun (MarkdownFile excludePath) -> path.StartsWith(excludePath, Path.stringComparison))

    markdownFiles
    |> Seq.filter (isExcludedFile >> not) 

let findMarkdownFiles (options: Options): MarkdownFile list =
    let markdownFiles = 
        if checkAllMarkdownFilesInDirectory options then
            options.Logger.Normal("Checking Markdown files in directory")
            markdownFilesInDirectory options
        else
            options.Logger.Normal("Checking specified Markdown files")
            markdownFilesAsIncluded options
    
    markdownFiles
    |> filterExcludedMarkdownFiles options
    |> Seq.toList
        