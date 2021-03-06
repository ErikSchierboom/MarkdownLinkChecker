module MarkdownLinkChecker.IntegrationTests.Runner

open System
open System.IO

open System.Reflection
open MarkdownLinkChecker.Program
open Xunit
open Xunit.Sdk

type CheckResults = { ExitCode: int; Output: string }

// Required due to redirecting of console output
[<assembly:CollectionBehavior(CollectionBehavior.CollectionPerAssembly)>]
do ()

let run argv =
    use stringWriter = new StringWriter()
    Console.SetOut(stringWriter)
    Console.SetError(stringWriter)

    { ExitCode = main argv
      Output = stringWriter.ToString() }

let runWithSingleFile file = run [| "--files"; file |]

let runWithMultipleFiles files = run (Array.append [| "--files" |] files)

let runWithDirectory directory = run [| "--directory"; directory |]

let (</>) path1 path2 = Path.Combine(path1, path2)

[<AttributeUsage(AttributeTargets.Class ||| AttributeTargets.Method, AllowMultiple = false, Inherited = true)>]
type ExecuteInDirectory(directory) =
    inherit BeforeAfterTestAttribute()

    let mutable currentDirectory = ""

    override _.Before(_) =
        currentDirectory <- Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        Directory.SetCurrentDirectory(Path.Combine(currentDirectory, directory))

    override _.After(_) =
        Directory.SetCurrentDirectory(currentDirectory)

module Assert =
    let ContainsFileName (fileName, results) =
        Assert.Contains(sprintf "FILE: %s" fileName, results.Output)

    let ContainsFileNames (fileNames, results) =
        Assert.All(fileNames, (fun fileName -> ContainsFileName(fileName, results)))

    let DoesNotContainFileName (fileName, results) =
        Assert.DoesNotContain(sprintf "FILE: %s" fileName, results.Output)

    let DoesNotContainFileNames (fileNames, results) =
        Assert.All(fileNames, (fun fileName -> DoesNotContainFileName(fileName, results)))

    let ContainsValidLinks (results) = Assert.Contains('✅', results.Output)

    let DoesNotContainValidLinks (results) =
        Assert.DoesNotContain('✅', results.Output)

    let ContainsInvalidLinks (results) = Assert.Contains('❌', results.Output)

    let DoesNotContainInvalidLinks (results) =
        Assert.DoesNotContain('❌', results.Output)

    let ExitedWithoutError (results) = Assert.Equal(0, results.ExitCode)

    let ExitedWithError (results) = Assert.Equal(1, results.ExitCode)
