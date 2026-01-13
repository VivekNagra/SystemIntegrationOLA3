# 7. Definition of Done (Model Readiness)

This document defines when the domain model and architecture work for the MyTrailer short-term rental solution is considered “done” for the purpose of this assignment.

The Definition of Done is intentionally concrete and maps to the required deliverables: requirements discovery, strategic/tactical DDD, and service integration/message flows.

## 7.1 Done criteria

The work is considered complete when all items below are satisfied.

### A) Scope and alignment to the case
- The short-term rental scope is clearly documented and consistent across all sections:
  - maximum rental duration of 24 hours
  - rental must end by midnight at the latest
- Out-of-scope items (long-term/overnight rental via website) are explicitly excluded.
- Assumptions and open questions are documented (and do not conflict with later design).

### B) Requirements discovery (Event Storming)
- Event Storming results identify the core flow:
  - booking → start rental → return → billing/payment → close
- At minimum, the Event Storming includes:
  - key commands
  - key domain events
  - key policies (midnight cut-off, late return detection, conflict prevention)
  - key read models (availability, status, receipts)
  - at least one external system boundary (payment provider)
- A readable Event Storming diagram (or equivalent clear documentation) is included in the repository.

### C) Strategic DDD
- Bounded contexts are defined and justified (at minimum Rental Management and Billing & Payments).
- A context map or clear relationship description exists, showing:
  - who is upstream/downstream
  - how contexts collaborate (preferably event-driven / published language)
- A high-level architecture is proposed and consistent with the bounded contexts and message flows.

### D) Tactical DDD
- At least two aggregates are documented with:
  - responsibilities
  - commands
  - domain events
  - invariants/business rules
- The model clearly enforces:
  - no overlap / availability rule per trailer
  - allowed end time rule: min(start + 24h, midnight cut-off)
  - late return detection and late fee triggering
  - insurance selection and fixed insurance fee (50 DKK)

### E) Message flows between services (integration)
- A clear description exists of:
  - synchronous user-driven commands (app → rental)
  - asynchronous event-driven collaboration (rental ↔ billing, partner usage)
- Both an on-time return flow and a late return flow are documented.
- Ownership is explicit:
  - Rental owns lifecycle/time rules
  - Billing owns monetary calculation and payment provider integration
- A minimal event catalogue is included to clarify integration contracts.

### F) Repository quality
- Documentation is structured and easy to navigate from `README.md`.
- All referenced files and diagrams (if any) exist in the repository.
- The repository content is self-contained enough to be evaluated without requiring private access to external tools.

## 7.2 Completion statement

When the above criteria are met, the model is considered ready to:
- start implementation experiments (prototype-level),
- validate flows end-to-end,
- and iterate on the design based on feedback.
