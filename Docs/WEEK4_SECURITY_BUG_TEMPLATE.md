# Week 4 Security Bug Template (PIN + Facilitator Access)

Use this template for security-related defects only.

---

## 1) Summary

- **Security Bug ID:** `SEC-YYYYMMDD-###`
- **Title:** short and specific (example: `PIN lockout ends early after scene reload`)
- **Reporter:**
- **Date/Time (UTC+3):**
- **Build Version / APK Name:**
- **Device Model:**
- **Android Version:**
- **Language Mode:** `EN` / `AM`

## 2) Security Classification

- **Severity:** `Blocker` / `Critical` / `High` / `Medium` / `Low`
- **Priority:** `P0` / `P1` / `P2` / `P3`
- **Security Area:** `PIN Validation` / `Cooldown` / `Lockout` / `Audit Logging` / `Maintenance Clear` / `Access Control`
- **Regression?:** `Yes` / `No` / `Unknown`

## 3) Preconditions

List required setup, for example:
- `requirePinToOpen = true`
- `maxFailedAttempts = 3`
- `lockoutDurationSeconds = 20`
- `failedAttemptCooldownSeconds = 2`

## 4) Steps to Reproduce

1.  
2.  
3.  
4.  

## 5) Expected Security Behavior

Describe expected secure behavior in one or two lines.

## 6) Actual Behavior

Describe what happened.

## 7) Reproducibility

- **Repro rate:** `Always (5/5)` / `Often (3-4/5)` / `Sometimes (1-2/5)` / `Unable`

## 8) Security Evidence

- **Screenshot/recording paths:**
- **PIN status text shown:**
- **Security state label shown (if wired):**
- **Audit log file path:**
- **Relevant audit event lines:**

## 9) Risk Assessment

- **Exploitability:** `High` / `Medium` / `Low`
- **Potential impact:** unauthorized facilitator access, audit gap, false lockout, etc.
- **Affected roles:** `Facilitator` / `Admin` / `QA` / `All`

## 10) Suggested Owner

- **Primary owner role:** `UI Dev` / `Data/Save Dev` / `Gameplay Dev` / `QA`

## 11) Resolution (filled by dev/lead)

- **Status:** `Open` / `In Progress` / `Fixed` / `Rejected` / `Duplicate`
- **Fix summary:**
- **Fixed in build:**
- **Retest result:**

---

## Security Example

- **Title:** `PIN accepted during cooldown period`
- **Severity:** `Critical`
- **Priority:** `P0`
- **Security Area:** `Cooldown`
- **Expected:** submit remains blocked until cooldown timer is complete.
- **Actual:** second submit accepted immediately and panel opened.
