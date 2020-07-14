module MarkdownLinkChecker.Parser

open System

open Markdig
open Markdig.Syntax
open Markdig.Syntax.Inlines

open MarkdownLinkChecker.Files
open MarkdownLinkChecker.Options

type LinkLocation =
    { Line: int
      Column: int }
    
type Link =
    | FileLink of path: FilePath * location: LinkLocation
    | UrlLink of url: string * location: LinkLocation
    
type Document =
    { Path: FilePath
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

let private parseLink (options: Options) documentPath (inlineLink: LinkInline) =
    match linkReference inlineLink with
    | UrlReference url ->
        UrlLink(url, linkLocation inlineLink)
    | FileReference path ->
        let pathRelativeToDocument = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(documentPath.Absolute), path)
        FileLink(toFilePath options.Directory pathRelativeToDocument, linkLocation inlineLink)
    
let private parseLinks (options: Options) file =
    let markdown = System.IO.File.ReadAllText(file.Absolute)
    Markdown.Parse(markdown).Descendants<LinkInline>()
    |> Seq.map (parseLink options file)
    |> Seq.toList

let private parseDocument (options: Options) file =
    { Path = file
      Links = parseLinks options file }
    
let parseDocuments (options: Options) files =
    List.map (parseDocument options) files
