# Kidy Road Safety Learning Game
## Complete MVP Document
### Version 1.0 — Safe Steps Addis

---

## What Is This Document?

This document explains everything about the **first playable version** of the Kidy Road Safety Learning Game. It covers what the game is, why it exists, what it includes, how it works, and how it will be built.

---

## 1. Executive Summary

### What Is the Game?

**Kidy Road Safety Learning Game** is a mobile educational game built for Ethiopian children. The game teaches children how to be safe on roads — through play, not lectures.

Children learn by **doing**. In the game, they walk up to roads, watch traffic lights, wait for the right moment, and cross safely. Every decision they make gets instant feedback. Every success earns them rewards.

### What Is an MVP?

> **MVP = Minimum Viable Product**

An MVP is the **smallest complete version** of a product that is ready to be tested with real users.

It is not a rough demo. It is not a prototype. It is a **real, working game** — but focused only on the most essential features.

The MVP for Kidy is designed to:

| Goal | Meaning |
|------|---------|
| **Validate the learning experience** | Does playing this actually teach children road safety? |
| **Test usability with children** | Can children use the game easily without adult help? |
| **Confirm Android performance** | Does it run smoothly on cheap Android phones? |
| **Build a solid foundation** | Can we add more features on top of this later? |

### What the MVP Is NOT

The MVP deliberately does **not** include:

- Multiplayer
- Online systems or cloud saving
- Advanced AI traffic
- Open-world city exploration
- Driver mode (controlling vehicles)
- Voice assistants or AR features

These will come in future versions. For now, the goal is:

> **"A small but complete educational gameplay experience."**

---

## 2. The Problem We Are Solving

### What Is Happening Right Now?

Children in cities like Addis Ababa walk on roads every single day. They encounter:

- Traffic lights
- Moving cars and buses
- Zebra crossings
- Intersections
- Pedestrians

But most children **do not fully understand** how to behave safely in these situations:

- When is it safe to cross?
- Why does the traffic light matter?
- What is a zebra crossing for?
- What should I do when a car is coming?

### Why Is Traditional Teaching Not Enough?

Most road safety education in schools today involves:
- Talking about road rules in class
- Looking at diagrams or posters
- Listening to lectures

This approach has clear weaknesses:

| Problem | What It Means |
|---------|---------------|
| No practice | Children hear rules but never "experience" them |
| Hard to visualize | Young children struggle to understand abstract rules |
| Not engaging | Children lose attention quickly |
| No feedback | Children don't know if they understood correctly |

### How This Game Solves the Problem

The Kidy game replaces passive learning with **active, hands-on practice** in a safe digital environment.

Children make real decisions in the game — and see the result immediately. If they cross on a red light, they see danger. If they wait for green and cross on the zebra crossing, they succeed. Learning happens through experience, not memorization.

---

## 3. MVP Goals and Scope

### 3.1 What We Are Trying to Achieve

**Core Goals (what the game must do):**

1. Teach children when it is safe to cross a road
2. Show how traffic lights work (red = stop, green = go)
3. Explain the purpose of zebra crossings
4. Deliver instant feedback after every player decision
5. Reward correct and safe behavior
6. Work completely offline on Android phones

**Educational Goals (what children must learn):**

1. Traffic light rules
2. Zebra crossing usage
3. How to check for traffic before crossing
4. Safe crossing behavior step by step

**Technical Goals (how the game must perform):**

1. Run smoothly on low-end Android devices
2. Save all player progress locally (no internet needed)
3. Support both English and Amharic languages
4. Be easy to maintain and expand after the MVP

---

### 3.2 What Is Included in the MVP

The following systems and features are included in Version 1:

| System | Included Features |
|--------|------------------|
| Main Menu | Start, Settings, Language Select |
| Player Profile | Name entry, avatar choice, local save |
| Tutorial | Guided first mission, movement help, road safety basics |
| Game Environment | One road map with zebra crossing, traffic lights, vehicles |
| Mission Gameplay | Walking to crossing, observing traffic, crossing safely |
| Traffic System | Cars moving, stopping at red lights, basic collision |
| NPC System | Pedestrians walking, basic animations |
| Feedback System | Instant visual and sound feedback after each action |
| Reward System | Coins earned for correct behavior |
| Mission Results | Score display, coins earned, completion status |
| Avatar System | 1–2 basic outfits, unlockable with coins |
| Save System | All data saved locally as JSON files |
| Localization | English and Amharic translations for all text |

