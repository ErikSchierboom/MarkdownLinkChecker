module MarkdownLinkChecker.Parser

open System
open System.IO

open Markdig
open Markdig.Syntax
open Markdig.Syntax.Inlines

open MarkdownLinkChecker.Files
open MarkdownLinkChecker.Options

type Link =
    | FileLink of FilePath
    | UrlLink of Uri

type Document = { Path: FilePath; Links: Link [] }

let private anchorCharacter = "#"

let private linkReference (inlineLink: LinkInline): string =
    match Option.ofObj inlineLink.Reference with
    | Some reference -> reference.Url
    | None -> inlineLink.Url

let private (|UrlReference|FileReference|) (reference: string) =
    let isUrlReference =
        reference.StartsWith("http:", StringComparison.OrdinalIgnoreCase)
        || reference.StartsWith("https:", StringComparison.OrdinalIgnoreCase)

    if isUrlReference then UrlReference reference else FileReference reference

let private parseLink (options: Options) documentPath (inlineLink: LinkInline) =
    let removeAnchor (link: string) =
        let anchorIndex = link.LastIndexOf(anchorCharacter)
        if anchorIndex = -1 then link else link.[..anchorIndex - 1]
    
    match linkReference inlineLink with
    | UrlReference url ->
        if options.Mode.CheckUrls then
            Some(UrlLink(Uri(removeAnchor url)))
        else
            None
    | FileReference path ->
        let isSelfLink = Path.GetFileName(removeAnchor path) = ""        
        if options.Mode.CheckFiles && isSelfLink then
            Some(FileLink(toFilePath options.Directory documentPath.Absolute))
        elif options.Mode.CheckFiles then
            let pathRelativeToDocument = Path.Combine(Path.GetDirectoryName(documentPath.Absolute), removeAnchor path)
            Some(FileLink(toFilePath options.Directory pathRelativeToDocument))
        else
            None

let private parseLinks (options: Options) file =
    async {
        let markdown =
            File.ReadAllText(file.Absolute)

        return
            Markdown.Parse(markdown).Descendants<LinkInline>()
            |> Seq.choose (parseLink options file)
            |> Seq.toArray
    }

let private parseDocument (options: Options) file =
    async {
        let! links = parseLinks options file
        return { Path = file; Links = links }
    }

let parseDocuments (options: Options) files =
    files
    |> Seq.map (parseDocument options)
    |> Async.Parallel
    |> Async.RunSynchronously
