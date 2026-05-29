# Final Acceptance Checklist

This checklist is the last gate before pilot handoff.

## 1) Core Functionality

- [ ] HomeScene opens without errors
- [ ] Mission loop works end-to-end (`P1_M1` minimum)
- [ ] Progression and rewards persist correctly
- [ ] Store buy/equip flow functions per profile

## 2) Localization and UX

- [ ] EN and AM switch correctly at runtime
- [ ] No visible localization key placeholders
- [ ] Critical child-facing feedback is understandable
- [ ] Touch targets are safe for small screens

## 3) Facilitator and Security

- [ ] PIN gate active in production scene
- [ ] Cooldown and lockout behavior validated
- [ ] PIN audit log creation validated
- [ ] Maintenance actions validated (clear/reset)
- [ ] Security release gate passed

## 4) Data and Reporting

- [ ] Local analytics logs recorded
- [ ] Analytics summary export works
- [ ] Facilitator report export works
- [ ] Export file paths shown in UI

## 5) Performance and Stability

- [ ] Low-end Android: 10-min run stable
- [ ] Mid-range Android: 10-min run stable
- [ ] No crash in repeated mission replay
- [ ] No major input lag in core actions

## 6) QA and Defect Status

- [ ] Week 1 QA checklist completed
- [ ] Week 2 regression checklist completed
- [ ] Security regression matrix updated
- [ ] No open P0 defects
- [ ] No open P1 defects in core/pilot scope

## 7) Handoff Readiness

- [ ] Pilot readiness checklist complete
- [ ] Pilot handoff package checklist complete
- [ ] Scope freeze policy acknowledged
- [ ] Build + known limitations documented for facilitators

## Final Approval

- **Acceptance result:** `PASS` / `FAIL`
- **Approved by (Tech):**
- **Approved by (QA):**
- **Approved by (Program/SME):**
- **Date (UTC+3):**
