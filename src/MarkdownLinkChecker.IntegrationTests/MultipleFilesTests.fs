module MarkdownLinkChecker.IntegrationTests.MultipleFilesTests

open Xunit

open MarkdownLinkChecker.IntegrationTests.Runner

[<Fact>]
let ``Only valid links`` () =
    let results = runOnMultipleFiles [| "Samples/valid-file-link.md"; "Samples/valid-url-link.md" |] 
    Assert.Equal(0, results.ExitCode)

[<Fact>]
let ``Only invalid links`` () =
    let results = runOnMultipleFiles [| "Samples/invalid-file-link.md"; "Samples/invalid-url-link.md" |] 
    Assert.Equal(1, results.ExitCode)
    
[<Fact>]
let ``Valid and invalid links`` () =
    let results = runOnMultipleFiles [| "Samples/valid-file-link.md"; "Samples/invalid-url-link.md" |]
    Assert.Equal(1, results.ExitCode)