---

## 4. Core Game Systems — Detailed Breakdown

---

### 4.1 Main Menu System

**Definition:** The main menu is the first screen a player sees when they open the game.

**Purpose:** To give the child a clear, simple starting point.

**What It Contains:**

| Button | What It Does |
|--------|-------------|
| **Start Game** | Takes the player to the game |
| **Settings** | Opens sound and language settings |
| **Language** | Switch between English and Amharic |
| **Exit** | Closes the game |

**Design Rules:**
- Buttons must be large (children have less precise finger control)
- Use icons alongside text (not all children can read)
- Colors must be bright, friendly, and clear
- No complex menus or sub-menus

---

### 4.2 Localization System

**Definition:** The localization system allows the game to display content in different languages.

**Why It Matters:** Addis Ababa children may speak Amharic at home and English at school. The game must work in both.

**Supported Languages:**
- English
- Amharic (አማርኛ)

**What Gets Translated:**
- All menu text
- Tutorial instructions
- Gameplay feedback messages
- Mission prompts and results

**Example:**

| English | Amharic |
|---------|---------|
| "Wait for the green light." | "አረንጓዴ መብራትን ይጠብቁ" |
| "Cross safely!" | "በጥንቃቄ ተሻጋሩ!" |
| "Use the zebra crossing." | "የዜብራ መሻገሪያ ይጠቀሙ" |

**How It Works:** The game reads a language file (JSON) and displays the correct text based on the player's chosen language. Switching language takes effect instantly.

---

### 4.3 Player Profile System

**Definition:** The player profile stores who is playing and what they have achieved.

**What the Player Sets Up:**
- Their name
- Their avatar (character appearance)

**What Gets Saved Automatically:**
- Coins collected
- Missions completed
- Tutorial completion status
- Chosen language
- Selected avatar

**Where It Is Saved:** On the device itself — no internet needed.

**Not Included in MVP:**
- Online accounts
- Cloud backup
- Multiple profiles on one device (one profile per install)

---

### 4.4 Tutorial and Onboarding System

**Definition:** The tutorial is a guided first experience that teaches the player how to play the game step by step.

**Purpose:** Make sure every child — even one who has never played a game before — can understand how to play without asking for help.

**Tutorial Flow:**

```
1. MOVE YOUR CHARACTER
   ↓
2. WALK TOWARD THE ROAD
   ↓
3. STOP AT THE CROSSING
   ↓
4. LOOK AT THE TRAFFIC LIGHT
   ↓
5. WAIT FOR THE GREEN LIGHT
   ↓
6. CROSS ON THE ZEBRA CROSSING
   ↓
7. REACH THE OTHER SIDE
   ↓
8. TUTORIAL COMPLETE ✓
```

**What the Tutorial Teaches:**
- How to move the character (joystick or tap)
- What traffic lights mean
- What a zebra crossing is and why to use it
- How to watch for cars before crossing

**Feedback During Tutorial:**

| Situation | Message Shown |
|-----------|--------------|
| Player moves correctly | "Great! Keep going!" |
| Player tries to cross on red | "Wait! The light is red." |
| Player ignores zebra crossing | "Use the zebra crossing — it's safer!" |
| Player crosses safely | "Excellent! You crossed safely!" |

---

## 5. Game Environment System

### 5.1 The MVP Map

**Definition:** The game environment is the 3D world the player moves through.

The MVP contains **one map only**. This is intentional — one well-designed map is better than two incomplete ones.

**What the Map Includes:**

| Element | Description |
|---------|-------------|
| **Road** | A two-lane road with moving vehicles |
| **Sidewalk** | Safe walking area on either side of the road |
| **Zebra Crossing** | Marked crossing area with lines on the road |
| **Traffic Light** | A working traffic light that cycles red → green |
| **Vehicles** | Cars that move along the road and stop at red lights |
| **NPC Pedestrians** | Other people walking to make the scene feel alive |

