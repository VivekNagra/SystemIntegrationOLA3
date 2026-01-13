# 02 – Architecture and DDD (Part 2)

## Tech stack
- .NET 8 (ASP.NET Core Web API)
- REST for synchronous APIs
- In-memory persistence for iteration 1 (simple demo)
- Service-to-service integration via HTTP (Rental → Billing)

## Bounded contexts → services
- Rental Management (Core Domain) → `rental-service`
- Billing & Payments (Supporting Domain) → `billing-service`

## Integration style
User-driven actions are synchronous:
- App/Client → Rental service (HTTP)

Cross-context collaboration is implemented as a synchronous call for the demo:
- Rental service → Billing service (HTTP)

(Using a message broker is optional and not required for the first iteration.)

## What is implemented in iteration 1
Rental service:
- Create booking (computes allowed end time)
- Start rental
- Return rental (detect late)
- Request Billing to create and capture a charge
- Close rental when Billing confirms payment captured
**Iteration 1 assumption (demo):** 
- Late fee is implemented as a fixed 100 kr to keep the flow deterministic.
- Insurance fee is fixed at 50 kr (from the case).

Billing service:
- Create a charge for a rental
- Add insurance fee (50 kr) if selected
- Add late fee if late (simple rule for iteration 1)
- Simulate payment capture and return confirmation