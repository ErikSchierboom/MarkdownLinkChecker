module MarkdownLinkChecker.Checker

open MarkdownLinkChecker.Documents
open System.Net.Http
open System.IO

let mutable private linkStatusCache: Map<string, LinkStatus> = Map.empty

let private httpClient = new HttpClient()

let private checkUrlLink (url: string) =
    let response =
        httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url))
        |> Async.AwaitTask
        |> Async.RunSynchronously

    if response.IsSuccessStatusCode then Found else NotFound
    
let private checkFileLink (path: string) =
    if File.Exists(path) then Found else NotFound

let private tryCheckAndUpdate (link: string) check: LinkStatus =
    match Map.tryFind link linkStatusCache with
    | Some linkStatus ->
        linkStatus
    | None ->
        let linkStatus = check link
        linkStatusCache <- Map.add link linkStatus linkStatusCache
        linkStatus

let private check (link: Link): LinkStatus =
    match link with
    | UrlLink(url, _) ->
        tryCheckAndUpdate url checkUrlLink
    | FileLink(path, location) ->
        let fullPath = Path.GetFullPath(Path.Combine(location.File.Directory, path))
        tryCheckAndUpdate fullPath checkFileLink
        
let private toCheckedLink (link: Link) =
    { Link = link; Status = check link }

let private toCheckedDocument (uncheckedDocument: UncheckedDocument) =
    let notFound (link: CheckedLink) = link.Status = NotFound
    let checkedLinks = List.map toCheckedLink uncheckedDocument.Links
    let status = if List.exists notFound checkedLinks then Invalid else Valid        
        
    { Path = uncheckedDocument.Path
      Links = checkedLinks
      Status = status }
    
let checkDocuments (UncheckedDocuments(uncheckedDocuments)) =
    uncheckedDocuments
    |> List.map toCheckedDocument 
    |> CheckedDocuments
    
    

