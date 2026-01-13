# 1. Scope and Assumptions

This document defines what is included in the System Integration OLA 3 case study for **MyTrailer** and what is explicitly excluded. The purpose is to keep the domain model and architectural decisions consistent with the case description and to document any assumptions made where the case leaves details open.

## 1.1 Scope (In Scope)

### Short-term rental via mobile app
- Customers can **book and rent a trailer using the MyTrailer mobile application**.
- The rental is considered **short-term**:
  - Maximum rental duration is **24 hours**, and
  - The rental must **always end by midnight at the latest**, even if 24 hours has not passed.
- A trailer is identified by:
  - `locationId` (partner location), and
  - `trailerNumber` (number within that location).

### Booking and rental lifecycle
- Booking a specific trailer for a short-term rental.
- Starting a rental (handover/pick-up) for a booking.
- Returning the trailer and closing the rental.
- Enforcing time-related business rules (max 24 hours and midnight cut-off).
- Handling booking conflicts (e.g., trailer already booked for the requested time).

### Pricing elements relevant to the case
- The base trailer rental price is **0 DKK** (the business model relies on partners).
- Insurance can be purchased as an add-on:
  - **50 DKK**, optional, and commonly purchased by customers.
- Late return is handled by applying an **excess rental fee** (late fee).

### Payments (conceptual integration)
- The model includes the concept of payment processing for:
  - Insurance, and/or
  - Late fees (if applicable).
- Payment processing is treated as integration with an external payment provider (details of provider APIs are not modeled in depth, but the interaction is represented through commands/events and service boundaries).

### Partner/location perspective (high-level)
- Trailers are hosted at partner companies (locations).
- The model captures that the business earns revenue through partner collaboration and that rentals can produce “usage” information used for settlement/accounting.
- Detailed accounting and invoicing flows are considered secondary, but the design acknowledges this aspect through events and bounded context separation.

## 1.2 Out of Scope (Excluded)

### Long-term / overnight rentals (website flow)
- “Long-term rental” (overnight) is **not handled by the mobile app** and follows a separate process via the website and specialist locations.
- The long-term rental business process and its operational setup is out of scope for this assignment.

### Physical operations not described in detail
- Exact hardware details (locks, sensors, physical key exchange) are not specified.
- The model may refer to “pickup” and “return” events, but the underlying physical mechanism is not implemented or detailed beyond what is necessary to describe the domain.

### Claims handling / insurance case management
- While insurance purchase is in scope, **claims processing** (damage assessment, reimbursement workflow, disputes) is not modeled in depth. This may be mentioned as a future extension.

## 1.3 Key Business Rules (Explicit)

1. **Maximum duration rule**  
   A short-term rental can last at most **24 hours**.

2. **Midnight cut-off rule**  
   Regardless of the start time, the rental must end by **midnight** at the latest.

3. **Trailer identification rule**  
   A trailer is uniquely identified by `(locationId, trailerNumber)`.

4. **Late return rule**  
   If a trailer is returned after its allowed end time, an **excess rental fee** (late fee) must be applied before closing the rental.

5. **Insurance rule**  
   Insurance is optional and costs **50 DKK**.

## 1.4 Assumptions

### Identity and access
- Customers are authenticated in the app and can be represented by a `customerId`.

### Booking and availability
- A trailer cannot have overlapping rentals. The system enforces this rule when accepting a booking.
- Availability is determined by existing bookings/rentals for the same `(locationId, trailerNumber)` within the requested time window.

### Time handling
- All times are treated consistently (e.g., a single timezone) and the “midnight” rule refers to local midnight for the relevant location.
- The allowed end time is calculated as:

  `allowedEndTime = min(startTime + 24 hours, midnight cut-off)`

  This is justified from the case requirement that short-term rentals must end by midnight at the latest.

### Payments
- Payment handling is modeled as a separate responsibility (bounded context/service).
- Insurance and late fees are represented as billable line items in a charge.
- A “payment provider” exists externally; integration is represented conceptually rather than implemented against a real provider API.

### Partner settlement
- Partner settlement is modeled at a high level as usage recording and potential invoicing.
- The detailed financial agreement is not specified; the model focuses on capturing rental usage events needed for settlement.

## 1.5 Open Questions (Not resolved by the case)

- Is insurance selected at booking time, at rental start, or both?
- Does the system authorize payment at booking, or only capture payment at rental closure?
- How is the excess rental fee calculated (fixed fee, per-hour, per-interval)?
- Are there grace periods for pickup/return around the booked window?
