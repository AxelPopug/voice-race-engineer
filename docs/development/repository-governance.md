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

## Protection Verification

При изменении правил проверить:

- direct push отклоняется;
- force push отклоняется;
- merge без CI отклоняется;
- после добавления второго reviewer merge без approval отклоняется;
- deletion `main` отклоняется.
