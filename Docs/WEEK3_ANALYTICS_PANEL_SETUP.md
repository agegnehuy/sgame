# Week 3 In-App Analytics Panel Setup

This panel provides quick analytics insights inside HomeScene (without exporting files).

## What it shows (active profile)

- Total decisions
- Safe ratio (%)
- Top mission (best score)
- Total challenge events
- Missions played count

## Scene wiring (HomeScene)

Add UI elements:

- `AnalyticsInsightsPanel` (`Panel`)
  - `AnalyticsTotalDecisionsText` (`Text`)
  - `AnalyticsSafeRatioText` (`Text`)
  - `AnalyticsTopMissionText` (`Text`)
  - `AnalyticsChallengeEventsText` (`Text`)
  - `AnalyticsMissionCountText` (`Text`)
  - `AnalyticsRefreshButton` (`Button`) [optional]
- `AnalyticsInsightsController` (empty object + `AnalyticsInsightsPanelController`)

Assign in `AnalyticsInsightsPanelController`:

- `totalDecisionsText` -> `AnalyticsTotalDecisionsText`
- `safeRatioText` -> `AnalyticsSafeRatioText`
- `topMissionText` -> `AnalyticsTopMissionText`
- `challengeEventsText` -> `AnalyticsChallengeEventsText`
- `missionCountText` -> `AnalyticsMissionCountText`
- `refreshButton` -> `AnalyticsRefreshButton` (optional)

## Runtime behavior

- Auto-refreshes when:
  - profile progress changes,
  - language changes.
- Uses analytics logs from:
  - `Application.persistentDataPath/analytics/events_YYYY_MM_DD.jsonl`

## Quick test

1. Play a mission and make several decisions.
2. Return to HomeScene.
3. Open analytics insights panel.
4. Verify values update after another mission run.
5. Switch EN/AM and confirm labels update.
