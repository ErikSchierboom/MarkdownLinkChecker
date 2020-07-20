module MarkdownLinkChecker.Checker

open System.Net.Http
open System.IO

open MarkdownLinkChecker.Parser
open MarkdownLinkChecker.Files
open MarkdownLinkChecker.Options
open MarkdownLinkChecker.Timing

module Dictionary =
    open System.Collections.Generic

    let empty<'TKey, 'TValue when 'TKey: equality> = Dictionary<'TKey, 'TValue>()

    let getOrAdd<'TKey, 'TValue> (dictionary: IDictionary<'TKey, 'TValue>) key fn =
        match dictionary.TryGetValue(key) with
        | (true, value) -> value
        | _ ->
            let value = fn key
            dictionary.Add(key, value)
            value

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
      CheckedLinks: CheckedLink list
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

let private checkUrlLink (options: Options) (url: string) =
    async {
        let! urlLinkStatus, elapsed =
            timeAsync (fun () ->
                async {
                    let! response =
                        httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url))
                        |> Async.AwaitTask

                    return if response.IsSuccessStatusCode then Found else NotFound
                })

        options.Logger.Detailed(sprintf "%c %s %.0fms" (linkStatusIcon urlLinkStatus) url elapsed.TotalMilliseconds)
        return urlLinkStatus
    }

let private checkFileLink (options: Options) (link: string) =
    async {
        let! fileLinkStatus, elapsed =
            timeAsync (fun () -> async {
                return if File.Exists(path) then Found else NotFound
            })
        
        options.Logger.Detailed(sprintf "%c %s %.0fms" (linkStatusIcon fileLinkStatus) path elapsed.TotalMilliseconds)    
        return fileLinkStatus
    }
    
let private checkLink (options: Options) (link: Link) =
    match link with
    | UrlLink(url, _) -> checkUrlLink options url
    | FileLink(path, _) -> checkFileLink options path.Absolute
    
let private checkLinks (options: Options) (links: Link list) =
    links
    |> Seq.distinctBy (linkValue)
    |> Seq.map (fun link -> linkValue link, checkLink options link)
    |> Async.Parallel
    |> Async.RunSynchronously
    |> Map.ofSeq
    

let private checkLinkStatus (options: Options) =
    let cache = Dictionary.empty

    fun (link: Link) ->
        match link with
        | UrlLink(url, _) -> Dictionary.getOrAdd cache url (checkUrlLink options)
        | FileLink(path, _) -> Dictionary.getOrAdd cache path.Absolute (checkFileLink options)

let private toCheckedLink (options: Options) link =
    { Link = link
      Status = checkLinkStatus options link }

let private toCheckedLinks (options: Options) (document: Document) = document.Links |> List.map (toCheckedLink options)

let private checkedDocumentStatus (checkedLinks: CheckedLink list) =
    if checkedLinks |> List.forall (fun checkedLink -> checkedLink.Status = Found)
    then Valid
    else Invalid

let private statusIcon (status: Status) =
    match status with
    | Valid -> '✅'
    | Invalid -> '❌'

let private checkDocument (options: Options) (document: Document) =
    let checkedLinks = toCheckedLinks options document
    let checkedDocument =
        { File = document.Path
          CheckedLinks = checkedLinks
          Status = checkedDocumentStatus checkedLinks }

    options.Logger.Normal(sprintf "%c %s" (statusIcon checkedDocument.Status) document.Path.Relative)
    checkedDocument

let checkDocuments (options: Options) documents =
    let checkedDocuments = documents |> List.map (checkDocument options)
    let documentsAreValid = checkedDocuments |> List.forall (fun checkedDocument -> checkedDocument.Status = Valid)

    if documentsAreValid then Valid else Invalid
