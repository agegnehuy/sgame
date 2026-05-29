# Week 2 Facilitator Report Export

This adds a local report export feature for teachers/facilitators.

## What it exports

A JSON report containing:

- generated timestamp (UTC),
- profile list,
- per profile:
  - display name,
  - avatar id,
  - coin balance,
  - mission results (best score, stars, attempts).

## Runtime behavior

- Export is local only (offline).
- File is written to:
  - `Application.persistentDataPath/exports/`
- Filename format:
  - `facilitator_report_YYYYMMDD_HHMMSS.json`

## HomeScene wiring

Inside `HomeScene` canvas add:

- `ExportReportButton` (`Button`)
- `ExportStatusText` (`Text`)
- `FacilitatorReportController` (empty with `FacilitatorReportExportController`)

Assign in `FacilitatorReportExportController`:

- `exportButton` -> `ExportReportButton`
- `statusText` -> `ExportStatusText`

## Quick test

1. Play and complete at least one mission.
2. Return to home.
3. Tap `ExportReportButton`.
4. Confirm `ExportStatusText` shows saved file path.
5. Open exported JSON file and verify profile + mission data exists.
