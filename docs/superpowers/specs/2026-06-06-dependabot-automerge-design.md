# Dependabot Auto-Merge Design

## Goal

Automatically squash-merge Dependabot pull requests for patch and minor dependency
updates after all required branch-protection checks pass.

## Design

A `pull_request` workflow handles only pull requests opened against `main`
by `dependabot[bot]`. It uses `dependabot/fetch-metadata` to classify the semantic
version update and enables GitHub auto-merge only for `semver-patch` and
`semver-minor` updates.

The metadata action is pinned to a commit SHA. The workflow does not check out or
execute pull-request code. GitHub branch protection remains the merge gate, so the
existing `Format, build, and test` required check and up-to-date branch
requirement must pass before GitHub performs the squash merge. Major updates
remain manual.

## Permissions And Failure Behavior

The job receives only `contents: write` and `pull-requests: write`, which are
required to enable auto-merge. If metadata classification fails, the update type
is unsupported, or required checks fail, the pull request remains open.

Repository auto-merge must be enabled server-side for the workflow to work.
