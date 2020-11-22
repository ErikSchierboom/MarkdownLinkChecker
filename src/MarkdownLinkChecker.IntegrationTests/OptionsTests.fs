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

    [<Theory>]
    [<InlineData("-e")>]
    [<InlineData("--exclude")>]
    let ``Exclude option`` (excludeArg) =
        let results =
            run [| excludeArg
                   "invalid-url-link.md"
                   "invalid-file-link.md"
                   "--directory"
                   "Fixtures" </> "ValidAndInvalid" |]

        Assert.ExitedWithoutError(results)

module FilesOptionTests =

    [<Theory>]
    [<InlineData("-f")>]
    [<InlineData("--files")>]
    let ``Files option`` (filesArg) =
        let results =
            run [| filesArg
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

    let runWithMode modeArg mode dir =
        run [| modeArg
               mode
               "--directory"
               dir |]
    
    module CheckAllLinksTests =

        [<Theory>]
        [<InlineData("-m", "a")>]
        [<InlineData("-m", "all")>]
        [<InlineData("--mode", "a")>]
        [<InlineData("--mode", "all")>]
        let ``Only valid links`` (modeArg, mode) =
            let results = runWithMode modeArg mode ("Fixtures" </> "OnlyValid")

            Assert.ExitedWithoutError(results)

        [<Theory>]
        [<InlineData("-m", "a")>]
        [<InlineData("-m", "all")>]
        [<InlineData("--mode", "a")>]
        [<InlineData("--mode", "all")>]
        let ``Only invalid links`` (modeArg, mode) =
            let results = runWithMode modeArg mode ("Fixtures" </> "OnlyInvalid")

            Assert.ExitedWithError(results)

        [<Theory>]
        [<InlineData("-m", "a")>]
        [<InlineData("-m", "all")>]
        [<InlineData("--mode", "a")>]
        [<InlineData("--mode", "all")>]
        let ``Valid and invalid links`` (modeArg, mode) =
            let results = runWithMode modeArg mode ("Fixtures" </> "ValidAndInvalid")

            Assert.ExitedWithError(results)

    module CheckFileLinksTests =

        [<Theory>]
        [<InlineData("-m", "f")>]
        [<InlineData("-m", "files")>]
        [<InlineData("--mode", "f")>]
        [<InlineData("--mode", "files")>]
        let ``Valid file links and invalid URLs`` (modeArg, mode) =
            let results = runWithMode modeArg mode ("Fixtures" </> "ValidFilesAndInvalidUrls")

            Assert.ExitedWithoutError(results)

        [<Theory>]
        [<InlineData("-m", "f")>]
        [<InlineData("-m", "files")>]
        [<InlineData("--mode", "f")>]
        [<InlineData("--mode", "files")>]
        let ``Invalid file links and valid URLs`` (modeArg, mode) =
            let results = runWithMode modeArg mode ("Fixtures" </> "ValidUrlsAndInvalidFiles")

            Assert.ExitedWithError(results)

    module CheckUrlLinksTests =

        [<Theory>]
        [<InlineData("-m", "u")>]
        [<InlineData("-m", "urls")>]
        [<InlineData("--mode", "u")>]
        [<InlineData("--mode", "urls")>]
        let ``Valid URLs and invalid file links`` (modeArg, mode) =
            let results = runWithMode modeArg mode ("Fixtures" </> "ValidUrlsAndInvalidFiles")

            Assert.ExitedWithoutError(results)

        [<Theory>]
        [<InlineData("-m", "u")>]
        [<InlineData("-m", "urls")>]
        [<InlineData("--mode", "u")>]
        [<InlineData("--mode", "urls")>]
        let ``Invalid URLs and valid file links`` (modeArg, mode) =
            let results = runWithMode modeArg mode ("Fixtures" </> "ValidFilesAndInvalidUrls")

            Assert.ExitedWithError(results)
