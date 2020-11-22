module MarkdownLinkChecker.IntegrationTests.SingleFileTests

open Xunit

open MarkdownLinkChecker.IntegrationTests.Runner

[<Fact>]
let ``Valid URL link`` () =
    let results =
        runWithSingleFile ("Fixtures" </> "valid-url-link.md")

    Assert.ExitedWithoutError(results)

[<Fact>]
let ``Valid URL link with anchor`` () =
    let results =
        runWithSingleFile ("Fixtures" </> "valid-url-link-with-anchor.md")

    Assert.ExitedWithoutError(results)

[<Fact>]
let ``Valid inline URL link`` () =
    let results =
        runWithSingleFile ("Fixtures" </> "valid-inline-url-link.md")

    Assert.ExitedWithoutError(results)

[<Fact>]
let ``Invalid URL link`` () =
    let results =
        runWithSingleFile ("Fixtures" </> "invalid-url-link.md")

    Assert.ExitedWithError(results)

[<Fact>]
let ``Valid and invalid URL links`` () =
    let results =
        runWithSingleFile ("Fixtures" </> "valid-and-invalid-url-links.md")

    Assert.ExitedWithError(results)

[<Fact>]
let ``Valid file link`` () =
    let results =
        runWithSingleFile ("Fixtures" </> "valid-file-link.md")

    Assert.ExitedWithoutError(results)

[<Fact>]
let ``Valid file link with anchor`` () =
    let results =
        runWithSingleFile ("Fixtures" </> "valid-file-link-with-anchor.md")

    Assert.ExitedWithoutError(results)

[<Fact>]
let ``Valid inline file link`` () =
    let results =
        runWithSingleFile ("Fixtures" </> "valid-inline-file-link.md")

    Assert.ExitedWithoutError(results)

[<Fact>]
let ``Invalid file link`` () =
    let results =
        runWithSingleFile ("Fixtures" </> "invalid-file-link.md")

    Assert.ExitedWithError(results)

[<Fact>]
let ``Valid and invalid file links`` () =
    let results =
        runWithSingleFile ("Fixtures" </> "valid-and-invalid-file-links.md")

    Assert.ExitedWithError(results)

[<Fact>]
let ``Valid file and url links`` () =
    let results =
        runWithSingleFile ("Fixtures" </> "valid-file-and-url-links.md")

    Assert.ExitedWithoutError(results)

[<Fact>]
let ``Multiple valid links to same file`` () =
    let results =
        runWithSingleFile ("Fixtures" </> "multiple-valid-links-to-same-file.md")

    Assert.ExitedWithoutError(results)

[<Fact>]
let ``Multiple valid links using different formats`` () =
    let results =
        runWithSingleFile ("Fixtures" </> "multiple-valid-links-using-different-formats.md")

    Assert.ExitedWithoutError(results)
