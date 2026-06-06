## Summary

Describe the behavior changed by this pull request.

## Task

- Task ID / issue:
- Ownership respected:

## Verification

- [ ] `dotnet restore VoiceRaceEngineer.slnx --locked-mode`
- [ ] `dotnet format VoiceRaceEngineer.slnx --verify-no-changes --no-restore`
- [ ] `dotnet build VoiceRaceEngineer.slnx --configuration Release --no-restore`
- [ ] `dotnet test VoiceRaceEngineer.slnx --configuration Release --no-build --no-restore`
- [ ] `git diff --check`
- [ ] New behavior has tests
- [ ] Documentation updated when contracts changed
- [ ] No analyzer rule was disabled to bypass a problem

## Assumptions And Risks

List assumptions, known limitations, and follow-up work.
