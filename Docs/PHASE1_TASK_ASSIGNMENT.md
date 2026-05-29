# Kidy Road Safety Learning Game
## Phase 1 — Level 1 Task Assignment
### Project Start Document

---

**Project Name:** Kidy Road Safety Learning Game (Safe Steps Addis)
**Phase:** Phase 1 — Level 1 Only
**Team:** Boss + Developer
**Status:** Not started — starting from zero
**Goal of Phase 1:** Build one complete, playable Level 1 that works on Android

---

## What Phase 1 Delivers

When Phase 1 is done, one person can pick up an Android phone, install the game, and:

1. See the Kidy splash screen
2. Choose their language (English or Amharic)
3. Enter their name and choose a character
4. Complete a short tutorial
5. Play Level 1 — walk to the road, wait for green, cross safely
6. See their score and coins
7. The progress saves on the phone

That is it. Nothing more. One complete experience from start to finish.

---

## Team

| Person | Role |
|--------|------|
| **Boss** | Project Lead — Art, Design, Assets, Decisions, Testing |
| **Developer (You)** | Technical — Programming, Unity Setup, Logic, UI Code |

---

## Full Task List — Phase 1

---

### SECTION A — Project Setup
*Get the tools and workspace ready before any building starts.*

| # | Task | Assigned To | Status |
|---|------|-------------|--------|
| A1 | Install Unity 2022 LTS on the development computer | Boss | ⬜ Not started |
| A2 | Install Android Build Support module in Unity | Boss | ⬜ Not started |
| A3 | Create the Unity project with the correct settings (URP, Android) | Developer | ⬜ Not started |
| A4 | Set up the folder structure inside Unity (Scripts, Scenes, Art, Audio, etc.) | Developer | ⬜ Not started |
| A5 | Connect the project to a version control system (GitHub or similar) | Boss | ⬜ Not started |
| A6 | Write a short project brief (name, goal, target device, target age 8–14) | Boss | ⬜ Not started |

---

### SECTION B — Art & Visual Assets
*All the images, characters, and 3D objects the game needs.*

| # | Task | Assigned To | Status |
|---|------|-------------|--------|
| B1 | Design the game's color palette and visual style (child-friendly, bright) | Boss | ⬜ Not started |
| B2 | Create or source the main character (child figure, simple 3D or 2D sprite) | Boss | ⬜ Not started |
| B3 | Create 2 character outfit options (default + 1 unlockable) | Boss | ⬜ Not started |
| B4 | Create the road environment art (road, sidewalk, zebra crossing lines) | Boss | ⬜ Not started |
| B5 | Create the traffic light model (red light, green light states) | Boss | ⬜ Not started |
| B6 | Create a simple car/vehicle model for traffic | Boss | ⬜ Not started |
| B7 | Create the NPC pedestrian figure (one simple walking character) | Boss | ⬜ Not started |
| B8 | Design the game logo and splash screen image | Boss | ⬜ Not started |
| B9 | Design the main menu background | Boss | ⬜ Not started |
| B10 | Design the coin icon and star icon for the results screen | Boss | ⬜ Not started |
| B11 | Create button designs for all main buttons (Play, Wait, Cross, etc.) | Boss | ⬜ Not started |
| B12 | Import all approved art assets into Unity | Developer | ⬜ Not started |

---

### SECTION C — Audio & Sound
*All sounds the game needs for Level 1.*

| # | Task | Assigned To | Status |
|---|------|-------------|--------|
| C1 | Source or create: traffic light change sound (red → green) | Boss | ⬜ Not started |
| C2 | Source or create: positive feedback sound (success chime) | Boss | ⬜ Not started |
| C3 | Source or create: warning/danger sound (wrong decision) | Boss | ⬜ Not started |
| C4 | Source or create: car passing sound effect | Boss | ⬜ Not started |
| C5 | Source or create: background ambient sound (city streets, light) | Boss | ⬜ Not started |
| C6 | Source or create: button tap sound | Boss | ⬜ Not started |
| C7 | Import all audio files into Unity and organize in Audio folder | Developer | ⬜ Not started |

