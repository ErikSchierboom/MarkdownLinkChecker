module MarkdownLinkChecker.IntegrationTests.OptionsTests

open Xunit

open MarkdownLinkChecker.IntegrationTests.Runner

module NoOptionsTests =

    [<Fact>]
    let ``Only valid links`` () =
        let results =
            run [| "--directory"
                   "Fixtures" </> "OnlyValid" |]

        Assert.ExitedWithoutError(results)

    [<Fact>]
    let ``Only invalid links`` () =
        let results =
            run [| "--directory"
                   "Fixtures" </> "OnlyInvalid" |]

        Assert.ExitedWithError(results)

    [<Fact>]
    let ``Valid and invalid links`` () =
        let results =
            run [| "--directory"
                   "Fixtures" </> "ValidAndInvalid" |]

        Assert.ExitedWithError(results)

module DirectoryOptionTests =

    [<Fact>]
    let ``Only valid links`` () =
        let results =
            run [| "--directory"
                   "Fixtures" </> "OnlyValid" |]

        Assert.ExitedWithoutError(results)

    [<Fact>]
    let ``Only invalid links`` () =
        let results =
            run [| "--directory"
                   "Fixtures" </> "OnlyInvalid" |]

        Assert.ExitedWithError(results)

    [<Fact>]
    let ``Valid and invalid links`` () =
        let results =
            run [| "--directory"
                   "Fixtures" </> "ValidAndInvalid" |]

        Assert.ExitedWithError(results)

module ExcludeOptionTests =

    [<Fact>]
    let ``Exclude option`` () =
        let results =
            run [| "--exclude"
                   "invalid-url-link.md"
                   "invalid-file-link.md"
                   "--directory"
                   "Fixtures" </> "ValidAndInvalid" |]

        Assert.ExitedWithoutError(results)

module FilesOptionTests =

    [<Fact>]
    let ``Files option`` () =
        let results =
            run [| "--files"
                   "valid-url-link.md"
                   "invalid-file-link.md"
                   "--directory"
                   "Fixtures" </> "ValidAndInvalid" |]

        Assert.ExitedWithError(results)

