# Bug Report Template

Use this template for every defect found during QA.  
One bug = one report.

---

## 1) Summary

- **Bug ID:** `BUG-YYYYMMDD-###`
- **Title:** short and specific (example: `P1-M2 stays locked after pass score`)
- **Reporter:**
- **Date/Time (UTC+3):**
- **Build Version / APK Name:**
- **Device Model:**
- **Android Version:**
- **Language Mode:** `EN` / `AM`

## 2) Classification

- **Severity:** `Blocker` / `Critical` / `High` / `Medium` / `Low`
- **Priority:** `P0` / `P1` / `P2` / `P3`
- **Area:** `Gameplay` / `UI` / `Save` / `Localization` / `Store` / `Progression` / `Performance` / `Export`
- **Regression?:** `Yes` / `No` / `Unknown`

## 3) Preconditions

List setup required before reproducing, for example:
- Existing profile with coins > 50
- `P1-M1` completed with score >= 55
- Language set to Amharic

## 4) Steps to Reproduce

1.  
2.  
3.  
4.  

## 5) Expected Result

Describe correct behavior in one or two lines.

## 6) Actual Result

Describe what actually happened.

## 7) Reproducibility

- **Repro rate:** `Always (5/5)` / `Often (3-4/5)` / `Sometimes (1-2/5)` / `Unable`

## 8) Evidence

- **Screenshot path(s):**
- **Screen recording path (if any):**
- **Log/console snippet:**
- **Exported JSON path (if data bug):**

## 9) Impact

- **User impact:** who is affected (child player, facilitator, both)
- **Learning impact:** does this block or corrupt learning outcomes?
- **Scope:** single feature or cross-feature impact

## 10) Suggested Owner

- **Primary owner role:** `Gameplay Dev` / `UI Dev` / `Data/Save Dev` / `Localization` / `QA`

## 11) Resolution (filled by dev/lead)

- **Status:** `Open` / `In Progress` / `Fixed` / `Rejected` / `Duplicate`
- **Fix summary:**
- **Fixed in build:**
- **Retest result:**

---

## Short Example

- **Title:** `Language switch does not refresh mission lock labels`
- **Severity:** `High`
- **Priority:** `P1`
- **Steps:** Switch EN -> AM in HomeScene; observe mission labels.
- **Expected:** Labels update to AM immediately.
- **Actual:** Labels remain in previous language until scene reload.
