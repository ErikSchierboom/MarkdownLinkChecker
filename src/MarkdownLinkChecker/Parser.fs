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

let private parseDocument file =
    { File = file
      Links = parseLinks file }
    
let private logAfter (options: Options) (elapsed: TimeSpan) =
    options.Logger.Normal(sprintf "Parsed Markdown documents [%.0fms]" elapsed.TotalMilliseconds)
    
let parseDocuments (options: Options) files =
    let documents, elapsed = time (fun () ->
        List.map parseDocument files)
    
    logAfter options elapsed
    documents