module VerbosityOptionTests =
    
    let private validFileNames =
        [ "valid-file-link.md"
          "valid-url-link.md" ]

    let private invalidFileNames =
        [ "invalid-file-link.md"
          "invalid-url-link.md" ]
    
    let private runWithoutVerbosity () =
        runWithDirectory ("Fixtures" </> "ValidAndInvalid")
        
    let private runWithVerbosity verbosityArg verbosity =
            run [| verbosityArg
                   verbosity
                   "--directory"
                   "Fixtures" </> "ValidAndInvalid" |]
    
    [<Fact>]
    let ``No verbosity outputs invalid files`` () =
        let results = runWithoutVerbosity ()

        Assert.ContainsFileNames(validFileNames, results)

    [<Fact>]
    let ``No verbosity outputs valid files`` () =
        let results = runWithoutVerbosity ()

        Assert.ContainsFileNames(invalidFileNames, results)

    [<Theory>]
    [<InlineData("-v", "m")>]
    [<InlineData("-v", "minimal")>]
    [<InlineData("--verbosity", "m")>]
    [<InlineData("--verbosity", "minimal")>]
    let ``Minimal verbosity outputs invalid files`` (verbosityArg, verbosity) =
        let results = runWithVerbosity verbosityArg verbosity

        Assert.ContainsFileNames(invalidFileNames, results)

    [<Theory>]
    [<InlineData("-v", "m")>]
    [<InlineData("-v", "minimal")>]
    [<InlineData("--verbosity", "m")>]
    [<InlineData("--verbosity", "minimal")>]
    let ``Minimal verbosity does not output valid files`` (verbosityArg, verbosity) =
        let results = runWithVerbosity verbosityArg verbosity

        Assert.DoesNotContainFileNames(validFileNames, results)

    [<Theory>]
    [<InlineData("-v", "n")>]
    [<InlineData("-v", "normal")>]
    [<InlineData("--verbosity", "n")>]
    [<InlineData("--verbosity", "normal")>]
    let ``Normal verbosity outputs invalid files`` (verbosityArg, verbosity) =
        let results = runWithVerbosity verbosityArg verbosity

        Assert.ContainsFileNames(invalidFileNames, results)
        
    [<Theory>]
    [<InlineData("-v", "n")>]
    [<InlineData("-v", "normal")>]
    [<InlineData("--verbosity", "n")>]
    [<InlineData("--verbosity", "normal")>]
    let ``Normal verbosity outputs valid files`` (verbosityArg, verbosity) =
        let results = runWithVerbosity verbosityArg verbosity

        Assert.ContainsFileNames(validFileNames, results)

    [<Theory>]
    [<InlineData("-v", "d")>]
    [<InlineData("-v", "detailed")>]
    [<InlineData("--verbosity", "d")>]
    [<InlineData("--verbosity", "detailed")>]
    let ``Detailed verbosity outputs invalid files`` (verbosityArg, verbosity) =
        let results = runWithVerbosity verbosityArg verbosity

        Assert.ContainsFileNames(invalidFileNames, results)

    [<Theory>]
    [<InlineData("-v", "d")>]
    [<InlineData("-v", "detailed")>]
    [<InlineData("--verbosity", "d")>]
    [<InlineData("--verbosity", "detailed")>]
    let ``Detailed verbosity outputs valid files`` (verbosityArg, verbosity) =
        let results = runWithVerbosity verbosityArg verbosity

        Assert.ContainsFileNames(validFileNames, results)

    [<Theory>]
    [<InlineData("-v", "q")>]
    [<InlineData("-v", "quiet")>]
    [<InlineData("--verbosity", "q")>]
    [<InlineData("--verbosity", "quiet")>]
    let ``Quiet verbosity does not output invalid files`` (verbosityArg, verbosity) =
        let results = runWithVerbosity verbosityArg verbosity

        Assert.DoesNotContainFileNames(invalidFileNames, results)

    [<Theory>]
    [<InlineData("-v", "q")>]
    [<InlineData("-v", "quiet")>]
    [<InlineData("--verbosity", "q")>]
    [<InlineData("--verbosity", "quiet")>]
    let ``Quiet verbosity does not output valid files`` (verbosityArg, verbosity) =
        let results = runWithVerbosity verbosityArg verbosity

        Assert.DoesNotContainFileNames(validFileNames, results)

module ModeOptionTests =

    module CheckAllLinksTests =

        [<Fact>]
        let ``Only valid links`` () =
            let results =
                run [| "--mode"
                       "all"
                       "--directory"
                       "Fixtures" </> "OnlyValid" |]

            Assert.ExitedWithoutError(results)

        [<Fact>]
        let ``Only invalid links`` () =
            let results =
                run [| "--mode"
                       "all"
                       "--directory"
                       "Fixtures" </> "OnlyInvalid" |]

            Assert.ExitedWithError(results)

        [<Fact>]
        let ``Valid and invalid links`` () =
            let results =
                run [| "--mode"
                       "all"
                       "--directory"
                       "Fixtures" </> "ValidAndInvalid" |]

            Assert.ExitedWithError(results)

    module CheckFileLinksTests =

        [<Fact>]
        let ``Valid file links and invalid URLs`` () =
            let results =
                run [| "--mode"
                       "files"
                       "--directory"
                       "Fixtures" </> "ValidFilesAndInvalidUrls" |]

            Assert.ExitedWithoutError(results)

        [<Fact>]
        let ``Invalid file links and valid URLs`` () =
            let results =
                run [| "--mode"
                       "files"
                       "--directory"
                       "Fixtures" </> "ValidUrlsAndInvalidFiles" |]

            Assert.ExitedWithError(results)

    module CheckUrlLinksTests =

        [<Fact>]
        let ``Valid URLs and invalid file links`` () =
            let results =
                run [| "--mode"
                       "urls"
                       "--directory"
                       "Fixtures" </> "ValidUrlsAndInvalidFiles" |]

            Assert.ExitedWithoutError(results)

        [<Fact>]
        let ``Invalid URLs and valid file links`` () =
            let results =
                run [| "--mode"
                       "urls"
                       "--directory"
                       "Fixtures" </> "ValidFilesAndInvalidUrls" |]

            Assert.ExitedWithError(results)
