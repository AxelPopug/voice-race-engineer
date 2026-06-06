# Dependabot Auto-Merge Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Enable squash auto-merge for Dependabot patch and minor pull requests after required checks pass.

**Architecture:** Add a narrowly scoped `pull_request` workflow that reads trusted Dependabot metadata and enables GitHub auto-merge without checking out PR code. Document the repository policy and enable the repository-level auto-merge setting.

**Tech Stack:** GitHub Actions, `dependabot/fetch-metadata`, GitHub CLI, Markdown

---

### Task 1: Add Dependabot auto-merge workflow

**Files:**
- Create: `.github/workflows/dependabot-automerge.yml`

- [ ] Add a `pull_request` workflow restricted to `dependabot[bot]` pull requests targeting `main`.
- [ ] Fetch Dependabot metadata without checking out or executing pull-request code.
- [ ] Run `gh pr merge --auto --squash` only for `semver-patch` and `semver-minor`.

### Task 2: Document and enable policy

**Files:**
- Modify: `docs/development/repository-governance.md`

- [ ] Document automatic patch/minor merge and manual major review.
- [ ] Enable repository-level auto-merge while retaining squash-only merge settings.

### Task 3: Verify

- [ ] Run the repository quality gate.
- [ ] Validate workflow YAML syntax.
- [ ] Confirm the repository reports auto-merge enabled.
