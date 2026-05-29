# Week 1 Execution (Started)

This workspace was initialized for Week 1 MVP delivery of `Safe Steps Addis`.

## Completed setup

- Created Unity-ready folder structure under `Assets/_Game`.
- Added core gameplay runtime state and mission definition models.
- Added safety evaluation logic for red/green crossing behavior.
- Added score calculator aligned with Week 1 scoring model.
- Added mission controller flow (`Briefing -> Play -> Evaluate -> Result`).
- Added traffic light cycle controller for scene runtime.
- Added simulated vehicle distance feed for safety checks.
- Added local JSON save service stubs for progress and settings.
- Added bilingual localization seed files (English and Amharic).
- Added localization bootstrap + localized text helper components.
- Added minimal HUD controller binding points.
- Added UI mission flow and result panel controllers.
- Added Unity scene wiring guide for immediate playtesting.
- Added home scene flow manager and menu controller.
- Added mission progression unlock service and map node controller.
- Added Home + Map wiring guide.
- Added profile session support and profile selector UI stub.
- Added mission coin rewards and coin balance label support.
- Added result panel back-home flow support.
- Added mission node scene launch support from map.
- Added lightweight local store flow (buy/equip cosmetics with coins).
- Added avatar loadout preview panel controller.
- Added save events so coin/loadout UI refreshes automatically.
- Added Week 1 QA checklist with pass/fail test cases.
- Added Week 2 foundation code for multi-mission selection and scenario presets.
- Added mission progress summary panel controller for HomeScene.
- Added facilitator local report export feature (JSON snapshot).
- Localized new progress/export UI labels for EN/AM keys.
- Localized remaining HUD/result/store/coin/map/common labels for EN/AM.
- Removed remaining default English profile-name strings (using neutral `P#` ids).
- Added runtime EN/AM language toggle with auto-refresh subscriptions.
- Added optional QA localization refresh controller for HomeScene.
- Added Week 2 regression checklist for multi-system QA coverage.
- Added QA bug reporting docs (template, severity rubric, triage workflow).
- Added Week 2 execution board and daily standup template.
- Added Week 3 execution board, pilot readiness checklist, and performance baseline template.
- Added Week 4 execution board, release freeze policy, and pilot handoff package checklist.
- Added Week 5-8 macro board and post-pilot feedback/backlog frameworks.
- Added master execution index as single docs entry point.
- Added root README plus runtime mission-identity fix for accurate multi-mission progression saves.
- Implemented Week 3 deterministic challenge events (distraction/pressure) with setup guide.
- Implemented Week 3 offline analytics logging for decisions/events/mission summaries.
- Added Week 3 analytics summary export feature and setup guide.
- Added Week 3 in-app analytics insights panel (active profile stats).
- Added Week 3 facilitator dashboard toggle panel setup and controller.
- Added Week 4 facilitator maintenance tools (clear/reset actions with confirmation).
- Hardened maintenance reset flow with timeout and optional danger-zone visuals.
- Added optional facilitator PIN lock gate for dashboard access.
- Hardened facilitator PIN entry with failed-attempt lockout timer.
- Added live lockout countdown/status refresh for facilitator PIN panel.
- Added per-attempt PIN cooldown to further slow repeated guesses.
- Added local facilitator PIN audit logging (success/failure/lockout events).
- Added facilitator maintenance action to clear PIN audit logs.
- Added live facilitator PIN security-state status label support.
- Added optional QA controls to simulate/clear PIN security states.
- Added a 3-minute PIN security quick-test checklist for recurring QA.
- Added dedicated security bug template for PIN/cooldown/lockout/audit triage.
- Added Week 4 security regression matrix for daily pass/fail tracking.
- Added example filled-run section to accelerate security matrix adoption.
- Added daily security log template for recurring PIN regression archives.
- Added Week 4 security docs map to simplify QA workflow navigation.
- Added README section linking the Week 4 facilitator PIN security doc stack.
- Added a 60-second security QA shortcut block for faster team onboarding.
- Added HomeScene security wiring checklist with exact inspector mapping.
- Added QA scene variant checklist to separate production vs test security settings.
- Added security release gate checklist for pilot/release go-no-go decisions.
- Added a filled security release-gate sample for first sign-off reference.
- Added first build-specific security sign-off record (`rc-0.5.0`).
- Added reusable security sign-off template for future RC builds.
- Added text-based UI wireframe pack for HomeScene/HUD/facilitator flows.
- Added Unity UI prefab checklist for drag-and-drop scene assembly.
- Added strict UI prefab binding table for exact inspector wiring.
- Added HomeScene final wiring pass sheet for setup-day end-to-end validation.
- Added 30-minute Day-0 Unity setup checklist for new teammate onboarding.
- Added printable combined Day-0 + final wiring checklist for setup sessions.
- Added ultra-condensed 1-page operator cheat sheet for onsite validation.
- Added Amharic operator cheat sheet plus endgame runbook and final acceptance checklist.
- Improved analytics summary export status controller to preserve state and refresh on language change.
- Improved facilitator report export status controller with the same stateful localization-safe behavior.

## Next immediate actions in Unity Editor

1. Create a Unity project in this folder (Unity LTS + URP mobile).
2. Drag scripts into scene objects:
   - `MissionController`
   - `HUDController`
3. Create first mission asset from `MissionDefinition`.
4. Wire UI buttons:
   - Cross button -> `MissionController.OnCrossAttempt()`
   - Wait button -> no penalty action
5. Build Android debug APK and validate mission loop.
6. Wire store panel items using `StoreItemController`.

## Week 1 done criteria

- P1-M1 playable loop from briefing to result.
- Red/green crossing safety logic active.
- Score and stars shown.
- Result JSON persisted locally.
- EN/AM key files loaded by localization layer.
- Coin balance updates after mission completion and store purchase.
