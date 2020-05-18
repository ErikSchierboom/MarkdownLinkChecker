module MarkdownLinkChecker.Parser

open System

open Markdig
open Markdig.Syntax
open Markdig.Syntax.Inlines

open MarkdownLinkChecker.Files
open MarkdownLinkChecker.Options
open MarkdownLinkChecker.Timing

type LinkLocation =
    { Line: int
      Column: int }
    
type Link =
    | FileLink of path: string * location: LinkLocation
    | UrlLink of url: string * location: LinkLocation
    
type Document =
    { File: File
      Links: Link list }

let private linkReference (inlineLink: LinkInline): string =
    match Option.ofObj inlineLink.Reference with
    | Some reference -> reference.Url
    | None -> inlineLink.Url

let private linkLocation (inlineLink: LinkInline): LinkLocation =
    { Line = inlineLink.Line
      Column = inlineLink.Column }
    
let private (|UrlReference|FileReference|) (reference: string) =
    let isUrlReference =
        reference.StartsWith("http:", StringComparison.OrdinalIgnoreCase) ||
        reference.StartsWith("https:", StringComparison.OrdinalIgnoreCase)
    
    if isUrlReference then UrlReference reference else FileReference reference

let private parseLink (inlineLink: LinkInline) =
    match linkReference inlineLink with
    | UrlReference url -> UrlLink(url, linkLocation inlineLink)
    | FileReference path -> FileLink(path, linkLocation inlineLink)
    
let private parseLinks (File path) =
    let markdown = System.IO.File.ReadAllText(path)
    Markdown.Parse(markdown).Descendants<LinkInline>()
    |> Seq.map parseLink
    |> Seq.toList

let private parseDocument (options: Options) file =
    let document, elapsed = time (fun () ->
        { File = file
          Links = parseLinks file })   
    
    let (File path) = file
    options.Logger.Detailed(sprintf "Parsed document %s. %d link(s) found [%.1fms]" path document.Links.Length elapsed.TotalMilliseconds)
    document
    
let parseDocuments (options: Options) files =
    let documents, elapsed = time (fun () ->
        options.Logger.Normal("Parsing Markdown documents ...")
        List.map (parseDocument options) files)
    
    options.Logger.Detailed(sprintf "Parsed Markdown documents [%.1fms]" elapsed.TotalMilliseconds)
    documents
