module MarkdownLinkChecker.IntegrationTests.OptionsTests

open Xunit

open MarkdownLinkChecker.IntegrationTests.Runner

module NoOptionsTests =

    [<Fact>]
    [<ExecuteInDirectory("Fixtures/OnlyValid")>]
    let ``Only valid links`` () =
        let results = run [||]

        Assert.ExitedWithoutError(results)

    [<Fact>]
    [<ExecuteInDirectory("Fixtures/OnlyInvalid")>]
    let ``Only invalid links`` () =
        let results = run [||]

        Assert.ExitedWithError(results)

    [<Fact>]
    [<ExecuteInDirectory("Fixtures/ValidAndInvalid")>]
    let ``Valid and invalid links`` () =
        let results = run [||]

        Assert.ExitedWithError(results)

module DirectoryOptionTests =

    let directoryArgs: obj [] [] = [| [| "-d" |]; [| "--directory" |] |]

    [<Theory>]
    [<MemberData(nameof directoryArgs)>]
    let ``Only valid links`` (directoryArg) =
        let results =
            run [| directoryArg
                   "Fixtures" </> "OnlyValid" |]

        Assert.ExitedWithoutError(results)

    [<Theory>]
    [<MemberData(nameof directoryArgs)>]
    let ``Only invalid links`` (directoryArg) =
        let results =
            run [| directoryArg
                   "Fixtures" </> "OnlyInvalid" |]

        Assert.ExitedWithError(results)

    [<Theory>]
    [<MemberData(nameof directoryArgs)>]
    let ``Valid and invalid links`` (directoryArg) =
        let results =
            run [| directoryArg
                   "Fixtures" </> "ValidAndInvalid" |]

        Assert.ExitedWithError(results)

module ExcludeOptionTests =

    [<Theory>]
    [<InlineData("-e")>]
    [<InlineData("--exclude")>]
    let ``Excludes passed using space as separator`` (excludeArg) =
        let results =
            run [| excludeArg
                   "invalid-url-link.md"
                   "invalid-file-link.md"
                   "--directory"
                   "Fixtures" </> "ValidAndInvalid" |]

        Assert.ExitedWithoutError(results)

    [<Theory>]
    [<InlineData("-e")>]
    [<InlineData("--exclude")>]
    let ``Excludes passed using newline as separator`` (excludeArg) =
        let results =
            run [| excludeArg
                   "invalid-url-link.md\ninvalid-file-link.md"
                   "--directory"
                   "Fixtures" </> "ValidAndInvalid" |]

        Assert.ExitedWithoutError(results)

