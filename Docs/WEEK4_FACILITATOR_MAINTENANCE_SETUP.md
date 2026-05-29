# Week 4 Facilitator Maintenance Setup

This adds safe maintenance actions inside the facilitator dashboard.

## Included actions

- Clear analytics log files
- Clear exported JSON files
- Clear PIN audit log files
- Reset active profile progress (double-press confirmation)

## Scene wiring (HomeScene)

Inside facilitator dashboard panel add:

- `ClearAnalyticsLogsButton` (`Button`)
- `ClearExportFilesButton` (`Button`)
- `ClearPinAuditLogsButton` (`Button`) [optional but recommended]
- `ResetProfileProgressButton` (`Button`)
- `MaintenanceStatusText` (`Text`)
- `MaintenanceStatusBackground` (`Image`) [optional]
- `DangerZoneIndicator` (`GameObject`) [optional]
- `FacilitatorMaintenanceController` (empty object + `FacilitatorMaintenanceController`)

Assign in `FacilitatorMaintenanceController`:

- `clearAnalyticsButton` -> `ClearAnalyticsLogsButton`
- `clearExportsButton` -> `ClearExportFilesButton`
- `clearPinAuditButton` -> `ClearPinAuditLogsButton` (optional)
- `resetProfileProgressButton` -> `ResetProfileProgressButton`
- `statusText` -> `MaintenanceStatusText`
- `statusBackground` -> `MaintenanceStatusBackground` (optional)
- `dangerZoneIndicator` -> `DangerZoneIndicator` (optional)
- `resetArmDurationSeconds` -> recommended `5`

## Safety behavior

- Reset profile progress requires two presses:
  1. first press arms confirmation,
  2. second press executes reset.
- If second press is not done within timeout window, confirmation auto-expires.
- Optional danger indicator becomes visible only while reset is armed.

## Quick validation

1. Create progress and analytics data by playing missions.
2. Press clear analytics button and verify analytics JSONL files are removed.
3. Press clear exports button and verify export JSON files are removed.
4. Trigger some PIN entry attempts, then press clear PIN audit and verify security JSONL files are removed.
5. Press reset profile once -> confirmation message appears.
6. Wait beyond timeout -> confirmation should expire automatically.
7. Press reset profile once more then again quickly -> active profile progress resets.
