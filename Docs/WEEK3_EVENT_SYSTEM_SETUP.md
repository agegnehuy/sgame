# Week 3 Event System Setup (Distraction and Pressure)

This setup enables deterministic challenge events for advanced Phase 1 missions (especially M4/M5).

## What was added in code

- `MissionChallengeEventController`
  - Triggers timed distraction and pressure events.
  - Emits guidance messages through `MissionController`.
  - Temporarily increases traffic pressure by overriding vehicle distance simulation.
- `MissionScenarioPreset` new fields
  - Distraction and pressure configuration values.
- `MissionController`
  - Runtime mission identity now used for progression save correctness.
  - Guidance event method `EmitGuidance(...)`.

## 1) Scene wiring

In `P1_M1` scene:

1. Create object `ChallengeEvents`.
2. Add `MissionChallengeEventController`.
3. Assign:
   - `missionController` -> `MissionRuntime`
   - `scenarioApplier` -> `ScenarioApplier`
   - `vehicleDistanceSimulator` -> `VehicleDistance`

## 2) Preset configuration (recommended)

### For M1-M3
- Keep:
  - `enableDistractionEvent = false`
  - `enablePressureEvent = false`

### For M4 (distraction mission)
- `enableDistractionEvent = true`
- `distractionDelaySeconds = 3.0`
- `distractionMessageKey = feedback.info.distraction`
- `enablePressureEvent = false` (optional)

### For M5 (rush challenge)
- `enableDistractionEvent = true`
- `distractionDelaySeconds = 2.5`
- `enablePressureEvent = true`
- `pressureStartDelaySeconds = 6.0`
- `pressureDurationSeconds = 6.0`
- `pressureMinVehicleDistanceMeters = 3.0`
- `pressureMaxVehicleDistanceMeters = 14.0`
- `pressureVehicleCycleSpeed = 2.3`
- `pressureStartMessageKey = feedback.info.pressure_start`
- `pressureEndMessageKey = feedback.info.pressure_end`

## 3) Validation steps

1. Start M4 and verify distraction guidance appears once after configured delay.
2. Start M5 and verify:
   - pressure start guidance appears,
   - traffic pressure increases temporarily,
   - pressure end guidance appears,
   - simulation values restore after event end or mission end.
3. Confirm mission completion saves to the correct mission ID (`P1-M4`, `P1-M5`) in progress JSON.

## 4) Notes

- Event timing is deterministic (not random), which improves QA repeatability.
- Pressure override auto-restores when mission ends to avoid state leakage.