module FilesOptionTests =

    [<Theory>]
    [<InlineData("-f")>]
    [<InlineData("--files")>]
    let ``Files passed using space as separator`` (filesArg) =
        let results =
            run [| filesArg
                   "valid-url-link.md"
                   "invalid-file-link.md"
                   "--directory"
                   "Fixtures" </> "ValidAndInvalid" |]

        Assert.ExitedWithError(results)

    [<Theory>]
    [<InlineData("-f")>]
    [<InlineData("--files")>]
    let ``Files passed using newline as separator`` (filesArg) =
        let results =
            run [| filesArg
                   "valid-url-link.md\nvalid-file-link.md"
                   "--directory"
                   "Fixtures" </> "ValidAndInvalid" |]

        Assert.ExitedWithoutError(results)

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

    module NoVerbosityTests =

        [<Fact>]
        let ``Outputs invalid files`` () =
            let results = runWithoutVerbosity ()

            Assert.ContainsFileNames(validFileNames, results)

        [<Fact>]
        let ``Outputs valid files`` () =
            let results = runWithoutVerbosity ()

            Assert.ContainsFileNames(invalidFileNames, results)

        [<Fact>]
        let ``Outputs invalid links`` () =
            let results = runWithoutVerbosity ()

            Assert.ContainsInvalidLinks(results)

        [<Fact>]
        let ``Does not output valid links`` () =
            let results = runWithoutVerbosity ()

            Assert.DoesNotContainValidLinks(results)

    module MinimalVerbosityTests =

        let minimalVerbosityArgs: obj [] [] =
            [| [| "-v"; "m" |]
               [| "-v"; "minimal" |]
               [| "--verbosity"; "m" |]
               [| "--verbosity"; "minimal" |] |]

        [<Theory>]
        [<MemberData(nameof minimalVerbosityArgs)>]
        let ``Outputs invalid files`` (verbosityArg, verbosity) =
            let results = runWithVerbosity verbosityArg verbosity

            Assert.ContainsFileNames(invalidFileNames, results)

        [<Theory>]
        [<MemberData(nameof minimalVerbosityArgs)>]
        let ``Does not output valid files`` (verbosityArg, verbosity) =
            let results = runWithVerbosity verbosityArg verbosity

            Assert.DoesNotContainFileNames(validFileNames, results)

        [<Theory>]
        [<MemberData(nameof minimalVerbosityArgs)>]
        let ``Outputs invalid links`` (verbosityArg, verbosity) =
            let results = runWithVerbosity verbosityArg verbosity

            Assert.ContainsInvalidLinks(results)

        [<Theory>]
        [<MemberData(nameof minimalVerbosityArgs)>]
        let ``Does not output valid links`` (verbosityArg, verbosity) =
            let results = runWithVerbosity verbosityArg verbosity

            Assert.DoesNotContainValidLinks(results)


    module NormalVerbosityTests =

        let normalVerbosityArgs: obj [] [] =
            [| [| "-v"; "n" |]
               [| "-v"; "normal" |]
               [| "--verbosity"; "n" |]
               [| "--verbosity"; "normal" |] |]

        [<Theory>]
        [<MemberData(nameof normalVerbosityArgs)>]
        let ``Outputs invalid files`` (verbosityArg, verbosity) =
            let results = runWithVerbosity verbosityArg verbosity

            Assert.ContainsFileNames(invalidFileNames, results)

        [<Theory>]
        [<MemberData(nameof normalVerbosityArgs)>]
        let ``Outputs valid files`` (verbosityArg, verbosity) =
            let results = runWithVerbosity verbosityArg verbosity

            Assert.ContainsFileNames(validFileNames, results)

        [<Theory>]
        [<MemberData(nameof normalVerbosityArgs)>]
        let ``Outputs invalid links`` (verbosityArg, verbosity) =
            let results = runWithVerbosity verbosityArg verbosity

            Assert.ContainsInvalidLinks(results)

        [<Theory>]
        [<MemberData(nameof normalVerbosityArgs)>]
        let ``Does not output valid links`` (verbosityArg, verbosity) =
            let results = runWithVerbosity verbosityArg verbosity

            Assert.DoesNotContainValidLinks(results)

    module DetailedVerbosityTests =

        let detailedVerbosityArgs: obj [] [] =
            [| [| "-v"; "d" |]
               [| "-v"; "detailed" |]
               [| "--verbosity"; "d" |]
               [| "--verbosity"; "detailed" |] |]

        [<Theory>]
        [<MemberData(nameof detailedVerbosityArgs)>]
        let ``Outputs invalid files`` (verbosityArg, verbosity) =
            let results = runWithVerbosity verbosityArg verbosity

            Assert.ContainsFileNames(invalidFileNames, results)

        [<Theory>]
        [<MemberData(nameof detailedVerbosityArgs)>]
        let ``Outputs valid files`` (verbosityArg, verbosity) =
            let results = runWithVerbosity verbosityArg verbosity

            Assert.ContainsFileNames(validFileNames, results)

        [<Theory>]
        [<MemberData(nameof detailedVerbosityArgs)>]
        let ``Outputs invalid links`` (verbosityArg, verbosity) =
            let results = runWithVerbosity verbosityArg verbosity

            Assert.ContainsInvalidLinks(results)

        [<Theory>]
        [<MemberData(nameof detailedVerbosityArgs)>]
        let ``Outputs valid links`` (verbosityArg, verbosity) =
            let results = runWithVerbosity verbosityArg verbosity

            Assert.ContainsInvalidLinks(results)

    module QuietVerbosityTests =

        let quietVerbosityArgs: obj [] [] =
            [| [| "-v"; "q" |]
               [| "-v"; "quiet" |]
               [| "--verbosity"; "q" |]
               [| "--verbosity"; "quiet" |] |]

        [<Theory>]
        [<MemberData(nameof quietVerbosityArgs)>]
        let ``Does not output invalid files`` (verbosityArg, verbosity) =
            let results = runWithVerbosity verbosityArg verbosity

            Assert.DoesNotContainFileNames(invalidFileNames, results)

        [<Theory>]
        [<MemberData(nameof quietVerbosityArgs)>]
        let ``Does not output valid files`` (verbosityArg, verbosity) =
            let results = runWithVerbosity verbosityArg verbosity

            Assert.DoesNotContainFileNames(validFileNames, results)

        [<Theory>]
        [<MemberData(nameof quietVerbosityArgs)>]
        let ``Does not output invalid links`` (verbosityArg, verbosity) =
            let results = runWithVerbosity verbosityArg verbosity

            Assert.DoesNotContainInvalidLinks(results)

        [<Theory>]
        [<MemberData(nameof quietVerbosityArgs)>]
        let ``Does not output valid links`` (verbosityArg, verbosity) =
            let results = runWithVerbosity verbosityArg verbosity

            Assert.DoesNotContainValidLinks(results)

