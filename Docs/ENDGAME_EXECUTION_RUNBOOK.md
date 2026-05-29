# Endgame Execution Runbook

Use this runbook to drive the project from current state to pilot-ready completion.

## Phase 1: Unity Scene Completion

- Finalize `HomeScene` wiring using:
  - `WEEK4_HOME_SCENE_FINAL_WIRING_PASS.md`
  - `WEEK4_UI_PREFAB_BINDING_TABLE.md`
- Verify `MissionScene` (`P1_M1`) full loop:
  - briefing -> play -> result -> back home
- Confirm map progression (`P1-M1` to `P1-M5`) in runtime preset mode.

Exit criteria:

- No missing references
- Home and mission loops both playable

## Phase 2: Security and Facilitator Reliability

- Run PIN + maintenance checks:
  - `WEEK4_PIN_SECURITY_QUICK_TEST.md`
  - `WEEK4_SECURITY_REGRESSION_MATRIX.md`
- Complete one daily archive:
  - `WEEK4_SECURITY_DAILY_LOG_TEMPLATE.md`
- For RC build, complete:
  - `WEEK4_SECURITY_RELEASE_GATE_CHECKLIST.md`
  - sign-off file via `SECURITY_SIGNOFF_TEMPLATE.md`

Exit criteria:

- No open Critical/High security defects
- Audit logging and clear actions validated

## Phase 3: QA, Regression, and Performance

- Run:
  - `WEEK1_QA_CHECKLIST.md`
  - `WEEK2_REGRESSION_CHECKLIST.md`
- Capture performance baseline:
  - `PERFORMANCE_BASELINE_TEMPLATE.md`

Exit criteria:

- No P0 bugs
- No P1 bugs in core loop
- Stable 10-minute test on low/mid devices

## Phase 4: Pilot Package Readiness

- Complete:
  - `PILOT_READINESS_CHECKLIST.md`
  - `PILOT_HANDOFF_PACKAGE.md`
- Freeze scope using:
  - `RELEASE_FREEZE_POLICY.md`

Exit criteria:

- Pilot go/no-go = GO
- Facilitator package complete

## Phase 5: Post-Pilot Feedback Loop

- Use:
  - `PILOT_FEEDBACK_SYNTHESIS_TEMPLATE.md`
  - `POST_PILOT_BACKLOG_FRAMEWORK.md`
- Convert findings into prioritized backlog for Weeks 5-8.

---

## Daily execution rhythm (recommended)

1. Standup (`WEEK2_DAILY_STANDUP_TEMPLATE.md`)
2. Build/wire tasks
3. Security + regression checks
4. Bug filing and triage
5. Update `WEEK1_EXECUTION.md`
