# Week 3 Analytics Logging Setup

This feature adds lightweight offline analytics logs for mission behavior and challenge events.

## What is logged

- Decision events:
  - safe/unsafe result,
  - reason key,
  - elapsed mission time.
- Challenge events:
  - distraction,
  - pressure start,
  - pressure end.
- Mission summary:
  - safe/unsafe decision counts,
  - final score,
  - stars,
  - earned coins,
  - mission duration.

## Output location

Logs are written to:

- `Application.persistentDataPath/analytics/`
- file format: `events_YYYY_MM_DD.jsonl`

Each line is one JSON event record.

## Runtime wiring

No additional Unity scene wiring is required if these components already exist:

- `MissionController`
- `MissionChallengeEventController`

Analytics logging is integrated directly in these controllers.

## Quick verification

1. Start mission and perform at least one `Cross` and one `Wait`.
2. Trigger challenge events in M4/M5 (if enabled in preset).
3. Complete mission.
4. Open the latest analytics file and verify it contains:
   - `decision` entries,
   - `challenge_event` entries (for M4/M5),
   - `mission_summary` entry.
