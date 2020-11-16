module MarkdownLinkChecker.IntegrationTests.MultipleFilesTests

open Xunit

open MarkdownLinkChecker.IntegrationTests.Runner

[<Fact>]
let ``Only valid links`` () =
    let results =
        runOnMultipleFiles [| "Fixtures/valid-file-link.md"
                              "Fixtures/valid-url-link.md" |]

    Assert.Equal(0, results.ExitCode)

[<Fact>]
let ``Only invalid links`` () =
    let results =
        runOnMultipleFiles [| "Fixtures/invalid-file-link.md"
                              "Fixtures/invalid-url-link.md" |]

    Assert.Equal(1, results.ExitCode)

[<Fact>]
let ``Valid and invalid links`` () =
    let results =
        runOnMultipleFiles [| "Fixtures/valid-file-link.md"
                              "Fixtures/invalid-url-link.md" |]

    Assert.Equal(1, results.ExitCode)
