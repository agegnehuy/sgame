# Week 3 Execution Board

Week 3 focuses on expanding playable Phase 1 content quality and preparing a stable pilot candidate path.

## Week 3 Objective

Ship a polished **Phase 1 mission set baseline** with:

- stronger mission differentiation (M1-M5 behavior),
- improved player guidance and feedback consistency,
- low/mid Android performance stabilization pass,
- pilot-oriented quality gates.

## Roles

- **TL**: Tech Lead
- **GE**: Gameplay Engineer
- **UIE**: UI Engineer
- **DE**: Data/Systems Engineer
- **TA**: Technical Artist
- **QA**: QA Tester
- **SME**: Education/Road-safety reviewer

---

## Day 1 (Monday) — Mission Script Expansion

### Tasks
- **GE**: Define mission-specific parameters for M4 and M5 in scenario presets.
- **GE**: Add mission notes for distraction and pressure events (non-random deterministic triggers).
- **SME**: Validate each mission objective maps to a clear safety behavior.
- **QA**: Smoke test all mission presets for completion feasibility.

### Done criteria
- M1-M5 have distinct behavior profiles.
- No mission has contradictory feedback logic.

---

## Day 2 (Tuesday) — Feedback and UX Consistency

### Tasks
- **UIE**: Standardize message hierarchy (safe/risk/unsafe) and on-screen timing.
- **UIE**: Verify all result/summary/store labels localize correctly in EN and AM.
- **GE**: Ensure all critical decisions produce a valid reason key.
- **QA**: Run bilingual UI consistency pass.

### Done criteria
- No raw localization keys shown in UI.
- Feedback appears consistently within acceptable delay.

---

## Day 3 (Wednesday) — Performance Baseline Pass

### Tasks
- **TA**: Review geometry/material budget in HomeScene and P1_M1 scene.
- **GE**: Reduce per-frame allocations in gameplay update paths.
- **TL**: Establish baseline metrics per device tier.
- **QA**: Profile 10-minute runs on low/mid test phones.

### Done criteria
- Stable gameplay with no severe frame drop spikes.
- No memory growth trend in repeated mission loops.

---

## Day 4 (Thursday) — Data and Export Robustness

### Tasks
- **DE**: Verify all save writes are resilient during rapid scene transitions.
- **DE**: Validate export snapshot after multi-profile usage.
- **QA**: Interrupt-flow tests (pause/resume, quick app close/reopen, rapid replay).

### Done criteria
- No progress loss in interruption scenarios.
- Export remains valid and readable after stress usage.

---

## Day 5 (Friday) — Full Regression and Defect Burn-Down

### Tasks
- **QA**: Run Week 1 + Week 2 checklists fully.
- **All Devs**: Fix all P0/P1 defects.
- **TL**: Update risk register and pilot blockers.
- **SME**: Validate educational integrity in final test run.

### Done criteria
- Zero open P0.
- Zero P1 in core flow.
- Safety-learning logic approved by SME.

---

## Saturday Buffer — Pilot Candidate Snapshot

### Tasks
- Build candidate APK.
- Capture known issues list.
- Freeze non-critical scope additions.

### Done criteria
- Candidate is suitable for controlled pilot demo/testing.

---

## Week 3 Deliverables

- Updated mission presets and balancing notes.
- Performance baseline report (low/mid devices).
- Regression completion report.
- Candidate APK with issue tracker summary.

## Week 3 Exit Gate

- Full mission loop stable and testable in both languages.
- Performance acceptable on target devices.
- Pilot blockers identified or resolved.
