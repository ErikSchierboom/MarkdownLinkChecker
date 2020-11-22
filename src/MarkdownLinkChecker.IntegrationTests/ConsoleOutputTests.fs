module MarkdownLinkChecker.IntegrationTests.ConsoleOutputTests

open Xunit

open MarkdownLinkChecker.IntegrationTests.Runner

[<Fact>]
let ``Single file included in output`` () =
    let fileName = "Fixtures" </> "valid-url-link.md"

    let results = runWithSingleFile fileName

    Assert.ContainsFileName(fileName, results)

[<Fact>]
let ``Multiple files included in output`` () =
    let fileNames =
        [| "Fixtures" </> "valid-url-link.md"
           "Fixtures" </> "valid-file-link.md" |]

    let results = runWithMultipleFiles fileNames

    Assert.ContainsFileNames(fileNames, results)

[<Fact>]
let ``Directory files included in output`` () =
    let results =
        runWithDirectory ("Fixtures" </> "Nesting")

    let fileNames = [ "docs.md"; "links.md" ]
    Assert.ContainsFileNames(fileNames, results)

[<Fact>]
let ``Sub-directory files included in output`` () =
    let results =
        runWithDirectory ("Fixtures" </> "Nesting")

    let fileNames =
        [ "docs.md"
          "links.md"
          "Deeper" </> "license.md" ]

    Assert.ContainsFileNames(fileNames, results)

[<Fact>]
let ``Excluded files not included in output`` () =
    let results =
        run [| "--directory"
               "Fixtures" </> "Nesting"
               "--exclude"
               "docs.md" |]

    Assert.DoesNotContainFileName("docs.md", results)

[<Fact>]
let ``No output when verbosity is quiet`` () =
    let results =
        run [| "--directory"
               "Fixtures" </> "Nesting"
               "--verbosity"
               "quiet" |]

    Assert.Empty(results.Output)
