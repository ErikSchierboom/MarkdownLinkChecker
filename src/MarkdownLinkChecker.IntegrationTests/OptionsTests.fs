module MarkdownLinkChecker.IntegrationTests.OptionsTests

open System.IO
open System.Reflection
open Xunit

open MarkdownLinkChecker.IntegrationTests.Runner

[<Fact>]
[<ExecuteInDirectory("Samples/OnlyValid")>]
let ``No options`` () =
    let results = run [|  |]
    Assert.Equal(0, results.ExitCode)

[<Fact>]
let ``Directory option`` () =
    let results = run [| "--directory"; "Samples/OnlyValid" |]
    Assert.Equal(0, results.ExitCode)

[<Fact>]
[<ExecuteInDirectory("Samples/ValidAndInvalid")>]
let ``Exclude option`` () =
    let results = run [| "--exclude"; "invalid-url-link.md"; "invalid-file-link.md" |]
    Assert.Equal(0, results.ExitCode)

[<Fact>]
[<ExecuteInDirectory("Samples/ValidAndInvalid")>]
let ``Files option`` () =
    let results = run [| "--files"; "valid-url-link.md"; "invalid-file-link.md" |]
    Assert.Equal(1, results.ExitCode)
