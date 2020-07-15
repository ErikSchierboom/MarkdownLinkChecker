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

let private checkUrlLink (url: string) =
    let response =
        httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url))
        |> Async.AwaitTask
        |> Async.RunSynchronously

    if response.IsSuccessStatusCode then Found else NotFound

let private checkFileLink (path: string) =
    if File.Exists(path) then Found else NotFound

let private checkLinkStatus =
    let cache = Dictionary.empty

    fun (link: Link) ->
        match link with
        | UrlLink(url, _) -> Dictionary.getOrAdd cache url checkUrlLink
        | FileLink(path, _) -> Dictionary.getOrAdd cache path.Absolute checkFileLink

let private checkLink link =
    { Link = link
      Status = checkLinkStatus link }

let private checkLinks (document: Document) = document.Links |> List.map checkLink

let private checkedDocumentStatus (checkedLinks: CheckedLink list) =
    if checkedLinks |> List.forall (fun checkedLink -> checkedLink.Status = Found)
    then Valid
    else Invalid

let private checkDocument (options: Options) (document: Document) =
    let checkedDocument, elapsed =
        time (fun () ->
            let checkedLinks = checkLinks document

            { File = document.Path
              CheckedLinks = checkedLinks
              Status = checkedDocumentStatus checkedLinks })

    let statusIcon =
        match checkedDocument.Status with
        | Valid -> '✅'
        | Invalid -> '❌'

    options.Logger.Log(sprintf "%c %s %.0fms" statusIcon document.Path.Relative elapsed.TotalMilliseconds)
    checkedDocument

let checkDocuments (options: Options) documents =
    let checkedDocuments = documents |> List.map (checkDocument options)
    let documentsAreValid = checkedDocuments |> List.forall (fun checkedDocument -> checkedDocument.Status = Valid)

    if documentsAreValid then Valid else Invalid
