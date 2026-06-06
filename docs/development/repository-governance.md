# Repository Governance

## Desired Main Branch Policy

Новый код должен попадать в `main` только через pull request.

Требуемые настройки:

- pull request обязателен перед merge;
- минимум одно approval;
- последний push сбрасывает старые approvals;
- review conversations должны быть закрыты;
- CI check `Format, build, and test` обязателен;
- branch должна быть актуальна перед merge;
- force push запрещен;
- deletion `main` запрещен;
- правила применяются и к администраторам;

## Current GitHub Limitation

На дату 2026-06-06 GitHub API возвращает `403` для Branch Protection и Repository Rulesets:

```text
Upgrade to GitHub Pro or make this repository public to enable this feature.
```

Репозиторий приватный и находится на бесплатном personal plan. Поэтому GitHub сейчас не может технически запретить direct push или force push.

Варианты включения реальной защиты:

1. Подключить GitHub Pro для владельца приватного репозитория.
2. Сделать репозиторий public.
3. Перенести репозиторий в организацию с тарифом, поддерживающим protected branches для private repositories.

До устранения ограничения команда следует процессу добровольно:

- любые изменения выполняются в `codex/*` или feature branches;
- изменения попадают в `main` через pull request;
- merge выполняется только после зеленого CI;
- прямой push и force push в `main` запрещены процессом, но пока не GitHub-политикой.

## Applying Protection After Upgrade

После изменения тарифа настроить branch protection или ruleset и проверить:

- direct push отклоняется;
- force push отклоняется;
- merge без CI отклоняется;
- merge без approval отклоняется;
- deletion `main` отклоняется.
