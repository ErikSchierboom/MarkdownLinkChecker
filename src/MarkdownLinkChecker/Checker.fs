module MarkdownLinkChecker.Checker

open System.Net.Http
open System.IO

open MarkdownLinkChecker.Parser
open MarkdownLinkChecker.Files
open MarkdownLinkChecker.Options

type LinkStatus =
    | Found
    | NotFound

type Status =
    | Valid
    | Invalid

type CheckedLink = { Link: Link; Status: LinkStatus }

type CheckedDocument =
    { File: FilePath
      CheckedLinks: CheckedLink []
      Status: Status }

let private httpClient = new HttpClient()

let private linkKey (link: Link) =
    match link with
    | UrlLink (url, _) -> url.AbsoluteUri
    | FileLink (path, _) -> path.Absolute

let private linkText (link: Link) =
    match link with
    | UrlLink (_, text) -> text
    | FileLink (_, text) -> text

let private checkUrlStatus (url: string) =
    async {
        let! response =
            httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url))
            |> Async.AwaitTask

        return if response.IsSuccessStatusCode then Found else NotFound
    }

let private checkFileStatus (path: string) =
    async { return if File.Exists(path) then Found else NotFound }

let private checkLinkStatus (link: Link) =
    async {
        let! status =
            match link with
            | UrlLink (_) -> checkUrlStatus (linkKey link)
            | FileLink (_) -> checkFileStatus (linkKey link)

        return (link, status)
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
    options.Logger.Normal(sprintf "\nFILE: %s" (Path.GetFileName(checkedDocument.File.Absolute)))

    let invalidLinksCount =
        checkedDocument.CheckedLinks
        |> Seq.filter (fun checkedLink -> checkedLink.Status = NotFound)
        |> Seq.length

    if invalidLinksCount = 0 then
        options.Logger.Normal(sprintf "%d links checked." checkedDocument.CheckedLinks.Length)
    else
        options.Logger.Normal
            (sprintf "%d links checked, %d dead links found." checkedDocument.CheckedLinks.Length invalidLinksCount)

    for checkedLink in checkedDocument.CheckedLinks do
        if checkedLink.Status = Found
        then options.Logger.Detailed(sprintf "✅ %s" (linkText checkedLink.Link))
        else options.Logger.Normal(sprintf "❌ %s" (linkText checkedLink.Link))

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
