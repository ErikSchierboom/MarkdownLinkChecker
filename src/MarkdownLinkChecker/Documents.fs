module MarkdownLinkChecker.Documents

open System.IO
open Microsoft.Extensions.FileSystemGlobbing
open Microsoft.Extensions.FileSystemGlobbing.Abstractions

open MarkdownLinkChecker.Logging

type Path =
    { Absolute: string
      Relative: string
      Directory: string }

type File =
    | MarkdownFile of Path
    | NonMarkdownFile of Path
    
type Status =
    | Valid
    | Invalid

type UncheckedDocument =
    { Path: Path
      Links: Link list }

type UncheckedDocuments = UncheckedDocuments of UncheckedDocument list

type CheckedLink =
    { Link: Link
      Status: LinkStatus }
    
type CheckedDocument =
    { Path: Path
      Links: CheckedLink list
      Status: Status }

type CheckedDocuments =
    { Documents: CheckedDocument list
      Status: Status }

type CheckerContext =
    { Exclude: string option
      Include: string
      Directory: string
      Logger: Logger }


let private toPath context (fileMatch: FilePatternMatch) =
    { Absolute = Path.Combine(context.Directory, fileMatch.Path)
      Relative = fileMatch.Stem
      Directory = Path.GetDirectoryName(Path.Combine(context.Directory, fileMatch.Path)) }

let private toUncheckedDocument context (fileMatch: FilePatternMatch) =
    let path = toPath context fileMatch
    let extension = Path.GetExtension(path.Absolute).ToLower() 
    if extension = ".md" then
        Some { Path = path; Links = parseLinks path (File.ReadAllText(path.Absolute)) }
    else
        None

let findUncheckedDocuments context =
    let matcher =
        Matcher()
            .AddInclude(context.Include)
            .AddExclude(context.Exclude |> Option.defaultValue "")

    let root = DirectoryInfoWrapper(DirectoryInfo(context.Directory))
    let matchResults = matcher.Execute(root)

    matchResults.Files
    |> Seq.choose (toUncheckedDocument context)
    |> Seq.toList
    |> UncheckedDocuments
        