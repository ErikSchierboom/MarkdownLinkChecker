module MarkdownLinkChecker.Parser

open MarkdownLinkChecker.Globbing

open Markdig
open Markdig.Syntax
open Markdig.Syntax.Inlines

type Location =
    { Line: int
      Column: int }
    
type Link =
    | FileLink of path: string * location: Location
    | UrlLink of url: string * location: Location

let private linkReference (inlineLink: LinkInline): string =
    match Option.ofObj inlineLink.Reference with
    | Some reference -> reference.Url
    | None -> inlineLink.Url

let private linkLocation (inlineLink: LinkInline): Location =
    { Line = inlineLink.Line
      Column = inlineLink.Column }
    
let private (|Url|File|) (reference: string) =
    if reference.StartsWith("http:") || reference.StartsWith("https:") then Url reference else File reference

let private parseLink (inlineLink: LinkInline) =
    match linkReference inlineLink with
    | Url url -> UrlLink(url, linkLocation inlineLink)
    | File path -> FileLink(path, linkLocation inlineLink)
    
let private parseLinksFromFile (path: string) =
    let markdown = System.IO.File.ReadAllText(path)
    Markdown.Parse(markdown).Descendants<LinkInline>()
    |> Seq.map parseLink
    |> Seq.toList

let parseLinks (file: File): Link list =
    match file with
    | NonMarkdownFile _ -> []
    | MarkdownFile path -> parseLinksFromFile path
    
//    
//    
//type LinkStatus =
//    | Found
//    | NotFound
//    
//type Status =
//    | Valid
//    | Invalid
//
//type UncheckedDocument =
//    { Path: Path
//      Links: Link list }
//
//type UncheckedDocuments = UncheckedDocuments of UncheckedDocument list
//
//type CheckedLink =
//    { Link: Link
//      Status: LinkStatus }
//    
//type CheckedDocument =
//    { Path: Path
//      Links: CheckedLink list
//      Status: Status }
//
//type CheckedDocuments =
//    { Documents: CheckedDocument list
//      Status: Status }
//
//type CheckerContext =
//    { Exclude: string option
//      Include: string
//      Directory: string
//      Logger: Logger }
//

//
//let private toPath context (fileMatch: FilePatternMatch) =
//    { Absolute = Path.Combine(context.Directory, fileMatch.Path)
//      Relative = fileMatch.Stem
//      Directory = Path.GetDirectoryName(Path.Combine(context.Directory, fileMatch.Path)) }
//
//let private toUncheckedDocument context (fileMatch: FilePatternMatch) =
//    let path = toPath context fileMatch
//    let extension = Path.GetExtension(path.Absolute).ToLower() 
//    if extension = ".md" then
//        Some { Path = path; Links = parseLinks path (File.ReadAllText(path.Absolute)) }
//    else
//        None

