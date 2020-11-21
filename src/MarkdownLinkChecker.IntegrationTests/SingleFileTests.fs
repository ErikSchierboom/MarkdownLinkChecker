module MarkdownLinkChecker.IntegrationTests.SingleFileTests

open Xunit

open MarkdownLinkChecker.IntegrationTests.Runner

[<Fact>]
let ``Valid URL link`` () =
    let results =
        runOnSingleFile "Fixtures/valid-url-link.md"

    Assert.Equal(0, results.ExitCode)

[<Fact>]
let ``Valid URL link with anchor`` () =
    let results =
        runOnSingleFile "Fixtures/valid-url-link-with-anchor.md"

    Assert.Equal(0, results.ExitCode)

[<Fact>]
let ``Valid inline URL link`` () =
    let results =
        runOnSingleFile "Fixtures/valid-inline-url-link.md"

    Assert.Equal(0, results.ExitCode)

[<Fact>]
let ``Invalid URL link`` () =
    let results =
        runOnSingleFile "Fixtures/invalid-url-link.md"

    Assert.Equal(1, results.ExitCode)

[<Fact>]
let ``Valid and invalid URL links`` () =
    let results =
        runOnSingleFile "Fixtures/valid-and-invalid-url-links.md"

    Assert.Equal(1, results.ExitCode)

[<Fact>]
let ``Valid file link`` () =
    let results =
        runOnSingleFile "Fixtures/valid-file-link.md"

    Assert.Equal(0, results.ExitCode)

[<Fact>]
let ``Valid file link with anchor`` () =
    let results =
        runOnSingleFile "Fixtures/valid-file-link-with-anchor.md"

    Assert.Equal(0, results.ExitCode)

[<Fact>]
let ``Valid inline file link`` () =
    let results =
        runOnSingleFile "Fixtures/valid-inline-file-link.md"

    Assert.Equal(0, results.ExitCode)

[<Fact>]
let ``Invalid file link`` () =
    let results =
        runOnSingleFile "Fixtures/invalid-file-link.md"

    Assert.Equal(1, results.ExitCode)

[<Fact>]
let ``Valid and invalid file links`` () =
    let results =
        runOnSingleFile "Fixtures/valid-and-invalid-file-links.md"

    Assert.Equal(1, results.ExitCode)

[<Fact>]
let ``Valid file and url links`` () =
    let results =
        runOnSingleFile "Fixtures/valid-file-and-url-links.md"

    Assert.Equal(0, results.ExitCode)

[<Fact>]
let ``Multiple valid links to same file`` () =
    let results =
        runOnSingleFile "Fixtures/multiple-valid-links-to-same-file.md"

    Assert.Equal(0, results.ExitCode)

[<Fact>]
let ``Multiple valid links using different formats`` () =
    let results =
        runOnSingleFile "Fixtures/multiple-valid-links-using-different-formats.md"

    Assert.Equal(0, results.ExitCode)
