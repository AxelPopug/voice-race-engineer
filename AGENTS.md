# Agent Working Agreement

Этот файл задает правила для Codex-агентов, работающих с репозиторием Voice Race Engineer.

## Product Rules

- Voice-first: overlay является вторичным интерфейсом.
- LLM не вычисляет и не изменяет гоночные числа.
- Все числовые ответы приходят из детерминированного Strategy Engine.
- Live iRacing integration остается read-only до отдельного решения.
- Непроверенные особенности SDK оформляются как hypothesis и проверяются replay или Windows spike.

## Architecture Boundaries

- `VoiceRaceEngineer.Domain` содержит unit types, immutable contracts и не зависит от других проектов.
- `VoiceRaceEngineer.Strategy` содержит чистые детерминированные вычисления и зависит только от Domain.
- Telemetry adapters не передают типы сторонних SDK за пределы infrastructure-слоя.
- Voice adapters не зависят от iRacing SDK.
- Windows-only код не добавляется в кроссплатформенные Domain и Strategy проекты.
- Не добавлять LLM, UI или storage dependencies в Strategy Core.

## Required Workflow

Перед изменениями:

1. Прочитать связанную задачу и документы из `docs/`.
2. Проверить `git status`.
3. Не изменять файлы вне указанной в задаче write-зоны без явной необходимости.

После изменений:

```bash
dotnet restore VoiceRaceEngineer.slnx --locked-mode
dotnet format VoiceRaceEngineer.slnx --verify-no-changes --no-restore
dotnet build VoiceRaceEngineer.slnx --configuration Release --no-restore
dotnet test VoiceRaceEngineer.slnx --configuration Release --no-build --no-restore
git diff --check
```

В sandbox для `dotnet` команд может потребоваться разрешение на локальный IPC.

## Code Quality

- Target framework: `net10.0`; Windows adapters используют `net10.0-windows`.
- C# 14 разрешен, но новая syntax применяется только когда улучшает читаемость.
- Nullable, analyzers и warnings-as-errors обязательны.
- Все style и analyzer rules определяются в `.editorconfig` и `Directory.Build.props`.
- Версии NuGet указываются только в `Directory.Packages.props`; lock files коммитятся.
- Для стратегии использовать `decimal`, а не `double`, если нет измеренной причины иначе.
- Публичные числовые значения должны иметь типизированные единицы.
- Ошибочные или недостаточные данные возвращают explicit result, а не правдоподобное число.
- Каждый расчет должен быть детерминированным и тестируемым.

## Testing

- Новая формула требует unit и boundary tests.
- Исправление бага требует regression test.
- Для математических инвариантов добавлять property-style tests.
- Не ослаблять analyzers и не отключать warnings для обхода ошибки.
- Windows-specific интеграции сопровождаются adapter contract tests и отдельным Windows spike.

## Parallel Agent Rules

- Один агент владеет одной задачей и четко указанными файлами/модулями.
- Параллельные агенты не редактируют один файл.
- Изменения solution, central package management и общих build-файлов выполняет только integration task.
- Контракты сначала согласуются отдельной задачей; implementation-задачи зависят от них.
- Агент не должен отменять или переписывать изменения других агентов.
- Агент никогда не пушит напрямую в `main`; используется feature branch и pull request.

## Task Completion Report

В финальном сообщении агент перечисляет:

- что реализовано;
- какие файлы изменены;
- какие решения или assumptions приняты;
- какие команды проверки выполнены;
- известные ограничения и последующие задачи.

## Source Documents

- [Technical roadmap](docs/plan/0001-technical-roadmap.md)
- [Agent execution plan](docs/plan/0002-agent-execution-plan.md)
- [Code quality](docs/development/code-quality.md)
- [Repository governance](docs/development/repository-governance.md)
- [Strategy Engine architecture](docs/architecture/0001-strategy-engine.md)
- [iRacing SDK research](docs/research/0002-dotnet-iracing-sdk-libraries.md)
- [Local STT/TTS research](docs/research/0003-local-stt-tts-stack.md)
