module MarkdownLinkChecker.IntegrationTests.OptionsTests

open Xunit

open MarkdownLinkChecker.IntegrationTests.Runner

module NoOptionsTests =

    [<Fact>]
    let ``Only valid links`` () =
        let results =
            run [| "--directory"
                   "Fixtures" </> "OnlyValid" |]

        Assert.Equal(0, results.ExitCode)

    [<Fact>]
    let ``Only invalid links`` () =
        let results =
            run [| "--directory"
                   "Fixtures" </> "OnlyInvalid" |]

        Assert.Equal(1, results.ExitCode)

    [<Fact>]
    let ``Valid and invalid links`` () =
        let results =
            run [| "--directory"
                   "Fixtures" </> "ValidAndInvalid" |]

        Assert.Equal(1, results.ExitCode)

module DirectoryOptionTests =

    [<Fact>]
    let ``Only valid links`` () =
        let results =
            run [| "--directory"
                   "Fixtures" </> "OnlyValid" |]

        Assert.Equal(0, results.ExitCode)

    [<Fact>]
    let ``Only invalid links`` () =
        let results =
            run [| "--directory"
                   "Fixtures" </> "OnlyInvalid" |]

        Assert.Equal(1, results.ExitCode)

    [<Fact>]
    let ``Valid and invalid links`` () =
        let results =
            run [| "--directory"
                   "Fixtures" </> "ValidAndInvalid" |]

        Assert.Equal(1, results.ExitCode)

module ExcludeOptionTests =

    [<Fact>]
    let ``Exclude option`` () =
        let results =
            run [| "--exclude"
                   "invalid-url-link.md"
                   "invalid-file-link.md"
                   "--directory"
                   "Fixtures" </> "ValidAndInvalid" |]

        Assert.Equal(0, results.ExitCode)

module FilesOptionTests =

    [<Fact>]
    let ``Files option`` () =
        let results =
            run [| "--files"
                   "valid-url-link.md"
                   "invalid-file-link.md"
                   "--directory"
                   "Fixtures" </> "ValidAndInvalid" |]

        Assert.Equal(1, results.ExitCode)

module VerbosityOptionTests =

    [<Fact>]
    let ``No verbosity specified outputs logging`` () =
        let results =
            run [| "--directory"
                   "Fixtures" </> "OnlyValid" |]

        Assert.NotEmpty(results.Output)

    [<Fact>]
    let ``Normal verbosity outputs logging`` () =
        let results =
            run [| "--verbosity"
                   "normal"
                   "--directory"
                   "Fixtures" </> "OnlyValid" |]

        Assert.NotEmpty(results.Output)

    [<Fact>]
    let ``Detailed verbosity outputs logging`` () =
        let results =
            run [| "--verbosity"
                   "detailed"
                   "--directory"
                   "Fixtures" </> "OnlyValid" |]

        Assert.NotEmpty(results.Output)

    [<Fact>]
    let ``Quiet verbosity does not output logging`` () =
        let results =
            run [| "--verbosity"
                   "quiet"
                   "--directory"
                   "Fixtures" </> "OnlyValid" |]

        Assert.Empty(results.Output)

module ModeOptionTests =

    module CheckAllLinksTests =

        [<Fact>]
        let ``Only valid links`` () =
            let results =
                run [| "--mode"
                       "all"
                       "--directory"
                       "Fixtures" </> "OnlyValid" |]

            Assert.Equal(0, results.ExitCode)

        [<Fact>]
        let ``Only invalid links`` () =
            let results =
                run [| "--mode"
                       "all"
                       "--directory"
                       "Fixtures" </> "OnlyInvalid" |]

            Assert.Equal(1, results.ExitCode)

        [<Fact>]
        let ``Valid and invalid links`` () =
            let results =
                run [| "--mode"
                       "all"
                       "--directory"
                       "Fixtures" </> "ValidAndInvalid" |]

            Assert.Equal(1, results.ExitCode)

    module CheckFileLinksTests =

        [<Fact>]
        let ``Valid file links and invalid URLs`` () =
            let results =
                run [| "--mode"
                       "files"
                       "--directory"
                       "Fixtures" </> "ValidFilesAndInvalidUrls" |]

            Assert.Equal(0, results.ExitCode)

        [<Fact>]
        let ``Invalid file links and valid URLs`` () =
            let results =
                run [| "--mode"
                       "files"
                       "--directory"
                       "Fixtures" </> "ValidUrlsAndInvalidFiles" |]

            Assert.Equal(1, results.ExitCode)

    module CheckUrlLinksTests =

        [<Fact>]
        let ``Valid URLs and invalid file links`` () =
            let results =
                run [| "--mode"
                       "urls"
                       "--directory"
                       "Fixtures" </> "ValidUrlsAndInvalidFiles" |]

            Assert.Equal(0, results.ExitCode)

        [<Fact>]
        let ``Invalid URLs and valid file links`` () =
            let results =
                run [| "--mode"
                       "urls"
                       "--directory"
                       "Fixtures" </> "ValidFilesAndInvalidUrls" |]

            Assert.Equal(1, results.ExitCode)
