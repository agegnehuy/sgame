# Week 4 Security Release Gate Checklist

Use this checklist before any pilot/release candidate that includes facilitator PIN security.

## Build Context

- **Build Version / APK:**
- **Date (UTC+3):**
- **Prepared by:**
- **Reviewed by:**
- **Target devices validated:**

## 1) Access Control Gate

- [ ] `requirePinToOpen` is enabled in production `HomeScene`.
- [ ] Correct PIN grants access reliably.
- [ ] Wrong PIN never grants access.
- [ ] QA simulate buttons are disabled/unwired in production scene.

## 2) Cooldown + Lockout Gate

- [ ] Per-attempt cooldown activates after wrong PIN.
- [ ] Lockout activates at configured threshold (`maxFailedAttempts`).
- [ ] Input/submit remain blocked during lockout.
- [ ] Block timers recover automatically when elapsed.
- [ ] Optional security-state label matches real state.

## 3) Audit Logging Gate

- [ ] Security log file is created under `persistentDataPath/security`.
- [ ] Event coverage includes:
  - [ ] `pin_failed`
  - [ ] `pin_cooldown_blocked`
  - [ ] `pin_lockout_started`
  - [ ] `pin_lockout_blocked`
  - [ ] `pin_success`
- [ ] Timestamps and profile IDs are present in records.

## 4) Maintenance Safety Gate

- [ ] `Clear PIN Audit` action removes audit files and reports count.
- [ ] Reset profile flow still requires double-confirm + timeout.
- [ ] Danger visual behavior is correct while reset is armed.
- [ ] Maintenance actions do not bypass facilitator PIN gate.

## 5) Localization Gate (EN/AM)

- [ ] PIN prompt/invalid/cooldown/lockout messages are localized.
- [ ] Security-state labels are localized.
- [ ] No raw localization keys are visible in UI.

## 6) Regression Documentation Gate

- [ ] `WEEK4_PIN_SECURITY_QUICK_TEST.md` completed.
- [ ] `WEEK4_SECURITY_REGRESSION_MATRIX.md` updated.
- [ ] Daily archive created from `WEEK4_SECURITY_DAILY_LOG_TEMPLATE.md`.
- [ ] Any failures filed using `WEEK4_SECURITY_BUG_TEMPLATE.md`.

## 7) Final Decision

- **Security gate result:** `PASS` / `FAIL`
- **Open Critical/High security bugs:** `Yes` / `No`
- **Release recommendation:** `GO` / `NO-GO`
- **Notes / risk acceptance:**

---

Release must be `NO-GO` if any `Critical` or `High` PIN security defect is unresolved.
