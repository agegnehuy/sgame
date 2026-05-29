# Week 1 Day-0 Unity Setup (30 Minutes)

Use this checklist for first-time project bootstrapping on a new machine.

## Goal (30 min)

By the end of this run:

- Unity project opens successfully
- `HomeScene` loads and core UI is visible
- Facilitator PIN flow opens/closes correctly
- One quick validation pass is completed

## 0) Prerequisites (2 min)

- [ ] Unity LTS installed (URP-compatible version)
- [ ] Android Build Support installed (SDK/NDK/OpenJDK)
- [ ] Project folder available locally

## 1) Open and prepare project (5 min)

1. Open Unity Hub -> Add project from this folder.
2. Open project and wait for script import/compile.
3. Set platform to Android:
   - `File -> Build Settings -> Android -> Switch Platform`
4. Open `HomeScene`.

Expected:

- No blocking compile errors
- Scene opens without missing-script popups

## 2) Scene baseline wiring (8 min)

Follow:

- `WEEK1_HOME_AND_MAP_WIRING.md`
- `WEEK4_HOME_SCENE_SECURITY_WIRING_CHECKLIST.md`

Minimum required objects:

- Facilitator toggle button + text
- Facilitator dashboard panel + title
- PIN panel + input + submit + cancel + status
- Analytics insights/export controls
- Maintenance controls (clear/reset)

## 3) Strict field binding pass (5 min)

Use:

- `WEEK4_UI_PREFAB_BINDING_TABLE.md`

Checklist:

- [ ] `FacilitatorDashboardController` required refs assigned
- [ ] `FacilitatorMaintenanceController` required refs assigned
- [ ] `AnalyticsInsightsPanelController` required refs assigned
- [ ] `AnalyticsSummaryExportController` required refs assigned

## 4) Security smoke test (6 min)

Use:

- `WEEK4_PIN_SECURITY_QUICK_TEST.md`

Minimum pass:

- [ ] Correct PIN opens dashboard
- [ ] Wrong PIN triggers cooldown
- [ ] Lockout triggers after threshold
- [ ] Recovery after cooldown/lockout works
- [ ] Clear PIN audit action returns success status

## 5) Record results (2 min)

- [ ] Update `WEEK4_SECURITY_REGRESSION_MATRIX.md`
- [ ] Create one archive entry using `WEEK4_SECURITY_DAILY_LOG_TEMPLATE.md`
- [ ] If failure found, file `SEC-*` issue using `WEEK4_SECURITY_BUG_TEMPLATE.md`

## Fast troubleshooting

- Missing reference errors:
  - Re-open `WEEK4_UI_PREFAB_BINDING_TABLE.md` and rebind exact fields.
- PIN panel not visible:
  - Ensure `requirePinToOpen = true` and `pinPanel` ref is assigned.
- No localized text:
  - Confirm localization bootstrap object exists in scene startup flow.

## Done criteria

- `HomeScene` interactive
- Facilitator PIN flow operational
- Security smoke pass completed and documented
