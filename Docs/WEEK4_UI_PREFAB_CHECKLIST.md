# Week 4 UI Prefab Checklist (Unity)

Use this checklist to build reusable UI prefabs and avoid scene-by-scene rework.

## 1) Core prefabs to create

- `PF_TopBar`
- `PF_MissionMapPanel`
- `PF_FacilitatorDashboardPanel`
- `PF_FacilitatorPinModal`
- `PF_FacilitatorMaintenancePanel`
- `PF_AnalyticsInsightsPanel`
- `PF_AnalyticsExportPanel`
- `PF_ResultPanel`
- `PF_MissionHUD`

## 2) Prefab component requirements

### `PF_FacilitatorDashboardPanel`

- `FacilitatorDashboardController`
- `CanvasGroup` (optional for show/hide transitions)
- Child refs required by controller:
  - dashboard root panel object
  - toggle button + toggle text
  - title text
  - PIN panel object
  - PIN input/submit/cancel/status/security-state text

### `PF_FacilitatorPinModal`

- `InputField` for PIN
- `Submit` and `Cancel` buttons
- status text (`pinStatusText`)
- security-state text (`pinSecurityStateText`, optional but recommended)
- layout group for responsive spacing

### `PF_FacilitatorMaintenancePanel`

- `FacilitatorMaintenanceController`
- clear analytics button
- clear exports button
- clear PIN audit button (recommended)
- reset profile progress button
- status text
- optional status background image
- optional danger indicator object

### `PF_AnalyticsInsightsPanel`

- `AnalyticsInsightsPanelController`
- text labels for:
  - total decisions
  - safe ratio
  - top mission
  - challenge events
  - mission count

### `PF_AnalyticsExportPanel`

- `AnalyticsSummaryExportController`
- export button
- status/result text

### `PF_MissionMapPanel`

- `MissionMapController`
- mission node buttons (P1-M1 ... P1-M5)
- lock/unlock visuals per node

### `PF_ResultPanel`

- `ResultPanelController`
- score text
- stars text
- coins text
- replay button
- back-home button

### `PF_MissionHUD`

- `HUDController`
- wait/cross action buttons
- feedback label
- score label

## 3) Localization hooks (required)

For every text in prefabs:

- either bind via `LocalizedText`
- or set/update text through controller localization keys

Verify no hardcoded language text remains in production-prefab labels.

## 4) Naming convention

- Prefabs: `PF_*`
- Panels: `Panel*` or `*Panel`
- Buttons: `*Button`
- Labels: `*Text`
- Inputs: `*Input`

Keep names stable to reduce wiring mistakes.

## 5) Scene assembly order

1. Place `PF_TopBar`.
2. Place `PF_MissionMapPanel`.
3. Place `PF_AnalyticsInsightsPanel` and `PF_AnalyticsExportPanel`.
4. Place `PF_FacilitatorDashboardPanel`.
5. Nest `PF_FacilitatorPinModal` and `PF_FacilitatorMaintenancePanel` under facilitator dashboard.
6. Validate all inspector references.

## 6) Final prefab validation

- [ ] All required controller fields assigned (no missing refs).
- [ ] EN/AM language switch updates visible text.
- [ ] Buttons remain usable on small-screen portrait ratio.
- [ ] Prefab overrides documented (if scene-specific).
- [ ] QA scene variant uses same prefabs with only config differences.
