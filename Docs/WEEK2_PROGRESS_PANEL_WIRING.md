# Week 2 Progress Panel Wiring (HomeScene)

This adds a mission summary board in `HomeScene` so children/teachers can see progress quickly.

## 1) Add progress summary panel

Inside `HomeScene` canvas add:

- `MissionProgressPanel` (`Panel`)
  - Row `Summary_P1_M1`
    - `MissionTitle_M1` (`Text`)
    - `BestScore_M1` (`Text`)
    - `Stars_M1` (`Text`)
    - `Attempts_M1` (`Text`)
    - `Unlock_M1` (`Text`)
  - Row `Summary_P1_M2`
    - `MissionTitle_M2` (`Text`)
    - `BestScore_M2` (`Text`)
    - `Stars_M2` (`Text`)
    - `Attempts_M2` (`Text`)
    - `Unlock_M2` (`Text`)
  - Row `Summary_P1_M3`
    - `MissionTitle_M3` (`Text`)
    - `BestScore_M3` (`Text`)
    - `Stars_M3` (`Text`)
    - `Attempts_M3` (`Text`)
    - `Unlock_M3` (`Text`)

## 2) Add controller object

1. Create empty object `ProgressSummaryController`.
2. Add `MissionProgressSummaryController`.
3. In inspector, add 3 entries in `missionBindings`.
4. For each entry, assign:
   - `missionId`: `P1-M1`, `P1-M2`, `P1-M3`
   - all text fields from matching row.

Recommended labels:
- `bestScorePrefix`: `Best: `
- `starsPrefix`: `Stars: `
- `attemptsPrefix`: `Attempts: `
- `unlockedLabel`: `Unlocked`
- `lockedLabel`: `Locked`

## 3) Connect profile switching refresh

In `ProfileSelectorController`, assign:
- `progressSummaryController` -> `ProgressSummaryController`

Now summary auto-refreshes when:
- mission results update,
- profile changes,
- coin/progress save emits progress event.

## 4) Quick validation

1. Start with fresh profile.
2. Verify:
   - M1 unlocked, M2/M3 locked.
   - best/attempts show `-` and `0`.
3. Complete P1-M1 once.
4. Return home:
   - P1-M1 best score and attempts updated.
   - P1-M2 unlock text changes to `Unlocked` once threshold is met.
