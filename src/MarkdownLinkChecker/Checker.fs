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

let linkStatusCache = System.Collections.Concurrent.ConcurrentDictionary<string, LinkStatus>()

let private httpClient = new HttpClient()

let private checkUrlLink (options: Options) (url: string) =
    options.Logger.Normal(sprintf "Checking URL: %s" url)
    
    let response =
        httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url))
        |> Async.AwaitTask
        |> Async.RunSynchronously

    if response.IsSuccessStatusCode then Found else NotFound
    
let private checkFileLink (options: Options) (path: string) =
    options.Logger.Normal(sprintf "Checking file %s" path)
    
    if File.Exists(path) then Found else NotFound

let private checkLinkStatus (options: Options) (document: Document) (link: Link) =
    match link with
    | UrlLink(url, _) ->
        linkStatusCache.GetOrAdd(url, checkUrlLink options)
    | FileLink(path, _) ->
        let (File documentPath) = document.File 
        let fullPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(documentPath), path))
        linkStatusCache.GetOrAdd(fullPath, checkFileLink options)
        
let private checkLink (options: Options) document link =
    { Link = link
      Status = checkLinkStatus options document link }

let private checkDocument (options: Options) (document: Document) =
    { File = document.File
      CheckedLinks = document.Links |> List.map (checkLink options document) }

let private checkedDocumentIsValid checkedDocument =
    checkedDocument.CheckedLinks
    |> List.forall (fun checkedLink -> checkedLink.Status = Found)

let checkDocuments (options: Options) documents =
    let valid, elapsed = time (fun () ->
        options.Logger.Normal("Checking links ...")
        let checkedDocuments = documents |> List.map (checkDocument options)   
        let documentsAreValid = checkedDocuments |> List.forall checkedDocumentIsValid

        if documentsAreValid then Valid else Invalid)
    
    options.Logger.Normal(sprintf "Checked links [%.1fms]" elapsed.TotalMilliseconds)
    valid
