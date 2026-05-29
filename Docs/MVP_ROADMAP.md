# Safe Steps Addis — MVP Roadmap
**Kidy Road Safety Learning Game** · v1.0

| | |
|---|---|
| **Kick-off** | May 18, 2026 |
| **Week 1** | May 20–26, 2026 |
| **Soft release** | July 20, 2026 |
| **Duration** | ~9 weeks |
| **Platform** | Android (offline) |
| **Players** | Ages **8–14** |

---

## Team

| Role | Name | Focus |
|------|------|--------|
| **Project Lead** | Agih | Scope, art, audio, content, Amharic review, testing, phase sign-off, stakeholder updates |
| **Developer** | Menaseh Kassahun | Unity, C#, gameplay, UI code, save, build, fixes |

**Weekly:** Monday sync · daily Notion task updates · phase gate = Agih sign-off before next phase.

---

## MVP in one line

One polished level: child **waits on green/yellow**, **crosses on red** at the **zebra**; instant feedback; EN/AM; offline save; APK on budget Android.

**Light cycle:** Green 5s → Yellow 2s → Red 6s.

---

## End-to-end timeline (short)

```
May 18     Kick-off
May 20–26  WEEK 1 — Setup & scope freeze
May 20–Jun 2   Phase 1 — Foundation
Jun 3–16       Phase 2 — Core gameplay
Jun 17–30      Phase 3 — Tutorial & feedback
Jul 1–7        Phase 4 — Progression & save
Jul 8–20       Phase 5 — Polish, test, release
Jul 20     SOFT RELEASE
```

---

## Phase gates (what “done” means)

| Phase | Dates | Done when |
|-------|--------|-----------|
| **1 Foundation** | May 20 – Jun 2 | Unity project runs; movement + camera; UI shell; localization loads EN/AM |
| **2 Core gameplay** | Jun 3 – 16 | Level 1 scene; G/Y/R light; cars; zebra; wait/cross; safe/unsafe rules work |
| **3 Educational** | Jun 17 – 30 | Tutorial end-to-end; all feedback text + sounds; child can finish tutorial alone |
| **4 Progression** | Jul 1 – 7 | Coins, stars, result screen, JSON save survives restart |
| **5 Polish & QA** | Jul 8 – 20 | Stable on 1–2 GB RAM phone; bugs fixed; APK ready; Agih sign-off |

---

## Week-by-week roadmap

| Week | Dates | Phase | Goal |
|------|--------|-------|------|
| **1** | May 20–26 | Foundation start | Project live, scope locked, Week 1 tasks done |
| **2** | May 27 – Jun 2 | Foundation end | Movement, camera, UI base, loc files wired |
| **3** | Jun 3–9 | Gameplay start | Scene, traffic light G/Y/R, cars stop/go |
| **4** | Jun 10–16 | Gameplay end | Zebra, evaluator, collision, mission loop |
| **5** | Jun 17–23 | Education start | Tutorial steps, feedback popups, sounds |
| **6** | Jun 24–30 | Education end | Full EN/AM strings; tutorial ≤60s target |
| **7** | Jul 1–7 | Progression | Coins, stars, results, save/load |
| **8** | Jul 8–14 | Polish start | Perf pass, bug bash, device test |
| **9** | Jul 15–20 | Release | Child test, final APK, soft release |

---

## WEEK 1 — May 20–26, 2026 (detailed)

**Sprint goal:** Start from zero — approved plan, working Unity project, clear tasks for both people.

### Agih (Project Lead)

| # | Task | Done |
|---|------|------|
| W1-A1 | Freeze MVP scope doc (one level, G/Y/R rules, cross on **red** only) | ⬜ |
| W1-A2 | Confirm Notion board + weekly task links for team | ⬜ |
| W1-A3 | Write project brief (goal, dates, devices, age 8–14) | ⬜ |
| W1-A4 | Start visual style (colors, UI mood, reference images) | ⬜ |
| W1-A5 | List all EN UI strings needed for Phase 1 | ⬜ |
| W1-A6 | Set up GitHub/repo access and folder naming rules | ⬜ |
| W1-A7 | Monday sync: assign Week 2 tasks | ⬜ |

### Menaseh (Developer)

| # | Task | Done |
|---|------|------|
| W1-M1 | Install Unity 2022 LTS + Android Build Support | ⬜ |
| W1-M2 | Create/open project; URP; Android target | ⬜ |
| W1-M3 | Folder structure (`Scripts`, `Scenes`, `Art`, `Audio`, `Localization`) | ⬜ |
| W1-M4 | Empty Level 1 scene + main camera | ⬜ |
| W1-M5 | Push project to repo; README with open/build steps | ⬜ |
| W1-M6 | Stub localization loader (EN JSON loads one label) | ⬜ |

### Week 1 exit checklist

- [ ] Both can open the same Unity project  
- [ ] Scope doc agreed (no M2–M5 missions in MVP)  
- [ ] Phase 1 task list for Weeks 2–9 drafted in Notion  
- [ ] Agih signed off → start Phase 1 build work Week 2  

---

## Full MVP scope (checklist — July 20)

### Onboarding & menu
- [ ] Main menu: Start, Settings, Language, Exit  
- [ ] EN / AM instant switch  
- [ ] Profile: name + avatar  
- [ ] Guided tutorial (first launch)  

### Environment (one map)
- [ ] Road, sidewalks, zebra crossing (Addis-style)  
- [ ] Traffic light: **Green → Yellow → Red** (5s / 2s / 6s)  
- [ ] Cars: go / slow / stop with light  
- [ ] Optional simple NPC pedestrians  

### Gameplay (Level 1)
- [ ] Walk to crossing  
- [ ] WAIT (green/yellow) / CROSS (red)  
- [ ] Safe = red + zebra + clear; unsafe otherwise  
- [ ] Mission result: score, stars, coins  

### Feedback
- [ ] Positive + negative messages (EN + AM)  
- [ ] Popup + sound + color (safe / unsafe)  

### Progression & save
- [ ] Coins + result screen + replay/home  
- [ ] Local JSON save (profile, tutorial, progress, language)  
- [ ] Auto-save after mission / exit  

### Release
- [ ] APK builds on real low-end Android (1–2 GB RAM)  
- [ ] ~30 FPS target; small APK  

**De-prioritized for July 20 if time tight:** avatar shop, multiple missions, facilitator dashboard.

---

## Soft release success (July 20)

- Child **8–14** completes tutorial + one mission without adult help  
- Understands: **green/yellow = wait**, **red = cross on zebra**  
- Feedback on every decision; EN/AM switch works  
- Progress survives app restart; offline only  
- Smooth on budget phone; voluntary replay 2–3 times  

---

## Out of scope (after July 20)

Multiple maps · multiplayer · cloud save · iOS · educator analytics · IAP · other safety topics.

---

## Tech stack

Unity 2022 LTS · URP · C# · Android APK · local JSON save · custom EN/AM JSON localization.

---

## Risks (short)

| Risk | Mitigation |
|------|------------|
| Scope creep | Freeze after Week 1; backlog only |
| Slow phones | Test real device every phase |
| Bad Amharic | Native review before Phase 5 |
| UI too hard for kids | Child test end of Phase 3 |
| Save loss | Atomic JSON write; kill-app test |
| Long tutorial | Cap ~60s |

---

## Communication

- **Monday:** weekly sync  
- **Daily:** Notion status + blockers  
- **Every 2 weeks:** Agih → stakeholder update  
- **Phase end:** Agih gate approval → next phase  

---

*Roadmap v1.0 · Aligns with `GAME_DESIGN_DOCUMENT.md` and Phase 1 polished MVP.*
