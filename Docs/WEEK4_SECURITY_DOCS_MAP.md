# Week 4 Security Docs Map

Use this file as the single navigator for facilitator PIN security QA and triage.

## Security QA in 60 Seconds

1. Run `WEEK4_PIN_SECURITY_QUICK_TEST.md`.
2. Mark outcomes in `WEEK4_SECURITY_REGRESSION_MATRIX.md`.
3. If any `FAIL`, create `SEC-*` report with `WEEK4_SECURITY_BUG_TEMPLATE.md`.
4. Save the day result in `WEEK4_SECURITY_DAILY_LOG_TEMPLATE.md`.
5. If no `Critical/High` open security bugs remain, mark status `Green`.
6. For scene setup parity, follow `WEEK4_HOME_SCENE_SECURITY_WIRING_CHECKLIST.md`.
7. If using QA scene split, apply `WEEK4_HOME_SCENE_QA_VARIANT_CHECKLIST.md`.
8. Before pilot/release candidate, pass `WEEK4_SECURITY_RELEASE_GATE_CHECKLIST.md`.

## Recommended Flow

1. Run quick validation  
   Use `WEEK4_PIN_SECURITY_QUICK_TEST.md`
2. Record pass/fail status by scenario  
   Use `WEEK4_SECURITY_REGRESSION_MATRIX.md`
3. Archive today’s run  
   Use `WEEK4_SECURITY_DAILY_LOG_TEMPLATE.md`
4. File defects for any failures  
   Use `WEEK4_SECURITY_BUG_TEMPLATE.md`
5. Apply severity/priority labels  
   Use `BUG_SEVERITY_PRIORITY_RUBRIC.md`
6. Move issues through triage and retest  
   Use `QA_TRIAGE_WORKFLOW.md`

## Which Doc For Which Need

- Need a fast pre-build PIN check  
  -> `WEEK4_PIN_SECURITY_QUICK_TEST.md`
- Need a scenario-by-scenario pass/fail view  
  -> `WEEK4_SECURITY_REGRESSION_MATRIX.md`
- Need to keep daily historical records  
  -> `WEEK4_SECURITY_DAILY_LOG_TEMPLATE.md`
- Need to report a security defect in detail  
  -> `WEEK4_SECURITY_BUG_TEMPLATE.md`
- Need general (non-security-specific) bug report format  
  -> `BUG_REPORT_TEMPLATE.md`
- Need go/no-go decision before pilot handoff  
  -> `WEEK4_SECURITY_RELEASE_GATE_CHECKLIST.md`

## Minimum Daily Security Package

At the end of each QA day, keep:

- One completed copy of `WEEK4_SECURITY_DAILY_LOG_TEMPLATE.md`
- Any linked `SEC-*` bug reports
- Evidence paths (screenshots, recordings, audit log snippets)

## Exit Criteria (Security Green)

Declare daily security status `Green` only when:

- No `Critical` or `High` open security defects for PIN access
- Lockout/cooldown behavior passes on target test devices
- Audit logging + maintenance clear flow both pass
