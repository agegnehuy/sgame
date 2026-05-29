# QA Triage Workflow (Daily)

This workflow keeps bug handling fast during the 2-month MVP timeline.

## 1) Daily cadence

- QA executes test cases from:
  - `WEEK1_QA_CHECKLIST.md`
  - `WEEK2_REGRESSION_CHECKLIST.md`
- Each failed case becomes one report using:
  - `BUG_REPORT_TEMPLATE.md`

## 2) Triage meeting (15-20 min/day)

Attendees:
- Tech lead
- Gameplay/UI dev representative
- QA lead/tester
- Product or education owner (if safety behavior affected)

Agenda:
1. Review new bugs since last triage.
2. Assign severity + priority using `BUG_SEVERITY_PRIORITY_RUBRIC.md`.
3. Assign owner and target build.
4. Mark blocked dependencies.

## 3) Status flow

- `Open` -> `In Progress` -> `Fixed` -> `Ready for Retest` -> `Closed`
- Optional:
  - `Duplicate`
  - `Rejected` (must include reason)
  - `Deferred` (with target milestone)

## 4) Retest rules

- QA retests only on build where fix is claimed.
- If failed:
  - reopen same bug id,
  - add new evidence and exact build version.
- If passed:
  - mark `Closed`,
  - link test case id.

## 5) Release gate rules (pilot build)

Cannot release pilot candidate if any remain:
- Open `P0`
- Open `P1` in core flow (mission start/finish/save/progression/language)
- Any educational safety override bug unresolved

## 6) Reporting dashboard (minimum)

Track daily:
- Total open bugs
- Open by priority (P0/P1/P2/P3)
- Reopened bug count
- Fix verification pass rate

## 7) Educational validation checkpoint

For any bug touching:
- safety evaluator,
- score logic,
- feedback messages,
- sign/signal interpretation,

require one explicit sign-off from education/road-safety reviewer before closing.
