module MarkdownLinkChecker.Files

open System
open System.IO
open System.Runtime.InteropServices
open Microsoft.Extensions.FileSystemGlobbing
open Microsoft.Extensions.FileSystemGlobbing.Abstractions

open MarkdownLinkChecker.Options
open MarkdownLinkChecker.Timing

type FilePath =
    { Absolute: string
      Relative: string }

let private isMarkdownFile (path: string) =
    Path.GetExtension(path) = ".md"

let toFilePath (directory: string) (relativePath: string) =
    let directoryPath = Path.Combine(directory, relativePath)
    let absolutePath = Path.GetFullPath(directoryPath)
    let relativePath = Path.GetRelativePath(directory, directoryPath)

    { Absolute = absolutePath
      Relative = relativePath }

let private filesInDirectory (options: Options) =
    let matcher = Matcher().AddInclude("**/*.md")
    let root = DirectoryInfoWrapper(DirectoryInfo(options.Directory))
    let matchResults = matcher.Execute(root)

    matchResults.Files
    |> Seq.map (fun fileMatch -> fileMatch.Path)
    |> Seq.filter isMarkdownFile
    |> Seq.map (toFilePath options.Directory)
    
let private includedFiles (options: Options) =
    options.Files
    |> Seq.filter isMarkdownFile
    |> Seq.map (toFilePath options.Directory)
    
let private excludedFiles (options: Options) =
    options.Exclude
    |> Seq.filter isMarkdownFile
    |> Seq.map (toFilePath options.Directory)

let private checkAllFilesInDirectory (options: Options) =
    List.isEmpty options.Files

let private filterExcludedFiles (options: Options) files =
    let osSpecificStringComparison =
        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
            StringComparison.OrdinalIgnoreCase
        else
            StringComparison.Ordinal
    
    let isExcludedFile file =
        excludedFiles options
        |> Seq.exists (fun excludePath -> file.Absolute.StartsWith(excludePath.Absolute, osSpecificStringComparison))

    files
    |> Seq.filter (isExcludedFile >> not) 

let findFiles (options: Options): FilePath list =
    let files, elapsed = time (fun () ->
        let files = 
            if checkAllFilesInDirectory options then
                options.Logger.Log(sprintf "Finding Markdown files in directory %s ..." options.Directory)
                filesInDirectory options
            else
                options.Logger.Log("Finding specified Markdown files ...")
                includedFiles options

        files
        |> filterExcludedFiles options
        |> Seq.toList)

    options.Logger.Log(sprintf "Found %d files [%.1fms]" files.Length elapsed.TotalMilliseconds)
    files
