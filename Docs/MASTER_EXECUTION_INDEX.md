# Safe Steps Addis — Master Execution Index

Use this file as the single entry point for planning, implementation, QA, and pilot delivery.

## 1) Start Here (Implementation Onboarding)

1. `WEEK1_EXECUTION.md`  
   Current running log of completed work and milestones.
2. `WEEK1_DAY0_UNITY_SETUP_30MIN.md`  
   Fast Day-0 onboarding to reach a validated HomeScene quickly.
3. `WEEK1_UNITY_SCENE_WIRING.md`  
   Core playable mission wiring (`P1_M1`).
4. `WEEK1_HOME_AND_MAP_WIRING.md`  
   HomeScene flow, mission map, store, profile, language, export wiring.

## 2) Week-by-Week Delivery Boards

- `WEEK2_EXECUTION_BOARD.md`
- `WEEK3_EXECUTION_BOARD.md`
- `WEEK4_EXECUTION_BOARD.md`
- `WEEK5_TO_WEEK8_MACRO_BOARD.md`

Use in sequence; each board builds on the previous week’s outputs.

## 3) Mission and Feature Expansion Guides

- `WEEK2_FOUNDATION_SETUP.md`  
  Multi-mission presets and scenario applier setup.
- `WEEK3_EVENT_SYSTEM_SETUP.md`  
  Deterministic distraction/pressure event wiring for M4/M5.
- `WEEK3_ANALYTICS_LOGGING_SETUP.md`  
  Offline analytics event logging for decisions/challenges/mission summaries.
- `WEEK3_ANALYTICS_SUMMARY_EXPORT_SETUP.md`  
  Facilitator-readable aggregated analytics export wiring.
- `WEEK3_ANALYTICS_PANEL_SETUP.md`  
  In-app analytics insights panel for active profile monitoring.
- `WEEK3_FACILITATOR_DASHBOARD_SETUP.md`  
  Unified HomeScene facilitator dashboard toggle and wiring.
- `WEEK4_FACILITATOR_MAINTENANCE_SETUP.md`  
  Safe facilitator maintenance actions (clear/reset tools).
- `WEEK4_HOME_SCENE_SECURITY_WIRING_CHECKLIST.md`  
  Copy-check HomeScene wiring checklist for PIN security + maintenance controls.
- `WEEK4_HOME_SCENE_QA_VARIANT_CHECKLIST.md`  
  Guidance for maintaining separate production and QA HomeScene security configurations.
- `WEEK4_UI_WIREFRAME_PACK.md`  
  Text wireframes for HomeScene, facilitator overlays, HUD, and result UI layout.
- `WEEK4_UI_PREFAB_CHECKLIST.md`  
  Prefab/component checklist for reusable UI assembly in Unity.
- `WEEK4_UI_PREFAB_BINDING_TABLE.md`  
  Strict inspector field-to-object mapping for key UI controllers.
- `WEEK4_HOME_SCENE_FINAL_WIRING_PASS.md`  
  One-pass final checklist combining UI wiring, security checks, and sign-off prep.
- `WEEK4_DAY0_AND_FINAL_WIRING_COMBINED_CHECKLIST.md`  
  Printable combined checklist for setup sessions (Day-0 through final wiring pass).
- `WEEK4_OPERATOR_CHEAT_SHEET_1PAGE.md`  
  Ultra-condensed onsite operator checklist for rapid setup/validation.
- `WEEK4_OPERATOR_CHEAT_SHEET_AM.md`  
  Amharic-first version of the onsite operator quick checklist.
- `WEEK4_PIN_SECURITY_QUICK_TEST.md`  
  Fast facilitator PIN security verification before builds/pilot drops.
- `WEEK2_PROGRESS_PANEL_WIRING.md`  
  Mission progress summary panel wiring.
- `WEEK2_FACILITATOR_EXPORT.md`  
  Facilitator report export setup.

## 4) QA and Regression

- `WEEK1_QA_CHECKLIST.md`  
  Initial MVP functional checks.
- `WEEK2_REGRESSION_CHECKLIST.md`  
  Cross-system regression after Week 2 additions.
- `WEEK4_SECURITY_REGRESSION_MATRIX.md`  
  Daily pass/fail tracker for facilitator PIN security scenarios.
- `WEEK4_SECURITY_DAILY_LOG_TEMPLATE.md`  
  Daily archive template for recording security matrix results.
- `WEEK4_SECURITY_DOCS_MAP.md`  
  Security QA navigation map (quick test -> matrix -> log -> bug report).
- `WEEK4_SECURITY_RELEASE_GATE_CHECKLIST.md`  
  Final security go/no-go gate before pilot/release candidate builds.
- `WEEK4_SECURITY_RELEASE_GATE_SAMPLE.md`  
  Filled reference example for completing the security release gate.
- `SECURITY_SIGNOFF_TEMPLATE.md`  
  Reusable template for build-specific security sign-off records.
- `SecuritySignoff_2026-05-07_rc-0.5.0.md`  
  Build-specific security sign-off record for rc-0.5.0.
- `BUG_REPORT_TEMPLATE.md`  
  Standard issue report format.
- `WEEK4_SECURITY_BUG_TEMPLATE.md`  
  Security-focused bug report format for PIN/cooldown/lockout/audit defects.
- `BUG_SEVERITY_PRIORITY_RUBRIC.md`  
  Severity/priority classification rules.
- `QA_TRIAGE_WORKFLOW.md`  
  Daily triage and retest process.

## 5) Performance and Stability

- `PERFORMANCE_BASELINE_TEMPLATE.md`  
  Device-tier performance capture and sprint-to-sprint comparison.

## 6) Release and Pilot Operations

- `RELEASE_FREEZE_POLICY.md`  
  Scope control during release stabilization.
- `PILOT_READINESS_CHECKLIST.md`  
  Go/no-go technical and educational gate.
- `PILOT_HANDOFF_PACKAGE.md`  
  Final package checklist for schools/facilitators.
- `ENDGAME_EXECUTION_RUNBOOK.md`  
  End-to-end final execution sequence from current state to pilot completion.
- `FINAL_ACCEPTANCE_CHECKLIST.md`  
  Last gate checklist before final pilot handoff approval.

## 7) Pilot Learning Loop and Next Roadmap

- `PILOT_FEEDBACK_SYNTHESIS_TEMPLATE.md`  
  Convert pilot observations into actionable fixes.
- `POST_PILOT_BACKLOG_FRAMEWORK.md`  
  Prioritize post-pilot backlog by safety/learning impact.

## 8) Daily Operating Templates

- `WEEK2_DAILY_STANDUP_TEMPLATE.md`  
  Reusable daily execution and blocker format.

## Recommended Daily Use Flow

1. Standup using `WEEK2_DAILY_STANDUP_TEMPLATE.md`
2. Execute tasks from current week board
3. Test with QA/regression checklist
4. Log issues via bug template and rubric
5. Triage with workflow rules
6. Update `WEEK1_EXECUTION.md`
