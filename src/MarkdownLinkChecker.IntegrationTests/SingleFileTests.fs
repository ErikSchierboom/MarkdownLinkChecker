module MarkdownLinkChecker.IntegrationTests.SingleFileTests

open Xunit

open MarkdownLinkChecker.IntegrationTests.Runner

[<Fact>]
let ``Valid URL link`` () =
    let results =
        runOnSingleFile "Samples/valid-url-link.md"

    Assert.Equal(0, results.ExitCode)

[<Fact>]
let ``Valid URL link with anchor`` () =
    let results =
        runOnSingleFile "Samples/valid-url-link-with-anchor.md"

    Assert.Equal(0, results.ExitCode)

[<Fact>]
let ``Invalid URL link`` () =
    let results =
        runOnSingleFile "Samples/invalid-url-link.md"

    Assert.Equal(1, results.ExitCode)

[<Fact>]
let ``Valid and invalid URL links`` () =
    let results =
        runOnSingleFile "Samples/valid-and-invalid-url-links.md"

    Assert.Equal(1, results.ExitCode)

[<Fact>]
let ``Valid file link`` () =
    let results =
        runOnSingleFile "Samples/valid-file-link.md"

    Assert.Equal(0, results.ExitCode)

[<Fact>]
let ``Valid file link with anchor`` () =
    let results =
        runOnSingleFile "Samples/valid-file-link-with-anchor.md"

    Assert.Equal(0, results.ExitCode)

[<Fact>]
let ``Invalid file link`` () =
    let results =
        runOnSingleFile "Samples/invalid-file-link.md"

    Assert.Equal(1, results.ExitCode)

[<Fact>]
let ``Valid and invalid file links`` () =
    let results =
        runOnSingleFile "Samples/valid-and-invalid-file-links.md"

    Assert.Equal(1, results.ExitCode)

[<Fact>]
let ``Valid file and url links`` () =
    let results =
        runOnSingleFile "Samples/valid-file-and-url-links.md"

    Assert.Equal(0, results.ExitCode)

[<Fact>]
let ``Multiple valid links to same  file`` () =
    let results =
        runOnSingleFile "Samples/multiple-valid-links-to-same-file.md"

    Assert.Equal(0, results.ExitCode)
