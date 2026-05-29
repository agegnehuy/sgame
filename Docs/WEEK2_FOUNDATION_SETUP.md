# Week 2 Foundation Setup (Missions 2 and 3)

This step upgrades the prototype from a single mission into a configurable multi-mission flow.

## What is now available in code

- Mission selection session state:
  - `PlayerSession.SelectedMissionId`
  - `PlayerSession.SelectedMissionSceneName`
- Home play button now loads selected mission (or default).
- Mission nodes set selected mission on click.
- Runtime scenario presets:
  - `MissionScenarioPreset`
  - `MissionScenarioApplier`
- Runtime-tunable systems:
  - mission rules (`MissionController.ConfigureRuntime`)
  - traffic signal timing (`TrafficLightController.ConfigureTimings`)
  - vehicle distance behavior (`VehicleDistanceSimulator.Configure`)

## Unity setup steps

## 1) Create mission presets

In `Assets/_Game/Gameplay`:

1. Create three assets:
   - `Preset_P1_M1`
   - `Preset_P1_M2`
   - `Preset_P1_M3`
2. Set values:

### `Preset_P1_M1`
- `missionId`: `P1-M1`
- `missionSceneName`: `P1_M1`
- `missionTitleKey`: `mission.p1m1.title`
- `missionBriefingKey`: `mission.p1m1.briefing`
- `redDurationSeconds`: `5.0`
- `greenDurationSeconds`: `4.0`
- `minVehicleDistanceMeters`: `6`
- `maxVehicleDistanceMeters`: `30`
- `vehicleCycleSpeed`: `1.2`
- `safeVehicleDistanceMeters`: `12`
- `requiredSafeCrosses`: `1`
- `allowCrossOnlyOnGreen`: `true`
- `timingDiscipline01`: `0.8`

### `Preset_P1_M2`
- `missionId`: `P1-M2`
- `missionSceneName`: `P1_M1` (reuse scene for now)
- `missionTitleKey`: `mission.p1m2.title`
- `missionBriefingKey`: `mission.p1m2.briefing`
- `redDurationSeconds`: `6.0`
- `greenDurationSeconds`: `3.0`
- `minVehicleDistanceMeters`: `5`
- `maxVehicleDistanceMeters`: `24`
- `vehicleCycleSpeed`: `1.5`
- `safeVehicleDistanceMeters`: `14`
- `requiredSafeCrosses`: `2`
- `allowCrossOnlyOnGreen`: `true`
- `timingDiscipline01`: `0.75`

### `Preset_P1_M3`
- `missionId`: `P1-M3`
- `missionSceneName`: `P1_M1` (reuse scene for now)
- `missionTitleKey`: `mission.p1m3.title`
- `missionBriefingKey`: `mission.p1m3.briefing`
- `redDurationSeconds`: `6.5`
- `greenDurationSeconds`: `2.8`
- `minVehicleDistanceMeters`: `4`
- `maxVehicleDistanceMeters`: `22`
- `vehicleCycleSpeed`: `1.7`
- `safeVehicleDistanceMeters`: `15`
- `requiredSafeCrosses`: `2`
- `allowCrossOnlyOnGreen`: `true`
- `timingDiscipline01`: `0.72`

## 2) Add scenario applier in mission scene

In `P1_M1` scene:

1. Create object `ScenarioApplier`.
2. Add `MissionScenarioApplier`.
3. Assign:
   - `missionController` -> `MissionRuntime`
   - `trafficLightController` -> `TrafficLight`
   - `vehicleDistanceSimulator` -> `VehicleDistance`
   - `presets` -> all three preset assets

## 2.1) Add mission briefing localization binding

In `P1_M1` scene briefing panel:

1. Add `MissionBriefingController` component on a new object `MissionBriefing`.
2. Assign:
   - `missionController` -> `MissionRuntime`
   - `missionTitleText` -> title text in `BriefingPanel`
   - `missionBriefingText` -> description text in `BriefingPanel`
3. Confirm mission title/briefing changes when starting different mission IDs.

## 3) Add mission nodes in home scene

In `HomeScene`:

1. Duplicate mission node to create `P1-M2` and `P1-M3` buttons.
2. Add a `MissionMapController` to each and assign:

### Node 1
- `missionId`: `P1-M1`
- `missionSceneName`: `P1_M1`

### Node 2
- `missionId`: `P1-M2`
- `missionSceneName`: `P1_M1`

### Node 3
- `missionId`: `P1-M3`
- `missionSceneName`: `P1_M1`

For all nodes:
- `sceneFlowManager` -> `SceneFlow`
- `profileId` -> leave empty
- `missionButton`, `lockOverlay`, `missionLabel` -> assign UI refs

## 4) Test progression

1. Start at `P1-M1` (unlocked).
2. Clear `P1-M1` with score >= 55.
3. Verify `P1-M2` unlocks.
4. Clear `P1-M2` with score >= 55.
5. Verify `P1-M3` unlocks.

## 5) Week 2 expected result

- Three mission nodes visible.
- Nodes unlock sequentially using saved scores.
- Each mission uses different runtime behavior from presets.
- Same mission scene can represent multiple mission difficulties.
