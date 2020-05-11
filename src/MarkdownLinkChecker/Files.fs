module MarkdownLinkChecker.Files

open System
open System.IO
open Microsoft.Extensions.FileSystemGlobbing
open Microsoft.Extensions.FileSystemGlobbing.Abstractions

open MarkdownLinkChecker.Options
open MarkdownLinkChecker.Logging
open MarkdownLinkChecker.Timing

type File = File of string

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
    
let private toFile (path: string) =
    if isMarkdownFile path then
        Some (File path) 
    else
        None

let private filesInDirectory (options: Options) =
    let matcher = Matcher().AddInclude("**/*.md")
    let root = DirectoryInfoWrapper(DirectoryInfo(options.Directory))
    let matchResults = matcher.Execute(root)

    matchResults.Files
    |> Seq.map (fun fileMatch -> Path.getFullPath options fileMatch.Path)
    |> Seq.choose toFile
    
let private includedFiles (options: Options) =
    options.Files
    |> Seq.map (Path.getFullPath options)
    |> Seq.choose toFile
    
let private excludedFiles (options: Options) =
    options.Exclude
    |> Seq.map (Path.getFullPath options)
    |> Seq.choose toFile

let private checkAllFilesInDirectory (options: Options) =
    List.isEmpty options.Files

let private filterExcludedFiles (options: Options) files =
    let isExcludedFile (File path) =
        excludedFiles options
        |> Seq.exists (fun (File excludePath) -> path.StartsWith(excludePath, Path.stringComparison))

    files
    |> Seq.filter (isExcludedFile >> not) 

let private logBefore (options: Options) =
    if checkAllFilesInDirectory options then                
        options.Logger.Normal("Checking Markdown files in directory...")
    else                
        options.Logger.Normal("Checking specified Markdown files...")

let private logAfter (options: Options) (files: File list) (elapsed: TimeSpan) =
    options.Logger.Normal(sprintf "Found %d files [%.0fms]" (List.length files) elapsed.TotalMilliseconds)
    options.Logger.Detailed("Files found:")
    List.iter (fun (File file) -> options.Logger.Detailed(sprintf "- %s" file)) files

let findFiles (options: Options): File list =
    let files, elapsed = time (fun () ->
        logBefore options
        
        let files = 
            if checkAllFilesInDirectory options then                
                filesInDirectory options
            else                
                includedFiles options
        
        files
        |> filterExcludedFiles options
        |> Seq.toList)

    logAfter options files elapsed
    files
        