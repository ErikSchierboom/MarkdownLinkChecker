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
        let pathRelativeToDocument = System.IO.Path.Combine(documentPath.Absolute.DirectoryName, path)
        FileLink(toFilePath options.Directory pathRelativeToDocument, linkLocation inlineLink)
    
let private parseLinks (options: Options) file =
    let markdown = System.IO.File.ReadAllText(file.Absolute.FullName)
    Markdown.Parse(markdown).Descendants<LinkInline>()
    |> Seq.map (parseLink options file)
    |> Seq.toList

let private parseDocument (options: Options) file =
    let document, elapsed = time (fun () ->
        { Path = file
          Links = parseLinks options file })   
    
    options.Logger.Detailed(sprintf "Parsed document %s. %d link(s) found [%.1fms]" file.Relative.Name document.Links.Length elapsed.TotalMilliseconds)
    document
    
let parseDocuments (options: Options) files =
    let documents, elapsed = time (fun () ->
        options.Logger.Detailed("Parsing Markdown documents ...")
        List.map (parseDocument options) files)
    
    options.Logger.Detailed(sprintf "Parsed Markdown documents [%.1fms]" elapsed.TotalMilliseconds)
    documents
