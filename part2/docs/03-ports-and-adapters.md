# 03 – Ports and Adapters (Hexagonal Architecture)

This implementation follows Ports & Adapters to keep business logic independent from frameworks and infrastructure concerns.

Each service is split into four projects:

## Domain project (`*.Domain`)
Contains:
- Aggregates, entities, value objects
- Business rules and invariants (e.g., allowed end time rule)
- Domain events as plain objects (where useful)

## Application project (`*.Application`)
Contains:
- Use cases (application services)
- Ports (interfaces) for persistence and integrations
- DTOs/contracts used by the application layer

## Infrastructure project (`*.Infrastructure`)
Contains:
- Adapters implementing ports (e.g., in-memory repositories)
- HTTP client adapter(s) for service-to-service calls
- Any technical concerns that should not leak into Domain/Application

## API project (`*.Api`)
Contains:
- REST controllers (inbound adapters)
- Dependency injection wiring
- Minimal request/response models for HTTP endpoints

## Example ports in this solution

Rental service (Application ports):
- `IRentalRepository` (persistence port)
- `IBillingClient` (integration port to Billing service)

Billing service (Application ports):
- `IChargeRepository` (persistence port)
- `IPaymentProvider` (external integration port; faked in iteration 1)

This structure demonstrates a DDD-friendly separation of concerns while keeping the demo implementation small.
