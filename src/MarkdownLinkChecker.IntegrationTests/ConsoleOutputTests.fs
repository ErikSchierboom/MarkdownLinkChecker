module MarkdownLinkChecker.IntegrationTests.ConsoleOutputTests

open Xunit

open MarkdownLinkChecker.IntegrationTests.Runner

[<Fact>]
let ``Single file included in output`` () =
    let results = runOnSingleFile "Fixtures/valid-url-link.md"

    Assert.Contains("valid-url-link.md", results.Output)

[<Fact>]
let ``Multiple files included in output`` () =
    let results = runOnMultipleFiles [|"Fixtures/valid-url-link.md"; "Fixtures/valid-file-link.md"|]

    let expectedFilenames = ["valid-url-link.md"; "valid-file-link.md"]
    Assert.All(expectedFilenames, fun expectedFilename -> Assert.Contains(expectedFilename, results.Output))
    
[<Fact>]
let ``Directory files included in output`` () =
    let results = runOnDirectory "Fixtures/Nesting"

    let expectedFilenames = ["docs.md"; "links.md"]
    Assert.All(expectedFilenames, fun expectedFilename -> Assert.Contains(expectedFilename, results.Output))

[<Fact>]
let ``Sub-directory files included in output`` () =
    let results = runOnDirectory "Fixtures/Nesting"

    let expectedFilenames = ["docs.md"; "links.md"; "license.md"]
    Assert.All(expectedFilenames, fun expectedFilename -> Assert.Contains(expectedFilename, results.Output))

[<Fact>]
let ``Excluded files not included in output`` () =
    let results = run [| "--directory"; "Fixtures/Nesting"; "--exclude"; "docs.md" |]
    Assert.DoesNotContain("docs.md", results.Output)

[<Fact>]
let ``No output when verbosity is quiet`` () =
    let results = run [| "--directory"; "Fixtures/Nesting"; "--verbosity"; "quiet" |]
    Assert.Empty(results.Output)