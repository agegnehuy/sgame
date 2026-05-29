# Safe Steps Addis — Complete MVP Overview

> This document explains the full app from start to finish in plain language.  
> No technical background needed to read this.

---

## What Is This App?

**Safe Steps Addis** is a mobile game for children aged **8 to 14 years old**.  
The goal of the game is simple: **teach children how to safely cross the road.**

The game is built specifically for Ethiopia. It works in **English and Amharic**, and it runs on **affordable Android phones** — even without internet.

---

## What Problem Does It Solve?

Many children in Addis Ababa face real danger crossing busy roads.  
This app gives children a safe, fun way to **practice road-safety decisions** before they face them in real life.

A teacher or facilitator (an adult who guides the session) can use the app with a group of children in a school or community setting.

---

## Who Uses It?

| Person | What They Do |
|---|---|
| **Child (player)** | Plays the game, learns road safety |
| **Facilitator / Teacher** | Manages the session, views reports, resets progress |

---

## What Tools Are Used to Build This App?

Here is everything being used, explained simply:

| Tool | What It Is | Why We Use It |
|---|---|---|
| **Unity** | A game-making software | It is used to build the 3D world the child plays in |
| **C#** | A programming language | The rules of the game (scoring, safety checks, saving data) are written in C# |
| **Android (APK)** | The phone app format | The final app is installed on Android phones |
| **JSON files** | Simple text files that store data | Used to save the child's progress, settings, and analytics locally on the device |
| **Unity URP** | A graphics system inside Unity | Makes the game look good on low-cost phones without slowing them down |
| **Localization files** | Text files with translations | Allow the app to switch between English and Amharic instantly |

**No internet is required.** Everything is saved directly on the phone.

---

## What Does the App Actually Look Like? (Screen by Screen)

### 1. Home Screen
The first thing you see when you open the app.

- You can **select a player profile** (up to a few players can be saved on one device)
- You can **switch the language** between English and Amharic
- You can **go to the Mission Map** to start playing
- You can **open the Store** to spend coins on cosmetics (character appearance items)
- A **Facilitator button** is hidden here for teachers (protected by a PIN code)

---

### 2. Mission Map
A visual map showing all the missions available.

- Missions are **unlocked one by one** — you must complete Mission 1 before Mission 2 opens
- Each mission is a different road-crossing scenario
- The current MVP has **5 missions** (called P1-M1 through P1-M5)

---

### 3. Mission Briefing
Before each mission starts, the child sees a short description of what they are about to do.

- Written in the selected language (English or Amharic)
- Tells the child what the challenge is (e.g., "Cross the road safely during heavy traffic")

---

### 4. Playing the Mission (The Main Game)
This is where the child actually plays.

- The child sees a road with a **traffic light** and **moving vehicles**
- They press one of two buttons:
  - **Cross** — attempt to cross the road
  - **Wait** — wait for a safer moment
- The game checks if their decision was safe or not
- **Distractions and pressure events** can appear (e.g., a friend calling them to hurry) to make it more realistic

**How the game decides if you are safe:**
- It checks the traffic light color (red = stop, green = go)
- It measures how far vehicles are from the crossing point
- If you cross on red or when a car is too close, you lose points

---

### 5. Result Screen
After the mission ends, the child sees their result.

- A **score** (number of points earned)
- A **star rating** (1, 2, or 3 stars based on how safe their decisions were)
- A summary of what they did well and what to improve
- **Coins are awarded** based on performance — used to buy items in the store

---

### 6. Store
A simple shop where children spend their earned coins.

- They can buy **cosmetic items** (things that change how their character looks)
- Nothing here affects safety or scoring — it is purely for fun and motivation
- Items are equipped from the **Avatar Preview Panel**

---

### 7. Progress Summary Panel
Available from the Home Screen.

- Shows how many missions have been completed
- Shows stars earned per mission
- Gives a quick overview of the child's learning progress

---

### 8. Facilitator Dashboard (Teacher Area)
A separate, locked section for teachers and facilitators.

- Protected by a **PIN code** so children cannot access it
- After too many wrong PIN attempts, the panel **locks temporarily** for security
- All PIN attempts are **logged automatically** (recorded with timestamps)

**What a facilitator can do here:**
- **View analytics** — see how each child is making decisions in the game
- **Export a report** — save a summary of the session as a file they can read later
- **Clear data** — reset progress or logs when starting a new session
- **View security status** — see if anyone tried to guess the PIN

---

## How Is Data Saved?

Everything is saved **locally on the phone** as simple files (JSON format).  
There is no server, no cloud, no account needed.

| What Is Saved | Where |
|---|---|
| Player progress (missions completed, stars, coins) | On the phone |
| Store items owned and equipped | On the phone |
| Analytics (decisions made during gameplay) | On the phone |
| Facilitator reports | On the phone (can be exported as a file) |
| PIN audit log (security events) | On the phone |
| Language preference | On the phone |

---

## How Does Language Switching Work?

The app supports **English** and **Amharic** at the same time.

- Every piece of text in the app has two versions — one in English, one in Amharic
- When the language is switched, **the whole app updates immediately** — no restart needed
- New languages can be added in the future by simply adding a new translation file

---

## What Is the Scoring System?

| Result | Stars | Explanation |
|---|---|---|
| Excellent safety decisions | 3 stars | Crossed only on green, no close calls |
| Good with minor mistakes | 2 stars | Mostly safe, one or two risky moments |
| Completed but unsafe | 1 star | Finished the mission but took unsafe actions |

Coins are also earned based on stars. More stars = more coins.

---

## What Happens Week by Week? (Delivery Plan Summary)

| Week | What Was Built |
|---|---|
| **Week 1** | Core game loop working: cross the road, get scored, see results, save progress |
| **Week 2** | Multiple missions, mission unlocking, progress panel, facilitator report export |
| **Week 3** | Distraction/pressure events in missions, full analytics logging, analytics panel, facilitator dashboard |
| **Week 4** | Facilitator PIN security, maintenance tools, UI polish, QA testing and release preparation |
| **Weeks 5–8** | Pilot testing in schools, collecting feedback, fixing issues, final delivery |

---

## What Does "MVP" Mean?

MVP stands for **Minimum Viable Product**.  
It means: the smallest, simplest working version of the app that can be tested with real users.

This MVP is **not the final version** — it is the version used in the first school pilot to learn what works and what needs improvement.

---

## Summary: The Full App Flow in One Paragraph

A child opens the app on an Android phone. They select their profile and see the Home Screen. They go to the Mission Map and pick the next unlocked mission. They read a short briefing and then play the mission — watching the traffic light and pressing Cross or Wait at the right time. After the mission ends, they see their score and stars, earn coins, and can spend those coins in the store. A teacher can log in to the Facilitator Dashboard using a PIN to view how the child is performing and export a report. All data is saved on the phone. The app works entirely offline in both English and Amharic.

---

*Built for Safe Steps Addis | Target: Ethiopia, Ages 8–14 | Platform: Android (offline)*
