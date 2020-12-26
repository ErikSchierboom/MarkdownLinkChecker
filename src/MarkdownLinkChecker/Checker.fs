module MarkdownLinkChecker.Checker

open System.Net.Http
open System.IO

open MarkdownLinkChecker.Parser
open MarkdownLinkChecker.Files
open MarkdownLinkChecker.Options

type LinkStatus =
    | Found
    | NotFound
    | Error

type Status =
    | Valid
    | Invalid

type CheckedLink = { Link: Link; Status: LinkStatus }

type CheckedDocument =
    { File: FilePath
      CheckedLinks: CheckedLink []
      Status: Status }

type HttpResponseMessage with
    member self.IsRedirectStatusCode = int self.StatusCode >= 300 && int self.StatusCode <= 399

let private httpClient = new HttpClient()

let private linkKey (link: Link) =
    match link with
    | UrlLink (url, _, _) -> url.AbsoluteUri
    | FileLink (path, _, _) -> path.Absolute

let private linkReference (link: Link) =
    match link with
    | UrlLink (_, reference, _) -> reference
    | FileLink (_, reference, _) -> reference

let private linkPosition (link: Link) =
    match link with
    | UrlLink (_, _, position) -> position
    | FileLink (_, _, position) -> position

let private checkUrlStatus (url: string) =
    async {
        let request = new HttpRequestMessage(HttpMethod.Head, url)
        let! response = httpClient.SendAsync(request) |> Async.AwaitTask

        return if response.IsSuccessStatusCode || response.IsRedirectStatusCode then Found else NotFound
    }

let private checkFileStatus (path: string) =
    async {
        return
            if File.Exists(path) || Directory.Exists(path)
            then Found
            else NotFound
    }

let private checkLinkStatus (link: Link) =
    async {
        let! result =
            match link with
            | UrlLink (_) -> checkUrlStatus (linkKey link) |> Async.Catch
            | FileLink (_) -> checkFileStatus (linkKey link) |> Async.Catch

        match result with
        | Choice1Of2 status -> return (link, status)
        | Choice2Of2 _ -> return (link, Error)
    }

let private checkLinkStatuses (documents: Document []) =
    documents
    |> Seq.collect (fun document -> document.Links)
    |> Seq.distinctBy linkKey
    |> Seq.map checkLinkStatus
    |> Async.Parallel
    |> Async.RunSynchronously

let private linkStatusForDocuments (options: Options) (documents: Document []) =
    checkLinkStatuses documents
    |> Seq.map (fun (link, status) -> (linkKey link, status))
    |> Map.ofSeq

let private toCheckedLinks (checkedLinks: Map<string, LinkStatus>) (document: Document) =
    let toCheckedLink link =
        { Link = link
          Status = Map.find (linkKey link) checkedLinks }

    Array.map toCheckedLink document.Links

let private checkedDocumentStatus (checkedLinks: CheckedLink []) =
    if checkedLinks
       |> Array.forall (fun checkedLink -> checkedLink.Status = Found) then
        Valid
    else
        Invalid

let private logCheckedDocument (options: Options) (checkedDocument: CheckedDocument) =
    let invalidLinksCount =
        checkedDocument.CheckedLinks
        |> Seq.filter (fun checkedLink -> checkedLink.Status <> Found)
        |> Seq.length

    let filePath =
        Path.GetRelativePath(options.Directory, checkedDocument.File.Absolute)

    if invalidLinksCount = 0 then
        options.Logger.Normal(sprintf "\nFILE: %s" filePath)
        options.Logger.Normal(sprintf "%d links checked." checkedDocument.CheckedLinks.Length)
    else
        options.Logger.Minimal(sprintf "\nFILE: %s" filePath)

        options.Logger.Normal
            (sprintf "%d links checked, %d dead links found." checkedDocument.CheckedLinks.Length invalidLinksCount)

    for checkedLink in checkedDocument.CheckedLinks do
        let position = linkPosition checkedLink.Link
        let reference = linkReference checkedLink.Link

        match checkedLink.Status with
        | Found -> options.Logger.Detailed(sprintf "✅ (%d,%d): %s" position.Line position.Column reference)
        | NotFound -> options.Logger.Minimal(sprintf "❌ (%d,%d): %s" position.Line position.Column reference)
        | Error -> options.Logger.Minimal(sprintf "❌ (%d,%d): %s (error)" position.Line position.Column reference)

let private checkDocument (options: Options) (checkedLinks: Map<string, LinkStatus>) (document: Document) =
    let checkedLinks = toCheckedLinks checkedLinks document

    let checkedDocument =
        { File = document.Path
          CheckedLinks = checkedLinks
          Status = checkedDocumentStatus checkedLinks }

    logCheckedDocument options checkedDocument
    checkedDocument

let checkDocuments (options: Options) (documents: Document []) =
    let checkedLinks = linkStatusForDocuments options documents

    let checkedDocuments =
        documents
        |> Seq.map (checkDocument options checkedLinks)
        |> Seq.toArray

    let documentsAreValid =
        checkedDocuments
        |> Seq.forall (fun checkedDocument -> checkedDocument.Status = Valid)

    if documentsAreValid then Valid else Invalid
