# Safe Steps Addis — Technical MVP Reference
### Full Architecture, Phase Breakdown, and Implementation Detail

---

## Tech Stack

| Layer | Technology | Version / Notes |
|---|---|---|
| **Engine** | Unity | LTS, URP 3D (Universal Render Pipeline) |
| **Language** | C# (.NET) | Namespace: `SGame.*` |
| **Platform** | Android | Debug APK → Release APK |
| **Persistence** | `System.IO` + `JsonUtility` | Local flat-file JSON, no SQLite, no cloud |
| **Rendering** | Unity URP Mobile | 30 FPS target, low/mid-tier device optimised |
| **Localization** | Custom key-value service | EN + AM seed files, runtime hot-swap |
| **Analytics** | Custom offline event logger | Append-only JSON log, no network |
| **Security** | Custom PIN gate + audit log | Per-attempt cooldown + timed lockout |
| **Build target** | `Application.persistentDataPath` | Survives app updates on device |

---

## Folder / Namespace Structure

```
Assets/_Game/
├── Core/               # SGame.Core       — bootstrap, scene flow
├── Gameplay/           # SGame.Gameplay   — mission, safety, scoring, events
├── Data/               # SGame.Data       — persistence, analytics, session
├── UI/                 # (UI controllers, no namespace layer)
└── Localization/       # SGame.Localization (implied)

Docs/                   # All planning, QA, wiring, runbooks
```

---

## Phase 1 — Core Mission Loop (Week 1)

**Goal:** A single playable mission from briefing to result, running on device.

### State Machine

`MissionController` drives a linear FSM:

```
None → Briefing → Playing → Evaluating → Playing (loop) → Result
```

`MissionRuntimeState` is an enum; transitions fire `StateChanged` event consumed by `MissionFlowUIController` and `HUDController`.

### Safety Evaluation

`SafetyEvaluator.EvaluateCrossAttempt()` — pure static, no side effects.

Three ordered checks (short-circuit on first failure):

```
1. Signal red + allowCrossOnlyOnGreen  → unsafe "feedback.unsafe.red_light"
2. nearestVehicleDistance < safeDistance → unsafe "feedback.unsafe.vehicle_close"
3. !usedCrosswalk                       → unsafe "feedback.unsafe.no_crosswalk"
4. All pass                             → safe  "feedback.safe.cross_now"
```

Input: `SafetyDecisionInput` (readonly struct)  
Output: `SafetyDecisionResult` (readonly struct, IsSafe + localisation key)

### Scoring

`ScoreCalculator.Calculate(MissionScoreInput, MissionDefinition)` — pure static.

Inputs: `safeDecisions`, `unsafeDecisions`, `timingDiscipline01 [0–1]`  
Outputs: `FinalScore (int)`, `Stars (1–3)`

Coin reward formula:
```
earnedCoins = Max(0, baseCoinReward + (stars * starBonusMultiplier))
```

### Persistence — `SaveService` (static)

All writes use an atomic pattern:
```
write → .tmp file
delete → original
rename → .tmp to original
```

Files written to `Application.persistentDataPath`:
- `profiles.json` — `ProfileCollection` (list of `ProfileRecord`)
- `progress_{profileId}.json` — `ProfileProgress` (coins, missions[], ownedItems[], equipped slots)

`SaveService.ProgressChanged` event (static) — subscribed by `CoinBalanceLabel`, store controllers, and progress panels for reactive UI refresh without polling.

### Scene Flow

```
HomeScene  →  [MissionMapController selects preset]
           →  P1_M1 (Mission Scene)
           →  [ResultPanelController back-home button]
           →  HomeScene
```

`SceneFlowManager` wraps `SceneManager.LoadScene` with a consistent API used across all scenes.

---

## Phase 2 — Multi-Mission, Progression, Store (Week 2)

**Goal:** Sequential unlock, facilitator export, progress summary, data-driven presets.

### Mission Progression

`ProgressionService` — queries `SaveService.GetMissionResult()` per mission ID.  
Unlock rule: mission `N+1` unlocks when mission `N` has `stars >= 1`.

`MissionMapController` — renders node states (locked/unlocked/completed) and launches scenes via `MissionScenarioApplier`.

### Data-Driven Missions — `MissionScenarioPreset` (ScriptableObject)

Per-mission configuration asset containing:
- `missionId` (e.g. `"P1-M3"`)
- `missionTitleKey` / `missionBriefingKey` (localisation keys)
- `safeVehicleDistanceMeters`
- `requiredSafeCrosses`
- `allowCrossOnlyOnGreen`
- `timingDiscipline01`

Applied at runtime by `MissionScenarioApplier` → calls `MissionController.ConfigureRuntime()` + `ConfigureMissionIdentity()` before `StartMission()`.

This means **zero scene duplication** — one mission scene handles all 5 presets via runtime config.

### Facilitator Report Export — `FacilitatorReportService`

Snapshot: `SaveService.GetProgressSnapshot(profileId)` → serialise to JSON  
Written to `Application.persistentDataPath/facilitator_report_{profileId}_{timestamp}.json`

