module MarkdownLinkChecker.IntegrationTests.IgnoreTests

open Xunit

open MarkdownLinkChecker.IntegrationTests.Runner


[<Fact>]
let ``Single file not ignored`` () =
    let unignoredFile =
        "Fixtures" </> "Ignores" </> "Deeper" </> "about.md"

    let results = runWithSingleFile unignoredFile

    Assert.Contains(unignoredFile, results.Output)

[<Fact>]
let ``Single file ignored`` () =
    let ignoredFile =
        "Fixtures"
        </> "Ignores"
        </> "Deeper"
        </> "EvenDeeper"
        </> "introduction.md"

    let results = runWithSingleFile ignoredFile

    Assert.DoesNotContain(ignoredFile, results.Output)

[<Fact>]
let ``Multiple files all ignored`` () =
    let ignoredFiles =
        [| "Fixtures"
           </> "Ignores"
           </> "Deeper"
           </> "MoreNesting"
           </> "part-4.md"
           "Fixtures"
           </> "Ignores"
           </> "Deeper"
           </> "MoreNesting"
           </> "part-5.md" |]

    let results = runWithMultipleFiles ignoredFiles

    Assert.All(ignoredFiles, (fun fileToCheck -> Assert.DoesNotContain(fileToCheck, results.Output)))

[<Fact>]
let ``Multiple files partially ignored`` () =
    let ignoredFile =
        "Fixtures" </> "Ignores" </> "Deeper" </> "EvenDeeper" </> "part-1.md"

    let unignoredFile =
        "Fixtures" </> "Ignores" </> "Deeper" </> "EvenDeeper" </> "part-2.md"

    let results =
        runWithMultipleFiles [| ignoredFile
                                unignoredFile |]

    Assert.DoesNotContain(ignoredFile, results.Output)
    Assert.Contains(unignoredFile, results.Output)

[<Fact>]
let ``Multiple files none ignored`` () =
    let unignoredFiles =
        [| "Fixtures" </> "Ignores" </> "docs.md"
           "Fixtures" </> "Ignores" </> "Deeper" </> "AnotherSub" </> "sub.md" |]

    let results = runWithMultipleFiles unignoredFiles

    Assert.All(unignoredFiles, (fun fileToCheck -> Assert.Contains(fileToCheck, results.Output)))

[<Fact>]
let ``Directory excludes ignored`` () =
    let results =
        runWithDirectory ("Fixtures" </> "Ignores")

    let ignoredFiles =
        [| "introduction.md"
           "Deeper" </> "license.md"
           "Deeper" </> "EvenDeeper" </> "introduction.md"
           "Deeper" </> "EvenDeeper" </> "part-1.md"
           "Deeper" </> "EvenDeeper" </> "part-3.md"
           "Deeper" </> "MoreNesting" </> "part-4.md"
           "Deeper" </> "MoreNesting" </> "part-5.md" |]

    Assert.All(ignoredFiles, (fun expectedFilename -> Assert.DoesNotContain(expectedFilename, results.Output)))

[<Fact>]
let ``Directory does not exclude unignored`` () =
    let results =
        runWithDirectory ("Fixtures" </> "Ignores")

    let unignoredFiles =
        [| "docs.md"
           "links.md"
           "Deeper" </> "about.md"
           "Deeper" </> "AnotherSub" </> "license.md"
           "Deeper" </> "AnotherSub" </> "sub.md"
           "Deeper" </> "EvenDeeper" </> "part-2.md"
           "Deeper" </> "EvenDeeper" </> "VeryDeep" </> "draft-1.md"
           "Deeper" </> "EvenDeeper" </> "VeryDeep" </> "draft-2.md" |]

    Assert.All(unignoredFiles, (fun expectedFilename -> Assert.Contains(expectedFilename, results.Output)))