**Design Inspiration:** The map is inspired by simplified street layouts from Addis Ababa — familiar to the children playing the game.

**Design Rules:**
- The map must be clean and easy to read visually
- No clutter or confusing details
- Must perform at a smooth frame rate on budget Android phones

**Not Included in MVP:**
- Multiple maps or districts
- Open-world city
- Complex intersections
- Night mode or weather effects

---

## 6. Core Mission Gameplay Loop

**Definition:** The gameplay loop is the repeating cycle of actions the player takes during a mission. This is the **heart of the game**.

### The Loop — Step by Step:

```
START MISSION
     ↓
CHARACTER SPAWNS ON SIDEWALK
     ↓
PLAYER WALKS TOWARD THE CROSSING
     ↓
PLAYER REACHES THE ZEBRA CROSSING
     ↓
TRAFFIC LIGHT IS RED — PLAYER MUST WAIT
     ↓
TRAFFIC LIGHT TURNS GREEN
     ↓
PLAYER PRESSES CROSS
     ↓
CHARACTER CROSSES THE ROAD
     ↓
PLAYER REACHES THE OTHER SIDE
     ↓
FEEDBACK IS SHOWN ("Well done!" or "Be careful!")
     ↓
COINS ARE AWARDED
     ↓
MISSION RESULT SCREEN
     ↓
PLAYER CAN REPLAY OR RETURN HOME
```

### Why This Loop Works for Learning

Each time the loop repeats, the child:
1. **Observes** — Looks at the traffic light
2. **Decides** — Wait or cross?
3. **Acts** — Presses a button
4. **Learns** — Gets immediate feedback on their decision
5. **Repeats** — Plays again with more confidence

This is how children actually learn — through practice and feedback, not through reading rules.

---

## 7. Road Safety Learning System

**Definition:** The learning system is the educational content embedded in every part of the game.

### What Children Will Learn

**Topic 1: Traffic Lights**
- Red light = Stop. Do not cross.
- Green light = Go. It is safe to cross.
- Children see this rule applied in real time during every mission.

**Topic 2: Zebra Crossings**
- A zebra crossing is the safest place to cross a road.
- Always cross at the zebra crossing, never in the middle of the road.
- The game enforces this — crossing outside the lines results in a warning.

**Topic 3: Crossing Awareness**
- Always look both ways before crossing.
- Even on green, check that cars have actually stopped.
- The game rewards careful behavior, not just fast behavior.

**Not Included in MVP:**
- Advanced road signs (stop signs, yield signs)
- Speed limit education
- Driving rules
- Complex traffic laws

---

## 8. Traffic and NPC System

**Definition:** The traffic system controls how vehicles behave on the road. The NPC system controls how other pedestrians behave.

### Vehicle Behavior

| Behavior | Description |
|----------|-------------|
| **Normal movement** | Cars drive from one side of the screen to the other |
| **Red light stopping** | Cars slow down and stop when the traffic light is red |
| **Green light moving** | Cars start moving when the light turns green |
| **Collision detection** | If the player walks in front of a car, the game registers danger |

### NPC Pedestrian Behavior

- Other pedestrians walk along the sidewalk
- Some pedestrians also cross the road (modeling correct behavior)
- Basic looping animations (walk cycle)

### System Goal

The traffic system must:
- Feel real enough to teach children respect for vehicles
- Stay lightweight enough to run on cheap Android phones
- Not be so complex that it distracts from the learning

---

## 9. Real-Time Feedback System

**Definition:** The feedback system gives the player an immediate response to every action they take.

> This is one of the most important educational systems in the game. Children learn best when they understand the result of their actions instantly.

### Types of Feedback

**Positive Feedback (when the player does something correctly):**

| Message | When It Appears |
|---------|----------------|
| "Excellent!" | After crossing safely |
| "Good waiting!" | After waiting correctly for green |
| "You crossed safely!" | After completing the crossing |
| "Well done!" | After finishing a mission |

**Negative Feedback (when the player makes a dangerous choice):**

