# GitVersion

**GitVersion** is a tool used to automatically generate semantic version numbers
based on your Git repository's history and branching strategy. It helps ensure
consistent versioning without manual updates, especially in CI/CD pipelines.

## How GitVersion Determines the Version 
GitVersion calculates the version by analyzing:
- The current branch (e.g., `main`, `develop`, `feature/*`, etc.)
- Git tags (especially `vX.Y.Z` style tags)
- Merge history and commit distances from tags
- Branch names and types, which influence pre-release labels

It uses Semantic Versioning (SemVer) as a base:
`MAJOR.MINOR.PATCH[-pre-release][+metadata]`

For example:
- main might produce `1.2.0`
- develop might produce `1.3.0-beta.1`
- feature/awesome might produce `1.3.0-feature-awesome.2`


## Main Configuration Modes

GitVersion supports two main operational modes that influence how versions are calculated:
1. Continuous Deployment (CD) (default)
    - Every commit gets a unique, incremented pre-release version.
    - Suitable for systems that deploy every commit (e.g., `1.0.0-alpha.3` → `1.0.0-alpha.4`).
    - Allows publishing to a package feed or deployment target without needing tags.
2. Continuous Delivery (CI)
    - Only tagged commits result in a new release version.
    - Other commits get incremented pre-release versions, but mainline version
        updates only happen on release.
    - More conservative—suitable for teams that release on milestones.


# Git branching strategy

The repository uses simplified **trunk-based strategy with feature branches**:
- `main` is always deployable
- Short-lived feature/<name> branches
- Releases triggered by merges to `main` and tagged automatically with appropriate semver tag

# GitHub Actions Workflow

Here’s how GitHub Actions Workflow works in the repository:
1. On merge to main:
    - GitVersion calculates next version (e.g., `v1.3.0`)
    - GHA workflow creates a git tag:
        `git tag v1.3.0 && git push origin v1.3.0`

2. On feature branches:
    - GitVersion calculates a preview version like `v1.3.0-feature-name.1`
    - Used for test builds but not tagged

The workflow uses a dockerized version of the GitVersion app as the simplest way to run the tool. The output of the tool is parsed using the `jq` utility.
```sh
$ docker run --rm -v "${{ github.workspace }}:/repository" \
        --env GITHUB_ACTIONS=true                        \
        --env GITHUB_REF=${{ github.ref }}               \
        gittools/gitversion:6.3.0                        \
        /repository                                      \
    | jq -r 'to_entries[] | "\(.key)=\(.value)"' >> $GITHUB_OUTPUT
```

# C# test application
The application does only one thing: it displays an embedded version number.
The version number is baked in during a build step in the GHA workflow.
```sh
$ dotnet publish -p:Version=${{ steps.gitversion.outputs.FullSemVer }}
```