`FacilitatorReportExportController` drives the UI state (idle → exporting → done/error), preserves state on language change.

### Progress Summary Panel

`MissionProgressSummaryController` — reads all mission results for active profile, renders star/completion counts per mission.

---

## Phase 3 — Events, Analytics, Facilitator Dashboard (Week 3)

**Goal:** Realistic challenge events during missions; full offline analytics pipeline; facilitator monitoring tools.

### Challenge Event System

`MissionChallengeEventController` — deterministic event scheduler.

Event types: `distraction`, `pressure`  
Triggered at configured timestamps during a mission.  
On trigger: calls `MissionController.EmitGuidance(reasonKey, positiveTone, guidanceType)` which:
1. Fires `DecisionEvaluated` event → UI feedback
2. Calls `AnalyticsEventService.LogChallengeEvent()`

### Analytics Pipeline

Three log methods in `AnalyticsEventService`:

| Method | Fires When | Payload |
|---|---|---|
| `LogDecision()` | Every Cross/Wait button press | profileId, missionId, isSafe, reasonKey, elapsed |
| `LogChallengeEvent()` | Challenge event triggered | profileId, missionId, eventType, reasonKey, elapsed |
| `LogMissionSummary()` | Mission completion | profileId, missionId, safeCount, unsafeCount, score, stars, coins, duration |

All events append to a local JSON log file (`analytics_log.json`).

### Analytics Summary Export — `AnalyticsSummaryExportService`

Aggregates raw log → per-profile summary (total decisions, safe%, missions completed, average stars).  
Written to `analytics_summary_{profileId}_{timestamp}.json`.

`AnalyticsSummaryExportController` — same stateful localization-safe pattern as report export controller.

### Analytics Insights Panel

`AnalyticsInsightsPanelController` / `AnalyticsInsightsService` — reads analytics log for active profile, computes live stats, renders in-app panel for facilitator review without exporting.

### Facilitator Dashboard

`FacilitatorDashboardController` — toggle panel in HomeScene.  
Surfaces: analytics insights panel, export button, maintenance tools.  
Access is gated by the PIN system (Phase 4).

---

## Phase 4 — Security, UI Polish, Release Gate (Week 4)

**Goal:** PIN-protected facilitator access, hardened security, full UI wiring, QA sign-off.

### Facilitator PIN System

`FacilitatorPinAuditService` — append-only local audit log of all PIN events.

Security model:

```
Per-attempt cooldown:   increasing delay after each wrong attempt
Failed-attempt lockout: after N wrong attempts → lock for T minutes
```

`FacilitatorMaintenanceController` drives:
- Confirm-before-execute pattern for destructive actions
- Timeout on confirmation (auto-cancel if not confirmed)
- Optional danger-zone visual state
- "Clear PIN audit logs" action (itself audited)

`FacilitatorMaintenanceService` — business logic for clear/reset operations called by the controller.

### PIN Audit Log Events

| Event Type | Logged When |
|---|---|
| `pin_success` | Correct PIN entered |
| `pin_failure` | Wrong PIN entered |
| `pin_lockout` | Lockout threshold reached |
| `pin_cooldown` | Per-attempt cooldown active |

Each entry: `{ timestamp, eventType, attemptCount }`.

### Security Live Status

`FacilitatorDashboardController` exposes a security-state label showing:
- Current lockout countdown (live refresh via coroutine)
- Number of failed attempts since last success
- Whether cooldown is active

### UI Architecture

All UI controllers follow the same contract:
- `Awake` — find/cache references
- `OnEnable` — subscribe to `SaveService.ProgressChanged` and `LocalizationService.LanguageChanged`
- `OnDisable` — unsubscribe (no memory leaks)
- `Refresh()` — idempotent full redraw, called on subscription events

Key controllers and their responsibilities:

| Controller | Scene | Responsibility |
|---|---|---|
| `HomeMenuController` | HomeScene | Entry point buttons, profile display |
| `MissionMapController` | HomeScene | Node rendering, scene launch |
| `HUDController` | MissionScene | Live signal/vehicle/decision feedback |
| `MissionFlowUIController` | MissionScene | Panel show/hide per FSM state |
| `MissionBriefingController` | MissionScene | Localised briefing text from preset |
| `ResultPanelController` | MissionScene | Score, stars, coins, back-home |
| `StorePanelController` | HomeScene | Item grid, buy/equip actions |
| `AvatarLoadoutPreviewController` | HomeScene | Equipped item display |
| `CoinBalanceLabel` | Multiple | Reactive coin display |
| `ProfileSelectorController` | HomeScene | Multi-profile switch |
| `LanguageToggleController` | HomeScene | EN/AM runtime toggle |
| `MissionProgressSummaryController` | HomeScene | Stars/completion per mission |
| `FacilitatorDashboardController` | HomeScene | Dashboard toggle, PIN gate |
| `FacilitatorReportExportController` | HomeScene | Export UI state machine |
| `AnalyticsSummaryExportController` | HomeScene | Analytics export UI state machine |
| `AnalyticsInsightsPanelController` | HomeScene | Live insights display |
| `FacilitatorMaintenanceController` | HomeScene | Maintenance actions, confirm flow |
| `LocalizedText` | All scenes | Auto-refresh text on language change |
| `LocalizationRefreshController` | HomeScene | Optional QA force-refresh trigger |