---

### SECTION D — Game World (Level 1 Scene)
*Building the actual game environment inside Unity.*

| # | Task | Assigned To | Status |
|---|------|-------------|--------|
| D1 | Build the Level 1 scene — road, sidewalk, zebra crossing | Developer | ⬜ Not started |
| D2 | Place the traffic light in the correct position in the scene | Developer | ⬜ Not started |
| D3 | Set up the camera to follow the player character | Developer | ⬜ Not started |
| D4 | Place NPC pedestrians in the scene with basic walk animation | Developer | ⬜ Not started |
| D5 | Add vehicles to the road and set their movement path | Developer | ⬜ Not started |
| D6 | Define the spawn point (where the player starts each mission) | Developer | ⬜ Not started |
| D7 | Define the goal point (where the player must reach to complete the crossing) | Developer | ⬜ Not started |
| D8 | Review the Level 1 scene — does it look right and feel correct? | Boss | ⬜ Not started |

---

### SECTION E — Game Logic & Programming
*The code that makes everything work.*

| # | Task | Assigned To | Status |
|---|------|-------------|--------|
| E1 | Write the character movement system (joystick or tap to move) | Developer | ⬜ Not started |
| E2 | Write the traffic light system (red → green timer, auto-cycle) | Developer | ⬜ Not started |
| E3 | Write the vehicle movement system (cars drive along road, stop at red) | Developer | ⬜ Not started |
| E4 | Write the crossing detection system (did the player use the zebra crossing?) | Developer | ⬜ Not started |
| E5 | Write the safety evaluator (was the decision correct or dangerous?) | Developer | ⬜ Not started |
| E6 | Write the collision system (player touched a moving car = danger) | Developer | ⬜ Not started |
| E7 | Write the mission controller (start mission → gameplay → end mission) | Developer | ⬜ Not started |
| E8 | Write the coin reward logic (correct behavior = coins earned) | Developer | ⬜ Not started |
| E9 | Connect all game logic together and test the full loop in the editor | Developer | ⬜ Not started |

---

### SECTION F — UI Screens
*All the screens the player sees.*

| # | Task | Assigned To | Status |
|---|------|-------------|--------|
| F1 | Build the Splash Screen (logo, loading animation) | Developer | ⬜ Not started |
| F2 | Build the Language Selection Screen (English / Amharic buttons) | Developer | ⬜ Not started |
| F3 | Build the Welcome Screen ("Welcome to Safe Steps Addis!") | Developer | ⬜ Not started |
| F4 | Build the Profile Creation Screen (name entry + character select) | Developer | ⬜ Not started |
| F5 | Build the Main Home Screen (mission map, play button, coin display) | Developer | ⬜ Not started |
| F6 | Build the Mission HUD (in-game display: traffic light status, coins) | Developer | ⬜ Not started |
| F7 | Build the Feedback Popup (the message that appears after each decision) | Developer | ⬜ Not started |
| F8 | Build the Mission Result Screen (score, stars, coins earned, play again) | Developer | ⬜ Not started |
| F9 | Review all screens — do they look good and feel easy for children? | Boss | ⬜ Not started |
| F10 | Approve all screens before connecting to game logic | Boss | ⬜ Not started |

---

### SECTION G — Tutorial
*The guided first-time experience for new players.*

| # | Task | Assigned To | Status |
|---|------|-------------|--------|
| G1 | Write the tutorial script — all instruction text, step by step | Boss | ⬜ Not started |
| G2 | Review and approve all tutorial messages for clarity (child language) | Boss | ⬜ Not started |
| G3 | Build the tutorial flow in Unity (step 1 → step 2 → ... → complete) | Developer | ⬜ Not started |
| G4 | Connect tutorial steps to actual game events (walk, stop, look, cross) | Developer | ⬜ Not started |
| G5 | Add tutorial completion flag to save system (never show again after done) | Developer | ⬜ Not started |
| G6 | Play through the full tutorial and check every step works correctly | Boss | ⬜ Not started |

---

