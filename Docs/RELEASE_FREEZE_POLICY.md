# Release Freeze Policy (Pilot Track)

This policy protects the 2-month schedule from late scope creep.

## Freeze start point

Start freeze at beginning of Week 4 Day 1.

## Allowed during freeze

- P0 and P1 bug fixes only.
- Critical performance/stability fixes.
- Critical localization corrections that affect comprehension.
- Build/release scripting and packaging tasks.

## Not allowed during freeze

- New gameplay features.
- New UI flows.
- Refactors not tied to active P0/P1 bug.
- New content scope (extra missions/assets) unless emergency approved.

## Exception process

Any exception must be approved by:
- Tech Lead, and
- Product/Program owner.

Required details for exception:
- reason,
- user/learning impact,
- risk to timeline,
- rollback plan.

## Branching guidance

- Keep a release branch (`release/pilot-rc`).
- Merge only reviewed freeze-approved changes.
- Tag every RC candidate build.

## Validation requirement for each freeze fix

- Must include:
  - bug id reference,
  - retest evidence,
  - build where verified.

## Freeze exit conditions

- All P0 closed.
- All P1 in core flow closed.
- Pilot readiness checklist passes.
- Go/no-go review completed.
