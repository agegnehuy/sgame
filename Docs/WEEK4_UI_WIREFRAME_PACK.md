# Week 4 UI Wireframe Pack (Text Layout)

Use these wireframes as direct scene-building guides in Unity Canvas.

## 1) HomeScene Wireframe

```text
+--------------------------------------------------+
| Profile: P1          Coins: 120      EN | AM    |
+--------------------------------------------------+
|                                                  |
|         Safe Steps Addis (logo/title)            |
|                                                  |
|        [ Mission Nodes / Map Section ]           |
|      [P1-M1] [P1-M2] [P1-M3] [P1-M4] [P1-M5]     |
|                                                  |
|                 [ PLAY MISSION ]                 |
|                                                  |
| [Store]     [Avatar]     [Reports]     [Exit]   |
|                                                  |
|                              [Open Facilitator]  |
+--------------------------------------------------+
```

### HomeScene hierarchy suggestion

- `TopBar`
  - `ProfileLabel`
  - `CoinBalanceLabel`
  - `LanguageToggle`
- `MissionMapPanel`
  - mission nodes/buttons
- `PrimaryActionPanel`
  - `PlayButton`
- `BottomActionsPanel`
  - `StoreButton`, `AvatarButton`, `ReportsButton`, `QuitButton`
- `FacilitatorDashboardToggleButton`

## 2) Facilitator Dashboard + PIN Overlay

```text
           +--------------------------------------+
           | Facilitator Dashboard                |
           |--------------------------------------|
           | Analytics Summary                    |
           | - Total decisions                    |
           | - Safe ratio                         |
           | - Top mission                        |
           |--------------------------------------|
           | [Export Report] [Export Analytics]   |
           | [Clear Analytics] [Clear Exports]    |
           | [Clear PIN Audit]                    |
           | [Reset Profile Progress]             |
           | Status: ...                          |
           +--------------------------------------+

                 +---------------------------+
                 | Enter Facilitator PIN     |
                 | [ _ _ _ _ ]              |
                 | [Submit]    [Cancel]      |
                 | Status: Incorrect PIN      |
                 | Security: Cooldown (2s)    |
                 +---------------------------+
```

### PIN modal essentials

- Large input field and buttons (child-safe touch size)
- `pinStatusText` for prompt/invalid/lockout feedback
- `pinSecurityStateText` for ready/cooldown/lockout state

## 3) MissionScene HUD Wireframe

```text
+--------------------------------------------------+
| Mission: P1-M1                  Coins: 120       |
+--------------------------------------------------+
|                                                  |
|                 3D Gameplay View                 |
|             (crosswalk / traffic / avatar)       |
|                                                  |
| Feedback: SAFE / UNSAFE                          |
| Reason: localized feedback text                  |
|                                                  |
|               [ WAIT ]   [ CROSS ]               |
+--------------------------------------------------+
```

### HUD essentials

- High contrast feedback chip (`SAFE`/`UNSAFE`)
- Two large action buttons, spaced for thumb input
- Minimal clutter while keeping learning feedback visible

## 4) Result Panel Wireframe

```text
              +-----------------------------+
              | Mission Complete            |
              | Score: 78                   |
              | Stars: ★★★                  |
              | +25 coins                   |
              |                             |
              | [Replay]   [Back Home]      |
              +-----------------------------+
```

### Result panel essentials

- Always show score, stars, coins in one glance
- Keep next actions obvious (`Replay`, `Back Home`)

## 5) Facilitator Maintenance Focus State

```text
           +--------------------------------------+
           | Maintenance                          |
           | [Clear Analytics] [Clear Exports]    |
           | [Clear PIN Audit]                    |
           | [Reset Profile Progress]             |
           |--------------------------------------|
           | Status: Press reset again to confirm |
           | (danger background + indicator on)   |
           +--------------------------------------+
```

### Reset safety UX

- First press arms reset (warning color)
- Second press executes reset
- Auto-timeout disarms and returns neutral status

## 6) Sizing + Spacing defaults (mobile portrait)

- Base reference: `1080x1920` Canvas Scaler
- Primary buttons: min height `96 px`
- Text sizes:
  - Titles: `44-52`
  - Body: `30-36`
  - Status labels: `28-32`
- Edge padding: `24-32 px`
- Inter-button spacing: `16-24 px`

## 7) Implementation order

1. Build HomeScene layout skeleton.
2. Add facilitator dashboard panel.
3. Add PIN modal and bind controller fields.
4. Add maintenance panel fields and confirm status visuals.
5. Build Mission HUD and Result panel.
6. Validate EN/AM overflow and touch-target comfort.
