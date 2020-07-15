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

type CheckedLink =
    { Link: Link
      Status: LinkStatus }

type CheckedDocument =
    { File: FilePath
      CheckedLinks: CheckedLink list }
    member this.IsValid = this.CheckedLinks |> List.forall (fun checkedLink -> checkedLink.Status = Found)

type Status =
    | Valid
    | Invalid

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

let private checkDocument (options: Options) (document: Document) =
    let checkedDocument, elapsed =
        time (fun () ->
            { File = document.Path
              CheckedLinks = document.Links |> List.map checkLink })

    let status =
        if checkedDocument.IsValid then '✅' else '❌'

    options.Logger.Log(sprintf "%c %s %.0fms" status document.Path.Relative elapsed.TotalMilliseconds)
    checkedDocument

let checkDocuments (options: Options) documents =
    let checkedDocuments = documents |> List.map (checkDocument options)
    let documentsAreValid = checkedDocuments |> List.forall (fun checkedDocument -> checkedDocument.IsValid)

    if documentsAreValid then Valid else Invalid
