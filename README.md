## markdownlinkchecker

[![Nuget](https://img.shields.io/nuget/v/markdownlinkchecker.svg)](https://www.nuget.org/packages/markdownlinkchecker) ![Test](https://github.com/ErikSchierboom/MarkdownLinkChecker/workflows/Test/badge.svg)

`markdownlinkchecker` is a tool to check Markdown links. Both file and URL links are checked.

### How To Install

The `markdownlinkchecker` nuget package is [published to nuget.org](https://www.nuget.org/packages/markdownlinkchecker/).

You can install the tool using the following command.

```console
dotnet tool install -g markdownlinkchecker
```

### How To Use

By default `markdownlinkchecker` will look in the current directory and its subdirectories for Markdown files. You can also exclude specific files using `-e` or check a specific list of files using `-f`. You can control how verbose the output will be by using the `-v` option.

```sh
Usage:
  markdownlinkchecker [options]

Options:
  -v, --verbosity <VERBOSITY> Set the verbosity level. Allowed values are q[uiet], m[inimal], n[ormal] (default) and [d]etailed.
  -d, --directory <DIRECTORY> The directory to operate on. Any relative file or directory paths specified in other options will be relative to this directory.
                              If not specified, the working directory is used.
  -e, --exclude <EXCLUDE>     A list of relative Markdown file or directory paths to exclude from checking.
  -f, --files <FILES>         A list of relative Markdown file or directory paths to check. All Markdown files are checked if empty.
  -m, --mode <MODE>           Which links to check. Allowed values are f[iles], u[rls] and a[ll] (default).
  --help                      Display how to use the tool.
  --version                   Display version information.
```

Add `markdownlinkchecker` after `dotnet` and before the command arguments that you want to run:

| Examples                                                    | Description                                                                |
| ----------------------------------------------------------- | -------------------------------------------------------------------------- |
| dotnet **markdownlinkchecker**                              | Check the Markdown files in the current directory and its subdirectories.  |
| dotnet **markdownlinkchecker** -d &lt;directory&gt;         | Check the Markdown files in a particular directory and its subdirectories. |
| dotnet **markdownlinkchecker** -v detailed                  | Check with very detailed logging.                                          |
| dotnet **markdownlinkchecker** -f README.md CONTRIBUTING.md | Check the README.md CONTRIBUTING.md files.                                 |
| dotnet **markdownlinkchecker** -e GENERATED.md              | Ignore the GENERATED.md file.                                              |
| dotnet **markdownlinkchecker** -m urls                      | Only check URL links.                                                      |

### Ignoring files

Files can be ignored through `.markdownignore` files. The format of this file matches that of the `.gitignore` file:

```
introduction.md
Contributing/
**/Docs/*.md
```

### How To Uninstall

You can uninstall the tool using the following command.

```console
dotnet tool uninstall -g markdownlinkchecker
```

### How To Build From Source

You can build and package the tool using the following commands. The instructions assume that you are in the root of the repository.

```console
dotnet pack -o artifacts
# The final line from the build will read something like
# Successfully created package '.\artifacts\MarkdownLinkChecker.0.1.0.nupkg'.
# Use the value that is in the form `0.1.0` as the version in the next command.
dotnet tool install --add-source .\artifacts markdownlinkchecker --version <version>
dotnet markdownlinkchecker
```

> Note: On macOS and Linux, `.\artifacts` will need be switched to `./artifacts` to accommodate for the different slash directions.
