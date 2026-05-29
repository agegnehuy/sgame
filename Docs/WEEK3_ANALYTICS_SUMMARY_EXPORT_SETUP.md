# Week 3 Analytics Summary Export Setup

This adds a facilitator-readable analytics summary file aggregated from local JSONL mission events.

## What it produces

A JSON report in:

- `Application.persistentDataPath/exports/analytics_summary_YYYYMMDD_HHMMSS.json`

The report includes:

- profile-level totals (safe/unsafe decisions),
- mission-level aggregates (best score, average score, attempts count via mission summaries),
- challenge event counts (distraction/pressure start/pressure end),
- average mission duration.

## Scene wiring (HomeScene)

Add these UI elements:

- `ExportAnalyticsSummaryButton` (`Button`)
- `ExportAnalyticsSummaryStatusText` (`Text`)
- `AnalyticsSummaryExportController` (empty object + `AnalyticsSummaryExportController`)

Assign in `AnalyticsSummaryExportController`:

- `exportButton` -> `ExportAnalyticsSummaryButton`
- `statusText` -> `ExportAnalyticsSummaryStatusText`

## Quick validation

1. Play missions and trigger some decisions/events.
2. Return to HomeScene.
3. Press analytics summary export button.
4. Confirm status text shows a saved file path.
5. Open exported JSON and verify aggregate values look reasonable.
