# Contribution guidelines for Rules Framework

Contribution to Rules.Framework is deeply appreciated as we evolve this project to serve the purpose of providing a easy to use, configurable and extensible rules framework for everyone, but some ground guidelines must be ensured to everyone aligned and allow the usage of some automation on top of this repository.

## Found a bug, what to do?

First of all, search [existent issues](https://github.com/luispfgarces/rules-framework/issues?q=is%3Aissue+is%3Aopen+label%3Abug+) for any open issue with the bug you found, it might have been already reported. If you find a matching issue to yout bug you can subscribe notifications for it and/or complement with additional information in the comments section.

If you don't see a matching issue, please go ahead and create a new one. Make sure you include the following information on issue description:

- OS (Windows, Ubuntu, Mac, etc)
- .Net SDK version you are using
- Exception message and stack trace (when applicable)
- A repro of the issue (optional, but can be requested later)

## New feature request/improvement

It might a good idea to first take a look at [all open issues w/ enhancement tag](https://github.com/luispfgarces/rules-framework/issues?q=is%3Aissue+is%3Aopen+label%3Aenhancement+) before opening a new feature request or improvement. If you are:

- **Requesting a new feature with widespread impacts on whole solution and you want to implement it** - please submit a new issue with a design proposal so that we can discuss it before proceeding with implementation.
- **Requesting a new feature with small changes on codebase or a improvement** - please submit a new issue and you can provide a implementation for it.
- **Only requesting a new feature or a improvement** - please submit a new issue.

Whatever you want to do, please make sure to outline sample usage scenarios so that everyone understands your intended usage scenarios.

## I want to contribute

First of all, ensure you have appropriate .Net SDK installed (need .Net Core 3.1 and .Net Standard 2.0) and ensure you have a MongoDB instance running locally at port 27017.

1. Fork repository.

1. Clone repository locally.

1. Create a new branch.

    ```shell
    $ git checkout -b my-branch
    ```

1. Perform your code changes, including covering unit tests and integration tests.

1. [Windows Environment] After your code changes are done, under a PowerShell terminal and located at local repository root, run:

    ```shell
    PS> .\run-tests.ps1
    ```
    Open \<your-repo-root>/coverage-outputs/report/index.html on your favorite browser to preview HTML coverage report.

1. Commit your changes to codebase - make sure you use [conventional commits](https://www.conventionalcommits.org/en/v1.0.0/) specification (changelog will be supported sometime in the future, and this will be important to use automatic tooling on top of commit messages).

1. Push changes to your forked repository remote.

    ```shell
    $ git push origin my-branch
    ```

1. Open a pull request with your changes targeting to `luispfgarces/rules-framework:master`. Link any issues solved on pull request.

1. Changes will be automatically run though Appveyor CI (build + test + SonarQube upload) for you pull request. Please make sure all checks are green on GitHub.

1. Changes might be suggested or requested. If that happens, please make sure to have all tests passing after changes are done.

1. Please do not merge incoming changes from master branch, perform a rebase instead.

    ```shell
    $ git fetch upstream master:master
    $ git rebase master -i
    $ git push --force origin master
    ```

After all designated reviewers accept the changes (a contributor will set the reviewers), changes will be merged master.

## Code rules

To keep source code clean and comprehensive, please make sure you follow these rules of thumb:

- Public classes, structures, interfaces, properties and methods must be documented. Private, protected and internal ones are excluded from this rule.
- Usings must be placed inside namespace. System usings always come first.
- Any warning suppression must be added to global suppression files, and a justification is mandatory.
- Unit tests are separated per each class - suppose you are testing class `Rule`, you should place all tests for its' members under `RuleTests`.
- Unit tests naming must be done under the pattern `MemberName_Conditions_ExpectedOutcome`. See examples above for guidance.

    ```csharp
    // RuleTests.cs
    public void NewRule_GivenRuleWithStringConditionTypeAndContainsOperator_BuildsAndReturnsRule() { }

    // RuleBuilderExtensionsTests.cs
    public void WithSerializedContent_GivenNullContentSerializationProvider_ThrowsArgumentNullException() { }
    ```

- Unit tests body structure must follow the *Arrange, Act & Assert* pattern.
- Solution line coverage must be kept over 90% at all times.
- `[ExcludeFromCodeCoverage]` can be used exceptionally where used dependencies of class are difficult to test. Make sure your design makes the best effort to abstract these dependencies to promote testability before considering using this attribute.

## Commit rules

Commit rules follow as baseline the [conventional commits](https://www.conventionalcommits.org/en/v1.0.0/) specification, to allow the build of automation over commit messages.

All commit messages should use following rules:

- Do not capitalize message first letter.
- Do not place a dot '.' at the end.
- Specify imperative and present tense messages (e.g. "fix rules evaluation").

### Allowed types

- ci: affecting build or continuous integration systems.
- docs: changes to documentation.
- feat: addition of new features.
- fix: solving bugs.
- refactor: changes to source code that do not solve a bug or add a new feature.
- test: adding new tests or fixing failing tests.

## Documentation rules

If source code changes affect a documented functionality or technical documentation, documentation must be updated on pull request with those changes. If the code changes add a new functionality and none of the existent documentation is affected, it is not required to document new functionality under same pull request, although it should be done at some point in time.