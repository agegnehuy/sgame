# Week 4 Execution Board

Week 4 focuses on stabilization, content freeze discipline, and pilot package readiness.

## Week 4 Objective

Produce a **pilot-ready release candidate** with:

- no blocker/critical core-flow defects,
- controlled scope freeze,
- validated build package and facilitator handoff assets.

## Roles

- **TL**: Tech Lead
- **GE**: Gameplay Engineer
- **UIE**: UI Engineer
- **DE**: Data/Systems Engineer
- **TA**: Technical Artist
- **QA**: QA Tester
- **SME**: Education/Road-safety reviewer
- **PM**: Delivery/coordination owner

---

## Day 1 (Monday) — Defect Prioritization Lock

### Tasks
- **QA**: Re-run high-risk cases from Week 2/Week 3 checklists.
- **TL + PM**: Freeze defect list and classify by P0/P1/P2.
- **All devs**: Start fixes for P0/P1 only.

### Done criteria
- Signed list of in-scope bugs for Week 4.
- No new feature work starts unless approved by TL/PM.

---

## Day 2 (Tuesday) — Core Flow Hardening

### Tasks
- **GE**: Resolve mission runtime and progression defects.
- **UIE**: Resolve localization/UI consistency defects.
- **DE**: Resolve save/export integrity defects.
- **QA**: Continuous retest loop on fixed issues.

### Done criteria
- Core loop failures are eliminated in latest candidate build.
- No reopened P0 fixes from Day 1.

---

## Day 3 (Wednesday) — Performance and Stability Gate

### Tasks
- **TA + GE**: Execute optimization pass from performance baseline findings.
- **QA**: Run 10–15 minute stress sessions on low and mid devices.
- **TL**: Review performance report and sign off or return for fixes.

### Done criteria
- Stable play sessions on target devices.
- No crash/ANR in stress tests.

---

## Day 4 (Thursday) — Packaging and Handoff Assets

### Tasks
- **PM + TL**: Generate release candidate APK (`RC`).
- **QA**: Execute pilot readiness checklist against RC.
- **PM**: Prepare facilitator handoff package (build + quick guide + known issues).
- **SME**: Final educational behavior pass in EN/AM.

### Done criteria
- RC package assembled and internally validated.
- Pilot readiness checklist has pass/fail log and owner notes.

---

## Day 5 (Friday) — Go/No-Go Decision

### Tasks
- **TL + PM + QA + SME**: Hold release gate review.
- Decide:
  - `GO` for pilot
  - `NO-GO` with blocker list and rollback plan
- **PM**: Publish decision summary and action list.

### Done criteria
- Formal go/no-go outcome documented.
- If GO: pilot package signed off.
- If NO-GO: concrete fix window and responsible owners assigned.

---

## Saturday Buffer — Emergency Fix Window

Use only for:
- unresolved P1 in core flow,
- high-impact localization misunderstanding,
- export/save reliability risks.

No scope expansion allowed.

---

## Week 4 Deliverables

- Release candidate APK.
- Completed pilot readiness checklist.
- Updated known issues list.
- Go/no-go decision record.

## Week 4 Exit Gate

- No open P0.
- No open P1 in mission, save, progression, localization, export.
- Educational behavior approved by SME.
