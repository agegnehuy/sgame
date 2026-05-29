# Week 4 Day-0 + Final Wiring Combined Checklist

Use this single sheet during setup sessions (print-friendly) to go from fresh machine to validated `HomeScene`.

## Session Info

- **Date (UTC+3):**
- **Operator:**
- **Reviewer:**
- **Machine:**
- **Unity Version:**

---

## A) Day-0 Bootstrapping (30 min)

### A1. Environment

- [ ] Unity LTS installed (URP-compatible)
- [ ] Android Build Support installed
- [ ] Project folder available locally

### A2. Project bring-up

- [ ] Open project from Unity Hub
- [ ] Wait for import + compile
- [ ] Switch platform to Android
- [ ] Open `HomeScene`

### A3. Minimum scene objects

- [ ] Top bar (`Profile`, `Coins`, `Language`)
- [ ] Mission map section
- [ ] Facilitator toggle button
- [ ] Facilitator dashboard panel + title
- [ ] PIN modal (input/submit/cancel/status)
- [ ] Analytics insights panel
- [ ] Analytics export panel
- [ ] Maintenance panel controls

Reference:

- `WEEK1_DAY0_UNITY_SETUP_30MIN.md`
- `WEEK4_UI_WIREFRAME_PACK.md`

---

## B) Strict Wiring Pass

- [ ] `FacilitatorDashboardController` required fields assigned
- [ ] `FacilitatorMaintenanceController` required fields assigned
- [ ] `AnalyticsInsightsPanelController` required fields assigned
- [ ] `AnalyticsSummaryExportController` required fields assigned
- [ ] No missing refs in Inspector

Reference:

- `WEEK4_UI_PREFAB_BINDING_TABLE.md`
- `WEEK4_UI_PREFAB_CHECKLIST.md`

---

## C) Security Config Pass

### Production `HomeScene`

- [ ] `requirePinToOpen = true`
- [ ] `enableSecurityTestControls = false`
- [ ] QA simulate buttons unassigned/disabled

### QA `HomeScene_QA` (if used)

- [ ] `enableSecurityTestControls = true`
- [ ] simulate/lockout/clear test buttons assigned
- [ ] QA scene excluded from release build list

Reference:

- `WEEK4_HOME_SCENE_QA_VARIANT_CHECKLIST.md`

---

## D) Functional Smoke (PIN + Maintenance)

- [ ] Correct PIN opens dashboard
- [ ] Wrong PIN triggers cooldown
- [ ] Lockout triggers at threshold
- [ ] Countdown/status updates visible
- [ ] Controls recover after timers
- [ ] Clear analytics works
- [ ] Clear exports works
- [ ] Clear PIN audit works
- [ ] Reset profile requires double-confirm + timeout

Reference:

- `WEEK4_PIN_SECURITY_QUICK_TEST.md`

---

## E) Data + Localization Verification

- [ ] Analytics JSONL created in persistent analytics folder
- [ ] PIN audit JSONL created in persistent security folder
- [ ] Export path appears in UI
- [ ] EN/AM switch updates security/facilitator labels
- [ ] No raw localization keys shown

---

## F) Release/QA Documentation Pass

- [ ] Update `WEEK4_SECURITY_REGRESSION_MATRIX.md`
- [ ] Save daily archive from `WEEK4_SECURITY_DAILY_LOG_TEMPLATE.md`
- [ ] File failures with `WEEK4_SECURITY_BUG_TEMPLATE.md`
- [ ] If RC build: complete `WEEK4_SECURITY_RELEASE_GATE_CHECKLIST.md`
- [ ] If RC build: create sign-off from `SECURITY_SIGNOFF_TEMPLATE.md`

---

## Final Session Result

- **Combined pass result:** `PASS` / `FAIL`
- **Blocking issues:** `None` / `Present`
- **Ready for next stage:** `Yes` / `No`
- **Notes:**
