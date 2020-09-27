module MarkdownLinkChecker.Parser

open System

open System.Threading.Channels
open System.Threading.Tasks
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
      Links: Link[] }

let private linkReference (inlineLink: LinkInline): string =
    match Option.ofObj inlineLink.Reference with
    | Some reference -> reference.Url
    | None -> inlineLink.Url

let private linkLocation (inlineLink: LinkInline): LinkLocation =
    { Line = inlineLink.Line
      Column = inlineLink.Column }

let private (|UrlReference|FileReference|) (reference: string) =
    let isUrlReference =
        reference.StartsWith("http:", StringComparison.OrdinalIgnoreCase)
        || reference.StartsWith("https:", StringComparison.OrdinalIgnoreCase)

    if isUrlReference then UrlReference reference else FileReference reference

let private parseLink (options: Options) documentPath (inlineLink: LinkInline) =
    match linkReference inlineLink with
    | UrlReference url ->
        if options.Mode.CheckUrls then
            Some (UrlLink(url, linkLocation inlineLink))
        else
            None
    | FileReference path ->
        if options.Mode.CheckFiles then
            let pathRelativeToDocument =
                System.IO.Path.Combine(System.IO.Path.GetDirectoryName(documentPath.Absolute), path)
            Some (FileLink(toFilePath options.Directory pathRelativeToDocument, linkLocation inlineLink))
        else
            None

let private parseLinks (options: Options) file =
    async {
        let markdown = System.IO.File.ReadAllText(file.Absolute)
        return
            Markdown.Parse(markdown).Descendants<LinkInline>()
            |> Seq.choose (parseLink options file)
            |> Seq.toArray
    }

let private parseDocument (options: Options) file =
    async {
        let! links = parseLinks options file
        return
          { Path = file
            Links = links }
    }

let parseDocuments (options: Options) files =
    files
    |> Seq.map (parseDocument options)
    |> Async.Parallel
    |> Async.RunSynchronously
