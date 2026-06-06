# Session Handoff

Актуально на 2026-06-06.

## Product Goal

Voice Race Engineer — локальный голосовой помощник для iRacing. Пилот задает вопросы во время гонки и получает короткие звуковые ответы о расходе топлива, оставшейся дистанции, необходимой экономии и стратегии.

LLM используется только для понимания вопроса и диалога. Все гоночные числа вычисляет детерминированный и тестируемый Strategy Engine.

## Current Baseline

- Repository: `AxelPopug/voice-race-engineer`
- Default branch: `main`
- Runtime: `.NET 10`, C# 14
- Solution: `VoiceRaceEngineer.slnx`
- Existing projects: Domain, Strategy, Strategy tests
- Current tests: 4
- CI: restore locked, format verification, Release build and tests
- Open agent-ready tasks: GitHub issues `#1` — `#4`

Основные документы:

- `AGENTS.md`
- `docs/adr/0001-local-ai-race-engineer-overlay.md`
- `docs/architecture/0001-strategy-engine.md`
- `docs/plan/0001-technical-roadmap.md`
- `docs/plan/0002-agent-execution-plan.md`
- `docs/development/code-quality.md`
- `docs/development/repository-governance.md`

## Decisions And Constraints

- Voice-first; overlay является вторичным debug-интерфейсом.
- Live iRacing integration остается read-only.
- Не использовать packet sniffing, process injection, управление машиной или автоматические pit commands.
- Domain и Strategy остаются кроссплатформенными и не зависят от SDK, voice backends, UI или LLM.
- Strategy Engine возвращает explicit result для недостаточных или ошибочных данных.
- Для стратегической математики предпочитать `decimal` и типизированные единицы.
- Разработка live adapter требует Windows, установленного iRacing и отдельного spike.
- До отдельного подтверждения использование ограничено личным тестированием, Test Drive, AI Racing и согласованными закрытыми Hosted-сессиями.

## Repository Governance

`main` защищен серверными правилами GitHub:

- изменения только через pull request;
- required check `Format, build, and test`;
- branch должна быть актуальна;
- force push и deletion запрещены;
- linear history и закрытие review conversations обязательны;
- правила применяются к администраторам.

Required approval временно отключен, потому что единственный collaborator не может одобрить собственный pull request. Merge strategy: squash only; merged branches удаляются автоматически.

## Next Development Wave

Ближайшая цель — завершить `W1: Fuel Mathematics`. Четыре задачи готовы для параллельных агентов:

| Issue | Task | Ownership |
|---|---|---|
| `#2` | `W1-01` Typed measurements and validation | `Measurements.cs`, новый Domain tests project |
| `#1` | `W1-02` Lap-limited remaining distance | новый `RemainingDistance.cs` и tests |
| `#3` | `W1-03` K-lap fuel-saving scenario | новый `FuelScenarioPlanning.cs` и tests |
| `#4` | `W1-04` FuelPlanning boundary/property-style tests | только новый Strategy test file |

После завершения волны выполнить одну integration-задачу `W1-05`: проверить математические контракты, устранить дублирование API и прогнать полный quality gate.

## Start Of Next Session

1. Проверить доступные в новой сессии Skills, Plugins, Connectors и multi-agent tools. Не полагаться на список инструментов предыдущей сессии.
2. Прочитать `AGENTS.md` и этот handoff.
3. Проверить `git status`, актуальность `main`, открытые PR и issues.
4. Для параллельной волны создать отдельные worktree/ветки и выдать агентам disjoint write zones из execution plan.
5. Не начинать Windows/iRacing integration до завершения Fuel Mathematics и replay-контрактов.

Quality gate:

```bash
dotnet restore VoiceRaceEngineer.slnx --locked-mode
dotnet format VoiceRaceEngineer.slnx --verify-no-changes --no-restore
dotnet build VoiceRaceEngineer.slnx --configuration Release --no-restore
dotnet test VoiceRaceEngineer.slnx --configuration Release --no-build --no-restore
git diff --check
```

