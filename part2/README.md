# System Integration – Trailer Rental System (Part 2 / OLA 4)

This folder contains the **implementation** for the Trailer Rental case study (MyTrailer).

Part 2 requirements (from the brief):
- Implement the system using a **Domain-Driven Design** approach
- Include **at least two services**
- Use the **bounded contexts** from Part 1 as service boundaries
- Document how the codebase was built, including **Ports & Adapters**
- Ensure there is an iteration that can be **demonstrated**
- Submit a GitHub repo link for OLA 4 (email or upload folder)

## Tech stack (this implementation)
- .NET 8 (ASP.NET Core Web API)
- REST for synchronous integration
- In-memory persistence for the first iteration (demo-focused)

## Iteration 1 fee rules / assumptions
- Base trailer rental fee: **0 kr** (case)
- Insurance fee: **50 kr** when selected (case)
- Late fee: **100 kr** (fixed demo assumption to keep the flow deterministic)
- Billing captures payment via a **fake payment provider** (always succeeds) to ensure a stable demo iteration.

## Services (bounded contexts)
- `services/rental-service` – Rental Management (core domain)
- `services/billing-service` – Billing & Payments (supporting domain)

Each service follows Ports & Adapters with four projects:
- `*.Domain` (domain model)
- `*.Application` (use cases + ports)
- `*.Infrastructure` (adapters)
- `*.Api` (REST controllers)

## Configuration
Rental calls Billing via `Billing:BaseUrl` (defaults to `http://localhost:8082`).

## Key endpoints

Rental (http://localhost:8080):
- `POST /bookings`
- `POST /rentals/{rentalId}/start`
- `POST /rentals/{rentalId}/return`
- `GET  /rentals/{rentalId}`

Billing (http://localhost:8082):
- `POST /charges/from-rental`

## How to run (local) — simplest path

Prereqs: .NET 8 SDK, PowerShell (`pwsh`), ports **8080/8082** free.

### Fast path (recommended)
```powershell
cd part2
pwsh ./run-demo.ps1                
pwsh ./run-demo.ps1 -LateReturn    
```
- Script auto-restores/builds both services, starts Billing (8082) then Rental (8080), runs the booking/start/return calls, prints statuses, then stops the services.

## Manual run 

Terminal 1 – Rental Service
```bash
cd part2/services/rental-service/src/RentalService.Api
dotnet restore
dotnet build
dotnet run
```

## Terminal 2 – Billing Service
```bash
cd part2/services/billing-service/src/BillingService.Api
dotnet restore
dotnet build
dotnet run
```

## Swagger

Rental Swagger: http://localhost:8080/swagger

Billing Swagger: http://localhost:8082/swagger

Manual API calls

See docs/04-demo-steps.md for request bodies and the sequence (booking → start → return on-time/late).