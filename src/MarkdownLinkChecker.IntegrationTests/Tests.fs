module MarkdownLinkChecker.IntegrationTests

open MarkdownLinkChecker.Program
open System
open Xunit
open System.IO

type private CheckResults =
    { ExitCode: int
      Output: string }

let private run argv =
    use stringWriter = new StringWriter()
    Console.SetOut(stringWriter)
    Console.SetError(stringWriter)
    
    { ExitCode = main argv
      Output = stringWriter.ToString() }

module SingleFile =
    
    [<Fact>]
    let ``Valid URL link`` () =
        let results = run [| "Samples/valid-url-link.md"  |]
        Assert.Equal(0, results.ExitCode)
        
    [<Fact>]
    let ``Invalid URL link`` () =
        let results = run [| "Samples/invalid-url-link.md"  |]
        Assert.Equal(1, results.ExitCode)
        
    [<Fact>]
    let ``Valid and invalid URL links`` () =
        let results = run [| "Samples/valid-and-invalid-url-links.md"  |]
        Assert.Equal(1, results.ExitCode)

    [<Fact>]
    let ``Valid file link`` () =
        let results = run [| "Samples/valid-file-link.md"  |]
        Assert.Equal(0, results.ExitCode)
        
    [<Fact>]
    let ``Invalid file link`` () =
        let results = run [| "Samples/invalid-file-link.md"  |]
        Assert.Equal(1, results.ExitCode)
        
    [<Fact>]
    let ``Valid and invalid file links`` () =
        let results = run [| "Samples/valid-and-invalid-file-links.md"  |]
        Assert.Equal(1, results.ExitCode)

    [<Fact>]
    let ``Valid file and url links`` () =
        let results = run [| "Samples/valid-file-and-url-links.md"  |]
        Assert.Equal(0, results.ExitCode)

    [<Fact>]
    let ``Multiple valid links to same  file`` () =
        let results = run [| "Samples/multiple-valid-links-to-same-file.md"  |]
        Assert.Equal(0, results.ExitCode)

module MultipleFiles =

    [<Fact>]
    let ``Only valid links`` () =
        let results = run [| "Samples/OnlyValid/*.md"  |]
        Assert.Equal(0, results.ExitCode)
        
    [<Fact>]
    let ``Valid and invalid links`` () =
        let results = run [| "Samples/**/*.md"  |]
        Assert.Equal(1, results.ExitCode)