| Message | When It Appears |
|---------|----------------|
| "Wait for the green light!" | Player tried to cross on red |
| "Use the zebra crossing!" | Player tried to cross at wrong location |
| "Be careful of cars!" | Player walked too close to moving vehicles |
| "Look before you cross!" | Player crossed without checking traffic |

### How Feedback Is Delivered

1. **On-screen popup message** — Large, clear text appears on screen
2. **Color coding** — Green for correct, red/orange for dangerous
3. **Sound effects** — Positive chime for correct, warning sound for dangerous
4. **UI highlight** — The relevant element (traffic light, crossing) is highlighted

---

## 10. Reward and Progression System

**Definition:** The reward system motivates children to keep playing by giving them something for doing the right thing.

### Coins

Coins are the in-game currency earned through good behavior.

| Action | Coins Earned |
|--------|-------------|
| Completing a mission | 20 coins |
| Crossing correctly on green | 10 coins |
| Waiting correctly on red | 5 coins |
| Perfect mission (no mistakes) | Bonus 15 coins |

### Mission Results Screen

After every mission, the player sees:

```
╔══════════════════════════════╗
║     MISSION COMPLETE!        ║
║                              ║
║  Score:        90%           ║
║  Stars:        ★★★           ║
║  Coins Earned: +25           ║
║                              ║
║  [PLAY AGAIN]  [HOME]        ║
╚══════════════════════════════╝
```

### Why Rewards Matter

Rewards do two things:
1. **Reinforce correct behavior** — Children associate safe crossing with positive outcomes
2. **Drive replayability** — Children want to earn more coins, so they play again and learn more

---

## 11. Avatar Customization System

**Definition:** The avatar system allows the player to personalize their in-game character.

**Why It Matters:** Children feel more connected to a character that looks like them or that they chose themselves.

### MVP Features

- 1 default outfit (available from the start)
- 1–2 additional outfits (unlockable with coins)
- Basic character appearance selection at profile setup

### How Unlocking Works

1. Player earns coins through gameplay
2. Player visits avatar screen
3. Player spends coins to unlock a new outfit
4. Outfit is saved to their profile

### Not Included in MVP

- Accessories (hats, bags, etc.)
- Skin tone or hair customization
- Animated outfit effects
- A full in-game store

---

## 12. Save System

**Definition:** The save system stores all player data on the device so progress is never lost.

### What Gets Saved

| Data | Description |
|------|-------------|
| Player name | The name entered at setup |
| Selected avatar | The character/outfit chosen |
| Coins | Total coins earned so far |
| Mission progress | Which missions are completed |
| Tutorial status | Whether the tutorial has been done |
| Language preference | English or Amharic |

### How It Works

All data is saved as a **JSON file** on the Android device.

> **JSON** = A simple text file format that stores data in an organized way. Example: `{"coins": 75, "language": "en", "tutorial_done": true}`

Saving happens automatically:
- After every mission
- After any profile change
- When the player exits the game

### Why This Approach

- Works completely offline (no internet required)
- Fast to read and write
- Simple to expand with new data fields later
- Reliable on all Android versions

---

## 13. Technical Architecture

**Definition:** The technical architecture describes the tools and structure used to build the game.

### Technology Stack

| Layer | Technology | Why |
|-------|------------|-----|
| **Game Engine** | Unity 2022 LTS | Industry standard for mobile games |
| **Rendering** | Universal Render Pipeline (URP) | Optimized for Android performance |
| **Language** | C# (.NET) | Unity's primary language, strongly typed |
| **Platform** | Android (APK) | Primary target device for Ethiopian children |
| **Save System** | Local JSON files | Simple, offline, reliable |
| **Localization** | Custom JSON language files | Easy to add new languages later |

### Core Code Systems

| System | Purpose |
|--------|---------|
| **GameBootstrap** | Starts the game, initializes all systems |
| **SceneFlowManager** | Controls which screen/scene is shown |
| **MissionController** | Runs the mission gameplay loop |
| **TrafficSystem** | Controls vehicle movement and traffic lights |
| **SaveService** | Reads and writes player data to storage |
| **LocalizationService** | Loads and serves translated text |
| **PlayerSession** | Tracks current session data (coins, progress) |
| **SafetyEvaluator** | Judges whether the player's crossing decision was correct |

