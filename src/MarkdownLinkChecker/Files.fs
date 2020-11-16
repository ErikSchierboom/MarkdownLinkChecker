module MarkdownLinkChecker.Files

open System
open System.IO
open System.Runtime.InteropServices
open Microsoft.Extensions.FileSystemGlobbing
open Microsoft.Extensions.FileSystemGlobbing.Abstractions

open MarkdownLinkChecker.Options

type FilePath = { Absolute: string; Relative: string }

let toFilePath (directory: string) (relativePath: string) =
    let platformSpecificRelativePath = relativePath.Replace('/', Path.DirectorySeparatorChar)

    let directoryPath = Path.Combine(directory, platformSpecificRelativePath)
    let absolutePath = Path.GetFullPath(directoryPath)

    let relativePath = Path.GetRelativePath(directory, directoryPath)

    { Absolute = absolutePath
      Relative = relativePath }

let private isMarkdownFile (path: string) = Path.GetExtension(path) = ".md"

let private toMarkdownFilePath (directory: string) (relativePath: string) =
    if isMarkdownFile relativePath then Some(toFilePath directory relativePath) else None

let private filesInDirectory (options: Options) =
    let matcher = Matcher().AddInclude("**/*.md")

    let root =
        DirectoryInfoWrapper(DirectoryInfo(options.Directory))

    let matchResults = matcher.Execute(root)

    matchResults.Files
    |> Seq.map (fun fileMatch -> fileMatch.Path)
    |> Seq.choose (toMarkdownFilePath options.Directory)

let private includedFiles (options: Options) =
    Seq.choose (toMarkdownFilePath options.Directory) options.Files

let private excludedFiles (options: Options) =
    Seq.choose (toMarkdownFilePath options.Directory) options.Exclude

let private checkAllFilesInDirectory (options: Options) = Array.isEmpty options.Files

let private filterExcludedFiles (options: Options) files =
    let osSpecificStringComparison =
        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        then StringComparison.OrdinalIgnoreCase
        else StringComparison.Ordinal
    
    let isExcludedFile file =
        excludedFiles options
        |> Seq.exists (fun excludePath -> file.Absolute.StartsWith(excludePath.Absolute, osSpecificStringComparison))

    Seq.filter (isExcludedFile >> not) files

let findFiles (options: Options): FilePath seq =
    let files =
        if checkAllFilesInDirectory options then filesInDirectory options else includedFiles options

    filterExcludedFiles options files
