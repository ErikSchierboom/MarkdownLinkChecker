module MarkdownLinkChecker.IntegrationTests.DirectoryFilesTests

open Xunit

open MarkdownLinkChecker.IntegrationTests.Runner

[<Fact>]
let ``Only valid links`` () =
    let results = runOnDirectory "Fixtures/OnlyValid"
    Assert.Equal(0, results.ExitCode)

[<Fact>]
let ``Only invalid links`` () =
    let results = runOnDirectory "Fixtures/OnlyInvalid"
    Assert.Equal(1, results.ExitCode)

[<Fact>]
let ``Valid and invalid links`` () =
    let results = runOnDirectory "Fixtures"
    Assert.Equal(1, results.ExitCode)

[<Fact>]
let ``No Markdown files`` () =
    let results =
        runOnDirectory "Fixtures/NoMarkdownFiles"

    Assert.Equal(0, results.ExitCode)
