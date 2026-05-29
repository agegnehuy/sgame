# Week 5 to Week 8 Macro Board

This board defines the second half of the 2-month schedule after pilot candidate readiness.

## Goal (Weeks 5-8)

Transition from release candidate to validated pilot delivery by:

- running controlled pilot sessions,
- fixing high-impact issues quickly,
- hardening reliability across target devices,
- producing a final MVP pilot package.

---

## Week 5 — Pilot Launch and Observation

### Focus
- Controlled pilot deployment to selected schools/groups.
- Observe real child interaction and facilitator usage.

### Key outputs
- Pilot session logs.
- First-round facilitator feedback.
- Initial issue list (technical + educational).

### Success criteria
- Pilot sessions run without blocker interruptions.
- Core loop usable by children with minimal assistance.

---

## Week 6 — High-Impact Iteration

### Focus
- Fix issues found in pilot with priority on learning-critical behaviors.
- Improve confusing feedback and flow friction.

### Key outputs
- Updated build with pilot-driven fixes.
- Updated known issue list.
- Retest report proving top issues resolved.

### Success criteria
- No unresolved pilot-discovered P0.
- Major confusion points reduced in retest sessions.

---

## Week 7 — Stabilization and Packaging

### Focus
- Regression pass on all previously fixed areas.
- Performance consistency and data integrity verification.
- Prepare final pilot handoff materials.

### Key outputs
- Stability-tested APK.
- Final facilitator package.
- Updated troubleshooting notes.

### Success criteria
- Full regression passes for core flow.
- Export and localization behavior remain stable.

---

## Week 8 — Final Pilot Delivery

### Focus
- Final sign-off and delivery.
- Handoff to foundation operations team.
- Plan next roadmap phase (Phase 2 expansion).

### Key outputs
- Final MVP pilot APK.
- Signed readiness checklist.
- Post-pilot roadmap recommendation.

### Success criteria
- Go decision confirmed by technical + educational stakeholders.
- Delivery package complete and accepted.

---

## Weekly operating rhythm (Weeks 5-8)

- Daily standup (use `WEEK2_DAILY_STANDUP_TEMPLATE.md`).
- Daily bug triage (use `QA_TRIAGE_WORKFLOW.md`).
- End-of-week review:
  - open bug count by priority,
  - regression pass rate,
  - educational feedback summary.

## Hard guardrails

- Keep freeze discipline for pilot branch.
- No new feature expansion unless critical for pilot success.
- Any scope addition must include timeline impact statement.
