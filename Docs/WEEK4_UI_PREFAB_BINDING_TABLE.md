# Week 4 UI Prefab Binding Table

Use this table for strict inspector wiring.  
`Required` means the feature is expected in production. `Optional` means safe to leave unassigned.

## 1) `FacilitatorDashboardController`

| Inspector Field | Expected Object Name | Type | Required | Notes |
|---|---|---|---|---|
| `dashboardPanel` | `FacilitatorDashboardPanel` | `GameObject` | Yes | Main dashboard root panel |
| `toggleButton` | `FacilitatorDashboardToggleButton` | `Button` | Yes | Opens/closes facilitator panel |
| `toggleButtonText` | `FacilitatorDashboardToggleText` | `Text` | Yes | Open/close localized label |
| `dashboardTitleText` | `FacilitatorDashboardTitleText` | `Text` | Yes | Panel title label |
| `analyticsInsightsPanelController` | `AnalyticsInsightsController` | `AnalyticsInsightsPanelController` | Yes | Refreshed when dashboard opens |
| `pinPanel` | `FacilitatorPinPanel` | `GameObject` | Yes (if PIN enabled) | PIN modal root |
| `pinInputField` | `FacilitatorPinInput` | `InputField` | Yes (if PIN enabled) | PIN entry field |
| `pinSubmitButton` | `FacilitatorPinSubmitButton` | `Button` | Yes (if PIN enabled) | Submit PIN |
| `pinCancelButton` | `FacilitatorPinCancelButton` | `Button` | Yes (if PIN enabled) | Close PIN modal |
| `pinStatusText` | `FacilitatorPinStatusText` | `Text` | Yes (if PIN enabled) | Invalid/cooldown/lockout text |
| `pinSecurityStateText` | `FacilitatorPinSecurityStateText` | `Text` | Optional (recommended) | Ready/Cooldown/Lockout state label |
| `simulateCooldownButton` | `SimulateCooldownButton` | `Button` | Optional | QA only (`enableSecurityTestControls=true`) |
| `simulateLockoutButton` | `SimulateLockoutButton` | `Button` | Optional | QA only |
| `clearSecurityBlockButton` | `ClearSecurityBlockButton` | `Button` | Optional | QA only |

## 2) `FacilitatorMaintenanceController`

| Inspector Field | Expected Object Name | Type | Required | Notes |
|---|---|---|---|---|
| `clearAnalyticsButton` | `ClearAnalyticsLogsButton` | `Button` | Yes | Clears analytics JSONL logs |
| `clearExportsButton` | `ClearExportFilesButton` | `Button` | Yes | Clears exported JSON files |
| `clearPinAuditButton` | `ClearPinAuditLogsButton` | `Button` | Optional (recommended) | Clears security audit logs |
| `resetProfileProgressButton` | `ResetProfileProgressButton` | `Button` | Yes | Double-press guarded reset |
| `statusText` | `MaintenanceStatusText` | `Text` | Yes | Feedback/status output |
| `statusBackground` | `MaintenanceStatusBackground` | `Image` | Optional | Colored state background |
| `dangerZoneIndicator` | `DangerZoneIndicator` | `GameObject` | Optional | Visible while reset is armed |

## 3) `AnalyticsInsightsPanelController`

| Inspector Field | Expected Object Name | Type | Required | Notes |
|---|---|---|---|---|
| `totalDecisionsText` | `AnalyticsTotalDecisionsText` | `Text` | Yes | Total decisions metric |
| `safeRatioText` | `AnalyticsSafeRatioText` | `Text` | Yes | Safe ratio percentage |
| `topMissionText` | `AnalyticsTopMissionText` | `Text` | Yes | Top mission + best score |
| `challengeEventsText` | `AnalyticsChallengeEventsText` | `Text` | Yes | Total challenge events |
| `missionCountText` | `AnalyticsMissionCountText` | `Text` | Yes | Missions played count |
| `refreshButton` | `AnalyticsInsightsRefreshButton` | `Button` | Optional | Auto-refresh already occurs on events/language |

## 4) `AnalyticsSummaryExportController`

| Inspector Field | Expected Object Name | Type | Required | Notes |
|---|---|---|---|---|
| `exportButton` | `ExportAnalyticsSummaryButton` | `Button` | Yes | Triggers summary export |
| `statusText` | `AnalyticsExportStatusText` | `Text` | Yes | Shows success path or fail message |

## 5) Baseline config values

For production `HomeScene`:

- `FacilitatorDashboardController.requirePinToOpen = true`
- `FacilitatorDashboardController.enableSecurityTestControls = false`
- `FacilitatorDashboardController.maxFailedAttempts = 3`
- `FacilitatorDashboardController.lockoutDurationSeconds = 20`
- `FacilitatorDashboardController.failedAttemptCooldownSeconds = 2`
- `FacilitatorMaintenanceController.resetArmDurationSeconds = 5`

For QA scene variant (`HomeScene_QA`):

- `enableSecurityTestControls = true`
- assign `simulateCooldownButton`, `simulateLockoutButton`, `clearSecurityBlockButton`