module ModeOptionTests =

    let runWithMode modeArg mode dir =
        run [| modeArg
               mode
               "--directory"
               dir |]

    module CheckAllLinksTests =

        let modeAllArgs: obj [] [] =
            [| [| "-m"; "a" |]
               [| "-m"; "all" |]
               [| "--mode"; "a" |]
               [| "--mode"; "all" |] |]

        [<Theory>]
        [<MemberData(nameof modeAllArgs)>]
        let ``Only valid links`` (modeArg, mode) =
            let results =
                runWithMode modeArg mode ("Fixtures" </> "OnlyValid")

            Assert.ExitedWithoutError(results)

        [<Theory>]
        [<MemberData(nameof modeAllArgs)>]
        let ``Only invalid links`` (modeArg, mode) =
            let results =
                runWithMode modeArg mode ("Fixtures" </> "OnlyInvalid")

            Assert.ExitedWithError(results)

        [<Theory>]
        [<MemberData(nameof modeAllArgs)>]
        let ``Valid and invalid links`` (modeArg, mode) =
            let results =
                runWithMode modeArg mode ("Fixtures" </> "ValidAndInvalid")

            Assert.ExitedWithError(results)

    module CheckFileLinksTests =

        let modeFilesArgs: obj [] [] =
            [| [| "-m"; "f" |]
               [| "-m"; "files" |]
               [| "--mode"; "f" |]
               [| "--mode"; "files" |] |]

        [<Theory>]
        [<MemberData(nameof modeFilesArgs)>]
        let ``Valid file links and invalid URLs`` (modeArg, mode) =
            let results =
                runWithMode modeArg mode ("Fixtures" </> "ValidFilesAndInvalidUrls")

            Assert.ExitedWithoutError(results)

        [<Theory>]
        [<MemberData(nameof modeFilesArgs)>]
        let ``Invalid file links and valid URLs`` (modeArg, mode) =
            let results =
                runWithMode modeArg mode ("Fixtures" </> "ValidUrlsAndInvalidFiles")

            Assert.ExitedWithError(results)

    module CheckUrlLinksTests =

        let modeUrlsArgs: obj [] [] =
            [| [| "-m"; "u" |]
               [| "-m"; "urls" |]
               [| "--mode"; "u" |]
               [| "--mode"; "urls" |] |]

        [<Theory>]
        [<MemberData(nameof modeUrlsArgs)>]
        let ``Valid URLs and invalid file links`` (modeArg, mode) =
            let results =
                runWithMode modeArg mode ("Fixtures" </> "ValidUrlsAndInvalidFiles")

            Assert.ExitedWithoutError(results)

        [<Theory>]
        [<MemberData(nameof modeUrlsArgs)>]
        let ``Invalid URLs and valid file links`` (modeArg, mode) =
            let results =
                runWithMode modeArg mode ("Fixtures" </> "ValidFilesAndInvalidUrls")

            Assert.ExitedWithError(results)
