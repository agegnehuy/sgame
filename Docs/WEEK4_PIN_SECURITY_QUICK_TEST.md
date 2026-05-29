# Week 4 PIN Security Quick Test (3 Minutes)

Use this checklist for a fast pass on facilitator PIN safety before each internal build or pilot handoff.

## Preconditions

- HomeScene includes `FacilitatorDashboardController` with PIN lock enabled.
- PIN panel is wired (`pinInputField`, `pinSubmitButton`, `pinCancelButton`, `pinStatusText`).
- Optional: `pinSecurityStateText` is wired for live state visibility.
- Optional QA controls can be enabled (`enableSecurityTestControls = true`).

## Fast test run

1. Open facilitator panel and enter correct PIN.
   - Expected: dashboard opens successfully.
2. Close dashboard, open again, enter wrong PIN once.
   - Expected: wrong PIN feedback appears and short cooldown starts.
3. During cooldown, try submitting again.
   - Expected: entry remains blocked until cooldown ends.
4. Enter wrong PIN repeatedly until lockout threshold is reached.
   - Expected: lockout starts and input/submit are disabled.
5. During lockout, verify countdown changes each second.
   - Expected: lockout message and optional security-state label both update.
6. After lockout expires, enter correct PIN.
   - Expected: access is restored and dashboard opens.

## Audit log checks

7. Confirm file exists:
   - `Application.persistentDataPath/security/pin_audit_YYYY_MM_DD.jsonl`
8. Confirm recent records include expected event types:
   - `pin_failed`
   - `pin_cooldown_blocked`
   - `pin_lockout_started`
   - `pin_lockout_blocked`
   - `pin_success`

If any step fails, file a report using `WEEK4_SECURITY_BUG_TEMPLATE.md`.

## Maintenance checks

9. Use facilitator maintenance `Clear PIN Audit` action.
   - Expected: success status with cleared file count.
10. Repeat one failed PIN attempt and confirm a new audit file/entry appears.

## Optional QA shortcut mode

If `enableSecurityTestControls` is enabled:

- Use `simulateCooldownButton` to force cooldown state.
- Use `simulateLockoutButton` to force lockout state.
- Use `clearSecurityBlockButton` to return to ready state.

This mode is for QA speed only and should remain disabled in production scenes.
