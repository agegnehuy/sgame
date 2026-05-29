# Week 2 Regression Checklist

Use this after integrating Week 2 features to ensure no core flow was broken.

## Scope

This checklist covers:

- multi-mission selection and unlock progression,
- scenario preset application,
- home progress summary panel,
- facilitator report export,
- runtime language switching and localization refresh.

## Pre-test setup

- Build includes scenes:
  - `HomeScene`
  - `P1_M1`
- Home scene has:
  - 3 mission nodes (`P1-M1`, `P1-M2`, `P1-M3`)
  - language toggle buttons
  - progress summary panel
  - report export button
- Scenario applier configured with presets for M1/M2/M3.

## 1) Core loop regression

### TC-RG-01: Home -> mission -> result -> home

- Steps:
  1. Launch app.
  2. Start mission from map or play button.
  3. Complete mission.
  4. Tap Back Home.
- Expected:
  - No navigation errors.
  - Home scene returns cleanly.
  - No duplicate UI listeners or stuck panels.

### TC-RG-02: Save continuity after restart

- Steps:
  1. Complete mission and earn coins.
  2. Force close app.
  3. Reopen app.
- Expected:
  - Progress and coins persist.
  - Last unlocked mission state persists.

## 2) Multi-mission selection and progression

### TC-RG-03: Mission selection state

- Steps:
  1. Tap `P1-M2` node (if unlocked).
  2. Return home and press Play.
- Expected:
  - Play launches selected mission scene/preset.

### TC-RG-04: Unlock chain

- Steps:
  1. Clear `P1-M1` with score >= 55.
  2. Verify `P1-M2` unlocks.
  3. Clear `P1-M2` with score >= 55.
  4. Verify `P1-M3` unlocks.
- Expected:
  - Sequential unlock logic works with no manual reset.

## 3) Scenario preset behavior

### TC-RG-05: Per-mission tuning applies

- Steps:
  1. Start M1 and note signal rhythm / difficulty.
  2. Start M2 and compare.
  3. Start M3 and compare.
- Expected:
  - Distinct behavior by mission (signal timing, vehicle distance pressure, required safe crossings).

### TC-RG-06: Preset fallback safety

- Steps:
  1. Temporarily remove one preset assignment.
  2. Launch corresponding mission.
- Expected:
  - Mission still runs with default values; no crash/null exception.

## 4) Progress summary panel

### TC-RG-07: Summary update after mission

- Steps:
  1. Complete mission.
  2. Return home.
- Expected:
  - Best score, stars, attempts, unlock status update correctly.

### TC-RG-08: Profile switch refresh

- Steps:
  1. Create/select profile A and play one mission.
  2. Switch to profile B.
- Expected:
  - Summary panel reflects profile B data immediately.

## 5) Store + loadout regression

### TC-RG-09: Purchase/equip still works

- Steps:
  1. Earn enough coins.
  2. Buy and equip item.
- Expected:
  - Coins decrease.
  - Equip state saved.
  - Avatar preview updates.

### TC-RG-10: Insufficient coin handling

- Steps:
  1. Attempt expensive purchase without enough coins.
- Expected:
  - Purchase is blocked.
  - Proper status message shown.

## 6) Facilitator export

### TC-RG-11: Export generation

- Steps:
  1. Tap Export button in home.
- Expected:
  - JSON file is created in `persistentDataPath/exports`.
  - Status text shows file path.

### TC-RG-12: Export data correctness

- Steps:
  1. Open exported JSON.
  2. Compare profile/mission values with in-app state.
- Expected:
  - Data matches app state (coins, mission best, stars, attempts).

## 7) Language switching and localization

### TC-RG-13: Runtime EN/AM switch

- Steps:
  1. Tap EN, then AM, then EN.
- Expected:
  - UI text updates immediately each time.
  - No scene reload required.

### TC-RG-14: Dynamic panel localization refresh

- Steps:
  1. Switch language on home.
  2. Check coin label, mission labels, summary panel, store statuses.
- Expected:
  - All localized labels update to selected language.

### TC-RG-15: Optional QA refresh button

- Steps:
  1. Tap refresh localization debug button.
- Expected:
  - Status text confirms refresh.
  - No visible label remains stale.

## 8) Stability checks

### TC-RG-16: 15-minute regression run

- Steps:
  1. Repeatedly switch language, start missions, return home, open store, export report.
- Expected:
  - No crash, no severe memory/performance degradation.

### TC-RG-17: Listener leak smoke test

- Steps:
  1. Re-enter HomeScene and mission scene 10+ times.
  2. Trigger language change and mission completion.
- Expected:
  - Single update per action (no duplicated UI events).

## Exit criteria

- All core-flow and data-persistence tests pass.
- No blocker/critical defects.
- Any medium issues are logged with workaround and owner.
