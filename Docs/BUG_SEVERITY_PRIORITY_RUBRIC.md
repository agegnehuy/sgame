# Bug Severity & Priority Rubric

Use this rubric to keep QA and development decisions consistent.

## Severity (impact-based)

### Blocker

Use when:
- App cannot launch or crashes immediately.
- Core flow is impossible (cannot start/finish mission).
- Data corruption/loss for profiles/progress in normal usage.

Examples:
- Crash on opening `HomeScene`.
- Mission completion always fails and cannot continue.

### Critical

Use when:
- Major feature broken with no workaround.
- Incorrect behavior severely damages learning outcomes.
- Save/export produces invalid or misleading data.

Examples:
- Unlock logic fails and progression cannot continue.
- Export report outputs wrong mission scores.

### High

Use when:
- Important feature partially broken but workaround exists.
- Frequent incorrect UI/logic affecting many users.
- Localization errors that block understanding of actions.

Examples:
- Language switch misses key gameplay labels.
- Store purchase succeeds but equip state not persisted.

### Medium

Use when:
- Feature works but with noticeable defects.
- Edge-case logic issues with limited scope.
- Visual/UX issues that do not block learning task completion.

Examples:
- Summary panel updates only after manual refresh.
- Minor score text formatting errors.

### Low

Use when:
- Cosmetic, typo, minor alignment issues.
- Improvement suggestions with no functional impact.

Examples:
- Spacing issue in status text.
- Non-critical wording refinement.

---

## Priority (delivery urgency)

### P0 (fix immediately)

- Must fix before any further testing/demo.
- Typically Blocker or top Critical in core loop.

### P1 (fix this sprint)

- Required for pilot candidate quality.
- Usually Critical/High affecting mission, save, progression, localization.

### P2 (fix if capacity allows)

- Important but can ship pilot with known workaround/risk accepted.
- Usually Medium.

### P3 (backlog/polish)

- Low impact items; schedule later.

---

## Mapping guideline

- **Blocker -> P0**
- **Critical -> P0/P1**
- **High -> P1**
- **Medium -> P2**
- **Low -> P3**

If uncertain, choose higher severity temporarily and let triage reduce it.

---

## Triage SLA (recommended)

- **P0:** acknowledge within 1 hour, fix same day.
- **P1:** acknowledge same day, fix within 1-2 days.
- **P2:** schedule in next sprint planning.
- **P3:** add to backlog with product owner review.

---

## Educational safety override

Even if technically minor, increase severity by one level if the bug:
- teaches unsafe road behavior,
- gives incorrect safety feedback,
- mis-scores unsafe behavior as safe.
