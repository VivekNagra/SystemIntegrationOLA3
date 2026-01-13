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

## Services (bounded contexts)
- `services/rental-service` – Rental Management (core domain)
- `services/billing-service` – Billing & Payments (supporting domain)

Each service follows Ports & Adapters with four projects:
- `*.Domain` (domain model)
- `*.Application` (use cases + ports)
- `*.Infrastructure` (adapters)
- `*.Api` (REST controllers)

## How to run (local)

### Terminal 1 – Rental Service
```bash
cd part2/services/rental-service/src/RentalService.Api
dotnet run
```

### Terminal 2 – Billing Service
```bash
cd part2/services/billing-service/src/BillingService.Api
dotnet run
```

## Default ports: 
- Rental: http://localhost:8080
- Billing: http://localhost:8082

## Demo (end-to-end checks)

Use these PowerShell snippets to exercise the flow. Ensure both services are running (Rental on 8080, Billing on 8082).

### 1) Create booking
```powershell
$booking = Invoke-RestMethod -Method Post `
  -Uri http://localhost:8080/bookings `
  -ContentType "application/json" `
  -Body @'
{
  "locationId": "LOC-1",
  "trailerNumber": 1,
  "customerId": "CUST-123",
  "desiredStartTime": "2026-01-14T18:00:00",
  "insuranceSelected": true
}
'@
$booking
$rentalId = $booking.rentalId
$rentalId
```

### 2) Start rental
```powershell
$start = Invoke-RestMethod -Method Post `
  -Uri ("http://localhost:8080/rentals/{0}/start" -f $rentalId)
$start
```

### 3) Return rental (on time)
```powershell
$return = Invoke-RestMethod -Method Post `
  -Uri ("http://localhost:8080/rentals/{0}/return" -f $rentalId) `
  -ContentType "application/json" `
  -Body @'
{
  "returnTime": "2026-01-14T20:00:00"
}
'@
$return
```

### Late return variant (to see late fee path)
```powershell
$lateReturn = Invoke-RestMethod -Method Post `
  -Uri ("http://localhost:8080/rentals/{0}/return" -f $rentalId) `
  -ContentType "application/json" `
  -Body @'
{
  "returnTime": "2026-01-15T01:00:00"
}
'@
$lateReturn
```

Expected behavior:
- Booking returns `rentalId` and `allowedEndTime`.
- Start sets status to Active.
- On-time return: Rental calls Billing (`/charges/from-rental`), Billing captures payment, Rental closes.
- Late return: Rental flags late; Billing adds late fee before capture; Rental closes after payment captured.