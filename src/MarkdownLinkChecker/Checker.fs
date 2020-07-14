module MarkdownLinkChecker.Checker

open System.Net.Http
open System.IO

open MarkdownLinkChecker.Parser
open MarkdownLinkChecker.Files
open MarkdownLinkChecker.Options
open MarkdownLinkChecker.Timing

module Dictionary =
    open System.Collections.Generic
    
    let empty<'TKey, 'TValue when 'TKey : equality> = Dictionary<'TKey, 'TValue>()
    
    let getOrAdd<'TKey, 'TValue> (dictionary: IDictionary<'TKey, 'TValue>) key fn =
        match dictionary.TryGetValue(key) with
        | (true, value) ->
            value
        | _ ->
            let value = fn key
            dictionary.Add(key, value)
            value

type LinkStatus =
    | Found
    | NotFound

type CheckedLink =
    { Link: Link
      Status: LinkStatus }
    
type CheckedDocument =
    { File: FilePath
      CheckedLinks: CheckedLink list }
    
type Status =
    | Valid
    | Invalid

let private httpClient = new HttpClient()

let private checkUrlLink (options: Options) (url: string) =
    let status, elapsed = time (fun () ->
        let response =
            httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url))
            |> Async.AwaitTask
            |> Async.RunSynchronously

        if response.IsSuccessStatusCode then Found else NotFound)
    
    options.Logger.Detailed(sprintf "Checked linked URL: %s [%.1fms]" url elapsed.TotalMilliseconds)
    status
    
let private checkFileLink (options: Options) (path: string) =
    let status, elapsed = time (fun () ->
        if File.Exists(path) then Found else NotFound)
    
    options.Logger.Detailed(sprintf "Checked linked file: %s [%.1fms]"path elapsed.TotalMilliseconds)
    status

let private checkLinkStatus =
    let cache = Dictionary.empty
    
    fun (options: Options) (link: Link) ->
        match link with
        | UrlLink(url, _) ->
            Dictionary.getOrAdd cache url (checkUrlLink options)
        | FileLink(path, _) ->
            Dictionary.getOrAdd cache path.Absolute (checkFileLink options)
        
let private checkLink (options: Options) link =
    { Link = link
      Status = checkLinkStatus options link }

let private checkDocument (options: Options) (document: Document) =
    let checkedDocument, elapsed = time (fun () ->
        { File = document.Path
          CheckedLinks = document.Links |> List.map (checkLink options) })

    options.Logger.Normal(sprintf "Checked file: %s [%.1fms]" document.Path.Relative elapsed.TotalMilliseconds)
    checkedDocument

let private checkedDocumentIsValid checkedDocument =
    checkedDocument.CheckedLinks
    |> List.forall (fun checkedLink -> checkedLink.Status = Found)

let checkDocuments (options: Options) documents =
    let valid, elapsed = time (fun () ->
        let checkedDocuments = documents |> List.map (checkDocument options)   
        let documentsAreValid = checkedDocuments |> List.forall checkedDocumentIsValid

        if documentsAreValid then Valid else Invalid)
    
    options.Logger.Detailed(sprintf "Checked links [%.1fms]" elapsed.TotalMilliseconds)
    valid
