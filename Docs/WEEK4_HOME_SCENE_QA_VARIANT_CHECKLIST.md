# Week 4 HomeScene QA Variant Checklist

Use this checklist to keep a QA scene variant (`HomeScene_QA`) separate from production (`HomeScene`).

## Goal

- `HomeScene` = production-safe configuration (no QA simulate controls enabled)
- `HomeScene_QA` = fast validation scene (QA simulate controls enabled)

## 1) Duplicate base scene

1. Open Unity and load `HomeScene`.
2. Save As -> `HomeScene_QA`.
3. Keep both scenes in source control.

## 2) Production scene guardrails (`HomeScene`)

In `FacilitatorDashboardController`:

- `requirePinToOpen = true`
- `enableSecurityTestControls = false`
- `simulateCooldownButton` unassigned
- `simulateLockoutButton` unassigned
- `clearSecurityBlockButton` unassigned

In `FacilitatorMaintenanceController`:

- Keep operational buttons only (clear analytics/exports/PIN audit + reset progress)
- Keep confirmation timeout active (`resetArmDurationSeconds >= 5`)

## 3) QA scene setup (`HomeScene_QA`)

In `FacilitatorDashboardController`:

- `requirePinToOpen = true`
- `enableSecurityTestControls = true`
- Assign:
  - `simulateCooldownButton`
  - `simulateLockoutButton`
  - `clearSecurityBlockButton`
- Recommended:
  - `qaSimulatedCooldownSeconds = 5`
  - `qaSimulatedLockoutSeconds = 20`
- Keep lock/cooldown values aligned with production baseline:
  - `maxFailedAttempts = 3`
  - `lockoutDurationSeconds = 20`
  - `failedAttemptCooldownSeconds = 2`

## 4) Build settings guidance

- Daily QA run: include `HomeScene_QA`
- Pilot/release build: include `HomeScene` only
- Before release candidate: remove `HomeScene_QA` from build list

## 5) Quick parity checks (every sprint)

1. Verify both scenes share the same gameplay/facilitator core content.
2. Verify only QA controls differ between scenes.
3. Run:
   - `WEEK4_PIN_SECURITY_QUICK_TEST.md`
   - `WEEK4_SECURITY_REGRESSION_MATRIX.md`
4. Archive run using `WEEK4_SECURITY_DAILY_LOG_TEMPLATE.md`.

## 6) Naming convention

Use these names consistently:

- Production: `HomeScene`
- QA variant: `HomeScene_QA`

Avoid ad-hoc names like `HomeSceneTest2` to keep build and QA tracking clean.
