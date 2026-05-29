# Pilot Readiness Checklist

Use this before sharing build with schools/facilitators.

## 1) Build and installation

- [ ] Signed APK generated.
- [ ] APK installs on target low-end Android phone.
- [ ] APK installs on target mid-range Android phone.
- [ ] First launch completes without crash.

## 2) Core gameplay readiness

- [ ] Home -> mission -> result -> home loop works.
- [ ] M1-M3 progression works (M4-M5 if included in pilot).
- [ ] No blocker in mission start or mission completion.
- [ ] Score/stars/coins update reliably.

## 3) Save and progression integrity

- [ ] Progress persists after app restart.
- [ ] Coins persist after app restart.
- [ ] Profile switching preserves separate progress states.
- [ ] Mission unlock state is accurate per profile.

## 4) Localization and accessibility

- [ ] EN/AM language switching works at runtime.
- [ ] No untranslated key strings visible.
- [ ] Critical feedback understandable in both languages.
- [ ] Touch targets usable for children on smaller screens.

## 5) Reporting and facilitator tools

- [ ] Export button generates report file.
- [ ] Report JSON includes expected profile and mission fields.
- [ ] Facilitator can locate exported file path from UI status.

## 6) Performance and stability

- [ ] 10-minute continuous gameplay test passes (low-end).
- [ ] 10-minute continuous gameplay test passes (mid-range).
- [ ] No crash during repeated mission replay.
- [ ] No major input lag in mission interactions.

## 7) Educational validation

- [ ] Each mission objective aligns with intended safety behavior.
- [ ] Unsafe actions always receive corrective feedback.
- [ ] No logic path rewards unsafe decisions.
- [ ] SME sign-off completed for pilot build.

## 8) QA and release gate

- [ ] No open P0 bugs.
- [ ] No open P1 bugs in core flow.
- [ ] Known issues documented with workaround if any.
- [ ] Pilot scope and limitations documented for facilitators.

## Pilot Go/No-Go Decision

- **Go** only if all critical categories above pass.
- **No-Go** if any safety/learning-critical behavior is unresolved.