---

## Localization Architecture

`LocalizationService` (singleton, loaded at `LocalizationBootstrap.Awake`):
- Loads `en.json` / `am.json` from `Resources/` or `StreamingAssets/`
- Exposes `Get(key)` → localised string
- Fires `LanguageChanged` event on switch
- `LocalizedText` components subscribe and auto-redraw on event

Switching language at runtime: `LocalizationService.SetLanguage(lang)` → fires event → all `LocalizedText` and subscribed controllers refresh synchronously.

---

## Data Flow Diagram

```
[Button Press]
     │
     ▼
MissionController.OnCrossAttempt()
     │
     ├──► SafetyEvaluator.EvaluateCrossAttempt()  ──► SafetyDecisionResult
     │         (pure, no side effects)
     │
     ├──► AnalyticsEventService.LogDecision()      ──► analytics_log.json (append)
     │
     ├──► DecisionEvaluated event                  ──► HUDController (UI feedback)
     │
     └──► [if safeDecisions >= required]
               │
               ▼
          ScoreCalculator.Calculate()              ──► FinalScore, Stars
               │
               ├──► SaveService.SaveMissionResult() ──► progress_{id}.json (atomic)
               ├──► SaveService.AddCoins()           ──► progress_{id}.json (atomic)
               ├──► AnalyticsEventService.LogMissionSummary()
               └──► MissionCompleted event          ──► ResultPanelController
```

---

## Persistence File Map

| File | Contains | Written By |
|---|---|---|
| `profiles.json` | All profile records | `SaveService.CreateProfile()` |
| `progress_{id}.json` | Coins, missions, owned/equipped items | `SaveService` (multiple methods) |
| `analytics_log.json` | Raw event stream | `AnalyticsEventService` |
| `analytics_summary_{id}_{ts}.json` | Aggregated per-profile stats | `AnalyticsSummaryExportService` |
| `facilitator_report_{id}_{ts}.json` | Progress snapshot export | `FacilitatorReportService` |
| `pin_audit_log.json` | PIN security events | `FacilitatorPinAuditService` |

All files live in `Application.persistentDataPath`. All writes are atomic (`.tmp` → rename).

---

## Phase 5–8 — Pilot and Post-Pilot (Weeks 5–8)

**Goal:** Real-world school pilot, feedback loop, bug triage, final handoff.

| Phase | Key Activities |
|---|---|
| **Week 5** | Device distribution, facilitator onboarding, first pilot session |
| **Week 6** | Collect facilitator exports + analytics summaries, synthesise feedback |
| **Week 7** | Fix P1/P2 bugs from pilot data, regression re-test |
| **Week 8** | Final release candidate, sign-off, handoff package delivery |

Governed by:
- `PILOT_READINESS_CHECKLIST.md` — go/no-go gate
- `PILOT_FEEDBACK_SYNTHESIS_TEMPLATE.md` — convert observations to actionable issues
- `POST_PILOT_BACKLOG_FRAMEWORK.md` — priority triage by safety/learning impact
- `FINAL_ACCEPTANCE_CHECKLIST.md` — last gate before handoff

---

## Key Design Decisions

| Decision | Rationale |
|---|---|
| Flat-file JSON over SQLite | No native plugin required, works on all Android targets, easy to inspect/export |
| Static `SaveService` with event bus | Avoids singleton MonoBehaviour lifecycle issues; any UI component can subscribe |
| Pure static `SafetyEvaluator` | Fully unit-testable without Unity context |
| ScriptableObject presets for missions | Zero scene duplication; mission tuning without code changes |
| Atomic `.tmp` write pattern | Prevents partial/corrupt saves on low-battery or interrupted writes |
| 30 FPS target frame rate | Low/mid Android tier headroom; educational app does not require 60 FPS |
| Offline-only analytics | No GDPR/COPPA surface for child users; facilitator exports data manually |
| PIN lockout + per-attempt cooldown | Discourages brute-force by children without requiring a server or account |

---

## Release Candidate Gate

Before any pilot build is distributed, all of the following must pass:

- [ ] `WEEK4_SECURITY_RELEASE_GATE_CHECKLIST.md` — PIN security scenarios
- [ ] `WEEK4_SECURITY_REGRESSION_MATRIX.md` — all rows green
- [ ] `WEEK1_QA_CHECKLIST.md` + `WEEK2_REGRESSION_CHECKLIST.md`
- [ ] `PERFORMANCE_BASELINE_TEMPLATE.md` — frame time within budget on target device tier
- [ ] `PILOT_READINESS_CHECKLIST.md` — technical + educational gate
- [ ] `SecuritySignoff_{date}_{rc}.md` — signed off by responsible engineer

---

*Safe Steps Addis | Platform: Android | Engine: Unity LTS URP | Language: C# | Offline-first*
