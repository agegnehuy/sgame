# Week 4 HomeScene Security Wiring Checklist

Use this during Unity scene setup to wire facilitator PIN security end-to-end without missing fields.

## A) Required scene objects

Create/confirm these objects in `HomeScene`:

- `FacilitatorDashboardToggleButton` (`Button`)
  - child `FacilitatorDashboardToggleText` (`Text`)
- `FacilitatorDashboardPanel` (`Panel`)
  - `FacilitatorDashboardTitleText` (`Text`)
  - existing analytics/export/maintenance controls
- `FacilitatorPinPanel` (`Panel`)
  - `FacilitatorPinInput` (`InputField`)
  - `FacilitatorPinSubmitButton` (`Button`)
  - `FacilitatorPinCancelButton` (`Button`)
  - `FacilitatorPinStatusText` (`Text`)
  - `FacilitatorPinSecurityStateText` (`Text`, optional but recommended)
- `FacilitatorDashboardController` (empty object + `FacilitatorDashboardController`)
- `FacilitatorMaintenanceController` (empty object + `FacilitatorMaintenanceController`)

Optional QA-only objects:

- `SimulateCooldownButton` (`Button`)
- `SimulateLockoutButton` (`Button`)
- `ClearSecurityBlockButton` (`Button`)

## B) Inspector wiring: FacilitatorDashboardController

Assign references:

- `dashboardPanel` -> `FacilitatorDashboardPanel`
- `toggleButton` -> `FacilitatorDashboardToggleButton`
- `toggleButtonText` -> `FacilitatorDashboardToggleText`
- `dashboardTitleText` -> `FacilitatorDashboardTitleText`
- `analyticsInsightsPanelController` -> `AnalyticsInsightsController`
- `pinPanel` -> `FacilitatorPinPanel`
- `pinInputField` -> `FacilitatorPinInput`
- `pinSubmitButton` -> `FacilitatorPinSubmitButton`
- `pinCancelButton` -> `FacilitatorPinCancelButton`
- `pinStatusText` -> `FacilitatorPinStatusText`
- `pinSecurityStateText` -> `FacilitatorPinSecurityStateText` (optional)

Recommended values:

- `startOpen = false`
- `requirePinToOpen = true`
- `facilitatorPin = <foundation pin>`
- `maxFailedAttempts = 3`
- `lockoutDurationSeconds = 20`
- `failedAttemptCooldownSeconds = 2`

QA mode values (editor only):

- `enableSecurityTestControls = true` (only for QA scene)
- `simulateCooldownButton` -> `SimulateCooldownButton`
- `simulateLockoutButton` -> `SimulateLockoutButton`
- `clearSecurityBlockButton` -> `ClearSecurityBlockButton`
- `qaSimulatedCooldownSeconds = 5`
- `qaSimulatedLockoutSeconds = 20`

## C) Inspector wiring: FacilitatorMaintenanceController

Create/confirm controls:

- `ClearAnalyticsLogsButton` (`Button`)
- `ClearExportFilesButton` (`Button`)
- `ClearPinAuditLogsButton` (`Button`, recommended)
- `ResetProfileProgressButton` (`Button`)
- `MaintenanceStatusText` (`Text`)
- `MaintenanceStatusBackground` (`Image`, optional)
- `DangerZoneIndicator` (`GameObject`, optional)

Assign references:

- `clearAnalyticsButton` -> `ClearAnalyticsLogsButton`
- `clearExportsButton` -> `ClearExportFilesButton`
- `clearPinAuditButton` -> `ClearPinAuditLogsButton` (optional)
- `resetProfileProgressButton` -> `ResetProfileProgressButton`
- `statusText` -> `MaintenanceStatusText`
- `statusBackground` -> `MaintenanceStatusBackground` (optional)
- `dangerZoneIndicator` -> `DangerZoneIndicator` (optional)
- `resetArmDurationSeconds = 5` (recommended)

## D) Quick smoke test (after wiring)

1. Press facilitator toggle -> PIN panel appears.
2. Correct PIN -> dashboard opens.
3. Wrong PIN once -> cooldown message appears.
4. Wrong PIN to threshold -> lockout message + disabled input/submit.
5. Wait lockout end -> controls recover.
6. Press clear PIN audit in maintenance -> success status with file count.

## E) Verification docs to run right after

- `WEEK4_PIN_SECURITY_QUICK_TEST.md`
- `WEEK4_SECURITY_REGRESSION_MATRIX.md`
- `WEEK4_SECURITY_DAILY_LOG_TEMPLATE.md`
