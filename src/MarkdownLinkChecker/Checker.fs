module MarkdownLinkChecker.Checker

open System.Net.Http
open System.IO

open MarkdownLinkChecker.Parser
open MarkdownLinkChecker.Files
open MarkdownLinkChecker.Options
open MarkdownLinkChecker.Timing

type LinkStatus =
    | Found
    | NotFound

type Status =
    | Valid
    | Invalid

type CheckedLink =
    { Link: Link
      Status: LinkStatus }

type CheckedDocument =
    { File: FilePath
      CheckedLinks: CheckedLink[]
      Status: Status }

let private httpClient = new HttpClient()

let private linkStatusIcon (status: LinkStatus) =
    match status with
    | Found -> '✅'
    | NotFound -> '❌'

let private linkValue (link: Link) =
    match link with
    | UrlLink(url, _) -> url
    | FileLink(path, _) -> path.Absolute

let private checkUrlStatus (url: string) =
    async {
        let! timed = time (fun () ->
            async {
                let! response =
                    httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url))
                    |> Async.AwaitTask

                return if response.IsSuccessStatusCode then Found else NotFound
            })

        return (url, timed)
    }

let private checkFileStatus (path: string) =
    async {
        let! timed = time (fun () -> async {
            return if File.Exists(path) then Found else NotFound
        })
        
        return (path, timed)
    }
    
let private checkLinkStatus (link: Link) =
    match link with
    | UrlLink(url, _) -> checkUrlStatus url
    | FileLink(path, _) -> checkFileStatus path.Absolute
    
let private checkLinkStatuses (documents: Document[]) =
    documents
    |> Seq.collect (fun document -> document.Links)
    |> Seq.distinctBy (linkValue)
    |> Seq.map checkLinkStatus
    |> Async.Parallel
    |> Async.RunSynchronously
    
let private linkStatusForDocuments (options: Options) (documents: Document[]) =
    let linkStatuses = checkLinkStatuses documents

    for (key, (Timed(status, elapsed))) in linkStatuses do
        options.Logger.Detailed(sprintf "%c %s %.0fms" (linkStatusIcon status) key elapsed.TotalMilliseconds)
        
    linkStatuses
    |> Seq.map (fun (key, (Timed(status, _))) -> (key, status))
    |> Map.ofSeq

let private toCheckedLinks (checkedLinks: Map<string, LinkStatus>) (document: Document) =
    let toCheckedLink link =
        { Link = link
          Status = Map.find (linkValue link) checkedLinks }

    Array.map toCheckedLink document.Links

let private checkedDocumentStatus (checkedLinks: CheckedLink[]) =
    if checkedLinks |> Array.forall (fun checkedLink -> checkedLink.Status = Found)
    then Valid
    else Invalid

let private statusIcon (status: Status) =
    match status with
    | Valid -> '✅'
    | Invalid -> '❌'

let private checkDocument (options: Options) (checkedLinks: Map<string, LinkStatus>) (document: Document) =
    let checkedLinks = toCheckedLinks checkedLinks document
    let checkedDocument =
        { File = document.Path
          CheckedLinks = checkedLinks
          Status = checkedDocumentStatus checkedLinks }

    options.Logger.Normal(sprintf "%c %s" (statusIcon checkedDocument.Status) document.Path.Relative)
    checkedDocument

let checkDocuments (options: Options) (documents: Document[]) =
    let checkedLinks = linkStatusForDocuments options documents
    let documentsAreValid =
        documents
        |> Seq.map (checkDocument options checkedLinks)
        |> Seq.forall (fun checkedDocument -> checkedDocument.Status = Valid)

    if documentsAreValid then Valid else Invalid
