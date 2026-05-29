# Week 4 HomeScene Final Wiring Pass

Use this as the single run sheet on final setup day for `HomeScene`.

## Build Context

- **Date (UTC+3):**
- **Scene:** `HomeScene` / `HomeScene_QA`
- **Owner:**
- **Reviewer:**

## 1) Base UI Structure Check

- [ ] Top bar present (`Profile`, `Coins`, `Language`).
- [ ] Mission map panel present with mission nodes.
- [ ] Home action buttons wired (`Play`, `Store`, `Avatar`, `Reports`, `Exit`).
- [ ] Facilitator toggle button visible and reachable.

Reference: `WEEK4_UI_WIREFRAME_PACK.md`

## 2) Prefab + Controller Wiring Check

- [ ] `FacilitatorDashboardController` fields fully wired.
- [ ] `FacilitatorMaintenanceController` fields fully wired.
- [ ] `AnalyticsInsightsPanelController` fields fully wired.
- [ ] `AnalyticsSummaryExportController` fields fully wired.
- [ ] No missing references in inspector.

Reference: `WEEK4_UI_PREFAB_BINDING_TABLE.md`

## 3) Security Configuration Check

### Production scene (`HomeScene`)

- [ ] `requirePinToOpen = true`
- [ ] `enableSecurityTestControls = false`
- [ ] QA simulate buttons unassigned/disabled

### QA scene (`HomeScene_QA`) if used

- [ ] `enableSecurityTestControls = true`
- [ ] Simulate cooldown/lockout/clear buttons assigned
- [ ] QA scene is excluded from release build list

Reference: `WEEK4_HOME_SCENE_QA_VARIANT_CHECKLIST.md`

## 4) PIN + Maintenance Functional Smoke

- [ ] Correct PIN opens dashboard.
- [ ] Wrong PIN triggers cooldown.
- [ ] Lockout triggers at max failed attempts.
- [ ] Lockout/cooldown timers recover controls.
- [ ] Security state label updates (if wired).
- [ ] Clear analytics works.
- [ ] Clear exports works.
- [ ] Clear PIN audit works.
- [ ] Reset profile requires double-confirm and timeout.

Reference: `WEEK4_PIN_SECURITY_QUICK_TEST.md`

## 5) Data Output Verification

- [ ] Analytics logs created under persistent analytics directory.
- [ ] PIN audit logs created under persistent security directory.
- [ ] Export output path shown in UI after export.
- [ ] Maintenance clear actions actually remove target files.

## 6) Localization + UX Pass (EN/AM)

- [ ] EN/AM switch updates facilitator labels and status text.
- [ ] No raw localization keys visible.
- [ ] Button touch targets are comfortable in portrait mode.
- [ ] No clipping/overflow on status and analytics text.

## 7) Regression + Documentation Gate

- [ ] `WEEK4_SECURITY_REGRESSION_MATRIX.md` updated.
- [ ] Daily log saved from `WEEK4_SECURITY_DAILY_LOG_TEMPLATE.md`.
- [ ] Failures filed with `WEEK4_SECURITY_BUG_TEMPLATE.md`.
- [ ] If release candidate: complete `WEEK4_SECURITY_RELEASE_GATE_CHECKLIST.md`.
- [ ] If release candidate: create/update build sign-off file using `SECURITY_SIGNOFF_TEMPLATE.md`.

## Final Sign-off

- **Wiring pass result:** `PASS` / `FAIL`
- **Blocking issues:** `None` / `Present`
- **Ready for pilot/release flow:** `Yes` / `No`
- **Notes:**
