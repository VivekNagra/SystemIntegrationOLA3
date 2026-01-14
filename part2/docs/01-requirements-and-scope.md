# 01 – Requirements and Scope (Part 2 / OLA 4)

This document summarizes what is implemented for Part 2 (OLA 4) and how it maps to the brief.

## Requirements (from the brief)

The implementation must:
- Use a **Domain-Driven Design (DDD)** approach
- Include **at least two services**
- Use the **bounded contexts** from Part 1 as service boundaries
- Document the codebase and explicitly show **Ports & Adapters**
- Provide at least one iteration that can be **demonstrated locally**
- Be submitted as a **GitHub repo link** (email or upload folder)

## Scope for this implementation (Iteration 1)

This implementation focuses on the short-term rental flow and its integration to billing.

### Implemented flow
1. **Create booking** (Rental service)
2. **Start rental** (Rental service)
3. **Return rental** (Rental service)
4. On return, Rental calls Billing to create a charge and capture payment
5. Rental closes the rental when payment is captured

### Business rules implemented
- Allowed end time is computed by the Rental domain using:
  `allowedEndTime = min(startTime + 24 hours, midnight cut-off)`
- Late return is detected when `returnTime > allowedEndTime`
- Insurance fee is applied when insurance is selected (**50 kr**)

### Iteration 1 assumptions (documented)
- Late fee is implemented as a fixed **100 kr** (demo assumption to keep the iteration deterministic)
- Payment provider is a fake adapter that always succeeds (demo stability)

## Service boundaries (bounded contexts)

The bounded contexts from Part 1 are used as service boundaries:
- **Rental Management** → `services/rental-service` (core domain)
- **Billing & Payments** → `services/billing-service` (supporting domain)

Identity and Partner/Settlement are treated as out of scope for this implementation iteration.