### Architecture Principle

The game is built in **modules** — each system does one job and works independently. This means:
- One system can be changed without breaking others
- New features can be added cleanly
- Bugs in one system don't affect the rest

---

## 14. Full System Flow

```
OPEN GAME
     ↓
SELECT LANGUAGE (English / Amharic)
     ↓
CREATE PLAYER PROFILE (name + avatar)
     ↓
START TUTORIAL
     ↓
LEARN: movement, traffic lights, zebra crossing
     ↓
TUTORIAL COMPLETE
     ↓
MAIN MENU — HOME SCREEN
     ↓
SELECT MISSION (M1 → M2 → M3 → ...)
     ↓
MISSION BRIEFING (what to do)
     ↓
GAMEPLAY: walk → wait → cross → reach goal
     ↓
FEEDBACK at each decision point
     ↓
MISSION RESULT SCREEN (score, stars, coins)
     ↓
SAVE PROGRESS (automatic)
     ↓
RETURN HOME or REPLAY
```

---

## 15. MVP Success Criteria

The MVP is considered **successful** when all of the following are true:

| Area | What Success Looks Like |
|------|------------------------|
| **Gameplay** | A child can complete a full mission from start to finish without help |
| **Education** | After playing, the child correctly understands when to cross a road |
| **Performance** | The game runs at smooth speed on a budget Android device |
| **Usability** | A child aged 8–14 can navigate all menus without confusion |
| **Stability** | Player progress saves correctly and never disappears |
| **Engagement** | Players voluntarily replay missions at least 2–3 times |

---

## 16. Features Excluded From MVP

These features are planned for future versions but are **not** part of Version 1:

### Gameplay Expansion
- Driver mode (controlling vehicles instead of a pedestrian)
- Multiple road maps or districts
- Advanced road signs (yield, one-way, speed limits)
- Open-world city exploration

### Online Systems
- Multiplayer (playing with friends)
- Online leaderboards
- Cloud save / sync
- Teacher or parent dashboard

### Advanced Features
- AI voice assistant for instructions
- School integration or classroom tools
- AR (Augmented Reality) mode
- Advanced AI traffic behavior
- Analytics reporting system

---

## 17. Development Build Plan

### Phase 1 — Foundation
- Set up Unity project structure
- Build character movement system
- Set up camera follow system
- Build base UI framework
- Set up localization loading

### Phase 2 — Core Gameplay
- Build traffic light system
- Build vehicle movement
- Build zebra crossing detection
- Build collision/danger detection
- Connect crossing decision to feedback

### Phase 3 — Educational Systems
- Build tutorial flow
- Connect learning prompts to actions
- Build real-time feedback display
- Add sound effects for feedback

### Phase 4 — Progression Systems
- Build coin system
- Build mission results screen
- Build avatar selection and unlock
- Build save/load system

### Phase 5 — Polish and Optimization
- Optimize for low-end Android
- Final bug fixing
- Sound and music additions
- Testing with children
- APK build and release preparation

---

## 18. Final MVP Definition

> **The Kidy Road Safety Learning Game MVP is a simple, offline Android educational game where children aged 8–14 learn basic road safety behaviors — including traffic light rules, zebra crossing usage, and safe crossing decisions — through guided gameplay missions, real-time feedback, coin rewards, and replayable scenarios. The game requires no internet connection, supports English and Amharic, and is optimized to run smoothly on low-cost Android devices.**

---

## 19. Core Engineering Principles

These rules guide every development decision:

| Rule | Why It Matters |
|------|---------------|
| Keep MVP simple | Complexity slows development and confuses users |
| Learning experience first | The game exists to educate, not just entertain |
| Android performance always | Most users will have budget phones |
| Build modular systems | Easy to expand, easy to fix |
| No feature overengineering | Only build what is needed right now |
| One strong gameplay loop | Master one thing before adding more |
| Design for children | Large buttons, clear text, instant feedback |
| Test on real devices continuously | Emulators don't reveal real performance issues |

---

*Document Version: 1.0 | Safe Steps Addis — Kidy Road Safety Learning Game*
