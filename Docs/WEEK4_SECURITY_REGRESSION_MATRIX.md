# Week 4 Security Regression Matrix

Use this matrix for daily pass/fail tracking of facilitator PIN security behavior.

## Build Info

- **Build Version / APK:**
- **Date (UTC+3):**
- **Tester:**
- **Device Model / Android:**
- **Language Mode Tested:** `EN` / `AM` / `Both`

## Status Legend

- `PASS` = behavior matches expected result
- `FAIL` = behavior deviates from expected result
- `BLOCKED` = cannot execute due to unrelated blocker
- `N/A` = scenario not applicable for this run

## Regression Matrix

| ID | Scenario | Expected Result | Status | Notes / Bug ID |
|---|---|---|---|---|
| SEC-01 | Correct PIN entry | Facilitator panel opens on valid PIN |  |  |
| SEC-02 | Wrong PIN feedback | Invalid PIN message is shown |  |  |
| SEC-03 | Per-attempt cooldown starts | Submit/input blocked for cooldown window |  |  |
| SEC-04 | Cooldown countdown updates | Cooldown seconds update while visible |  |  |
| SEC-05 | Lockout threshold reached | Lockout starts after max failed attempts |  |  |
| SEC-06 | Lockout controls blocked | Input and submit remain disabled during lockout |  |  |
| SEC-07 | Lockout countdown updates | Lockout timer changes each second |  |  |
| SEC-08 | Auto-recovery after lockout | PIN controls restore after lockout expiry |  |  |
| SEC-09 | Security state label (optional) | Ready/Cooldown/Lockout text matches state |  |  |
| SEC-10 | Audit log file creation | `pin_audit_YYYY_MM_DD.jsonl` exists after PIN events |  |  |
| SEC-11 | Audit event coverage | failed/cooldown/lockout/success events are recorded |  |  |
| SEC-12 | Clear PIN audit maintenance | Clear action removes audit files and reports count |  |  |
| SEC-13 | Post-clear audit regeneration | New PIN activity creates fresh audit entries |  |  |
| SEC-14 | QA simulate cooldown (optional) | Simulate cooldown forces blocked state |  |  |
| SEC-15 | QA simulate lockout (optional) | Simulate lockout forces lockout state |  |  |
| SEC-16 | QA clear security block (optional) | Clear action returns security state to ready |  |  |

## Daily Summary

- **Total scenarios executed:**
- **Pass count:**
- **Fail count:**
- **Blocked count:**
- **Overall security status:** `Green` / `Yellow` / `Red`

## Filing Defects

- Use `WEEK4_SECURITY_BUG_TEMPLATE.md` for any `FAIL`.
- Include matrix `ID` (for traceability) in bug title or notes.

## Example Filled Run (Reference)

Use this as a formatting example for real daily runs.

### Example Build Info

- **Build Version / APK:** `safe-steps-addis-dev-0.4.2.apk`
- **Date (UTC+3):** `2026-05-07`
- **Tester:** `QA-01`
- **Device Model / Android:** `Samsung A12 / Android 12`
- **Language Mode Tested:** `Both`

### Example Matrix Rows

| ID | Scenario | Expected Result | Status | Notes / Bug ID |
|---|---|---|---|---|
| SEC-01 | Correct PIN entry | Facilitator panel opens on valid PIN | PASS | Opened immediately with correct PIN |
| SEC-03 | Per-attempt cooldown starts | Submit/input blocked for cooldown window | PASS | Cooldown observed for ~2s |
| SEC-05 | Lockout threshold reached | Lockout starts after max failed attempts | PASS | Lockout triggered on 3rd wrong attempt |
| SEC-09 | Security state label (optional) | Ready/Cooldown/Lockout text matches state | PASS | Label changed in EN and AM |
| SEC-12 | Clear PIN audit maintenance | Clear action removes audit files and reports count | FAIL | Count shows cleared, but old file still present (`SEC-20260507-001`) |

### Example Daily Summary

- **Total scenarios executed:** `5`
- **Pass count:** `4`
- **Fail count:** `1`
- **Blocked count:** `0`
- **Overall security status:** `Yellow`
