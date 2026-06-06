# Documentation Workflow

Voice Race Engineer uses docs-as-code: documentation is part of the repository,
reviewed with code, and updated when a change affects decisions, product
direction, architecture, research, or user-visible behavior.

## Documentation Layers

| Layer | Path | Update when |
|---|---|---|
| Product direction | `docs/plan/`; future dedicated home: `docs/vision/` | Product goals, milestones, or non-goals change |
| Architecture decisions | `docs/adr/` | A meaningful architecture, technology, policy, or workflow decision is made |
| Current architecture | `docs/architecture/` | The current shape of modules, contracts, or boundaries changes |
| Development process | `docs/development/` | Workflow, quality gate, governance, or agent rules change |
| Research | `docs/research/` | New source-backed findings replace or qualify earlier assumptions |
| Agent plans/specs | `docs/superpowers/` | A Superpowers-driven implementation plan or design is intentionally preserved |

## Doc Impact Check

Every commit must answer whether documentation is affected. Use these rules:

- If a decision is made or reversed, create a new ADR or supersede an existing
  ADR. Do not rewrite accepted decisions as if history never changed.
- If architecture boundaries, public contracts, or module responsibilities
  change, update `docs/architecture/` or add an ADR.
- If product direction, milestone scope, or non-goals change, update the
  relevant plan or vision document.
- If research-backed knowledge changes, update `docs/research/` and mark
  unresolved items as hypotheses, assumptions, or unknowns.
- If only implementation details change and no durable knowledge changes, record
  `Doc Impact: none` with a short reason in the evidence audit or PR body.

## Evidence And Uncertainty

Documentation may contain uncertainty, but it must be labeled:

- `Confirmed`: backed by code, tests, source documents, replay data, Windows
  spike output, or tool output.
- `Assumption`: accepted for now but not directly verified.
- `Hypothesis`: requires later validation.
- `Unknown`: insufficient data.
- `Conflict`: contradictory evidence exists.

Correctness-critical `Conflict`, `Unknown`, and `Hypothesis` findings block a
commit until they are resolved or explicitly scoped out of the change.

## Pull Request Expectations

Every PR should include a short `Docs Impact` section:

```markdown
## Docs Impact

- Updated:
- Not updated because:
- Follow-up docs:
```

Use `None` explicitly for empty categories. Do not silently omit documentation
impact.

## Tooling Direction

The next tooling step is to add a small MkDocs setup:

- `mkdocs.yml`;
- `docs/index.md`;
- strict local/CI build;
- optional snippets for code/config references.

That step is intentionally separate from this workflow decision.
