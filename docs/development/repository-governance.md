# Repository Governance

## Desired Main Branch Policy

Новый код должен попадать в `main` только через pull request.

Требуемые настройки:

- pull request обязателен перед merge;
- approval пока не обязателен, так как у репозитория один collaborator;
- review conversations должны быть закрыты;
- CI check `Format, build, and test` обязателен;
- branch должна быть актуальна перед merge;
- force push запрещен;
- deletion `main` запрещен;
- правила применяются и к администраторам;

## Current GitHub Protection

На дату 2026-06-06 публичный репозиторий защищает `main` серверными правилами GitHub:

- изменения попадают в `main` только через pull request;
- required check `Format, build, and test` должен пройти, а branch должна быть актуальна;
- review conversations должны быть закрыты;
- force push и deletion запрещены;
- linear history обязательна;
- правила применяются к администраторам.

Обязательный approval временно отключен: единственный collaborator не может одобрить собственный pull request. После добавления второго reviewer следует включить минимум одно approval и сброс approvals после последнего push.

## Dependabot Auto-Merge

Dependabot pull requests с patch- и minor-обновлениями автоматически получают
squash auto-merge. GitHub выполняет merge только после прохождения всех правил
защиты `main`, включая required check `Format, build, and test` и требование
актуальной branch.

Major-обновления остаются открытыми для ручной проверки. Workflow не checkout-ит
и не исполняет код из Dependabot pull request.

## Protection Verification

При изменении правил проверить:

- direct push отклоняется;
- force push отклоняется;
- merge без CI отклоняется;
- Dependabot patch/minor PR автоматически merge-ится после CI;
- Dependabot major PR остается открытым;
- после добавления второго reviewer merge без approval отклоняется;
- deletion `main` отклоняется.
