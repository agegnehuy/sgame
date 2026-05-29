# Week 2 Execution Board

This board turns Week 2 goals into daily, owner-assigned execution tasks.

## Week 2 Objective

Deliver a stable **multi-mission MVP foundation** with:

- P1-M1, P1-M2, P1-M3 mission node flow,
- scenario preset-based difficulty,
- progression summary panel,
- facilitator report export,
- runtime EN/AM language switching.

## Team Roles

- **TL**: Tech Lead
- **GE**: Gameplay Engineer
- **UIE**: UI Engineer
- **DE**: Data/Systems Engineer
- **QA**: QA Tester
- **SME**: Road-safety/Education reviewer

---

## Day 1 (Monday) — Mission Foundation Wiring

### Tasks

- **GE**: Create and configure `Preset_P1_M1`, `Preset_P1_M2`, `Preset_P1_M3`.
- **GE**: Add `MissionScenarioApplier` in `P1_M1` and assign all references.
- **UIE**: Add mission nodes for M1/M2/M3 in HomeScene.
- **UIE**: Wire `MissionMapController` on each node.
- **TL**: Validate mission selection state flow with `PlayerSession`.

### Done criteria

- Mission nodes load without null errors.
- Clicking an unlocked node enters mission scene.
- Mission presets visibly change mission behavior.

---

## Day 2 (Tuesday) — Progression and UI Visibility

### Tasks

- **UIE**: Add `MissionProgressPanel` rows for M1/M2/M3.
- **UIE**: Wire `MissionProgressSummaryController` bindings.
- **DE**: Validate score persistence and unlock chain read logic.
- **QA**: Run unlock regression (`P1-M1 -> P1-M2 -> P1-M3`).

### Done criteria

- Summary panel updates best score/stars/attempts after each mission.
- Unlock labels match progression rules.
- Profile switching updates summary immediately.

---

## Day 3 (Wednesday) — Export + Data Integrity

### Tasks

- **DE**: Wire `FacilitatorReportExportController` in HomeScene.
- **DE**: Validate generated JSON structure and field completeness.
- **QA**: Compare exported data against in-app data for at least 2 profiles.
- **TL**: Define export file retention guideline (manual cleanup policy).

### Done criteria

- Export button creates JSON file in `persistentDataPath/exports`.
- Status text shows saved path.
- Exported profile/mission values are accurate.

---

## Day 4 (Thursday) — Runtime Localization Stabilization

### Tasks

- **UIE**: Wire `LanguageToggleController` buttons and status text.
- **UIE**: Confirm all panels refresh on `LanguageChanged`.
- **GE/UIE**: Wire optional `LocalizationRefreshController` QA button.
- **QA**: Run language-switch regression in all Home/Mission panels.

### Done criteria

- EN <-> AM switches without scene reload.
- No stale labels remain after switch.
- Store/summary/export labels localize correctly.

---

## Day 5 (Friday) — Full Regression + Triage Sweep

### Tasks

- **QA**: Execute `WEEK2_REGRESSION_CHECKLIST.md` end-to-end.
- **All devs**: Fix P0/P1 defects discovered during regression.
- **TL + QA**: Run daily triage using `QA_TRIAGE_WORKFLOW.md`.
- **SME**: Verify safety feedback and mission correctness in EN/AM.

### Done criteria

- No open P0 defects.
- No open P1 defects in core loop:
  - mission start/complete
  - save/progression
  - language switching
  - export integrity

---

## Saturday Buffer — Stabilization

### Tasks

- **All devs**: Address remaining P2 issues if time allows.
- **QA**: Re-run failed cases from prior days.
- **TL**: Prepare Week 2 completion summary.

### Done criteria

- Reopened bug count reduced.
- Build is demo-ready for stakeholders.

---

## Week 2 Deliverables

- Updated HomeScene with:
  - mission map nodes (M1/M2/M3),
  - progress summary panel,
  - language toggle,
  - report export control.
- Scenario preset assets and applier wiring completed.
- Regression report with bug status and closure summary.

## Week 2 Exit Gate

- Core flow stable on at least 2 Android devices.
- All mission/data/localization/export regressions pass.
- QA artifacts complete:
  - checklist results,
  - bug reports,
  - triage log.
