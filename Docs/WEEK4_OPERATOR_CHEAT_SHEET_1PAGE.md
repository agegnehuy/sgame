# Week 4 Operator Cheat Sheet (1 Page)

Use this quick sheet during onsite setup/testing.

## 1) Fast Start (5 mins)

- Open `HomeScene` (or `HomeScene_QA` for test mode).
- Confirm top bar shows `Profile`, `Coins`, and `EN/AM`.
- Press `Open Facilitator Panel`.
- Enter correct facilitator PIN.

If panel does not open:

- Check `requirePinToOpen = true`
- Check PIN fields are assigned in inspector

## 2) Security Quick Checks (5 mins)

- Wrong PIN once -> cooldown appears.
- Wrong PIN repeatedly -> lockout appears.
- Wait for timer -> controls recover.
- If shown, `Security:` label changes Ready/Cooldown/Lockout correctly.

## 3) Maintenance Quick Checks (5 mins)

- Press `Clear Analytics` -> success status.
- Press `Clear Exports` -> success status.
- Press `Clear PIN Audit` -> success status.
- Press `Reset Profile` once -> warning appears.
- Press again quickly -> reset completes.

## 4) EN/AM Check (2 mins)

- Switch EN -> AM -> EN.
- Confirm facilitator labels/status messages update.
- Confirm no raw localization keys appear.

## 5) Must-Record Outputs (3 mins)

- Update `WEEK4_SECURITY_REGRESSION_MATRIX.md`
- Save one log from `WEEK4_SECURITY_DAILY_LOG_TEMPLATE.md`
- If failure: file `SEC-*` using `WEEK4_SECURITY_BUG_TEMPLATE.md`

## Go / No-Go Rule

- **GO** only if no Critical/High open security bugs.
- **NO-GO** if PIN access, lockout, or audit flow is broken.

## Where to look next

- Full flow map: `WEEK4_SECURITY_DOCS_MAP.md`
- Combined setup sheet: `WEEK4_DAY0_AND_FINAL_WIRING_COMBINED_CHECKLIST.md`
