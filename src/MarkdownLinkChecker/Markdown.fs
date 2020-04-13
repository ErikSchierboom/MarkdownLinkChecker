module MarkdownLinkChecker.Markdown

open Markdig
open Markdig.Syntax
open Markdig.Syntax.Inlines
open MarkdownLinkChecker.Documents

type LinkLocation =
    { Line: int
      Column: int
      File: Path }
    
type Link =
    | FileLink of string * LinkLocation
    | UrlLink of string * LinkLocation
    
type LinkStatus =
    | Found
    | NotFound

let private linkReference (inlineLink: LinkInline): string =
    match Option.ofObj inlineLink.Reference with
    | Some reference -> reference.Url
    | None -> inlineLink.Url

let private linkLocation (file: Path) (inlineLink: LinkInline): LinkLocation =
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