# Safe Steps Addis — GDD (Phase 1 MVP)
**8–14** · Android **offline** · One level

**MVP:** Wait on **green/yellow**, cross on **red** at the **zebra**; instant feedback; no internet.

**Lights:** Green cars go / people wait · Yellow cars slow / people wait · Red cars stop / people cross on zebra.  
**Timing:** Green **5s** → Yellow **2s** → Red **6s** (13s loop, tune ±1s).

**Loops:** App: Splash → [first launch: language, welcome, profile, tutorial] → Home → Briefing → Play → Result → save. Level: sidewalk → zebra → wait (G/Y) → cross (R) → feedback until win (~5–8 min).

**Mission:** One road, one crossing, few cars; goal = opposite sidewalk. **Win:** 1 safe cross + result + save. **States:** Briefing → Playing ⇄ Evaluating → Result. **Buttons:** WAIT on green/yellow; CROSS on red only.

**Safety:** G/Y + cross = unsafe · R + zebra + clear = safe · off zebra or car close = unsafe.

**Win / lose:** Win = safe crosses met + saved. Soft fail = wrong try → message, keep playing. Prefer no harsh Game Over.

**Score:** `safety×70 + accuracy×20 + timing×10` (rounded). Stars: 85+ ★★★, 70+ ★★, 55+ ★. Coins: base + stars; local JSON (name, language, tutorial, progress, scores).

**Tutorial (first time):** move → crossing → G/Y/R → wait G+Y → cross R → save `tutorial_done`.

**Screens:** Splash, language, welcome, profile, tutorial, home (one “Play Level 1”), briefing, HUD, feedback, result.

**Tech:** Small scene, ~30 FPS low-end phones, small APK, no network in play.

**Done when:** Child finishes mostly alone; explains **G/Y = wait**, **R = cross on zebra**.

*UX detail: `FIRST_TIME_EXPERIENCE.md`*
