module MarkdownLinkChecker.IntegrationTests.DirectoryFilesTests

open Xunit

open MarkdownLinkChecker.IntegrationTests.Runner

[<Fact>]
let ``Only valid links`` () =
    let results =
        runWithDirectory ("Fixtures" </> "OnlyValid")

    Assert.ExitedWithoutError(results)

[<Fact>]
let ``Only invalid links`` () =
    let results =
        runWithDirectory ("Fixtures" </> "OnlyInvalid")

    Assert.ExitedWithError(results)

[<Fact>]
let ``Valid and invalid links`` () =
    let results = runWithDirectory "Fixtures"
    Assert.ExitedWithError(results)

[<Fact>]
let ``No Markdown files`` () =
    let results =
        runWithDirectory ("Fixtures" </> "NoMarkdownFiles")

    Assert.ExitedWithoutError(results)