### SECTION H — Localization (Language System)
*Making the game work in both English and Amharic.*

| # | Task | Assigned To | Status |
|---|------|-------------|--------|
| H1 | Write all English text for Level 1 (all UI, feedback, tutorial messages) | Boss | ⬜ Not started |
| H2 | Translate all text into Amharic (all UI, feedback, tutorial messages) | Boss | ⬜ Not started |
| H3 | Review Amharic translations for accuracy and child-appropriate language | Boss | ⬜ Not started |
| H4 | Create the English JSON language file | Developer | ⬜ Not started |
| H5 | Create the Amharic JSON language file | Developer | ⬜ Not started |
| H6 | Build the localization system (loads correct language on startup) | Developer | ⬜ Not started |
| H7 | Connect all UI text elements to the localization system | Developer | ⬜ Not started |
| H8 | Test: switch language from English to Amharic — does everything update? | Boss | ⬜ Not started |

---

### SECTION I — Save System
*Storing player progress on the device.*

| # | Task | Assigned To | Status |
|---|------|-------------|--------|
| I1 | Build the save service (write player data to local JSON file) | Developer | ⬜ Not started |
| I2 | Build the load service (read saved data when game opens) | Developer | ⬜ Not started |
| I3 | Save: player name, avatar, coins, Level 1 completion, language | Developer | ⬜ Not started |
| I4 | Test: close the game and reopen — is all data still there? | Boss | ⬜ Not started |

---

### SECTION J — Android Build & Testing
*Getting the game onto a real phone.*

| # | Task | Assigned To | Status |
|---|------|-------------|--------|
| J1 | Configure Unity Android build settings (package name, icon, orientation) | Developer | ⬜ Not started |
| J2 | Create and attach the app icon | Boss | ⬜ Not started |
| J3 | Do the first Android build — create the APK file | Developer | ⬜ Not started |
| J4 | Install the APK on a test Android phone | Boss | ⬜ Not started |
| J5 | Play through the entire game on the phone — note all issues | Boss | ⬜ Not started |
| J6 | Test on a second phone (low-end device if possible) | Boss | ⬜ Not started |
| J7 | Fix all critical bugs found during testing | Developer | ⬜ Not started |
| J8 | Do a final clean build of the APK for review | Developer | ⬜ Not started |
| J9 | Final approval — is Phase 1 / Level 1 complete? | Boss | ⬜ Not started |

---

## Task Count Summary

| Person | Total Tasks |
|--------|------------|
| **Boss** | 36 tasks |
| **Developer** | 28 tasks |
| **Total** | 64 tasks |

---

## Suggested Order of Work

Do not try to do everything at once. Follow this order:

```
WEEK 1 — Setup + Assets
  → Boss: Section A setup, start Section B art
  → Developer: Section A technical setup, folder structure

WEEK 2 — Art Completion + Scene Build
  → Boss: Finish all Section B art, Section C audio
  → Developer: Build Level 1 scene (Section D), start movement code

WEEK 3 — Game Logic
  → Developer: All Section E programming
  → Boss: Review scene and assets

WEEK 4 — Screens + Tutorial
  → Developer: All Section F screens, Section G tutorial build
  → Boss: Write tutorial script, approve screens

WEEK 5 — Localization + Save
  → Boss: Write and translate all text (Section H)
  → Developer: Section H tech + Section I save system

WEEK 6 — Android Testing + Fix
  → Developer: Section J build
  → Boss: Section J testing and final review
```

---

## Definition of "Phase 1 Done"

Phase 1 is complete when:

- [ ] A new player can open the game on Android for the first time
- [ ] They can select English or Amharic
- [ ] They can create a profile (name + character)
- [ ] They can complete the tutorial
- [ ] They can play Level 1 from start to finish
- [ ] They receive feedback, score, and coins
- [ ] Their progress saves and loads correctly
- [ ] The game runs smoothly on a budget Android phone
- [ ] Boss has approved the final build

**When all boxes above are checked — Phase 2 (Level 2) begins.**

---

*Phase 1 Task Document | Kidy Road Safety Learning Game | Version 1.0*
