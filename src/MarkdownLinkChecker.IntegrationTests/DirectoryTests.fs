module MarkdownLinkChecker.IntegrationTests.DirectoryFilesTests

open Xunit

open MarkdownLinkChecker.IntegrationTests.Runner

[<Fact>]
let ``Only valid links`` () =
    let results = runOnDirectory "Samples/OnlyValid"
    Assert.Equal(0, results.ExitCode)

[<Fact>]
let ``Only invalid links`` () =
    let results = runOnDirectory "Samples/OnlyInvalid"
    Assert.Equal(1, results.ExitCode)
    
[<Fact>]
let ``Valid and invalid links`` () =
    let results = runOnDirectory "Samples"
    Assert.Equal(1, results.ExitCode)
    
[<Fact>]
let ``No Markdown files`` () =
    let results = runOnDirectory "Samples/NoMarkdownFiles"
    Assert.Equal(0, results.ExitCode)