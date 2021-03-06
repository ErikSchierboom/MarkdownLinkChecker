module MarkdownLinkChecker.IntegrationTests.MultipleFilesTests

open Xunit

open MarkdownLinkChecker.IntegrationTests.Runner

[<Fact>]
let ``Only valid links`` () =
    let results =
        runWithMultipleFiles [| "Fixtures" </> "valid-file-link.md"
                                "Fixtures" </> "valid-url-link.md" |]

    Assert.ExitedWithoutError(results)

[<Fact>]
let ``Only invalid links`` () =
    let results =
        runWithMultipleFiles [| "Fixtures" </> "invalid-file-link.md"
                                "Fixtures" </> "invalid-url-link.md" |]

    Assert.ExitedWithError(results)

[<Fact>]
let ``Valid and invalid links`` () =
    let results =
        runWithMultipleFiles [| "Fixtures" </> "valid-file-link.md"
                                "Fixtures" </> "invalid-url-link.md" |]

    Assert.ExitedWithError(results)
