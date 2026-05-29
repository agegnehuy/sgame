# Week 1 Home + Mission Map Wiring

This adds a simple front door flow before entering `P1_M1`.

## 1) Create `HomeScene`

1. Create scene: `Assets/_Game/Scenes/HomeScene.unity`.
2. Add empty object `SceneFlow`.
   - Attach `SceneFlowManager`.
   - Set:
     - `homeSceneName`: `HomeScene`
     - `missionSceneName`: `P1_M1`
3. Add `Canvas` with:
   - `PlayButton` (`Button`)
   - `QuitButton` (`Button`)
   - `CoinText` (`Text`)
   - `AvatarPanel` (`Panel`)
     - `ProfileNameText` (`Text`)
     - `AvatarIdText` (`Text`)
     - `EquippedClothesText` (`Text`)
     - `EquippedShoesText` (`Text`)
     - `EquippedAccessoryText` (`Text`)
   - `StorePanel` (`Panel`)
     - `StoreItem_Shirt` (`Button` + `StoreItemController`)
     - `StoreItem_Shoes` (`Button` + `StoreItemController`)
   - `ProfileDropdown` (`Dropdown`)
   - `NewProfileNameInput` (`InputField`)
   - `CreateProfileButton` (`Button`)
   - `LanguageEnglishButton` (`Button`)
   - `LanguageAmharicButton` (`Button`)
   - `CurrentLanguageText` (`Text`)
   - `RefreshLocalizationButton` (`Button`) [optional QA]
   - `RefreshLocalizationStatusText` (`Text`) [optional QA]
   - `HomeController` (empty with `HomeMenuController`)
   - `ProfileController` (empty with `ProfileSelectorController`)
   - `CoinBalanceController` (empty with `CoinBalanceLabel`)
   - `AvatarPreviewController` (empty with `AvatarLoadoutPreviewController`)
   - `ProgressSummaryController` (empty with `MissionProgressSummaryController`)
   - `StoreController` (empty with `StorePanelController`)
   - `LanguageController` (empty with `LanguageToggleController`)
   - `LocalizationRefreshController` (empty with `LocalizationRefreshController`) [optional QA]
   - `ExportReportButton` (`Button`)
   - `ExportStatusText` (`Text`)
   - `FacilitatorReportController` (empty with `FacilitatorReportExportController`)
   - `ExportAnalyticsSummaryButton` (`Button`) [optional Week 3]
   - `ExportAnalyticsSummaryStatusText` (`Text`) [optional Week 3]
   - `AnalyticsSummaryController` (empty with `AnalyticsSummaryExportController`) [optional Week 3]
   - `AnalyticsInsightsPanel` (`Panel`) [optional Week 3]
   - `AnalyticsInsightsController` (empty with `AnalyticsInsightsPanelController`) [optional Week 3]
   - `FacilitatorDashboardToggleButton` (`Button`) [optional Week 3]
   - `FacilitatorDashboardPanel` (`Panel`) [optional Week 3]
   - `FacilitatorDashboardController` (empty with `FacilitatorDashboardController`) [optional Week 3]
   - `FacilitatorMaintenanceController` (empty with `FacilitatorMaintenanceController`) [optional Week 4]
4. Assign in `HomeMenuController`:
   - `sceneFlowManager` -> `SceneFlow`
   - `playButton` -> `PlayButton`
   - `quitButton` -> `QuitButton`
5. Assign in `ProfileSelectorController`:
   - `profileDropdown` -> `ProfileDropdown`
   - `newProfileNameInput` -> `NewProfileNameInput`
   - `createProfileButton` -> `CreateProfileButton`
   - `coinBalanceLabel` -> `CoinBalanceController`
   - `avatarLoadoutPreview` -> `AvatarPreviewController`
   - `progressSummaryController` -> `ProgressSummaryController`
6. Assign in `CoinBalanceLabel`:
   - `coinText` -> `CoinText`
7. Assign in `AvatarLoadoutPreviewController`:
   - `profileNameText` -> `ProfileNameText`
   - `avatarIdText` -> `AvatarIdText`
   - `clothesText` -> `EquippedClothesText`
   - `shoesText` -> `EquippedShoesText`
   - `accessoryText` -> `EquippedAccessoryText`
