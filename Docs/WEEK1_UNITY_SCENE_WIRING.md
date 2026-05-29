# Week 1 Unity Scene Wiring (P1-M1)

Use this to make a playable first mission quickly.

## 1) Create project and import scripts

1. Open Unity Hub and create a **3D URP** project in this folder.
2. Let Unity import all files under `Assets/_Game`.
3. Create a new scene: `Assets/_Game/Scenes/P1_M1.unity`.

## 2) Create mission config asset

1. Right click in `Assets/_Game/Gameplay`.
2. Create -> `SGame` -> `Mission Definition`.
3. Name it `Mission_P1_M1`.
4. In Inspector set:
   - `missionId`: `P1-M1`
   - `requiredSafeCrosses`: `1`
   - `safeVehicleDistanceMeters`: `12`

## 3) Scene GameObjects

Create these objects:

- `Bootstrap` (empty)
  - Add `GameBootstrap`
- `SceneFlow` (empty)
  - Add `SceneFlowManager`
  - Set:
    - `homeSceneName`: `HomeScene`
    - `missionSceneName`: `P1_M1`
- `Localization` (empty)
  - Add `LocalizationBootstrap`
  - Assign:
    - `englishFile`: `localization_en.json` text asset
    - `amharicFile`: `localization_am.json` text asset
    - `defaultLanguage`: `en`
- `MissionRuntime` (empty)
  - Add `MissionController`
  - Assign `missionDefinition` = `Mission_P1_M1`
- `TrafficLight` (empty)
  - Add `TrafficLightController`
  - Assign `missionController` = `MissionRuntime`
- `VehicleDistance` (empty)
  - Add `VehicleDistanceSimulator`
  - Assign `missionController` = `MissionRuntime`
- `Canvas` (UI)
  - Add panel and buttons described below

## 4) UI hierarchy

Inside `Canvas`, create:

- `BriefingPanel` (panel)
  - `MissionTitleText` (`Text`)
  - `MissionBriefingText` (`Text`)
  - `StartMissionButton` (`Button`)
- `GameplayPanel` (panel)
  - `FeedbackText` (`Text`)
  - `ScoreText` (`Text`)
  - `CrossButton` (`Button`)
  - `WaitButton` (`Button`)
- `ResultPanel` (panel)
  - `ResultTitleText` (`Text`)
  - `ResultDetailText` (`Text`)
  - `StarsText` (`Text`)
  - `CoinsText` (`Text`)
  - `BackHomeButton` (`Button`)
  - `RetryButton` (`Button`)
- `HUD` (empty under `GameplayPanel`)
  - Add `HUDController`
  - Assign:
    - `missionController` = `MissionRuntime`
    - `crossButton` = `CrossButton`
    - `waitButton` = `WaitButton`
    - `feedbackText` = `FeedbackText`
    - `scoreText` = `ScoreText`
- `MissionFlowUI` (empty under `Canvas`)
  - Add `MissionFlowUIController`
  - Assign:
    - `missionController` = `MissionRuntime`
    - `briefingPanel` = `BriefingPanel`
    - `gameplayPanel` = `GameplayPanel`
    - `resultPanel` = `ResultPanel`
    - `startMissionButton` = `StartMissionButton`
    - `retryButton` = `RetryButton`
- `MissionBriefing` (empty under `BriefingPanel`)
  - Add `MissionBriefingController`
  - Assign:
    - `missionController` = `MissionRuntime`
    - `missionTitleText` = `MissionTitleText`
    - `missionBriefingText` = `MissionBriefingText`
- `ResultLogic` (empty under `ResultPanel`)
  - Add `ResultPanelController`
  - Assign:
    - `missionController` = `MissionRuntime`
    - `resultTitleText` = `ResultTitleText`
    - `resultDetailText` = `ResultDetailText`
    - `starsText` = `StarsText`
    - `coinsText` = `CoinsText`
    - `backHomeButton` = `BackHomeButton`
    - `sceneFlowManager` = `SceneFlow`

## 5) Basic level geometry (graybox)

Add cubes/planes:

- Road plane
- Two sidewalk blocks
- Zebra crossing strip (white cubes/texture)

Optional:
- Add car placeholder object and manually set vehicle distance in `MissionController`.

## 6) Test flow

1. Enter Play mode.
2. Press `Start Mission`.
3. Keep signal red and press `Cross` -> should return unsafe.
4. Toggle signal by waiting for auto cycle, then press `Cross` on green -> should pass.
5. Score and stars should appear in `ScoreText`.
6. Check JSON output in `Application.persistentDataPath`:
   - `progress_p1.json`

## 7) Optional polish in Week 1

- Add `LocalizedText` component to mission labels/buttons.
- Add two materials for red/green light renderer fields.
- Add simple vehicle movement and feed distance to `MissionController.UpdateNearestVehicleDistance()`.
