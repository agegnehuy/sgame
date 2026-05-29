# Week 1 QA Checklist (MVP Prototype)

Use this checklist to validate the current Week 1 build before moving to Week 2.

## Test setup

- Build includes scenes:
  - `HomeScene`
  - `P1_M1`
- Device A: low-end Android test phone
- Device B: mid-range Android test phone
- App starts from `HomeScene`

## 1) Home flow

### TC-HOME-01: App launch

- Steps:
  1. Launch app fresh.
- Expected:
  - Home UI loads without errors.
  - Play and mission node controls are visible.

### TC-HOME-02: Play button mission entry

- Steps:
  1. Tap `PlayButton`.
- Expected:
  - `P1_M1` scene loads.
  - Briefing panel is visible.

### TC-HOME-03: Mission node mission entry

- Steps:
  1. Return to home.
  2. Tap mission node button.
- Expected:
  - Mission scene opens if unlocked.

## 2) Mission runtime

### TC-MSN-01: Mission start

- Steps:
  1. Tap `Start Mission`.
- Expected:
  - Gameplay panel appears.
  - Cross/Wait buttons are interactive.

### TC-MSN-02: Unsafe crossing on red

- Steps:
  1. Start mission.
  2. Press `Cross` while signal is red.
- Expected:
  - Unsafe feedback appears.
  - Mission does not complete.

### TC-MSN-03: Safe crossing on green

- Steps:
  1. Wait for green signal.
  2. Press `Cross`.
- Expected:
  - Safe feedback appears.
  - Mission completes.
  - Result panel opens.

### TC-MSN-04: Result values shown

- Steps:
  1. Complete mission.
- Expected:
  - Score text visible.
  - Stars text visible.
  - Coin reward text visible.

## 3) Save and persistence

### TC-SAVE-01: Mission result persistence

- Steps:
  1. Complete mission once.
  2. Close app fully.
  3. Reopen app and replay mission.
- Expected:
  - No save reset/crash.
  - Progress file exists in persistent path.

### TC-SAVE-02: Coin persistence

- Steps:
  1. Complete mission and note coins.
  2. Restart app.
- Expected:
  - Same coin total shown after restart.

## 4) Store and equip flow

### TC-STORE-01: Purchase with enough coins

- Steps:
  1. Earn enough coins.
  2. Tap store item buy/equip button.
- Expected:
  - Coins decrease by cost.
  - Item is owned/equipped.
  - Status text updates.

### TC-STORE-02: Purchase without enough coins

- Steps:
  1. Ensure coin balance is lower than item cost.
  2. Tap buy.
- Expected:
  - Purchase blocked.
  - "Not enough coins" message appears.
  - Coins remain unchanged.

### TC-STORE-03: Loadout preview update

- Steps:
  1. Buy/equip an item.
- Expected:
  - Avatar preview panel updates equipped slot text.

## 5) Profile system

### TC-PROF-01: Create profile

- Steps:
  1. Enter profile name.
  2. Tap create.
- Expected:
  - New profile appears in dropdown.
  - Max profile count is capped at configured limit.

### TC-PROF-02: Switch profile

- Steps:
  1. Select another profile from dropdown.
- Expected:
  - Active profile changes.
  - Coin label and loadout preview refresh.

## 6) Localization sanity

### TC-LANG-01: English load

- Steps:
  1. Set default language to `en`.
  2. Run scene.
- Expected:
  - EN keys resolve to English values.

### TC-LANG-02: Amharic load

- Steps:
  1. Set default language to `am`.
  2. Run scene.
- Expected:
  - AM keys resolve to Amharic values.

## 7) Scene navigation

### TC-NAV-01: Back to home from result

- Steps:
  1. Complete mission.
  2. Tap `Back Home`.
- Expected:
  - Home scene loads.
  - No stale mission UI remains.

## 8) Performance smoke checks

### TC-PERF-01: 10-minute run

- Steps:
  1. Play/replay mission for 10 minutes.
- Expected:
  - No crash.
  - No major input lag or UI freeze.

### TC-PERF-02: Signal and UI responsiveness

- Steps:
  1. During mission, press Wait/Cross repeatedly across cycles.
- Expected:
  - Feedback appears quickly.
  - No delayed scene transitions after completion.

## Exit criteria for Week 1

- All high-priority tests above pass.
- No blocker defects in:
  - Mission start/completion
  - Save and coin persistence
  - Home -> mission -> home loop
  - Store buy/equip flow