8. Assign in `StoreItemController` for each store item:
   - `itemId` -> unique item id (example: `shirt_blue_01`)
   - `slot` -> `clothes`, `shoes`, or `accessory`
   - `cost` -> coin price
   - `coinBalanceLabel` -> `CoinBalanceController`
9. Assign in `StorePanelController`:
   - `coinBalanceLabel` -> `CoinBalanceController`
   - `itemControllers` -> all store item controllers in panel
10. Assign in `FacilitatorReportExportController`:
   - `exportButton` -> `ExportReportButton`
   - `statusText` -> `ExportStatusText`
11. Optional Week 3 analytics summary wiring:
   - `exportButton` -> `ExportAnalyticsSummaryButton`
   - `statusText` -> `ExportAnalyticsSummaryStatusText`
12. Optional Week 3 analytics insights panel wiring:
   - `totalDecisionsText` -> `AnalyticsTotalDecisionsText`
   - `safeRatioText` -> `AnalyticsSafeRatioText`
   - `topMissionText` -> `AnalyticsTopMissionText`
   - `challengeEventsText` -> `AnalyticsChallengeEventsText`
   - `missionCountText` -> `AnalyticsMissionCountText`
   - `refreshButton` -> `AnalyticsRefreshButton` (optional)
13. Optional Week 3 facilitator dashboard wiring:
   - `dashboardPanel` -> `FacilitatorDashboardPanel`
   - `toggleButton` -> `FacilitatorDashboardToggleButton`
   - `toggleButtonText` -> `FacilitatorDashboardToggleText`
   - `dashboardTitleText` -> `FacilitatorDashboardTitleText`
   - `analyticsInsightsPanelController` -> `AnalyticsInsightsController`
14. Optional Week 4 maintenance wiring:
   - `clearAnalyticsButton` -> `ClearAnalyticsLogsButton`
   - `clearExportsButton` -> `ClearExportFilesButton`
   - `resetProfileProgressButton` -> `ResetProfileProgressButton`
   - `statusText` -> `MaintenanceStatusText`
15. Assign in `LanguageToggleController`:
   - `localizationBootstrap` -> `Localization`
   - `englishButton` -> `LanguageEnglishButton`
   - `amharicButton` -> `LanguageAmharicButton`
   - `currentLanguageText` -> `CurrentLanguageText`
16. Optional: add `LocalizedText` to language button labels:
   - EN button key: `ui.language.button.en`
   - AM button key: `ui.language.button.am`
17. Optional QA wiring in `LocalizationRefreshController`:
   - `refreshButton` -> `RefreshLocalizationButton`
   - `statusText` -> `RefreshLocalizationStatusText`

## 2) Add mission map lock display (optional in Week 1)

Inside `HomeScene` canvas:

1. Add `MissionNode_P1_M1` (`Button`) and optional lock icon overlay object.
2. Add `MapController` empty object with `MissionMapController`.
3. Assign:
   - `profileId`: leave empty to use active profile
   - `missionId`: `P1-M1`
   - `missionButton` -> `MissionNode_P1_M1`
   - `lockOverlay` -> lock icon object
   - `missionLabel` -> text object under mission node
   - `sceneFlowManager` -> `SceneFlow`
   - `missionSceneName` -> `P1_M1`

For Week 1, `P1-M1` is always unlocked by default.

## 3) Build settings

Open Build Settings and add scenes in order:

1. `HomeScene`
2. `P1_M1`

Set `HomeScene` as index 0 start scene.

## 4) Test expected behavior

1. Launch app -> `HomeScene` opens.
2. Press mission node or Play -> loads `P1_M1`.
3. Complete mission -> score and coin rewards are saved.
4. Press Back Home on result panel -> returns to `HomeScene`.
5. Buy one store item if enough coins -> coin balance decreases and item equips.
6. Confirm avatar panel updates equipped item text after purchase/equip.
7. Confirm progress summary panel updates best score/attempts after mission completion.
8. Tap export report button and confirm JSON file path is shown in status text.
9. Optional Week 3: tap analytics summary export and confirm JSON path is shown.
10. Optional Week 3: verify analytics insights panel updates after mission play.
11. Tap language buttons and confirm labels update immediately without restart.
12. Optional QA: tap refresh localization button and verify status text updates.
