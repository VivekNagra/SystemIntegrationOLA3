# 04 – Demo Steps (Iteration 1)

This iteration demonstrates the end-to-end flow across two services:
- Rental Service (8080)
- Billing Service (8082)

## Prerequisites
Open two terminals:

Terminal 1 (Rental):
- `cd part2/services/rental-service/src/RentalService.Api`
- `dotnet run`

Terminal 2 (Billing):
- `cd part2/services/billing-service/src/BillingService.Api`
- `dotnet run`

## Demo flow (PowerShell)

### 1) Create booking
POST `http://localhost:8080/bookings`

- Computes allowed end time using the rule:
  `allowedEndTime = min(start + 24h, midnight cut-off)`
- Stores the rental in status `Booked`

### 2) Start rental
POST `http://localhost:8080/rentals/{rentalId}/start`

- Transitions rental to `Active`

### 3) Return rental
POST `http://localhost:8080/rentals/{rentalId}/return`

- Transitions rental to `Returned`
- Detects late return (`returnTime > allowedEndTime`)
- Calls Billing service:
  POST `http://localhost:8082/charges/from-rental`
- Billing calculates fees:
  - Insurance fee: 50 kr if selected
  - Late fee: 100 kr if late (iteration 1 assumption)
- Billing simulates payment capture and returns `Captured`
- Rental closes the rental when payment is captured (`Closed`)
