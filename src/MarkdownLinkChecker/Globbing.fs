module MarkdownLinkChecker.Globbing

open System
open System.IO
open System.Runtime.InteropServices
open Microsoft.Extensions.FileSystemGlobbing
open Microsoft.Extensions.FileSystemGlobbing.Abstractions

open MarkdownLinkChecker.Options

type MarkdownFile = MarkdownFile of string

let private isMarkdownFile (path: string) =
    Path.GetExtension(path) = ".md"

let private toFullPath (options: Options) (path: string) =
    Path.Combine(options.Directory, path)

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
    |> Seq.map (fun fileMatch -> toFullPath options fileMatch.Path)
    |> Seq.choose toMarkdownFile
    
let private markdownFilesAsIncluded (options: Options) =
    options.Files
    |> Seq.map (toFullPath options)
    |> Seq.choose toMarkdownFile

let private checkAllMarkdownFilesInDirectory (options: Options) =
    List.isEmpty options.Files

let private pathComparison =
    if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
        StringComparison.OrdinalIgnoreCase
    else
        StringComparison.Ordinal

let private filterExcludedMarkdownFiles (options: Options) markdownFiles =
    let excludedFilePaths =
        options.Exclude
        |> Seq.map (toFullPath options)
        |> Seq.toList
        
    let isExcludedFile (MarkdownFile path) =
        excludedFilePaths
        |> Seq.exists (fun excludePath -> path.Equals(excludePath, pathComparison))
    
    markdownFiles
    |> Seq.filter (isExcludedFile >> not) 

let findMarkdownFiles (options: Options): MarkdownFile list =
    let markdownFiles = 
        if checkAllMarkdownFilesInDirectory options then
            markdownFilesInDirectory options
        else
            markdownFilesAsIncluded options
    
    markdownFiles
    |> filterExcludedMarkdownFiles options
    |> Seq.toList
        