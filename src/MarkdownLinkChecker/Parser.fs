module MarkdownLinkChecker.Parser

open MarkdownLinkChecker.Globbing

open Markdig
open Markdig.Syntax
open Markdig.Syntax.Inlines

type LinkLocation =
    { Line: int
      Column: int }
    
type MarkdownLink =
    | FileLink of path: string * location: LinkLocation
    | UrlLink of url: string * location: LinkLocation
    
type MarkdownDocument =
    { File: MarkdownFile
      Links: MarkdownLink list }

let private linkReference (inlineLink: LinkInline): string =
    match Option.ofObj inlineLink.Reference with
    | Some reference -> reference.Url
    | None -> inlineLink.Url

let private linkLocation (inlineLink: LinkInline): LinkLocation =
    { Line = inlineLink.Line
      Column = inlineLink.Column }
    
let private (|Url|File|) (reference: string) =
    if reference.StartsWith("http:") || reference.StartsWith("https:") then Url reference else File reference

let private parseLink (inlineLink: LinkInline) =
    match linkReference inlineLink with
    | Url url -> UrlLink(url, linkLocation inlineLink)
    | File path -> FileLink(path, linkLocation inlineLink)
    
let private parseLinks (MarkdownFile path) =
    let markdown = System.IO.File.ReadAllText(path)
    Markdown.Parse(markdown).Descendants<LinkInline>()
    |> Seq.map parseLink
    |> Seq.toList

let private parseDocument markdownFile =
    { File = markdownFile
      Links = parseLinks markdownFile }
    
let parseDocuments markdownFiles = List.map parseDocument markdownFiles
