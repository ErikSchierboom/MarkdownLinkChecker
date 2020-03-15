module MarkdownLinkChecker.Documents

open System.IO
open Microsoft.Extensions.FileSystemGlobbing
open Microsoft.Extensions.FileSystemGlobbing.Abstractions

open Markdig
open Markdig.Syntax
open Markdig.Syntax.Inlines

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

type DocumentStatus =
    | Valid
    | Invalid

type CheckedLink =
    { Link: Link
      Status: LinkStatus }
    
type CheckedDocument =
    { Path: Path
      Links: CheckedLink list
      Status: DocumentStatus }

type UncheckedDocument =
    { Path: Path
      Links: Link list }

type UncheckedDocuments = UncheckedDocuments of UncheckedDocument list

type CheckedDocuments = CheckedDocuments of CheckedDocument list

type FindOptions =
    { Exclude: string option
      Include: string }

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

let private toPath (fileMatch: FilePatternMatch) =
    { Absolute = fileMatch.Path
      Relative = fileMatch.Stem
      Directory = Path.GetDirectoryName(fileMatch.Path) }

let private toUncheckedDocument (fileMatch: FilePatternMatch) =
    let path = toPath fileMatch
    let extension = Path.GetExtension(path.Absolute).ToLower() 
    let links = if extension = ".md" then parseLinks path (File.ReadAllText(path.Absolute)) else []
    { Path = path; Links = links }

let findUncheckedDocuments options =
    let matcher =
        Matcher()
            .AddInclude(options.Include)
            .AddExclude(options.Exclude |> Option.defaultValue "")

    let root = DirectoryInfoWrapper(DirectoryInfo(Directory.GetCurrentDirectory()))
    let matchResults = matcher.Execute(root)

    matchResults.Files
    |> Seq.map toUncheckedDocument
    |> Seq.toList
    |> UncheckedDocuments