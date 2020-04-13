module MarkdownLinkChecker.Documents

open System.IO
open Microsoft.Extensions.FileSystemGlobbing
open Microsoft.Extensions.FileSystemGlobbing.Abstractions

open Markdig
open Markdig.Syntax
open Markdig.Syntax.Inlines

open MarkdownLinkChecker.Logging

type Path =
    { Absolute: string
      Relative: string
      Directory: string }

type Location =
    { Line: int
      Column: int
      File: Path }
    
type Link =
    | FileLink of string * Location
    | UrlLink of string * Location
    
type LinkStatus =
    | Found
    | NotFound
    
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

let private linkReference (inlineLink: LinkInline): string =
    match Option.ofObj inlineLink.Reference with
    | Some reference -> reference.Url
    | None -> inlineLink.Url

let private linkLocation (file: Path) (inlineLink: LinkInline): Location =
    { Line = inlineLink.Line
      Column = inlineLink.Column
      File = file }
    
let private (|Url|File|) (url: string) =
    if url.StartsWith("http:") || url.StartsWith("https:") then Url else File

let private parseLink (file: Path) (inlineLink: LinkInline): Link =
    let reference = linkReference inlineLink
    let location = linkLocation file inlineLink

    match reference with
    | Url -> UrlLink(reference, location)
    | File -> FileLink(reference, location)

let parseLinks (file: Path) (markdown: string): Link list =
    Markdown.Parse(markdown).Descendants<LinkInline>()
    |> Seq.map (parseLink file)
    |> Seq.toList

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
        
