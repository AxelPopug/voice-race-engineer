# Code Quality

Проект использует встроенные Roslyn и .NET analyzers как основной линтер. Правила одинаково применяются локально, агентами и GitHub Actions.

## Quality Gate

Перед завершением любой code task выполнить:

```bash
dotnet restore VoiceRaceEngineer.slnx --locked-mode
dotnet format VoiceRaceEngineer.slnx --verify-no-changes --no-restore
dotnet build VoiceRaceEngineer.slnx --configuration Release --no-restore
dotnet test VoiceRaceEngineer.slnx --configuration Release --no-build --no-restore
git diff --check
```

Для автоматического исправления форматирования:

```bash
dotnet format VoiceRaceEngineer.slnx
```

## Где находятся правила

| Файл | Ответственность |
|---|---|
| `.editorconfig` | Форматирование, стиль C# и naming rules |
| `.gitattributes` | Единые line endings и binary-file policy |
| `Directory.Build.props` | C# 14, nullable, analyzers, warnings-as-errors, deterministic build |
| `Directory.Packages.props` | Централизованные версии NuGet |
| `packages.lock.json` | Воспроизводимое восстановление зависимостей |
| `.github/workflows/ci.yml` | Обязательная проверка format/build/test |
| `.github/dependabot.yml` | Регулярные обновления NuGet и GitHub Actions |
| `AGENTS.md` | Правила работы агентов |

## Политика предупреждений

- Все compiler и analyzer warnings являются ошибками.
- Code-style diagnostics с severity `warning` проверяются во время build.
- Нельзя глобально отключать analyzer rule для обхода конкретной проблемы.
- Локальное подавление допустимо только с коротким объяснением и отдельным review.
- `dotnet format --verify-no-changes` обязан завершаться без изменений.

## Основные соглашения C#

- File-scoped namespaces.
- `var` для non-primitive выражений; явные типы для встроенных числовых и строковых значений.
- Всегда использовать braces.
- Interfaces начинаются с `I`.
- Types и non-field members используют PascalCase.
- Private/internal fields используют `_camelCase`.
- Constants используют PascalCase.
- Максимальная длина строки: 130 символов.
- Публичные гоночные значения используют типизированные единицы.
- В Strategy Core для расчетов используется `decimal`.
- C# 14 применяется там, где улучшает читаемость, а не ради демонстрации новой syntax.

## Dependency Policy

- NuGet-версии задаются только в `Directory.Packages.props`.
- После изменения зависимости обновляются и коммитятся lock files.
- NuGet audit проверяет известные уязвимости direct и transitive dependencies.
- Новая dependency требует проверки лицензии и причины, почему стандартной библиотеки недостаточно.
- Не добавлять analyzer packages, дублирующие встроенные правила, без измеримой пользы.

## CI

GitHub Actions запускает quality gate на каждом push в `main` и на pull request. Windows-specific adapters позднее получат отдельный Windows job; кроссплатформенные Domain и Strategy проекты остаются проверяемыми на Linux.

## Источник соглашений

Базовые naming, spacing и Rider/ReSharper wrapping rules адаптированы из локальных репозиториев:

- `/Users/gorshkov/RiderProjects/scenarios/.editorconfig`;
- `/Users/gorshkov/RiderProjects/scenarios-runtime/.editorconfig`.

Не перенесены исторические отключения analyzer rules, табы, CRLF и отсутствие final newline. Для нового проекта сохранен более строгий quality gate.
