# Safe Steps Addis (MVP)

3D mobile educational game prototype for child road-safety training (ages 8-14), localized for Ethiopia (English + Amharic), built for low/mid Android and offline usage.

## Quick Start (5 minutes)

1. Open this folder in Unity LTS (URP 3D project).
2. Review `Docs/MASTER_EXECUTION_INDEX.md`.
3. Wire scenes using:
   - `Docs/WEEK1_UNITY_SCENE_WIRING.md`
   - `Docs/WEEK1_HOME_AND_MAP_WIRING.md`
4. Configure mission presets using:
   - `Docs/WEEK2_FOUNDATION_SETUP.md`
5. Add scenes to build settings:
   - `HomeScene`
   - `P1_M1`

## Current Implementation Highlights

- Mission runtime flow (`Briefing -> Playing -> Result`)
- Safety evaluation and score/star calculation
- Sequential mission unlock logic (`P1-M1` to `P1-M5`)
- Profile, coins, store buy/equip flow
- Progress summary panel and facilitator report export
- Runtime language switching (EN/AM) with localization refresh support
- Data-driven mission tuning via scenario presets
- Offline analytics logging (decisions, challenge events, mission summaries)
- Aggregated analytics summary export for facilitators
- In-app analytics insights panel for active profile
- Optional facilitator dashboard toggle panel in HomeScene
- Optional facilitator maintenance tools (clear logs/exports, reset profile progress)

## Important Docs

- Master index: `Docs/MASTER_EXECUTION_INDEX.md`
- Execution log: `Docs/WEEK1_EXECUTION.md`
- QA baseline: `Docs/WEEK1_QA_CHECKLIST.md`
- Regression: `Docs/WEEK2_REGRESSION_CHECKLIST.md`
- Pilot gate: `Docs/PILOT_READINESS_CHECKLIST.md`

## Facilitator PIN Security Docs

Use this sequence for security QA and triage:

1. Quick validation: `Docs/WEEK4_PIN_SECURITY_QUICK_TEST.md`
2. Scenario matrix: `Docs/WEEK4_SECURITY_REGRESSION_MATRIX.md`
3. Daily archive: `Docs/WEEK4_SECURITY_DAILY_LOG_TEMPLATE.md`
4. Security bug report: `Docs/WEEK4_SECURITY_BUG_TEMPLATE.md`
5. Full navigation map: `Docs/WEEK4_SECURITY_DOCS_MAP.md`

## Week 2/3 Implementation Notes

- Mission identity and progression are now runtime-driven from selected mission presets.
- Ensure each `MissionScenarioPreset` has:
  - `missionId`
  - `missionTitleKey`
  - `missionBriefingKey`
- Add `MissionBriefingController` in mission briefing panel to show localized mission-specific text.

## Project Status

This repository is currently focused on MVP pilot readiness within a 2-month delivery plan.
