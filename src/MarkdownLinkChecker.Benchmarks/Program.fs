open System.IO
open System.Net.Http
open System.Reflection
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running

type FileExists() =
    let executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)

    let rootDirectory =
        executingDirectory.Split(Path.DirectorySeparatorChar)
        |> Array.takeWhile (fun com -> com <> "MarkdownLinkChecker.Benchmarks")
        |> Path.Combine

    let files =
        Directory.EnumerateFiles(rootDirectory, "*.*", SearchOption.AllDirectories)
        |> Seq.toArray
    
    [<Benchmark>]
    member this.Sync() =
        files
        |> Array.map (fun file -> File.Exists(file))
        
    [<Benchmark>]
    member this.AsyncParallel() =
        files
        |> Seq.map (fun file -> async { return File.Exists(file) })
        |> Async.Parallel
        |> Async.RunSynchronously

type UrlExists() =
    
    let httpClient = new HttpClient()

    let urls =
        [| "https://httpbin.org/status/200"
           "https://httpbin.org/status/201"
           "https://httpbin.org/status/202"
           "https://httpbin.org/status/400"
           "https://httpbin.org/status/401"
           "https://httpbin.org/status/402" |]
        
    [<Benchmark>]
    member this.Sync() =
        urls
        |> Array.map (fun url ->
            let response =
                httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url))
                |> Async.AwaitTask
                |> Async.RunSynchronously

            response.IsSuccessStatusCode)
        
    [<Benchmark>]
    member this.AsyncParallel() =
        urls
        |> Seq.map (fun url ->
            async {
                let! response =
                    httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url))
                    |> Async.AwaitTask
                return response.IsSuccessStatusCode
            })
        |> Async.Parallel
        |> Async.RunSynchronously

[<EntryPoint>]
let main argv =
    let _ = BenchmarkSwitcher.FromAssembly(typeof<FileExists>.Assembly).Run(argv)
    0
