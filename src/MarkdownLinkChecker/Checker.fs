module MarkdownLinkChecker.Checker

open System
open System.Net.Http
open System.IO

open MarkdownLinkChecker.Parser
open MarkdownLinkChecker.Files
open MarkdownLinkChecker.Options
open MarkdownLinkChecker.Timing

type LinkStatus =
    | Found
    | NotFound

type CheckedLink =
    { Link: Link
      Status: LinkStatus }
    
type CheckedDocument =
    { File: File
      CheckedLinks: CheckedLink list }
    
type Status =
    | Valid
    | Invalid

let private memoize fn =
  let cache = System.Collections.Concurrent.ConcurrentDictionary<string, LinkStatus>()
  (fun key -> cache.GetOrAdd(key, fun key -> fn key))

let private httpClient = new HttpClient()

let private checkUrlLink (url: string) =
    let response =
        httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url))
        |> Async.AwaitTask
        |> Async.RunSynchronously

    if response.IsSuccessStatusCode then Found else NotFound
    
let private checkFileLink (path: string) =
    if File.Exists(path) then Found else NotFound

let private checkLinkStatus =
    let checkUrlLinkMemoized = memoize checkUrlLink
    let checkFileLinkMemoized = memoize checkFileLink
    
    fun (document: Document) (link: Link) ->
        match link with
        | UrlLink(url, _) ->
            printfn "Checking URL %s" url
            checkUrlLinkMemoized url
        | FileLink(path, _) ->
            let (File documentPath) = document.File 
            let fullPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(documentPath), path))
            checkFileLinkMemoized fullPath
        
let private checkLink document link =
    { Link = link
      Status = checkLinkStatus document link }

let private checkDocument (document: Document) =
    { File = document.File
      CheckedLinks = document.Links |> List.map (checkLink document) }

let private checkedDocumentIsValid checkedDocument =
    checkedDocument.CheckedLinks
    |> List.forall (fun checkedLink -> checkedLink.Status = Found)
    
let private logAfter (options: Options) (elapsed: TimeSpan) =
    options.Logger.Normal(sprintf "Checked links [%.0fms]" elapsed.TotalMilliseconds)

let checkDocuments (options: Options) documents =
    let valid, elapsed = time (fun () ->
        let checkedDocuments = documents |> List.map checkDocument    
        let documentsAreValid = checkedDocuments |> List.forall checkedDocumentIsValid

        if documentsAreValid then Valid else Invalid)
    
    logAfter options elapsed
    valid
