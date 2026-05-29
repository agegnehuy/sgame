# Week 3 Facilitator Dashboard Setup

This adds a compact HomeScene panel that groups facilitator tools in one place.

## Included in dashboard

- Facilitator report export control
- Analytics summary export control
- In-app analytics insights panel

## Scene wiring (HomeScene)

Add UI objects:

- `FacilitatorDashboardToggleButton` (`Button`)
  - child text: `FacilitatorDashboardToggleText` (`Text`)
- `FacilitatorDashboardPanel` (`Panel`)
  - `FacilitatorDashboardTitleText` (`Text`)
  - existing export/report controls
  - existing analytics summary export controls
  - existing analytics insights panel
- `FacilitatorDashboardController` (empty object + `FacilitatorDashboardController`)

Assign in `FacilitatorDashboardController`:

- `dashboardPanel` -> `FacilitatorDashboardPanel`
- `toggleButton` -> `FacilitatorDashboardToggleButton`
- `toggleButtonText` -> `FacilitatorDashboardToggleText`
- `dashboardTitleText` -> `FacilitatorDashboardTitleText`
- `analyticsInsightsPanelController` -> `AnalyticsInsightsController`
- `startOpen` -> `false` (recommended)

Optional PIN lock wiring:

- `requirePinToOpen` -> `true` (recommended for production)
- `facilitatorPin` -> set foundation PIN (example `2580`)
- `pinPanel` -> `FacilitatorPinPanel`
- `pinInputField` -> `FacilitatorPinInput` (`InputField`)
- `pinSubmitButton` -> `FacilitatorPinSubmitButton`
- `pinCancelButton` -> `FacilitatorPinCancelButton`
- `pinStatusText` -> `FacilitatorPinStatusText`
- `pinSecurityStateText` -> `FacilitatorPinSecurityStateText` (optional status label)
- `maxFailedAttempts` -> `3` (recommended)
- `lockoutDurationSeconds` -> `20` (recommended)
- `failedAttemptCooldownSeconds` -> `2` (recommended)

Optional QA security controls (editor/testing only):

- `enableSecurityTestControls` -> `true` to use test buttons
- `simulateCooldownButton` -> optional button to force cooldown state
- `simulateLockoutButton` -> optional button to force lockout state
- `clearSecurityBlockButton` -> optional button to clear active blocked state
- `qaSimulatedCooldownSeconds` -> recommended `5`
- `qaSimulatedLockoutSeconds` -> recommended `20`

## Localization keys used

- `ui.facilitator.open`
- `ui.facilitator.close`
- `ui.facilitator.title`
- `ui.facilitator.pin_prompt`
- `ui.facilitator.pin_invalid`
- `ui.facilitator.pin_locked`
- `ui.facilitator.pin_attempts_left`
- `ui.facilitator.pin_cooldown`
- `ui.facilitator.pin_state_ready`
- `ui.facilitator.pin_state_cooldown`
- `ui.facilitator.pin_state_lockout`

## Offline PIN audit log

- PIN security events are now written to:
  - `Application.persistentDataPath/security/pin_audit_YYYY_MM_DD.jsonl`
- Event types logged:
  - `pin_success`
  - `pin_failed`
  - `pin_cooldown_blocked`
  - `pin_lockout_started`
  - `pin_lockout_blocked`

## Quick validation

1. Launch HomeScene.
2. Press facilitator toggle button.
3. Confirm dashboard panel opens and insights refresh.
4. Press toggle again and confirm dashboard closes.
5. Switch EN/AM and verify toggle/title text updates.
6. With PIN enabled, verify panel opens only after correct PIN entry.
7. Enter wrong PIN 3 times and verify the lockout message appears.
8. During lockout, verify PIN input + submit are disabled.
9. During lockout, confirm countdown seconds update while panel is visible.
10. Enter one wrong PIN and verify a short cooldown message appears.
11. After cooldown/lockout timer expires, verify PIN input accepts new attempts again.
12. Confirm JSONL audit file is created under persistent `security` folder after PIN interactions.
13. If `pinSecurityStateText` is wired, verify it switches between ready/cooldown/lockout states.
14. If QA controls are enabled, verify simulate buttons force state transitions and clear button restores ready state.
