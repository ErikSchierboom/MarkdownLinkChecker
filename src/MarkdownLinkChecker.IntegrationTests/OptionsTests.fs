module MarkdownLinkChecker.IntegrationTests.OptionsTests

open Xunit

open MarkdownLinkChecker.IntegrationTests.Runner

module NoOptionsTests =

    [<Fact>]
    [<ExecuteInDirectory("Fixtures/OnlyValid")>]
    let ``Only valid links`` () =
        let results = run [||]
        Assert.Equal(0, results.ExitCode)

    [<Fact>]
    [<ExecuteInDirectory("Fixtures/OnlyInvalid")>]
    let ``Only invalid links`` () =
        let results = run [||]
        Assert.Equal(1, results.ExitCode)

    [<Fact>]
    [<ExecuteInDirectory("Fixtures/ValidAndInvalid")>]
    let ``Valid and invalid links`` () =
        let results = run [||]
        Assert.Equal(1, results.ExitCode)

module DirectoryOptionTests =

    [<Fact>]
    let ``Only valid links`` () =
        let results =
            run [| "--directory"
                   "Fixtures/OnlyValid" |]

        Assert.Equal(0, results.ExitCode)

    [<Fact>]
    let ``Only invalid links`` () =
        let results =
            run [| "--directory"
                   "Fixtures/OnlyInvalid" |]

        Assert.Equal(1, results.ExitCode)

    [<Fact>]
    let ``Valid and invalid links`` () =
        let results =
            run [| "--directory"
                   "Fixtures/ValidAndInvalid" |]

        Assert.Equal(1, results.ExitCode)

module ExcludeOptionTests =

    [<Fact>]
    [<ExecuteInDirectory("Fixtures/ValidAndInvalid")>]
    let ``Exclude option`` () =
        let results =
            run [| "--exclude"
                   "invalid-url-link.md"
                   "invalid-file-link.md" |]

        Assert.Equal(0, results.ExitCode)

module FilesOptionTests =

    [<Fact>]
    [<ExecuteInDirectory("Fixtures/ValidAndInvalid")>]
    let ``Files option`` () =
        let results =
            run [| "--files"
                   "valid-url-link.md"
                   "invalid-file-link.md" |]

        Assert.Equal(1, results.ExitCode)

module VerbosityOptionTests =

    [<Fact>]
    [<ExecuteInDirectory("Fixtures/OnlyValid")>]
    let ``No verbosity specified outputs logging`` () =
        let results = run [||]
        Assert.NotEmpty(results.Output)

    [<Fact>]
    [<ExecuteInDirectory("Fixtures/OnlyValid")>]
    let ``Normal verbosity outputs logging`` () =
        let results = run [| "--verbosity"; "normal" |]
        Assert.NotEmpty(results.Output)

    [<Fact>]
    [<ExecuteInDirectory("Fixtures/OnlyValid")>]
    let ``Detailed verbosity outputs logging`` () =
        let results = run [| "--verbosity"; "detailed" |]
        Assert.NotEmpty(results.Output)

    [<Fact>]
    [<ExecuteInDirectory("Fixtures/OnlyValid")>]
    let ``Quiet verbosity does not output logging`` () =
        let results = run [| "--verbosity"; "quiet" |]
        Assert.Empty(results.Output)

module ModeOptionTests =

    module CheckAllLinksTests =

        [<Fact>]
        [<ExecuteInDirectory("Fixtures/OnlyValid")>]
        let ``Only valid links`` () =
            let results = run [| "--mode"; "all" |]
            Assert.Equal(0, results.ExitCode)

        [<Fact>]
        [<ExecuteInDirectory("Fixtures/OnlyInvalid")>]
        let ``Only invalid links`` () =
            let results = run [| "--mode"; "all" |]
            Assert.Equal(1, results.ExitCode)

        [<Fact>]
        [<ExecuteInDirectory("Fixtures/ValidAndInvalid")>]
        let ``Valid and invalid links`` () =
            let results = run [| "--mode"; "all" |]
            Assert.Equal(1, results.ExitCode)

    module CheckFileLinksTests =

        [<Fact>]
        [<ExecuteInDirectory("Fixtures/ValidFilesAndInvalidUrls")>]
        let ``Valid file links and invalid URLs`` () =
            let results = run [| "--mode"; "files" |]
            Assert.Equal(0, results.ExitCode)

        [<Fact>]
        [<ExecuteInDirectory("Fixtures/ValidUrlsAndInvalidFiles")>]
        let ``Invalid file links and valid URLs`` () =
            let results = run [| "--mode"; "files" |]
            Assert.Equal(1, results.ExitCode)

    module CheckUrlLinksTests =

        [<Fact>]
        [<ExecuteInDirectory("Fixtures/ValidUrlsAndInvalidFiles")>]
        let ``Valid URLs and invalid file links`` () =
            let results = run [| "--mode"; "urls" |]
            Assert.Equal(0, results.ExitCode)

        [<Fact>]
        [<ExecuteInDirectory("Fixtures/ValidFilesAndInvalidUrls")>]
        let ``Invalid URLs and valid file links`` () =
            let results = run [| "--mode"; "urls" |]
            Assert.Equal(1, results.ExitCode)
